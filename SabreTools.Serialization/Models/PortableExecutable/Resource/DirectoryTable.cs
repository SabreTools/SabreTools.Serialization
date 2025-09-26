namespace SabreTools.Data.Models.PortableExecutable.Resource
{
    /// <summary>
    /// Each directory table is followed by a series of directory entries that
    /// give the name or identifier (ID) for that level (Type, Name, or Language
    /// level) and an address of either a data description or another directory
    /// table. If the address points to a data description, then the data is a
    /// leaf in the tree. If the address points to another directory table,
    /// then that table lists directory entries at the next level down.
    /// 
    /// Each resource directory table has the following format. This data
    /// structure should be considered the heading of a table because the table
    /// actually consists of directory entries.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class DirectoryTable
    {
        /// <summary>
        /// Resource flags. This field is reserved for future use. It is currently
        /// set to zero.
        /// </summary>
        public uint Characteristics { get; set; }

        /// <summary>
        /// The time that the resource data was created by the resource compiler.
        /// </summary>
        public uint TimeDateStamp { get; set; }

        /// <summary>
        /// The major version number, set by the user.
        /// </summary>
        public ushort MajorVersion { get; set; }

        /// <summary>
        /// The minor version number, set by the user.
        /// </summary>
        public ushort MinorVersion { get; set; }

        /// <summary>
        /// The number of directory entries immediately following the table that use
        /// strings to identify Type, Name, or Language entries (depending on the
        /// level of the table).
        /// </summary>
        public ushort NumberOfNameEntries { get; set; }

        /// <summary>
        /// The number of directory entries immediately following the Name entries that
        /// use numeric IDs for Type, Name, or Language entries.
        /// </summary>
        public ushort NumberOfIDEntries { get; set; }

        /// <summary>
        /// Directory entries immediately following the table that use
        /// strings to identify Type, Name, or Language entries (depending on the
        /// level of the table).
        /// </summary>
        public DirectoryEntry[]? Entries { get; set; }
    }
}
