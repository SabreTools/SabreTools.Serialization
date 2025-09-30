namespace SabreTools.Data.Models.XZ
{
    /// <summary>
    /// Represents the pre-block data in the stream
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Header magic number (0xFD, '7', 'z', 'X', 'Z', 0x00)
        /// </summary>
        /// <remarks>6 bytes</remarks>
        public byte[]? Signature { get; set; }

        /// <summary>
        /// The first byte of Stream Flags is always a null byte. In the
        /// future, this byte may be used to indicate a new Stream version
        /// or other Stream properties.
        /// </summary>
        public HeaderFlags Flags { get; set; }

        /// <summary>
        /// The CRC32 is calculated from the Stream Flags field. It is
        /// stored as an unsigned 32-bit little endian integer.
        /// </summary>
        public uint Crc32 { get; set; }
    }
}
