using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class HeaderV4 : Header
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
        /// Number of bytes per hunk
        /// </summary>
        public uint HunkBytes;

        /// <summary>
        /// Combined raw+meta SHA1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[]? SHA1 = new byte[20];

        /// <summary>
        /// Combined raw+meta SHA1 of parent
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[]? ParentSHA1 = new byte[20];

        /// <summary>
        /// Raw data SHA1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[]? RawSHA1 = new byte[20];
    }
}
