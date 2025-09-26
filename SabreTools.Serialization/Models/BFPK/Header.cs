using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BFPK
{
    /// <summary>
    /// Header
    /// </summary>
    /// <see cref="https://forum.xentax.com/viewtopic.php?t=5102"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Header
    {
        /// <summary>
        /// "BFPK"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string? Magic;

        /// <summary>
        /// Version
        /// </summary>
        public int Version;

        /// <summary>
        /// Files
        /// </summary>
        public int Files;
    }
}
