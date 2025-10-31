using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.WAD3
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Header
    {
        /// <summary>
        /// "WAD3"
        /// </summary>
        /// <remarks>4 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Signature;

        /// <summary>
        /// Number of Directory entries
        /// </summary>
        public uint NumDirs;

        /// <summary>
        /// Offset from the WAD3 data's beginning for first Directory entry
        /// </summary>
        public uint DirOffset;
    }
}
