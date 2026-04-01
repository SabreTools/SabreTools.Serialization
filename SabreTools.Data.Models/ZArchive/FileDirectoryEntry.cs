namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Node in the FileTree
    /// Represents either a file or a directory
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public abstract class FileDirectoryEntry
    {
        /// <summary>
        /// MSB is the type flag, 0 is Directory, 1 is File
        /// Remaining 31 bits are the offset in the NameTable
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NameOffsetAndTypeFlag { get; set; }
    }
}
