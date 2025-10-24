namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory extent, containing only directory records
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DirectoryExtent
    {
        /// <summary>
        /// Associated directory records within a directory extent
        /// </summary>
        public DirectoryRecord[]? DirectoryRecords { get; set; }

        /// <summary>
        /// Zero or more file extents pointed to by the directory records in DirectoryRecords
        /// Optional field
        /// </summary>
        public FileExtent[]? FileExtents { get; set; }
    }
}
