namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// Data descriptor (ZIP64)
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class DataDescriptor64
    {
        /// <summary>
        /// Recommended, but optional, signature (0x08074B50)
        /// </summary>
        public uint? Signature { get; set; }

        /// <summary>
        /// CRC-32
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// Compressed size
        /// </summary>
        public ulong CompressedSize { get; set; }

        /// <summary>
        /// Uncompressed size
        /// </summary>
        public ulong UncompressedSize { get; set; }
    }
}
