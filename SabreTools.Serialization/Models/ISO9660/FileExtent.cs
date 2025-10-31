namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 File Extent, the file data itself
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public class FileExtent
    {
        /// <summary>
        /// File's extended attribute record
        /// Optional field, and never present for Directory-type File Extents
        /// </summary>
        public ExtendedAttributeRecord? ExtendedAttributeRecord { get; set; }

        /// <summary>
        /// Byte array of data within the file extent (after the Extended Attribyte Record)
        /// </summary>
        public byte[] Data { get; set; }
    }
}
