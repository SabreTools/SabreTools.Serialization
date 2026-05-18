namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// File header for Zstd-compressed 3DS data
    /// </summary>
    /// <see href="https://github.com/azahar-emu/azahar/blob/master/src/common/zstd_compression.h"/>
    public class Z3DSFileHeader
    {
        /// <summary>
        /// Magic ID, always 'Z3DS'
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public string Magic { get; set; } = string.Empty;

        /// <summary>
        /// Magic ID of the compressed structure
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public string UnderlyingMagic { get; set; } = string.Empty;

        /// <summary>
        /// Header version
        /// </summary>
        /// <remarks>Currently only a value of 1 is expected</remarks>
        public byte Version { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved { get; set; }

        /// <summary>
        /// Header size
        /// </summary>
        public ushort HeaderSize { get; set; }

        /// <summary>
        /// Metadata size
        /// </summary>
        public uint MetadataSize { get; set; }

        /// <summary>
        /// Compressed size of the structure data
        /// </summary>
        public ulong CompressedSize { get; set; }

        /// <summary>
        /// Uncompressed size of the structure data
        /// </summary>
        public ulong UncompressedSize { get; set; }
    }
}
