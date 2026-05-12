using System.IO;
using SabreTools.Data.Models.WIA;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    // TODO: Full round-trip write (including compressed group data) requires a source
    // IBlobReader and compression pipeline. This implementation serializes only
    // the structural metadata (Header1, Header2, and all lookup tables).
    public class WIA : IFileWriter<DiscImage>
    {
        /// <inheritdoc/>
        public bool Debug { get; set; } = false;

        /// <inheritdoc/>
        public bool SerializeFile(DiscImage? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (obj is null || !ValidateArchive(obj))
                return false;

            using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            return SerializeStream(obj, fs);
        }

        /// <summary>
        /// Serialize the WIA / RVZ structural metadata to a stream.
        /// Writes Header1, Header2, partition entries, raw data entries, and group entries.
        /// The caller is responsible for writing group (compressed block) data.
        /// </summary>
        public bool SerializeStream(DiscImage? obj, Stream? stream)
        {
            if (stream is null || !stream.CanWrite)
                return false;

            if (obj is null || !ValidateArchive(obj))
                return false;

            WriteHeader1(stream, obj.Header1);
            WriteHeader2(stream, obj.Header2);

            // Partition entries
            if (obj.PartitionEntries != null)
            {
                foreach (var pe in obj.PartitionEntries)
                    WritePartitionEntry(stream, pe);
            }

            // Raw data entries
            foreach (var re in obj.RawDataEntries)
                WriteRawDataEntry(stream, re);

            // Group entries
            if (obj.Header1.Magic == Constants.RvzMagic && obj.RvzGroupEntries != null)
            {
                foreach (var ge in obj.RvzGroupEntries)
                    WriteRvzGroupEntry(stream, ge);
            }
            else if (obj.Header1.Magic != Constants.RvzMagic && obj.GroupEntries != null)
            {
                foreach (var ge in obj.GroupEntries)
                    WriteWiaGroupEntry(stream, ge);
            }

            stream.Flush();
            return true;
        }

        private static bool ValidateArchive(DiscImage obj)
        {
            if (obj.Header1 is null || obj.Header2 is null)
                return false;
            if (obj.Header1.Magic != Constants.WiaMagic && obj.Header1.Magic != Constants.RvzMagic)
                return false;
            return true;
        }

        #region Write helpers

        private static void WriteHeader1(Stream s, WiaHeader1 h)
        {
            s.WriteLittleEndian(h.Magic);
            s.WriteBigEndian(h.Version);
            s.WriteBigEndian(h.VersionCompatible);
            s.WriteBigEndian(h.Header2Size);
            s.Write(h.Header2Hash, 0, 20);
            s.WriteBigEndian(h.IsoFileSize);
            s.WriteBigEndian(h.WiaFileSize);
            s.Write(h.Header1Hash, 0, 20);
        }

        private static void WriteHeader2(Stream s, WiaHeader2 h)
        {
            s.WriteBigEndian((uint)h.DiscType);
            s.WriteBigEndian((uint)h.CompressionType);
            s.WriteBigEndian(h.CompressionLevel);
            s.WriteBigEndian(h.ChunkSize);
            s.Write(h.DiscHeader, 0, 0x80);
            s.WriteBigEndian(h.NumberOfPartitionEntries);
            s.WriteBigEndian(h.PartitionEntrySize);
            s.WriteBigEndian(h.PartitionEntriesOffset);
            s.Write(h.PartitionEntriesHash, 0, 20);
            s.WriteBigEndian(h.NumberOfRawDataEntries);
            s.WriteBigEndian(h.RawDataEntriesOffset);
            s.WriteBigEndian(h.RawDataEntriesSize);
            s.WriteBigEndian(h.NumberOfGroupEntries);
            s.WriteBigEndian(h.GroupEntriesOffset);
            s.WriteBigEndian(h.GroupEntriesSize);
            s.WriteByte(h.CompressorDataSize);
            s.Write(h.CompressorData, 0, 7);
        }

        private static void WritePartitionDataEntry(Stream s, PartitionDataEntry e)
        {
            s.WriteBigEndian(e.FirstSector);
            s.WriteBigEndian(e.NumberOfSectors);
            s.WriteBigEndian(e.GroupIndex);
            s.WriteBigEndian(e.NumberOfGroups);
        }

        private static void WritePartitionEntry(Stream s, PartitionEntry e)
        {
            s.Write(e.PartitionKey, 0, 16);
            WritePartitionDataEntry(s, e.DataEntry0);
            WritePartitionDataEntry(s, e.DataEntry1);
        }

        private static void WriteRawDataEntry(Stream s, RawDataEntry e)
        {
            s.WriteBigEndian(e.DataOffset);
            s.WriteBigEndian(e.DataSize);
            s.WriteBigEndian(e.GroupIndex);
            s.WriteBigEndian(e.NumberOfGroups);
        }

        private static void WriteWiaGroupEntry(Stream s, WiaGroupEntry e)
        {
            // DataOffset stored as actual_offset >> 2
            s.WriteBigEndian((uint)(e.DataOffset >> 2));
            s.WriteBigEndian(e.DataSize);
        }

        private static void WriteRvzGroupEntry(Stream s, RvzGroupEntry e)
        {
            // DataOffset stored as actual_offset >> 2
            s.WriteBigEndian((uint)(e.DataOffset >> 2));
            s.WriteBigEndian(e.DataSize);
            s.WriteBigEndian(e.RvzPackedSize);
        }

        #endregion
    }
}
