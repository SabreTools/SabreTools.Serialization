namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Optional header present in STFS packages for installers
    /// Original research, field not mentioned on free60 wiki
    /// </summary>
    public class InstallerHeader
    {
        /// <summary>
        /// String indicating type of installer
        /// See Constants.InstallerType*
        /// </summary>
        /// <remarks>4 bytes, ASCII</remarks>
        public byte[] InstallerType { get; set; } = new byte[4];
    }
}
