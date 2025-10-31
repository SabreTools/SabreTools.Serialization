using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PAK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/PAKFile.h"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Header
    {
        /// <summary>
        /// Signature
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string Signature;

        /// <summary>
        /// Directory Offset
        /// </summary>
        public uint DirectoryOffset;

        /// <summary>
        /// Directory Length
        /// </summary>
        public uint DirectoryLength;
    }
}
