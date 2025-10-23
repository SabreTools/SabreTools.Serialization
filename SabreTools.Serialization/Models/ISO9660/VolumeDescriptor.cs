namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Abstract ISO9660 Volume Descriptor
    /// Each VolumeDescriptor consists of 1 logical sector (usually 2048 bytes)
    /// The first 7 bytes are a fixed header, the remaining bytes are application specific
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public abstract class VolumeDescriptor
    {
        /// <summary>
        /// The type of VolumeDescriptor
        /// 0 = Boot Record Volume Descriptor
        /// 1 = Primary Volume Descriptor
        /// 2 = Supplementary/Enhanced Volume Descriptor (including Joliet extension)
        /// 3 = Volume Partition Descriptor
        /// 255 = Volume Descriptor Set Terminator
        /// Values 4-254 are reserved
        /// </summary>
        public byte VolumeDescriptorType { get; set; }

        /// <summary>
        /// 5-byte magic
        /// Set to Constants.StandardIdentifier ("CD001")
        /// On non-ISO9660 CD-i discs, set to Constants.StandardIdentifierCDI ("CD-I ")
        /// </summary>
        public byte[] VolumeDescriptorIdentifier { get; set; }

        /// <summary>
        /// The Volume Descriptor version number, fixed at Constants.VolumeDescriptorVersion (0x01)
        /// </summary>
        public byte VolumeDescriptorVersion { get; set; }
    }
}
