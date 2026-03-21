using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.LZ
{
    /// <summary>
    /// LZ variant with variable compression
    /// </summary>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class KWAJHeader
    {
        /// <summary>
        /// "KWAJ" signature
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public byte[] Magic = new byte[8];

        /// <summary>
        /// Compression method
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public KWAJCompressionType CompressionType;

        /// <summary>
        /// File offset of compressed data
        /// </summary>
        public ushort DataOffset;

        /// <summary>
        /// Header flags to mark header extensions
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public KWAJHeaderFlags HeaderFlags;
    }
}
