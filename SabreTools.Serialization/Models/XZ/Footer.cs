namespace SabreTools.Data.Models.XZ
{
    /// <summary>
    /// Represents the post-block data in the stream
    /// </summary>
    public class Footer
    {
        /// <summary>
        /// The CRC32 is calculated from the Backward Size and Stream Flags
        /// fields. It is stored as an unsigned 32-bit little endian
        /// integer.
        /// </summary>
        public uint Crc32 { get; set; }

        /// <summary>
        /// Backward Size is stored as a 32-bit little endian integer,
        /// which indicates the size of the Index field as multiple of
        /// four bytes, minimum value being four bytes.
        /// </summary>
        /// <remarks>
        /// The real index size can be calculated by the following:
        /// (BackwardSize + 1) * 4
        /// </remarks>
        public uint BackwardSize { get; set; }

        /// <summary>
        /// This is a copy of the Stream Flags field from the Stream
        /// Header. The information stored to Stream Flags is needed
        /// when parsing the Stream backwards.
        /// </summary>
        public HeaderFlags Flags { get; set; }

        /// <summary>
        /// Header magic number ("YZ")
        /// </summary>
        /// <remarks>2 bytes</remarks>
        public byte[] Signature { get; set; } = new byte[2];
    }
}
