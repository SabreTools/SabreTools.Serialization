namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Node in the FileTree representing a file
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public sealed class FileEntry : FileDirectoryEntry
    {
        /// <summary>
        /// Lowest 8 bits of the file's offset
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint FileOffsetLow { get; set; }

        /// <summary>
        /// Lowest 8 bits of the file's size
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint FileSizeLow { get; set; }

        /// <summary>
        /// Highest 4 bits of the file's size
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ushort FileSizeHigh { get; set; }

        /// <summary>
        /// Highest 4 bits of the file's offset
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ushort FileOffsetHigh { get; set; }
    }
}
