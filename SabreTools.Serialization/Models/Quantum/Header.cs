using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.Quantum
{
    /// <summary>
    /// Quantum archive file header
    /// </summary>
    /// <see href="https://handwiki.org/wiki/Software:Quantum_compression"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class Header
    {
        /// <summary>
        /// Quantum signature: 0x44 0x53
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string Signature;

        /// <summary>
        /// Quantum major version number
        /// </summary>
        public byte MajorVersion;

        /// <summary>
        /// Quantum minor version number
        /// </summary>
        public byte MinorVersion;

        /// <summary>
        /// Number of files within this archive
        /// </summary>
        public ushort FileCount;

        /// <summary>
        /// Table size required for decompression
        /// </summary>
        public byte TableSize;

        /// <summary>
        /// Compression flags
        /// </summary>
        public byte CompressionFlags;
    }
}
