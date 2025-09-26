using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.VPK
{
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ArchiveMD5SectionEntry
    {
        public uint ArchiveIndex;

        /// <summary>
        /// Where to start reading bytes
        /// </summary>
        public uint StartingOffset;

        /// <summary>
        /// How many bytes to check
        /// </summary>
        public uint Count;

        /// <summary>
        /// Expected checksum
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[]? MD5Checksum = new byte[16];
    }
}