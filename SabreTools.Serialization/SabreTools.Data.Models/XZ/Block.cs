namespace SabreTools.Data.Models.XZ
{
    /// <summary>
    /// Represents a single compressed block in the stream
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Size of the header
        /// </summary>
        /// <remarks>
        /// The real header size can be calculated by the following:
        /// (HeaderSize + 1) * 4
        /// </remarks>
        public byte HeaderSize { get; set; }

        /// <summary>
        /// The Block Flags field is a bit field
        /// </summary>
        public BlockFlags Flags { get; set; }

        /// <summary>
        /// Size of the compressed data
        /// Present if <see cref="BlockFlags.CompressedSize"/> is set.
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong CompressedSize { get; set; }

        /// <summary>
        /// Size of the block after decompression
        /// Present if <see cref="BlockFlags.UncompressedSize"/> is set.
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong UncompressedSize { get; set; }

        /// <summary>
        /// List of filter flags
        /// </summary>
        /// <remarks>
        /// The number of filter flags is given by the first two
        /// bits of <see cref="Flags"/>
        /// </remarks>
        public FilterFlag[] FilterFlags { get; set; } = [];

        /// <summary>
        /// This field contains as many null byte as it is needed to make
        /// the Block Header have the size specified in Block Header Size.
        /// </summary>
        public byte[] HeaderPadding { get; set; } = [];

        /// <summary>
        /// The CRC32 is calculated over everything in the Block Header
        /// field except the CRC32 field itself. It is stored as an
        /// unsigned 32-bit little endian integer.
        /// </summary>
        public uint Crc32 { get; set; }

        /// <summary>
        /// The format of Compressed Data depends on Block Flags and List
        /// of Filter Flags
        /// </summary>
        public byte[] CompressedData { get; set; } = [];

        /// <summary>
        /// Block Padding MUST contain 0-3 null bytes to make the size of
        /// the Block a multiple of four bytes. This can be needed when
        /// the size of Compressed Data is not a multiple of four.
        /// </summary>
        public byte[] BlockPadding { get; set; } = [];

        /// <summary>
        /// The type and size of the Check field depends on which bits
        /// are set in the Stream Flags field.
        ///
        /// The Check, when used, is calculated from the original
        /// uncompressed data.
        /// </summary>
        public byte[] Check { get; set; } = [];
    }
}
