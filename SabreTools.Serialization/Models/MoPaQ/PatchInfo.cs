using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.MoPaQ
{
    /// <summary>
    /// This structure contains size of the patch, flags and also MD5 of the patch.
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class PatchInfo
    {
        /// <summary>
        /// Length of patch info header, in bytes
        /// </summary>
        public uint Length;

        /// <summary>
        /// Flags. 0x80000000 = MD5 (?)
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Uncompressed size of the patch file
        /// </summary>
        public uint DataSize;

        /// <summary>
        /// MD5 of the entire patch file after decompression
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] MD5 = new byte[0x10];

        /// <summary>
        /// The sector offset table (variable length)
        /// </summary>
        /// <remarks>0 entries</remarks>
        /// TODO: Determine the number of entries
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0)]
        public uint[] SectorOffsetTable = [];
    }
}
