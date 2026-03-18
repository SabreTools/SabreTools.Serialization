using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class HeaderV2 : Header
    {
        /// <summary>
        /// Flags
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public Flags Flags;

        /// <summary>
        /// Compression type
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public CompressionType Compression;

        /// <summary>
        /// Seclen-byte sectors per hunk
        /// </summary>
        public uint HunkSize;

        /// <summary>
        /// Total # of hunks represented
        /// </summary>
        public uint TotalHunks;

        /// <summary>
        /// Number of cylinders on hard disk
        /// </summary>
        public uint Cylinders;

        /// <summary>
        /// Number of heads on hard disk
        /// </summary>
        public uint Heads;

        /// <summary>
        /// Number of sectors on hard disk
        /// </summary>
        public uint Sectors;

        /// <summary>
        /// MD5 checksum of raw data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] MD5 = new byte[16];

        /// <summary>
        /// MD5 checksum of parent file
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] ParentMD5 = new byte[16];

        /// <summary>
        /// Number of bytes per sector
        /// </summary>
        public uint BytesPerSector;
    }
}
