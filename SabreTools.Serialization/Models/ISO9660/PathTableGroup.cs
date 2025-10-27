namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Path Table Group, lists of path table records for each directory on the volume
    /// Each path table is intended to point to the same set of directories. All non-null path tables should be identical!
    /// For each directory on the filesystem (except root), the Path Table contains a record which identifies the directory, its parent, and its location.
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class PathTableGroup
    {
        /// <summary>
        /// Type-L Path Table (Little Endian)
        /// </summary>
        public PathTableRecord[]? PathTableL { get; set; }

        /// <summary>
        /// Optional Type-L Path Table (Little Endian)
        /// </summary>
        public PathTableRecord[]? OptionalPathTableL { get; set; }

        /// <summary>
        /// Type-M Path Table (Big Endian)
        /// </summary>
        public PathTableRecord[]? PathTableM { get; set; }

        /// <summary>
        /// Optional Type-M Path Table (Big Endian)
        /// </summary>
        public PathTableRecord[]? OptionalPathTableM { get; set; }
    }
}
