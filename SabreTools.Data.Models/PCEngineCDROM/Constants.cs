namespace SabreTools.Data.Models.PCEngineCDROM
{
    /// <summary>
    /// PC Engine CDROM constant values and arrays
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Standard block size for PC Engine CDROM disc images
        /// </summary>
        public static readonly int SectorSize = 2048;

        /// <summary>
        /// Start of a PC Engine CDROM Header
        /// </summary>
        public static readonly byte[] MagicBytes = [0x82, 0xB1, 0x82, 0xCC, 0x83, 0x76, 0x83, 0x8D, 0x83, 0x4F, 0x83, 0x89, 0x83, 0x80, 0x82, 0xCC];

        /// <summary>
        /// Start of an empty CD-ROM pregap sector
        /// </summary>
        public static readonly byte[] PregapBytes = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    }
}
