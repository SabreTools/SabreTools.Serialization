namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Entity Identifier (regid, 1/7.4)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class EntityIdentifier
    {
        /// <summary>
        /// Flags (bit 0: dirty, bit 1: protected)
        /// </summary>
        public byte Flags { get; set; }

        /// <summary>
        /// Identifier, padded with 0x00
        /// </summary>
        public byte[] Identifier { get; set; } = new byte[23];

        /// <summary>
        /// Identifier suffix (UDF domain identifiers: revision as a little-endian ushort, then domain flags)
        /// </summary>
        public byte[] IdentifierSuffix { get; set; } = new byte[8];
    }
}
