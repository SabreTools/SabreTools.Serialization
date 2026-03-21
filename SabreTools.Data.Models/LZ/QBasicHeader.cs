using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.LZ
{
    /// <summary>
    /// LZ variant used in QBasic 4.5 installer
    /// </summary>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class QBasicHeader
    {
        /// <summary>
        /// "SZ" signature
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public byte[] Magic = new byte[8];

        /// <summary>
        /// The integer length of the file when unpacked
        /// </summary>
        public uint RealLength;
    }
}
