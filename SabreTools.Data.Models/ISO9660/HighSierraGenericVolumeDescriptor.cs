namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// High Sierra Generic Volume Descriptor
    /// High Sierra Volume Descriptor with contents not read by this library
    /// </summary>
    /// <see href="https://github.com/torvalds/linux/blob/master/include/uapi/linux/iso_fs.h"/>
    public sealed class HighSierraGenericVolumeDescriptor : HighSierraVolumeDescriptor
    {
        /// <summary>
        /// 2033 bytes
        /// </summary>
        public byte[] Data { get; set; } = new byte[2033];
    }
}
