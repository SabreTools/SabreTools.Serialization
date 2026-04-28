namespace SabreTools.Data.Models.PCEngineCDROM
{
    /// <summary>
    /// This is the standard header of PC Engine CD-ROM
    /// Located at the first non-zero user data sector of the first data track
    /// </summary>
    public sealed class Header
    {
        /// <summary>
        /// Standard sector of data to verify the disc
        /// </summary>
        public BootSector BootSector { get; set; } = new();

        /// <summary>
        /// Initial Program Loader for the PC Engine
        /// </summary>
        public IPL IPL { get; set; } = new();
    }
}
