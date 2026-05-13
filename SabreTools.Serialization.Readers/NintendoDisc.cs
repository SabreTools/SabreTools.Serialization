using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class NintendoDisc : BaseBinaryReader<Disc>
    {
        /// <inheritdoc/>
        public override Disc? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Need at least the disc header
            if (data.Length - data.Position < Constants.DiscHeaderSize)
                return null;

            try
            {
                long initialOffset = data.Position;

                var disc = new Disc();

                // Parse the disc header
                disc.Header = ParseDiscHeader(data);

                // Determine platform from magic words; fall back to GameId prefix for
                // GC discs that omit the magic word (e.g. some redump/scene ISOs)
                Platform platform = disc.Header.GetPlatform();

                // Parse Wii-specific structures
                if (platform == Platform.Wii)
                {
                    // Partition table starts at 0x40000
                    long partTableEnd = initialOffset
                        + Constants.WiiPartitionTableAddress
                        + (Constants.WiiPartitionGroupCount * 8);
                    if (partTableEnd < data.Length)
                    {
                        data.Seek(initialOffset + Constants.WiiPartitionTableAddress, SeekOrigin.Begin);
                        disc.PartitionTableEntries = ParsePartitionTable(data, initialOffset);
                    }

                    // Region data at 0x4E000
                    long regionEnd = initialOffset + Constants.WiiRegionDataAddress + Constants.WiiRegionDataSize;
                    if (regionEnd < data.Length)
                    {
                        data.Seek(initialOffset + Constants.WiiRegionDataAddress, SeekOrigin.Begin);
                        disc.RegionData = ParseWiiRegionData(data);
                    }
                }

                return disc;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a DiscHeader
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DiscHeader on success, null on error</returns>
        public static DiscHeader ParseDiscHeader(Stream data)
        {
            var obj = new DiscHeader();

            // 0x000: 4-char title code + 2-char maker code stored as one 6-byte GameId field
            byte[] gameIdBytes = data.ReadBytes(6);
            obj.GameId = Encoding.ASCII.GetString(gameIdBytes);

            obj.DiscNumber = data.ReadByteValue();
            obj.DiscVersion = data.ReadByteValue();
            obj.AudioStreaming = data.ReadByteValue();
            obj.StreamingBufferSize = data.ReadByteValue();
            obj.Padding00A = data.ReadBytes(0x0E);
            obj.WiiMagic = data.ReadUInt32BigEndian();
            obj.GCMagic = data.ReadUInt32BigEndian();

            byte[] titleBytes = data.ReadBytes(0x60);
            obj.GameTitle = Encoding.ASCII.GetString(titleBytes);

            obj.DisableHashVerification = data.ReadByteValue();
            obj.DisableDiscEncryption = data.ReadByteValue();
            obj.Padding082 = data.ReadBytes(0x39E);
            obj.DolOffset = data.ReadUInt32BigEndian();
            obj.FstOffset = data.ReadUInt32BigEndian();
            obj.FstSize = data.ReadUInt32BigEndian();
            obj.Padding42C = data.ReadBytes(0x14);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a DOLHeader
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled DOLHeader on success, null on error</returns>
        public static DOLHeader ParseDOLHeader(byte[] data, ref int offset)
        {
            var obj = new DOLHeader();

            obj.SectionOffsetTable = new uint[18];
            for (int i = 0; i < obj.SectionOffsetTable.Length; i++)
            {
                obj.SectionOffsetTable[i] = data.ReadUInt32BigEndian(ref offset);
            }

            obj.SectionAddressTable = new uint[18];
            for (int i = 0; i < obj.SectionAddressTable.Length; i++)
            {
                obj.SectionAddressTable[i] = data.ReadUInt32BigEndian(ref offset);
            }

            obj.SectionLengthsTable = new uint[18];
            for (int i = 0; i < obj.SectionLengthsTable.Length; i++)
            {
                obj.SectionLengthsTable[i] = data.ReadUInt32BigEndian(ref offset);
            }

            obj.BSSAddress = data.ReadUInt32BigEndian(ref offset);
            obj.BSSLength = data.ReadUInt32BigEndian(ref offset);
            obj.EntryPoint = data.ReadUInt32BigEndian(ref offset);
            obj.Padding = data.ReadBytes(ref offset, 0x1C);

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a ParseFileSystemTable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled ParseFileSystemTable on success, null on error</returns>
        public static FileSystemTable? ParseFileSystemTable(byte[] data)
        {
            // Check that the root entry exists
            if (data.Length < 12)
                return null;

            var obj = new FileSystemTable();

            // Read the root entry first
            int offset = 0;
            obj.Unknown = data.ReadBytes(ref offset, 8);
            obj.EntryCount = data.ReadUInt32BigEndian(ref offset);
            if (obj.EntryCount < 1 || (obj.EntryCount * 12) > data.Length)
                return null;

            // Read all entries
            offset = 0;
            obj.Entries = new FileSystemTableEntry[obj.EntryCount];
            for (int i = 0; i < obj.Entries.Length; i++)
            {
                obj.Entries[i] = ParseFileSystemTableEntry(data, ref offset);
            }

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a FileSystemTableEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled FileSystemTableEntry on success, null on error</returns>
        public static FileSystemTableEntry ParseFileSystemTableEntry(byte[] data, ref int offset)
        {
            var obj = new FileSystemTableEntry();

            obj.NameOffset = data.ReadUInt32BigEndian(ref offset);
            obj.FileOffset = data.ReadUInt32BigEndian(ref offset);
            obj.FileSize = data.ReadUInt32BigEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a partition table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled partition table on success, null on error</returns>
        public static WiiPartitionTableEntry[]? ParsePartitionTable(Stream data, long initialOffset)
        {
            // Read 4 partition groups; each group has a count and a shifted offset
            var allEntries = new List<WiiPartitionTableEntry>();

            for (int i = 0; i < Constants.WiiPartitionGroupCount; i++)
            {
                var group = ParseWiiPartitionGroup(data, initialOffset);
                if (group.Count == 0)
                    continue;

                // TODO: Keep group entries separate
                allEntries.AddRange(group.Entries);
            }

            return allEntries.Count > 0 ? [.. allEntries] : null;
        }

        /// <summary>
        /// Parse a Stream into a WiiPartitionGroup
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="initialOffset">Initial offset into the stream</param>
        /// <returns>Filled WiiPartitionTableEntry on success, null on error</returns>
        public static WiiPartitionGroup ParseWiiPartitionGroup(Stream data, long initialOffset)
        {
            var obj = new WiiPartitionGroup();

            obj.Count = data.ReadUInt32BigEndian();
            obj.Offset = data.ReadUInt32BigEndian();

            // Empty groups should not attempt parsing
            if (obj.Count == 0)
                return obj;

            // Determine the table offset
            long tableOffset = initialOffset + (obj.Offset << 2);
            if (tableOffset + (obj.Count * 8) > data.Length)
                return obj;

            // Seek to the table, if possible
            long savedPosition = data.Position;
            data.Seek(tableOffset, SeekOrigin.Begin);

            // Parse the table
            List<WiiPartitionTableEntry> entries = [];
            for (uint i = 0; i < obj.Count; i++)
            {
                var entry = ParseWiiPartitionTableEntry(data);
                entries.Add(entry);
            }

            // Set the entries and reset the stream
            obj.Entries = [.. entries];
            data.Seek(savedPosition, SeekOrigin.Begin);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a WiiPartitionTableEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled WiiPartitionTableEntry on success, null on error</returns>
        public static WiiPartitionTableEntry ParseWiiPartitionTableEntry(Stream data)
        {
            var obj = new WiiPartitionTableEntry();

            obj.Offset = data.ReadUInt32BigEndian();
            obj.Type = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a WiiRegionData
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled WiiRegionData on success, null on error</returns>
        public static WiiRegionData ParseWiiRegionData(Stream data)
        {
            var obj = new WiiRegionData();

            obj.RegionSetting = data.ReadUInt32BigEndian();
            obj.AgeRatings = data.ReadBytes(16);

            return obj;
        }
    }
}
