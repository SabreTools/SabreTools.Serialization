namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class Directory
    {
        /// <summary>
        /// Associated directory records (each pointing to a directory or a file extents)
        /// </summary>
        public DirectoryRecord[]? DirectoryRecords { get; set; }
    }
}
