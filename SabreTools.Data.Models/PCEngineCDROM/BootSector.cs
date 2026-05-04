namespace SabreTools.Data.Models.PCEngineCDROM
{
    /// <summary>
    /// Standard format for the first sector of the CD-ROM header
    /// This is likely read by the system to verify the disc's authenticity
    /// </summary>
    public sealed class BootSector
    {
        /// <summary>
        /// Copyright notice and credits
        /// </summary>
        /// <remarks>Shift-JIS format, lines separated by NULL character, ends in NULL</remarks>
        public byte[] CopyrightString { get; set; } = new byte[806];

        /// <summary>
        /// HuC6280 machine code, presumably for initialization
        /// Note that the actual Boot ROM is on the System Card
        /// </summary>
        public byte[] BootROM { get; set; } = new byte[432];

        /// <summary>
        /// Reserved zeroed padding bytes to fill remainder of sector
        /// </summary>
        public byte[] Padding { get; set; } = new byte[810];
    }
}
