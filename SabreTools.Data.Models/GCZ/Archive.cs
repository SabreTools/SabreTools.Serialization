namespace SabreTools.Data.Models.GCZ
{
    /// <summary>
    /// Represents a parsed GCZ (GameCube Zip) compressed disc image.
    /// Contains header metadata and block lookup tables.
    /// Actual compressed block data is accessed via the source stream.
    /// </summary>
    public class DiscImage
    {
        /// <summary>
        /// GCZ file header
        /// </summary>
        public GczHeader Header { get; set; } = new();

        /// <summary>
        /// Block pointer table (one entry per block).
        /// Each value encodes both the offset of the block within the compressed data section
        /// and a compression flag in the top bit:
        /// <list type="bullet">
        ///   <item>Top bit CLEAR → block is zlib/deflate-compressed at that offset.</item>
        ///   <item>Top bit SET   → block is stored uncompressed at that offset.</item>
        /// </list>
        /// Offset is <c>value &amp; ~UncompressedFlag</c>.
        /// </summary>
        public ulong[] BlockPointers { get; set; } = [];

        /// <summary>
        /// Adler-32 checksums of the uncompressed block data, one per block.
        /// Used for integrity verification after decompression.
        /// </summary>
        public uint[] BlockHashes { get; set; } = [];
    }
}
