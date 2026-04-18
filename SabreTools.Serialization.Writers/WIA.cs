using System.IO;
using SabreTools.Data.Models.WIA;

namespace SabreTools.Serialization.Writers
{
    // TODO: Full round-trip write (including compressed group data) requires a source
    // IBlobReader and compression pipeline. This implementation serializes only
    // the structural metadata (Header1, Header2, and all lookup tables).
    public class WIA : IFileWriter<Archive>
    {
        /// <inheritdoc/>
        public bool Debug { get; set; } = false;

        /// <inheritdoc/>
        public bool SerializeFile(Archive? obj, string? path)
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
        public bool SerializeStream(Archive? obj, Stream? stream)
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
            if (obj.IsRvz && obj.RvzGroupEntries != null)
            {
                foreach (var ge in obj.RvzGroupEntries)
                    WriteRvzGroupEntry(stream, ge);
            }
            else if (!obj.IsRvz && obj.GroupEntries != null)
            {
                foreach (var ge in obj.GroupEntries)
                    WriteWiaGroupEntry(stream, ge);
            }

            stream.Flush();
            return true;
        }

        private static bool ValidateArchive(Archive obj)
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
            WriteUInt32LE(s, h.Magic);
            WriteUInt32BE(s, h.Version);
            WriteUInt32BE(s, h.VersionCompatible);
            WriteUInt32BE(s, h.Header2Size);
            s.Write(h.Header2Hash, 0, 20);
            WriteUInt64BE(s, h.IsoFileSize);
            WriteUInt64BE(s, h.WiaFileSize);
            s.Write(h.Header1Hash, 0, 20);
        }

        private static void WriteHeader2(Stream s, WiaHeader2 h)
        {
            WriteUInt32BE(s, (uint)h.DiscType);
            WriteUInt32BE(s, (uint)h.CompressionType);
            WriteInt32BE(s, h.CompressionLevel);
            WriteUInt32BE(s, h.ChunkSize);
            s.Write(h.DiscHeader, 0, 0x80);
            WriteUInt32BE(s, h.NumberOfPartitionEntries);
            WriteUInt32BE(s, h.PartitionEntrySize);
            WriteUInt64BE(s, h.PartitionEntriesOffset);
            s.Write(h.PartitionEntriesHash, 0, 20);
            WriteUInt32BE(s, h.NumberOfRawDataEntries);
            WriteUInt64BE(s, h.RawDataEntriesOffset);
            WriteUInt32BE(s, h.RawDataEntriesSize);
            WriteUInt32BE(s, h.NumberOfGroupEntries);
            WriteUInt64BE(s, h.GroupEntriesOffset);
            WriteUInt32BE(s, h.GroupEntriesSize);
            s.WriteByte(h.CompressorDataSize);
            s.Write(h.CompressorData, 0, 7);
        }

        private static void WritePartitionDataEntry(Stream s, PartitionDataEntry e)
        {
            WriteUInt32BE(s, e.FirstSector);
            WriteUInt32BE(s, e.NumberOfSectors);
            WriteUInt32BE(s, e.GroupIndex);
            WriteUInt32BE(s, e.NumberOfGroups);
        }

        private static void WritePartitionEntry(Stream s, PartitionEntry e)
        {
            s.Write(e.PartitionKey, 0, 16);
            WritePartitionDataEntry(s, e.DataEntry0);
            WritePartitionDataEntry(s, e.DataEntry1);
        }

        private static void WriteRawDataEntry(Stream s, RawDataEntry e)
        {
            WriteUInt64BE(s, e.DataOffset);
            WriteUInt64BE(s, e.DataSize);
            WriteUInt32BE(s, e.GroupIndex);
            WriteUInt32BE(s, e.NumberOfGroups);
        }

        private static void WriteWiaGroupEntry(Stream s, WiaGroupEntry e)
        {
            // DataOffset stored as actual_offset >> 2
            WriteUInt32BE(s, (uint)(e.DataOffset >> 2));
            WriteUInt32BE(s, e.DataSize);
        }

        private static void WriteRvzGroupEntry(Stream s, RvzGroupEntry e)
        {
            // DataOffset stored as actual_offset >> 2
            WriteUInt32BE(s, (uint)(e.DataOffset >> 2));
            WriteUInt32BE(s, e.DataSize);
            WriteUInt32BE(s, e.RvzPackedSize);
        }

        private static void WriteUInt32LE(Stream s, uint v)
        {
            s.WriteByte((byte)v);
            s.WriteByte((byte)(v >> 8));
            s.WriteByte((byte)(v >> 16));
            s.WriteByte((byte)(v >> 24));
        }

        private static void WriteUInt32BE(Stream s, uint v)
        {
            s.WriteByte((byte)(v >> 24));
            s.WriteByte((byte)(v >> 16));
            s.WriteByte((byte)(v >> 8));
            s.WriteByte((byte)v);
        }

        private static void WriteInt32BE(Stream s, int v) => WriteUInt32BE(s, (uint)v);

        private static void WriteUInt64BE(Stream s, ulong v)
        {
            WriteUInt32BE(s, (uint)(v >> 32));
            WriteUInt32BE(s, (uint)v);
        }

        #endregion
    }
}
