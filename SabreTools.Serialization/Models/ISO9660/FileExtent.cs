namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 File extent, the file data itself
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class FileExtent
    {
        /// <summary>
        /// Byte array of data within the file extent
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// File's extended attribute record
        /// Optional field
        /// </summary>
        public ExtendedAttributeRecord ExtendedAttributeRecord { get; set; }
    }
}
