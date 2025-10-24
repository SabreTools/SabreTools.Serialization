namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Primary Volume Descriptor
    /// Volume Descriptor with VolumeDescriptorType = 0x01
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class PrimaryVolumeDescriptor : BaseVolumeDescriptor
    {
        /// <summary>
        /// 1 unused byte at offset 7, should be 0x00
        /// Note: This is used for VolumeFlags on SupplementaryVolumeDescriptor
        /// </summary>
        public byte UnusedByte { get; set; }

        /// <summary>
        /// 32 unused bytes at offset 88, should be all 0x00
        /// Note: These is used for EscapeSequences on SupplementaryVolumeDescriptor
        /// </summary>
        public byte[]? Unused32Bytes { get; set; }
    }
}
