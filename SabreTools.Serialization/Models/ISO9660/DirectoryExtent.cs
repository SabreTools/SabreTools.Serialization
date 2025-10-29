namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory Extent containing file and directory descriptors parsed from the file extent into directory records
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DirectoryExtent : FileExtent
    {
        /// <summary>
        /// Directory records (each a descriptor of a directory or a file)
        /// </summary>
        public DirectoryRecord[] DirectoryRecords { get; set; }
    }
}
