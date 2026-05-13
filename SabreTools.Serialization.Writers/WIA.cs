using System;
using System.IO;
using SabreTools.Data.Models.WIA;
using SabreTools.Numerics.Extensions;
using static SabreTools.Data.Models.WIA.Constants;

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

            if (obj is null || !ValidateDiscImage(obj))
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

            if (obj is null || !ValidateDiscImage(obj))
                return false;

            WriteWiaHeader1(stream, obj.Header1);
            WriteWiaHeader2(stream, obj.Header2);

            // Partition entries
            if (obj.PartitionEntries is not null)
            {
                foreach (var pe in obj.PartitionEntries)
                {
                    WritePartitionEntry(stream, pe);
                }
            }

            // Raw data entries
            foreach (var re in obj.RawDataEntries)
            {
                WriteRawDataEntry(stream, re);
            }

            // Group entries
            if (obj.Header1.Magic == RvzMagic && obj.RvzGroupEntries is not null)
            {
                foreach (var ge in obj.RvzGroupEntries)
                {
                    WriteRvzGroupEntry(stream, ge);
                }
            }
            else if (obj.Header1.Magic != RvzMagic && obj.GroupEntries is not null)
            {
                foreach (var ge in obj.GroupEntries)
                {
                    WriteWiaGroupEntry(stream, ge);
                }
            }

            stream.Flush();
            return true;
        }

        /// <summary>
        /// Write PartitionDataEntry data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WritePartitionDataEntry(Stream stream, PartitionDataEntry obj)
        {
            stream.WriteBigEndian(obj.FirstSector);
            stream.WriteBigEndian(obj.NumberOfSectors);
            stream.WriteBigEndian(obj.GroupIndex);
            stream.WriteBigEndian(obj.NumberOfGroups);
            stream.Flush();
        }

        /// <summary>
        /// Write PartitionEntry data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WritePartitionEntry(Stream stream, PartitionEntry obj)
        {
            stream.Write(obj.PartitionKey);
            WritePartitionDataEntry(stream, obj.DataEntry0);
            WritePartitionDataEntry(stream, obj.DataEntry1);
            stream.Flush();
        }

        /// <summary>
        /// Write RawDataEntry data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WriteRawDataEntry(Stream stream, RawDataEntry obj)
        {
            stream.WriteBigEndian(obj.DataOffset);
            stream.WriteBigEndian(obj.DataSize);
            stream.WriteBigEndian(obj.GroupIndex);
            stream.WriteBigEndian(obj.NumberOfGroups);
            stream.Flush();
        }

        /// <summary>
        /// Write RvzGroupEntry data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WriteRvzGroupEntry(Stream stream, RvzGroupEntry obj)
        {
            stream.WriteBigEndian(obj.DataOffset);
            stream.WriteBigEndian(obj.DataSize);
            stream.WriteBigEndian(obj.RvzPackedSize);
            stream.Flush();
        }

        /// <summary>
        /// Write WiaGroupEntry data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WriteWiaGroupEntry(Stream stream, WiaGroupEntry obj)
        {
            stream.WriteBigEndian(obj.DataOffset);
            stream.WriteBigEndian(obj.DataSize);
            stream.Flush();
        }

        /// <summary>
        /// Write WiaHeader1 data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WriteWiaHeader1(Stream stream, WiaHeader1 obj)
        {
            stream.WriteLittleEndian(obj.Magic);
            stream.WriteBigEndian(obj.Version);
            stream.WriteBigEndian(obj.VersionCompatible);
            stream.WriteBigEndian(obj.Header2Size);
            stream.Write(obj.Header2Hash);
            stream.WriteBigEndian(obj.IsoFileSize);
            stream.WriteBigEndian(obj.WiaFileSize);
            stream.Write(obj.Header1Hash);
            stream.Flush();
        }

        /// <summary>
        /// Write WiaHeader2 data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WriteWiaHeader2(Stream stream, WiaHeader2 obj)
        {
            stream.WriteBigEndian((uint)obj.DiscType);
            stream.WriteBigEndian((uint)obj.CompressionType);
            stream.WriteBigEndian(obj.CompressionLevel);
            stream.WriteBigEndian(obj.ChunkSize);

            byte[] dh = obj.DiscHeader;
            stream.Write(dh, 0, Math.Min(dh.Length, DiscHeaderStoredSize));
            if (dh.Length < DiscHeaderStoredSize)
                stream.Write(new byte[DiscHeaderStoredSize - dh.Length], 0, DiscHeaderStoredSize - dh.Length);

            stream.WriteBigEndian(obj.NumberOfPartitionEntries);
            stream.WriteBigEndian(obj.PartitionEntrySize);
            stream.WriteBigEndian(obj.PartitionEntriesOffset);
            stream.Write(obj.PartitionEntriesHash);
            stream.WriteBigEndian(obj.NumberOfRawDataEntries);
            stream.WriteBigEndian(obj.RawDataEntriesOffset);
            stream.WriteBigEndian(obj.RawDataEntriesSize);
            stream.WriteBigEndian(obj.NumberOfGroupEntries);
            stream.WriteBigEndian(obj.GroupEntriesOffset);
            stream.WriteBigEndian(obj.GroupEntriesSize);
            stream.WriteByte(obj.CompressorDataSize);

            byte[] prop = obj.CompressorData;
            stream.Write(prop, 0, Math.Min(prop.Length, 7));
            if (prop.Length < 7)
                stream.Write(new byte[7 - prop.Length], 0, 7 - prop.Length);

            stream.Flush();
        }

        /// <summary>
        /// Validate that disc image is writable
        /// </summary>
        private static bool ValidateDiscImage(DiscImage obj)
        {
            if (obj.Header1.Magic != WiaMagic && obj.Header1.Magic != RvzMagic)
                return false;

            return true;
        }
    }
}
