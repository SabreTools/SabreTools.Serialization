using System.IO;
using SabreTools.Data.Models.GCZ;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    // TODO: Full round-trip write (including compressed block data) requires a source
    // IBlobReader. This implementation serializes only the structural metadata
    // (header + block pointer table + block hash table) to a stream or file.
    public class GCZ : IFileWriter<Archive>
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
        /// Serialize the GCZ structural metadata (header + tables) to a stream.
        /// Writes: 32-byte header, block pointer table, block hash table.
        /// The caller is responsible for writing compressed block data afterward.
        /// </summary>
        public bool SerializeStream(Archive? obj, Stream? stream)
        {
            if (stream is null || !stream.CanWrite)
                return false;

            if (obj is null || !ValidateArchive(obj))
                return false;

            // Header (32 bytes, little-endian)
            WriteUInt32LE(stream, obj.Header.MagicCookie);
            WriteUInt32LE(stream, obj.Header.SubType);
            WriteUInt64LE(stream, obj.Header.CompressedDataSize);
            WriteUInt64LE(stream, obj.Header.DataSize);
            WriteUInt32LE(stream, obj.Header.BlockSize);
            WriteUInt32LE(stream, obj.Header.NumBlocks);

            // Block pointer table (8 bytes per block, little-endian)
            foreach (ulong ptr in obj.BlockPointers)
                WriteUInt64LE(stream, ptr);

            // Block hash table (4 bytes per block, little-endian)
            foreach (uint hash in obj.BlockHashes)
                WriteUInt32LE(stream, hash);

            stream.Flush();
            return true;
        }

        private static bool ValidateArchive(Archive obj)
        {
            if (obj.Header is null)
                return false;
            if (obj.Header.MagicCookie != Constants.MagicCookie)
                return false;
            if (obj.Header.NumBlocks == 0)
                return false;
            if (obj.BlockPointers is null || obj.BlockPointers.Length != (int)obj.Header.NumBlocks)
                return false;
            if (obj.BlockHashes is null || obj.BlockHashes.Length != (int)obj.Header.NumBlocks)
                return false;
            return true;
        }

        #region Little-endian write helpers

        private static void WriteUInt32LE(Stream s, uint value)
        {
            s.WriteByte((byte)(value));
            s.WriteByte((byte)(value >> 8));
            s.WriteByte((byte)(value >> 16));
            s.WriteByte((byte)(value >> 24));
        }

        private static void WriteUInt64LE(Stream s, ulong value)
        {
            WriteUInt32LE(s, (uint)value);
            WriteUInt32LE(s, (uint)(value >> 32));
        }

        #endregion
    }
}
