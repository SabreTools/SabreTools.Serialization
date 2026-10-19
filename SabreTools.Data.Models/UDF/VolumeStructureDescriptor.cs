namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Volume Structure Descriptor (2/9.1), one logical sector of the Volume Recognition Sequence
    /// starting at sector 16, e.g. "BEA01", "NSR02"/"NSR03", "TEA01", or "CD001" on bridge discs
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class VolumeStructureDescriptor
    {
        /// <summary>
        /// Structure type
        /// </summary>
        public byte StructureType { get; set; }

        /// <summary>
        /// Standard identifier, 5 bytes
        /// </summary>
        public byte[] StandardIdentifier { get; set; } = new byte[5];

        /// <summary>
        /// Structure version
        /// </summary>
        public byte StructureVersion { get; set; }

        /// <summary>
        /// Structure data, the rest of the logical sector
        /// </summary>
        public byte[] StructureData { get; set; } = [];
    }
}
