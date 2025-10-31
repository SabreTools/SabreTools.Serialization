namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Boot Record Volume Descriptor
    /// Volume Descriptor with VolumeDescriptorType = 0x00
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class BootRecordVolumeDescriptor : VolumeDescriptor
    {
        /// <summary>
        /// 32-byte name of the intended system that can use this record
        /// a-characters only
        /// </summary>
        public byte[] BootSystemIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// 32-byte name of this boot system
        /// a-characters only
        /// </summary>
        public byte[] BootIdentifier { get; set; } = new byte[32];

        /// <summary>
        /// 1997 bytes for Boot System Use, contents not defined by ISO9660
        /// </summary>
        public byte[] BootSystemUse { get; set; } = new byte[1997];
    }
}
