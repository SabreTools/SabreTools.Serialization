namespace SabreTools.Data.Models.GCZ
{
    /// <summary>
    /// GCZ (GameCube Zip) file header — 32 bytes at the start of the file
    /// </summary>
    /// <see href="https://github.com/dolphin-emu/dolphin/blob/master/Source/Core/DiscIO/CompressedBlob.h"/>
    public sealed class GczHeader
    {
        /// <summary>
        /// Magic cookie identifying a GCZ file (0xB10BC001)
        /// </summary>
        public uint MagicCookie { get; set; }

        /// <summary>
        /// Sub-type; always 0 for GameCube / Wii disc images
        /// </summary>
        public uint SubType { get; set; }

        /// <summary>
        /// Total size of the compressed block data section in bytes
        /// </summary>
        public ulong CompressedDataSize { get; set; }

        /// <summary>
        /// Total decompressed (ISO) size in bytes
        /// </summary>
        public ulong DataSize { get; set; }

        /// <summary>
        /// Size of each uncompressed block in bytes (must be 32 KiB, 64 KiB, or 128 KiB)
        /// </summary>
        public uint BlockSize { get; set; }

        /// <summary>
        /// Number of blocks in the image
        /// </summary>
        public uint NumBlocks { get; set; }
    }
}
