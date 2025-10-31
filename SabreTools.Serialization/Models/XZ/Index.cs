namespace SabreTools.Data.Models.XZ
{
    public class Index
    {
        /// <summary>
        /// The value of Index Indicator is always 0x00
        /// </summary>
        public byte IndexIndicator { get; set; }

        /// <summary>
        /// This field indicates how many Records there are in the List
        /// of Records field, and thus how many Blocks there are in the
        /// Stream
        /// </summary>
        /// <remarks>Stored as a variable-length integer</remarks>
        public ulong NumberOfRecords { get; set; }

        /// <summary>
        /// One record per block
        /// </summary>
        public Record[] Records { get; set; }

        /// <summary>
        /// This field MUST contain 0-3 null bytes to pad the Index to
        /// a multiple of four bytes.
        /// </summary>
        public byte[] Padding { get; set; }

        /// <summary>
        /// The CRC32 is calculated over everything in the Index field
        /// except the CRC32 field itself. The CRC32 is stored as an
        /// unsigned 32-bit little endian integer.
        /// </summary>
        public uint Crc32 { get; set; }
    }
}
