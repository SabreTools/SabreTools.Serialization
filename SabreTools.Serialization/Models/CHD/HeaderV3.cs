using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class HeaderV3 : Header
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
        /// Total # of hunks represented
        /// </summary>
        public uint TotalHunks;

        /// <summary>
        /// Logical size of the data (in bytes)
        /// </summary>
        public ulong LogicalBytes;

        /// <summary>
        /// Offset to the first blob of metadata
        /// </summary>
        public ulong MetaOffset;

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
        /// Number of bytes per hunk
        /// </summary>
        public uint HunkBytes;

        /// <summary>
        /// SHA1 checksum of raw data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] SHA1 = new byte[20];

        /// <summary>
        /// SHA1 checksum of parent file
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ParentSHA1 = new byte[20];
    }
}
