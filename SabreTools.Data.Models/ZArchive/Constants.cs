namespace SabreTools.Data.Models.ZArchive
{
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public static class Constants
    {
        /// <summary>
        /// Number of compressed blocks referred to by a record
        /// </summary>
        public const int BlockSize = 64 * 1024;

        /// <summary>
        /// Number of compressed blocks referred to by a record
        /// </summary>
        public const int BlocksPerOffsetRecord = 16;

        /// <summary>
        /// Number of bytes stored in an offset record
        /// </summary>
        public const int OffsetRecordSize = sizeof(ulong) + (sizeof(ushort) * BlocksPerOffsetRecord);

        /// <summary>
        /// Number of bytes stored in a file/directory entry
        /// </summary>
        public const int FileDirectoryEntrySize = 16;

        /// <summary>
        /// Number of bytes stored in the footer
        /// 6 OffsetInfo fields,
        /// </summary>
        public const int FooterSize = 144;

        /// <summary>
        /// NameOffsetAndTypeFlag value for the root node in the FileTree
        /// </summary>
        public const uint RootNode = 0x7FFFFFFF;

        /// <summary>
        /// Mask for the NameOffsetAndTypeFlag value when checking if it is a file
        /// </summary>
        public const uint FileFlag = 0x80000000;

        /// <summary>
        /// Maximum size of the Offset Records section
        /// </summary>
        public const ulong MaxOffsetRecordsSize = 0xFFFFFFFF;

        /// <summary>
        /// Maximum size of the Offset Records section
        /// </summary>
        public const ulong MaxNameTableSize = 0x7FFFFFFF;

        /// <summary>
        /// Maximum size of the File Tree section
        /// </summary>
        public const ulong MaxFileTreeSize = 0x7FFFFFFF;

        /// <summary>
        /// ZArchive magic bytes at end of file
        /// </summary>
        public static readonly byte[] MagicBytes = [0x16, 0x9F, 0x52, 0xD6];

        /// <summary>
        /// ZArchive version field that acts as an extended magic immediately before final 4 magic bytes
        /// Currently only version 1 is implemented, any future version bytes are not suppported yet
        /// </summary>
        public static readonly byte[] Version1Bytes = [0x61, 0xBF, 0x3A, 0x01];
    }
}
