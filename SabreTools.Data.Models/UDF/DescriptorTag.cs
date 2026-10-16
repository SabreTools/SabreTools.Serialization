namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Descriptor Tag (3/7.2), the first 16 bytes of every tagged descriptor
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class DescriptorTag
    {
        /// <summary>
        /// Type of the descriptor
        /// </summary>
        public TagIdentifier TagIdentifier { get; set; }

        /// <summary>
        /// Version of the descriptor, 2 (ECMA-167 2nd edition) or 3 (3rd edition)
        /// </summary>
        public ushort DescriptorVersion { get; set; }

        /// <summary>
        /// Sum modulo 256 of bytes 0-3 and 5-15 of the tag
        /// </summary>
        public byte TagChecksum { get; set; }

        /// <summary>
        /// Reserved, 0x00
        /// </summary>
        public byte Reserved { get; set; }

        /// <summary>
        /// Identification of a set of descriptors
        /// </summary>
        public ushort TagSerialNumber { get; set; }

        /// <summary>
        /// CRC-CCITT of the DescriptorCRCLength bytes following the tag
        /// </summary>
        public ushort DescriptorCRC { get; set; }

        /// <summary>
        /// Number of bytes after the tag covered by the DescriptorCRC
        /// </summary>
        public ushort DescriptorCRCLength { get; set; }

        /// <summary>
        /// Number of the logical sector (volume structures) or logical block (file structures)
        /// in which the descriptor is recorded
        /// </summary>
        public uint TagLocation { get; set; }
    }
}
