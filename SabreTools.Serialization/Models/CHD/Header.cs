using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public abstract class Header
    {
        /// <summary>
        /// 'MComprHD'
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? Tag;

        /// <summary>
        /// Length of header (including tag and length fields)
        /// </summary>
        public uint Length;

        /// <summary>
        /// Drive format version
        /// </summary>
        public uint Version;
    }
}