using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.WAD3
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/WADFile.h"/>
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class DirEntry
    {
        /// <summary>
        /// Offset from the beginning of the WAD3 data
        /// </summary>
        public uint Offset;

        /// <summary>
        /// The entry's size in the archive in bytes
        /// </summary>
        public uint DiskLength;

        /// <summary>
        /// The entry's uncompressed size
        /// </summary>
        public uint Length;

        /// <summary>
        /// File type of the entry
        /// </summary>
        public FileType Type;

        /// <summary>
        /// Whether the file was compressed
        /// </summary>
        /// <remarks>Actually a boolean value</remarks>
        public byte Compression;

        /// <summary>
        /// Padding
        /// </summary>
        public ushort Padding;

        /// <summary>
        /// Null-terminated texture name
        /// </summary>
        /// <remarks>16 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string? Name;
    }
}
