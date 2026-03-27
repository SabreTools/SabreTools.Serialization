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
        /// Volume Descriptor signature at start of sector 32
        /// </summary>
        public static readonly byte[] VOLUME_DESCRIPTOR_SIG = [.."MICROSOFT*XBOX*MEDIA"u8];

        /// <summary>
        /// Xbox DVD Layout Descriptor signature at start of sector 33
        /// </summary>
        public static readonly byte[] LAYOUT_DESCRIPTOR_SIG = [.."XBOX_DVD_LAYOUT_TOOL_SIG"u8];
    }
}
