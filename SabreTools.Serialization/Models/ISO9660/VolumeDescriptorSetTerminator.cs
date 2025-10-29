namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Volume Descriptor Set Terminator
    /// Blank Descriptor with VolumeDescriptorType = 0xFF
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class VolumeDescriptorSetTerminator : VolumeDescriptor
    {
        /// <summary>
        /// 2041 reserved bytes, should be 0x00
        /// </summary>
        public byte[] Reserved2041Bytes { get; set; }
    }
}
