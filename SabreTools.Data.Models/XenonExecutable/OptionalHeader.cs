namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Xenon (Xbox 360) Executable format optional header
    /// </summary>
    /// <see href="http://oskarsapps.mine.nu/xexdump"/>
    /// <see href="https://free60.org/System-Software/Formats/XEX/"/>
    public class OptionalHeader
    {
        /// <summary>
        /// Header type identifier
        /// Known values are stored in Constants.OptionalHeaderTypes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint HeaderID { get; set; }

        /// <summary>
        /// If lowest byte of HeaderID is 0x01 (and maybe 0x00 ?), then HeaderData is the data itself
        /// If lowest byte of HeaderID is 0xFF, then HeaderData is the data offset
        /// Otherwise, HeaderData is the entry size in DWORDs
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint HeaderData { get; set; }
    }
}
