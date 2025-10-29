namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Supplementary or Enhanced (including Joliet Extension) Volume Descriptor
    /// Volume Descriptor with VolumeDescriptorType = 0x02
    /// Supplementary/Enhanced Volume Descriptors are typically utilised when an alternate character encoding for the identifiers is desired.
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class SupplementaryVolumeDescriptor : BaseVolumeDescriptor
    {
        /// <summary>
        /// Remaining bits are reserved (set to 0)
        /// Note: Joliet Extension implies Constants.JolietVolumeFlags (0x00)
        /// </summary>
        public VolumeFlags VolumeFlags { get; set; }

        /// <summary>
        /// List of escape sequences to use, up to 32 bytes, padded with 0x00 to the right
        /// If all bytes are set to 0x00, then a1-characters are identical to a-characters
        /// Note: Joliet Extension implies Constants.JolietEscapeSequences
        /// </summary>
        public byte[] EscapeSequences { get; set; }
    }
}
