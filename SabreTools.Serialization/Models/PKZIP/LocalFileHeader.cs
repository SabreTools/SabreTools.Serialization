namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// PKZIP local file header
    /// </summary>
    /// <see href="https://petlibrary.tripod.com/ZIP.HTM"/> 
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class LocalFileHeader
    {
        /// <summary>
        /// Signature (0x04034B50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Version needed to extract
        /// decimal value/10 = major version #
        /// decimal value%10 = minor version #
        /// </summary>
        public ushort Version { get; set; }

        /// <summary>
        /// General purpose bit flag
        /// </summary>
        public GeneralPurposeBitFlags Flags { get; set; }

        /// <summary>
        /// Compression method
        /// </summary>
        public CompressionMethod CompressionMethod { get; set; }

        /// <summary>
        /// Last modified file time
        /// </summary>
        public ushort LastModifedFileTime { get; set; }

        /// <summary>
        /// Last modified file date
        /// </summary>
        public ushort LastModifiedFileDate { get; set; }

        /// <summary>
        /// CRC-32
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// Compressed size
        /// </summary>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// Uncompressed size
        /// </summary>
        public uint UncompressedSize { get; set; }

        /// <summary>
        /// File name length
        /// </summary>
        public ushort FileNameLength { get; set; }

        /// <summary>
        /// Extra field length
        /// </summary>
        public ushort ExtraFieldLength { get; set; }

        /// <summary>
        /// File name (variable size)
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Extra fields (variable size)
        /// </summary>
        public ExtensibleDataField[]? ExtraFields { get; set; }
    }
}
