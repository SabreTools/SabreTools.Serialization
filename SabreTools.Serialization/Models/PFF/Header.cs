using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.PFF
{
    /// <summary>
    /// PFF archive header
    /// </summary>
    /// <remarks>Versions 2, 3, and 4 supported</remarks>
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Header
    {
        /// <summary>
        /// Size of the following header
        /// </summary>
        public uint HeaderSize;

        /// <summary>
        /// Signature
        /// </summary>
        /// <remarks>Versions 2 and 3 share the same signature but different header sizes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string? Signature;

        /// <summary>
        /// Number of files
        /// </summary>
        public uint NumberOfFiles;

        /// <summary>
        /// File segment size
        /// </summary>
        public uint FileSegmentSize;

        /// <summary>
        /// File list offset
        /// </summary>
        public uint FileListOffset;
    }
}