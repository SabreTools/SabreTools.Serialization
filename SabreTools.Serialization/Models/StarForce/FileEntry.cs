using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.StarForce
{
    /// <see href="https://web.archive.org/web/20231020050651/https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class FileEntry
    {
        /// <summary>
        /// MD5 hash of filename (not encrypted,)
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[]? FilenameMD5Hash;

        /// <summary>
        /// Index of fileheader (encrypted with filename)
        /// </summary>
        public ulong FileHeaderIndex;
    }
}
