using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DirectoryEntry
    {
        /// <summary>
        /// Offset to the directory item name from the end of the directory items.
        /// </summary>
        public uint NameOffset;

        /// <summary>
        /// Size of the item.  (If file, file size.  If folder, num items.)
        /// </summary>
        public uint ItemSize;

        /// <summary>
        /// Checksome index. (0xFFFFFFFF == None).
        /// </summary>
        public uint ChecksumIndex;

        /// <summary>
        /// Flags for the directory item.  (0x00000000 == Folder).
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public HL_GCF_FLAG DirectoryFlags;

        /// <summary>
        /// Index of the parent directory item.  (0xFFFFFFFF == None).
        /// </summary>
        public uint ParentIndex;

        /// <summary>
        /// Index of the next directory item.  (0x00000000 == None).
        /// </summary>
        public uint NextIndex;

        /// <summary>
        /// Index of the first directory item.  (0x00000000 == None).
        /// </summary>
        public uint FirstIndex;
    }
}
