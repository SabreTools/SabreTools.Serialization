namespace SabreTools.Data.Models.XDVDFS
{
    /// <see href="https://github.dev/Deterous/XboxKit/"/>
    public static class Constants
    {
        /// <summary>
        /// Number of bytes in a sector
        /// </summary>
        public const int SectorSize = 2048;

        /// <summary>
        /// Number of sectors reserved at beginning of volume
        /// </summary>
        public const int ReservedSectors = 32;

        /// <summary>
        /// Minimum length of a directory record
        /// </summary>
        public const int MinimumRecordLength = 14;

        /// <summary>
        /// Volume Descriptor magic bytes at start of sector 32
        /// </summary>
        public static readonly byte[] VolumeDescriptorMagic = [0x4D, 0x49, 0x43, 0x52, 0x4F, 0x53, 0x4F, 0x46, 0x54, 0x2A, 0x58, 0x42, 0x4F, 0x58, 0x2A, 0x4D, 0x45, 0x44, 0x49, 0x41];

        /// <summary>
        /// Volume Descriptor signature at start of sector 32
        /// </summary>
        public const string VolumeDescriptorSignature = "MICROSOFT*XBOX*MEDIA";

        /// <summary>
        /// Xbox DVD Layout Descriptor signature at start of sector 33
        /// </summary>
        public const string LayoutDescriptorSignature = "XBOX_DVD_LAYOUT_TOOL_SIG";
    }
}
