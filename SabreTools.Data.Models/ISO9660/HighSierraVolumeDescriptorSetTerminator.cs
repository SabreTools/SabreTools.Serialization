namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// High Sierra Volume Descriptor Set Terminator
    /// High Sierra Volume Descriptor with VolumeDescriptorType = 0xFF
    /// </summary>
    /// <see href="https://github.com/torvalds/linux/blob/master/include/uapi/linux/iso_fs.h"/>
    public sealed class HighSierraVolumeDescriptorSetTerminator : HighSierraVolumeDescriptor
    {
        /// <summary>
        /// 2033 reserved bytes
        /// </summary>
        public byte[] Reserved2033Bytes { get; set; } = new byte[2033];
    }
}
