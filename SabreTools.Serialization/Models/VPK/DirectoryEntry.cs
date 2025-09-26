using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryEntry
    {
        /// <summary>
        /// A 32bit CRC of the file's data.
        /// </summary>
        public uint CRC;

        /// <summary>
        /// The number of bytes contained in the index file.
        /// </summary>
        public ushort PreloadBytes;

        /// <summary>
        /// A zero based index of the archive this file's data is contained in.
        /// If 0x7fff, the data follows the directory.
        /// </summary>
        public ushort ArchiveIndex;

        /// <summary>
        /// If ArchiveIndex is 0x7fff, the offset of the file data relative to the
        /// end of the directory (see the header for more details).
        /// Otherwise, the offset of the data from the start of the specified archive.
        /// </summary>
        public uint EntryOffset;

        /// <summary>
        /// If zero, the entire file is stored in the preload data.
        /// Otherwise, the number of bytes stored starting at EntryOffset.
        /// </summary>
        public uint EntryLength;

        /// <summary>
        /// Always 0xffff.
        /// </summary>
        public ushort Dummy0;
    }
}
