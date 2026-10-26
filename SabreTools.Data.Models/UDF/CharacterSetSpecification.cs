namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Character Set Specification (charspec, 1/7.2.1)
    /// UDF requires CS0 with the information "OSTA Compressed Unicode"
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class CharacterSetSpecification
    {
        /// <summary>
        /// Character set type (0 = CS0)
        /// </summary>
        public byte CharacterSetType { get; set; }

        /// <summary>
        /// Character set information, padded with 0x00
        /// </summary>
        public byte[] CharacterSetInformation { get; set; } = new byte[63];
    }
}
