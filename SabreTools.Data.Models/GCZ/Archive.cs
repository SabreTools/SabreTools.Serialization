namespace SabreTools.Data.Models.GCZ
{
    /// <summary>
    /// Represents a parsed GCZ (GameCube Zip) compressed disc image.
    /// Contains header metadata and block lookup tables.
    /// Actual compressed block data is accessed via the source stream.
    /// </summary>
    public class Archive
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
        /// Adler-32 (stored as CRC32) hashes of the uncompressed block data,
        /// one per block. Used for integrity verification.
        /// </summary>
        public uint[] BlockHashes { get; set; } = [];

        /// <summary>
        /// Byte offset within the GCZ file where the compressed block data begins.
        /// Computed as: <c>HeaderSize + (NumBlocks * 8) + (NumBlocks * 4)</c>.
        /// </summary>
        public long DataOffset { get; set; }
    }
}
