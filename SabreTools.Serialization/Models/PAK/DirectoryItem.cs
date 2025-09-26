using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PAK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/PAKFile.h"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class DirectoryItem
    {
        /// <summary>
        /// Item Name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 56)]
        public string? ItemName;

        /// <summary>
        /// Item Offset
        /// </summary>
        public uint ItemOffset;

        /// <summary>
        /// Item Length
        /// </summary>
        public uint ItemLength;
    }
}
