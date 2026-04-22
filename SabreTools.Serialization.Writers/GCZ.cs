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
        public bool SerializeStream(DiscImage? obj, Stream? stream)
        {
            if (stream is null || !stream.CanWrite)
                return false;

            if (obj is null || !ValidateArchive(obj))
                return false;

            // Header (32 bytes, little-endian)
            stream.WriteLittleEndian(obj.Header.MagicCookie);
            stream.WriteLittleEndian(obj.Header.SubType);
            stream.WriteLittleEndian(obj.Header.CompressedDataSize);
            stream.WriteLittleEndian(obj.Header.DataSize);
            stream.WriteLittleEndian(obj.Header.BlockSize);
            stream.WriteLittleEndian(obj.Header.NumBlocks);

            // Block pointer table (8 bytes per block, little-endian)
            foreach (ulong ptr in obj.BlockPointers)
                stream.WriteLittleEndian(ptr);

            // Block hash table (4 bytes per block, little-endian)
            foreach (uint hash in obj.BlockHashes)
                stream.WriteLittleEndian(hash);

            stream.Flush();
            return true;
        }

        private static bool ValidateArchive(DiscImage obj)
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
    }
}
