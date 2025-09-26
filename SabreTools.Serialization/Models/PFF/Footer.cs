using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PFF
{
    /// <summary>
    /// PFF file footer
    /// </summary>
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Footer
    {
        /// <summary>
        /// Current system IP
        /// </summary>
        public uint SystemIP;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved;

        /// <summary>
        /// King tag
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string? KingTag;
    }
}