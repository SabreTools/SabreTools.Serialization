namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Supplementary or Enhanced (including Joliet Extension) Volume Descriptor
    /// Volume Descriptor with VolumeDescriptorType = 0x02
    /// Supplementary/Enhanced Volume Descriptors are typically utilised when an alternate character encoding for the identifiers is desired.
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class SupplementaryVolumeDescriptor : BaseVolumeDescriptor
    {
        /// <summary>
        /// Flag 1 (Bit 0, LSB):
        ///     If 1, then EscapeSequences has at least 1 unregistered escape sequence
        ///     If 0, then all escape sequences in EscapeSequences are registered
        /// Remaining bits are reserved (set to 0)
        /// Note: Joliet Extension implies Constants.JolietVolumeFlags (0x00)
        /// </summary>
        public byte VolumeFlags { get; set; }

        /// <summary>
        /// List of escape sequences to use, up to 32 bytes, padded with 0x00 to the right
        /// If all bytes are set to 0x00, then a1-characters are identical to a-characters
        /// Note: Joliet Extension implies Constants.JolietEscapeSequences
        /// </summary>
        public byte[] EscapeSequences { get; set; }
    }
}
