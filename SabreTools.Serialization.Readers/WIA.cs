using System.IO;
using SabreTools.Data.Models.WIA;
using SabreTools.IO.Extensions;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class WIA : BaseBinaryReader<DiscImage>
    {
        /// <inheritdoc/>
        public override DiscImage? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Need at least Header1
            if (data.Length - data.Position < Constants.Header1Size)
                return null;

            try
            {
                long initialOffset = data.Position;

                var archive = new DiscImage();

                // Parse Header1
                archive.Header1 = ParseWiaHeader1(data);
                if (archive.Header1.Magic != Constants.WiaMagic && archive.Header1.Magic != Constants.RvzMagic)
                    return null;

                // Parse Header2
                archive.Header2 = ParseWiaHeader2(data);

                // Parse partition entries (Wii discs only)
                if (archive.Header2.NumberOfPartitionEntries > 0 && archive.Header2.PartitionEntriesOffset > 0)
                {
                    data.Seek(initialOffset + (long)archive.Header2.PartitionEntriesOffset, SeekOrigin.Begin);

                    archive.PartitionEntries = new PartitionEntry[archive.Header2.NumberOfPartitionEntries];
                    for (int i = 0; i < archive.PartitionEntries.Length; i++)
                    {
                        archive.PartitionEntries[i] = ParsePartitionEntry(data); ;
                    }
                }

                // Parse raw data entries
                if (archive.Header2.NumberOfRawDataEntries > 0 && archive.Header2.RawDataEntriesOffset > 0)
                {
                    data.Seek(initialOffset + (long)archive.Header2.RawDataEntriesOffset, SeekOrigin.Begin);

                    archive.RawDataEntries = new RawDataEntry[archive.Header2.NumberOfRawDataEntries];
                    for (int i = 0; i < archive.Header2.NumberOfRawDataEntries; i++)
                    {
                        archive.RawDataEntries[i] = ParseRawDataEntry(data);
                    }
                }

                // Parse group entries
                if (archive.Header2.NumberOfGroupEntries > 0 && archive.Header2.GroupEntriesOffset > 0)
                {
                    data.Seek(initialOffset + (long)archive.Header2.GroupEntriesOffset, SeekOrigin.Begin);
                    if (archive.Header1.Magic == Constants.RvzMagic)
                    {
                        archive.RvzGroupEntries = new RvzGroupEntry[archive.Header2.NumberOfGroupEntries];
                        for (int i = 0; i < archive.Header2.NumberOfGroupEntries; i++)
                        {
                            archive.RvzGroupEntries[i] = ParseRvzGroupEntry(data);
                        }
                    }
                    else
                    {
                        archive.GroupEntries = new WiaGroupEntry[archive.Header2.NumberOfGroupEntries];
                        for (int i = 0; i < archive.Header2.NumberOfGroupEntries; i++)
                        {
                            archive.GroupEntries[i] = ParseWiaGroupEntry(data); ;
                        }
                    }
                }

                return archive;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a PartitionDataEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled PartitionDataEntry on success, null on error</returns>
        public static PartitionDataEntry ParsePartitionDataEntry(Stream data)
        {
            var obj = new PartitionDataEntry();

            obj.FirstSector = data.ReadUInt32BigEndian();
            obj.NumberOfSectors = data.ReadUInt32BigEndian();
            obj.GroupIndex = data.ReadUInt32BigEndian();
            obj.NumberOfGroups = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PartitionEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled PartitionEntry on success, null on error</returns>
        public static PartitionEntry ParsePartitionEntry(Stream data)
        {
            var obj = new PartitionEntry();

            obj.PartitionKey = data.ReadBytes(16);
            obj.DataEntry0 = ParsePartitionDataEntry(data);
            obj.DataEntry1 = ParsePartitionDataEntry(data);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a RawDataEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled RawDataEntry on success, null on error</returns>
        public static RawDataEntry ParseRawDataEntry(Stream data)
        {
            var obj = new RawDataEntry();

            obj.DataOffset = data.ReadUInt64BigEndian();
            obj.DataSize = data.ReadUInt64BigEndian();
            obj.GroupIndex = data.ReadUInt32BigEndian();
            obj.NumberOfGroups = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a RawDataEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled RawDataEntry on success, null on error</returns>
        public static RawDataEntry ParseRawDataEntry(byte[] data, ref int offset)
        {
            var obj = new RawDataEntry();

            obj.DataOffset = data.ReadUInt64BigEndian(ref offset);
            obj.DataSize = data.ReadUInt64BigEndian(ref offset);
            obj.GroupIndex = data.ReadUInt32BigEndian(ref offset);
            obj.NumberOfGroups = data.ReadUInt32BigEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a RvzGroupEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled RvzGroupEntry on success, null on error</returns>
        public static RvzGroupEntry ParseRvzGroupEntry(Stream data)
        {
            var obj = new RvzGroupEntry();

            obj.DataOffset = data.ReadUInt32BigEndian();
            obj.DataSize = data.ReadUInt32BigEndian();
            obj.RvzPackedSize = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a RvzGroupEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled RvzGroupEntry on success, null on error</returns>
        public static RvzGroupEntry ParseRvzGroupEntry(byte[] data, ref int offset)
        {
            var obj = new RvzGroupEntry();

            obj.DataOffset = data.ReadUInt32BigEndian(ref offset);
            obj.DataSize = data.ReadUInt32BigEndian(ref offset);
            obj.RvzPackedSize = data.ReadUInt32BigEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a WiaGroupEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled WiaGroupEntry on success, null on error</returns>
        public static WiaGroupEntry ParseWiaGroupEntry(Stream data)
        {
            var obj = new WiaGroupEntry();

            obj.DataOffset = data.ReadUInt32BigEndian();
            obj.DataSize = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a byte array into a WiaGroupEntry
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Filled WiaGroupEntry on success, null on error</returns>
        public static WiaGroupEntry ParseWiaGroupEntry(byte[] data, ref int offset)
        {
            var obj = new WiaGroupEntry();

            obj.DataOffset = data.ReadUInt32BigEndian(ref offset);
            obj.DataSize = data.ReadUInt32BigEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a WiaHeader1
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled WiaHeader1 on success, null on error</returns>
        public static WiaHeader1 ParseWiaHeader1(Stream data)
        {
            var obj = new WiaHeader1();

            obj.Magic = data.ReadUInt32LittleEndian();
            obj.Version = data.ReadUInt32BigEndian();
            obj.VersionCompatible = data.ReadUInt32BigEndian();
            obj.Header2Size = data.ReadUInt32BigEndian();
            obj.Header2Hash = data.ReadBytes(20);
            obj.IsoFileSize = data.ReadUInt64BigEndian();
            obj.WiaFileSize = data.ReadUInt64BigEndian();
            obj.Header1Hash = data.ReadBytes(20);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a WiaHeader2
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled WiaHeader2 on success, null on error</returns>
        public static WiaHeader2 ParseWiaHeader2(Stream data)
        {
            var obj = new WiaHeader2();

            obj.DiscType = (WiaDiscType)data.ReadUInt32BigEndian();
            obj.CompressionType = (WiaRvzCompressionType)data.ReadUInt32BigEndian();
            obj.CompressionLevel = data.ReadInt32BigEndian();
            obj.ChunkSize = data.ReadUInt32BigEndian();
            obj.DiscHeader = data.ReadBytes(0x80);
            obj.NumberOfPartitionEntries = data.ReadUInt32BigEndian();
            obj.PartitionEntrySize = data.ReadUInt32BigEndian();
            obj.PartitionEntriesOffset = data.ReadUInt64BigEndian();
            obj.PartitionEntriesHash = data.ReadBytes(20);
            obj.NumberOfRawDataEntries = data.ReadUInt32BigEndian();
            obj.RawDataEntriesOffset = data.ReadUInt64BigEndian();
            obj.RawDataEntriesSize = data.ReadUInt32BigEndian();
            obj.NumberOfGroupEntries = data.ReadUInt32BigEndian();
            obj.GroupEntriesOffset = data.ReadUInt64BigEndian();
            obj.GroupEntriesSize = data.ReadUInt32BigEndian();
            obj.CompressorDataSize = data.ReadByteValue();
            obj.CompressorData = data.ReadBytes(7);

            return obj;
        }
    }
}
