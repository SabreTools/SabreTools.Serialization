using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.LZ
{
    /// <summary>
    /// Standard LZ variant
    /// </summary>
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/kernel32/lzexpand.c"/>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class SZDDHeader
    {
        /// <summary>
        /// "SZDD" signature
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public byte[]? Magic;

        /// <summary>
        /// Compression mode
        /// </summary>
        /// <remarks>Only <see cref="ExpandCompressionType.A"/> is supported</remarks> 
        [MarshalAs(UnmanagedType.U1)]
        public ExpandCompressionType CompressionType;

        /// <summary>
        /// The character missing from the end of the filename
        /// </summary>
        /// <remarks>0 means unknown</remarks>
        [MarshalAs(UnmanagedType.U1)]
        public char LastChar;

        /// <summary>
        /// The integer length of the file when unpacked
        /// </summary>
        public uint RealLength;
    }
}