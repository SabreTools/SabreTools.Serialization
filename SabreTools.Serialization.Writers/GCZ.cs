using System.IO;
using SabreTools.Data.Models.GCZ;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Serialization.Writers
{
    // TODO: Full round-trip write (including compressed block data) requires a source
    // IBlobReader. This implementation serializes only the structural metadata
    // (header + block pointer table + block hash table) to a stream or file.
    public class GCZ : IFileWriter<DiscImage>
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
        /// Serialize the GCZ structural metadata (header + tables) to a stream.
        /// Writes: 32-byte header, block pointer table, block hash table.
        /// The caller is responsible for writing compressed block data afterward.
        /// </summary>
        public bool SerializeStream(DiscImage? obj, Stream? stream)
        {
            if (stream is null || !stream.CanWrite)
                return false;

            if (obj is null || !ValidateDiscImage(obj))
                return false;

            // Header (32 bytes, little-endian)
            WriteHeader(stream, obj.Header);

            // Block pointer table (8 bytes per block, little-endian)
            foreach (ulong pointer in obj.BlockPointers)
            {
                stream.WriteLittleEndian(pointer);
            }

            // Block hash table (4 bytes per block, little-endian)
            foreach (uint hash in obj.BlockHashes)
            {
                stream.WriteLittleEndian(hash);
            }

            stream.Flush();
            return true;
        }

        /// <summary>
        /// Write GczHeader data to the stream
        /// </summary>
        /// <param name="stream">Stream to write to</param>
        public static void WriteHeader(Stream stream, GczHeader obj)
        {
            stream.WriteLittleEndian(obj.MagicCookie);
            stream.WriteLittleEndian(obj.SubType);
            stream.WriteLittleEndian(obj.CompressedDataSize);
            stream.WriteLittleEndian(obj.DataSize);
            stream.WriteLittleEndian(obj.BlockSize);
            stream.WriteLittleEndian(obj.NumBlocks);
        }

        /// <summary>
        /// Validate that disc image is writable
        /// </summary>
        private static bool ValidateDiscImage(DiscImage obj)
        {
            if (obj.Header.MagicCookie != Constants.MagicCookie)
                return false;
            if (obj.Header.NumBlocks == 0)
                return false;
            if (obj.BlockPointers.Length != obj.Header.NumBlocks)
                return false;
            if (obj.BlockHashes.Length != obj.Header.NumBlocks)
                return false;

            return true;
        }
    }
}
