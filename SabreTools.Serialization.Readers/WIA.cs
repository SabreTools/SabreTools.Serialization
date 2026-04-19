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
                archive.Header1 = ParseHeader1(data);

                // Validate magic
                if (archive.Header1.Magic != Constants.WiaMagic && archive.Header1.Magic != Constants.RvzMagic)
                    return null;

                archive.IsRvz = archive.Header1.Magic == Constants.RvzMagic;

                // Parse Header2
                archive.Header2 = ParseHeader2(data);

                // Parse partition entries (Wii discs only)
                if (archive.Header2.NumberOfPartitionEntries > 0
                    && archive.Header2.PartitionEntriesOffset > 0)
                {
                    data.Seek(initialOffset + (long)archive.Header2.PartitionEntriesOffset, SeekOrigin.Begin);
                    archive.PartitionEntries = ParsePartitionEntries(
                        data, (int)archive.Header2.NumberOfPartitionEntries);
                }

                // Parse raw data entries
                if (archive.Header2.NumberOfRawDataEntries > 0
                    && archive.Header2.RawDataEntriesOffset > 0)
                {
                    data.Seek(initialOffset + (long)archive.Header2.RawDataEntriesOffset, SeekOrigin.Begin);
                    archive.RawDataEntries = ParseRawDataEntries(
                        data, (int)archive.Header2.NumberOfRawDataEntries);
                }

                // Parse group entries
                if (archive.Header2.NumberOfGroupEntries > 0
                    && archive.Header2.GroupEntriesOffset > 0)
                {
                    data.Seek(initialOffset + (long)archive.Header2.GroupEntriesOffset, SeekOrigin.Begin);
                    if (archive.IsRvz)
                        archive.RvzGroupEntries = ParseRvzGroupEntries(
                            data, (int)archive.Header2.NumberOfGroupEntries);
                    else
                        archive.GroupEntries = ParseWiaGroupEntries(
                            data, (int)archive.Header2.NumberOfGroupEntries);
                }

                return archive;
            }
            catch
            {
                return null;
            }
        }

        #region Header parsing

        private static WiaHeader1 ParseHeader1(Stream data)
        {
            var h = new WiaHeader1();
            h.Magic = data.ReadUInt32LittleEndian();
            h.Version = data.ReadUInt32BigEndian();
            h.VersionCompatible = data.ReadUInt32BigEndian();
            h.Header2Size = data.ReadUInt32BigEndian();
            h.Header2Hash = data.ReadBytes(20);
            h.IsoFileSize = data.ReadUInt64BigEndian();
            h.WiaFileSize = data.ReadUInt64BigEndian();
            h.Header1Hash = data.ReadBytes(20);
            return h;
        }

        private static WiaHeader2 ParseHeader2(Stream data)
        {
            var h = new WiaHeader2();
            h.DiscType = (WiaDiscType)data.ReadUInt32BigEndian();
            h.CompressionType = (WiaRvzCompressionType)data.ReadUInt32BigEndian();
            h.CompressionLevel = data.ReadInt32BigEndian();
            h.ChunkSize = data.ReadUInt32BigEndian();
            h.DiscHeader = data.ReadBytes(0x80);
            h.NumberOfPartitionEntries = data.ReadUInt32BigEndian();
            h.PartitionEntrySize = data.ReadUInt32BigEndian();
            h.PartitionEntriesOffset = data.ReadUInt64BigEndian();
            h.PartitionEntriesHash = data.ReadBytes(20);
            h.NumberOfRawDataEntries = data.ReadUInt32BigEndian();
            h.RawDataEntriesOffset = data.ReadUInt64BigEndian();
            h.RawDataEntriesSize = data.ReadUInt32BigEndian();
            h.NumberOfGroupEntries = data.ReadUInt32BigEndian();
            h.GroupEntriesOffset = data.ReadUInt64BigEndian();
            h.GroupEntriesSize = data.ReadUInt32BigEndian();
            h.CompressorDataSize = data.ReadByteValue();
            h.CompressorData = data.ReadBytes(7);
            return h;
        }

        #endregion

        #region Table parsing

        private static PartitionEntry[] ParsePartitionEntries(Stream data, int count)
        {
            var entries = new PartitionEntry[count];
            for (int i = 0; i < count; i++)
            {
                var e = new PartitionEntry();
                e.PartitionKey = data.ReadBytes(16);
                e.DataEntry0 = ParsePartitionDataEntry(data);
                e.DataEntry1 = ParsePartitionDataEntry(data);
                entries[i] = e;
            }

            return entries;
        }

        private static PartitionDataEntry ParsePartitionDataEntry(Stream data)
        {
            var e = new PartitionDataEntry();
            e.FirstSector = data.ReadUInt32BigEndian();
            e.NumberOfSectors = data.ReadUInt32BigEndian();
            e.GroupIndex = data.ReadUInt32BigEndian();
            e.NumberOfGroups = data.ReadUInt32BigEndian();
            return e;
        }

        private static RawDataEntry[] ParseRawDataEntries(Stream data, int count)
        {
            var entries = new RawDataEntry[count];
            for (int i = 0; i < count; i++)
            {
                var e = new RawDataEntry();
                e.DataOffset = data.ReadUInt64BigEndian();
                e.DataSize = data.ReadUInt64BigEndian();
                e.GroupIndex = data.ReadUInt32BigEndian();
                e.NumberOfGroups = data.ReadUInt32BigEndian();
                entries[i] = e;
            }

            return entries;
        }

        private static WiaGroupEntry[] ParseWiaGroupEntries(Stream data, int count)
        {
            var entries = new WiaGroupEntry[count];
            for (int i = 0; i < count; i++)
            {
                var e = new WiaGroupEntry();
                // DataOffset stored as actual_offset >> 2
                e.DataOffset = (ulong)data.ReadUInt32BigEndian() << 2;
                e.DataSize = data.ReadUInt32BigEndian();
                entries[i] = e;
            }

            return entries;
        }

        private static RvzGroupEntry[] ParseRvzGroupEntries(Stream data, int count)
        {
            var entries = new RvzGroupEntry[count];
            for (int i = 0; i < count; i++)
            {
                var e = new RvzGroupEntry();
                // DataOffset stored as actual_offset >> 2
                e.DataOffset = (ulong)data.ReadUInt32BigEndian() << 2;
                e.DataSize = data.ReadUInt32BigEndian();
                e.RvzPackedSize = data.ReadUInt32BigEndian();
                entries[i] = e;
            }

            return entries;
        }

        #endregion
    }
}
