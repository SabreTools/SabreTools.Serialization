using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PlayStation3
{
    /// <see href="https://psdevwiki.com/ps3/PS3_DISC.SFB"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class SFB
    {
        /// <summary>
        /// ".SFB"
        /// </summary>
        public uint Magic;

        /// <summary>
        /// File version(?)
        /// </summary>
        public uint FileVersion;

        /// <summary>
        /// Unknown (zeroes)
        /// </summary>
        /// <remarks>0x18 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
        public byte[] Reserved1 = new byte[0x18];

        /// <summary>
        /// "HYBRID_FLAG" (Flags type)
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
        public string? FlagsType;

        /// <summary>
        /// Disc Content Data Offset
        /// </summary>
        public uint DiscContentDataOffset;

        /// <summary>
        /// Disc Content Data Length
        /// </summary>
        public uint DiscContentDataLength;

        /// <summary>
        /// Unknown (zeroes)
        /// </summary>
        /// <remarks>0x08 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
        public byte[] Reserved2 = new byte[0x08];

        /// <summary>
        /// "TITLE_ID" (Disc Title Name)
        /// </summary>
        /// <remarks>0x08 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x08)]
        public string? DiscTitleName;

        /// <summary>
        /// Unknown (zeroes)
        /// </summary>
        /// <remarks>0x08 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
        public byte[] Reserved3 = new byte[0x08];

        /// <summary>
        /// Disc Version Data Offset
        /// </summary>
        public uint DiscVersionDataOffset;

        /// <summary>
        /// Disc Version Data Length
        /// </summary>
        public uint DiscVersionDataLength;

        /// <summary>
        /// Unknown (zeroes)
        /// </summary>
        /// <remarks>0x188 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x188)]
        public byte[] Reserved4 = new byte[0x188];

        /// <summary>
        /// Disc Content (Hybrid Flags)
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
        public string? DiscContent;

        /// <summary>
        /// Disc Title
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
        public string? DiscTitle;

        /// <summary>
        /// Disc Version
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
        public string? DiscVersion;

        /// <summary>
        /// Unknown (zeroes)
        /// </summary>
        /// <remarks>0x3C0 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3C0)]
        public byte[] Reserved5 = new byte[0x3C0];
    }
}
