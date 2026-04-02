namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Node in the FileTree representing a directory
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public sealed class DirectoryEntry : FileDirectoryEntry
    {
        /// <summary>
        /// Starting index of the directory node
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NodeStartIndex { get; set; }

        /// <summary>
        /// Number of 
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Count { get; set; }

        /// <summary>
        /// Reserved field
        /// </summary>
        public uint Reserved { get; set; }
    }
}
