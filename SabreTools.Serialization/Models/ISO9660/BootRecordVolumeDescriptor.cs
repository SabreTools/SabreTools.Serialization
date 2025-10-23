namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Boot Record Volume Descriptor
    /// Volume Descriptor with VolumeDescriptorType = 0x00
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class BootRecordVolumeDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// 32-byte name of the intended system that can use this record
        /// a-characters only, padded to the right with spaces
        /// </summary>
        public byte[] BootSystemIdentifier { get; set; }

        /// <summary>
        /// 32-byte name of this boot system
        /// a-characters only, padded to the right with spaces
        /// </summary>
        public byte[] BootIdentifier { get; set; }

        /// <summary>
        /// 1997 bytes for Boot System Use, contents not defined by ISO9660
        /// </summary>
        public byte[] BootSystemUse { get; set; }
    }
}
