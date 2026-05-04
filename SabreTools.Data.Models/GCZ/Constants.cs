namespace SabreTools.Data.Models.GCZ
{
    public static class Constants
    {
        /// <summary>GCZ magic cookie (little-endian u32 at offset 0)</summary>
        public const uint MagicCookie = 0xB10BC001;

        /// <summary>Size of the GCZ file header in bytes</summary>
        public const int HeaderSize = 32;

        // Valid GCZ block sizes (Dolphin-compatible)
        public const uint BlockSize32K = 0x8000;
        public const uint BlockSize64K = 0x10000;
        public const uint BlockSize128K = 0x20000;
        public const uint DefaultBlockSize = BlockSize32K;

        /// <summary>
        /// Top bit of a block-pointer value: when CLEAR the block is zlib/deflate compressed;
        /// when SET the block is stored uncompressed.
        /// </summary>
        public const ulong UncompressedFlag = 0x8000000000000000;
    }
}
