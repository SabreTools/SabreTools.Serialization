namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Generic Volume Descriptor
    /// Volume Descriptor with contents not defined by ISO9660
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class GenericVolumeDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// 2041 bytes
        /// </summary>
        public byte[] Data { get; set; } = new byte[2041];
    }
}
