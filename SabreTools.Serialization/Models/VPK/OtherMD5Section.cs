using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.VPK
{
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class OtherMD5Section
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[]? TreeChecksum = new byte[16];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[]? ArchiveMD5SectionChecksum = new byte[16];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[]? WholeFileChecksum = new byte[16];
    }
}