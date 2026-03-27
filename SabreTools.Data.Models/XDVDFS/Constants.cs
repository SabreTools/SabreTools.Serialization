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
        public static readonly byte[] VolumeDescriptorSignature = System.Text.Encoding.ASCII.GetString("MICROSOFT*XBOX*MEDIA");

        /// <summary>
        /// Xbox DVD Layout Descriptor signature at start of sector 33
        /// </summary>
        public static readonly byte[] LayoutDescriptorSignature = System.Text.Encoding.ASCII.GetString("XBOX_DVD_LAYOUT_TOOL_SIG");
    }
}
