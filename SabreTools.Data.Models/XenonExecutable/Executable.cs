namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Xenon (Xbox 360) Executable format (XEX2)
    /// It is based on PE format and is PPC architecutre (therefore Big-Endian)
    /// During alpha stage, Xenon was a slightly modified Apple Power Mac G5
    /// Early (Before March 2005) builds used pure PE-formatted executables, June 2005 XDK began requiring XEX-format
    /// Early (August 2005 and earlier) XEX-format images (XEX0, XEX?, XEX-, XEX%, XEX1) are not supported.
    /// </summary>
    /// <see href="http://oskarsapps.mine.nu/xexdump"/>
    /// <see href="https://free60.org/System-Software/Formats/XEX/"/>
    public class Executable
    {
        /// <summary>
        /// XEX header
        /// </summary>
        public Header Header { get; set; } = new();

        /// <summary>
        /// XEX certificate structure
        /// </summary>
        public Certificate Certificate { get; set; } = new();

        /// <summary>
        /// PE data, too large to be read into memory
        /// Encrypted and/or compressed blob on most XEX files
        /// Occassionally padded with zeroes at the end
        /// </summary>
        public byte[]? CompressedData { get; set; }
    }
}
