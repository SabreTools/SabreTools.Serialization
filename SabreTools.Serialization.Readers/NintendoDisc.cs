using System;
using System.IO;
using System.Text;
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

                // Determine platform from magic words
                if (disc.Header.WiiMagic == Constants.WiiMagicWord)
                    disc.Platform = Platform.Wii;
                else if (disc.Header.GCMagic == Constants.GCMagicWord)
                    disc.Platform = Platform.GameCube;
                else
                    disc.Platform = Platform.Unknown;

                // Parse Wii-specific structures
                if (disc.Platform == Platform.Wii)
                {
                    // Partition table starts at 0x40000
                    long partTableEnd = initialOffset + Constants.WiiPartitionTableAddress
                        + Constants.WiiPartitionGroupCount * 8;
                    if (data.Length >= partTableEnd)
                        disc.PartitionTableEntries = ParsePartitionTable(data, initialOffset);

                    // Region data at 0x4E000
                    long regionEnd = initialOffset + Constants.WiiRegionDataAddress + Constants.WiiRegionDataSize;
                    if (data.Length >= regionEnd)
                    {
                        data.Seek(initialOffset + Constants.WiiRegionDataAddress, SeekOrigin.Begin);
                        disc.RegionData = ParseRegionData(data);
                    }
                }

                return disc;
            }
            catch
            {
                return null;
            }
        }

        #region Header parsing

        private static DiscHeader ParseDiscHeader(Stream data)
        {
            var header = new DiscHeader();

            byte[] gameIdBytes = data.ReadBytes(Constants.GameIdLength);
            header.GameId = Encoding.ASCII.GetString(gameIdBytes).TrimEnd('\0');

            byte[] makerBytes = data.ReadBytes(Constants.MakerCodeLength);
            header.MakerCode = Encoding.ASCII.GetString(makerBytes).TrimEnd('\0');

            header.DiscNumber = data.ReadByteValue();
            header.DiscVersion = data.ReadByteValue();
            header.AudioStreaming = data.ReadByteValue();
            header.StreamingBufferSize = data.ReadByteValue();

            // Skip unused 0x0E bytes (offsets 0x00C–0x019)
            data.ReadBytes(0x0E);

            header.WiiMagic = data.ReadUInt32BigEndian();
            header.GCMagic = data.ReadUInt32BigEndian();

            byte[] titleBytes = data.ReadBytes(Constants.GameTitleLength);
            header.GameTitle = Encoding.ASCII.GetString(titleBytes).TrimEnd('\0');

            header.DisableHashVerification = data.ReadByteValue();
            header.DisableDiscEncryption = data.ReadByteValue();

            // Skip to DOL/FST offset fields at 0x420
            // We are currently at: 6+2+1+1+1+1+14+4+4+96+1+1 = 132 = 0x84
            // Need to reach 0x420
            int skipToBootBlock = Constants.DolOffsetField - 0x84;
            data.ReadBytes(skipToBootBlock);

            header.DolOffset = data.ReadUInt32BigEndian();
            header.FstOffset = data.ReadUInt32BigEndian();
            header.FstSize = data.ReadUInt32BigEndian();

            // Skip the remaining bytes to complete the 0x440 header
            // We are at 0x420 + 12 = 0x42C; need to reach 0x440
            data.ReadBytes(Constants.DiscHeaderSize - (Constants.DolOffsetField + 12));

            return header;
        }

        #endregion

        #region Wii partition table parsing

        private static WiiPartitionTableEntry[]? ParsePartitionTable(Stream data, long baseOffset)
        {
            data.Seek(baseOffset + Constants.WiiPartitionTableAddress, SeekOrigin.Begin);

            // Read 4 partition groups; each group has a count and a shifted offset
            var allEntries = new System.Collections.Generic.List<WiiPartitionTableEntry>();

            for (int g = 0; g < Constants.WiiPartitionGroupCount; g++)
            {
                uint count = data.ReadUInt32BigEndian();
                uint shiftedOffset = data.ReadUInt32BigEndian();

                if (count == 0)
                    continue;

                long tableOffset = baseOffset + ((long)shiftedOffset << 2);
                long savedPosition = data.Position;

                if (tableOffset + (long)count * 8 > data.Length)
                {
                    data.Seek(savedPosition, SeekOrigin.Begin);
                    continue;
                }

                data.Seek(tableOffset, SeekOrigin.Begin);
                for (uint i = 0; i < count; i++)
                {
                    var entry = new WiiPartitionTableEntry();
                    uint rawOffset = data.ReadUInt32BigEndian();
                    entry.Offset = (long)rawOffset << 2;
                    entry.Type = data.ReadUInt32BigEndian();
                    allEntries.Add(entry);
                }

                data.Seek(savedPosition, SeekOrigin.Begin);
            }

            return allEntries.Count > 0 ? allEntries.ToArray() : null;
        }

        #endregion

        #region Wii region data parsing

        private static WiiRegionData ParseRegionData(Stream data)
        {
            var region = new WiiRegionData();
            region.RegionSetting = data.ReadUInt32BigEndian();
            region.AgeRatings = data.ReadBytes(16);
            return region;
        }

        #endregion
    }
}
