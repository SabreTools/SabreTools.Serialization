namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory Record
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DirectoryRecord
    {
        /// <summary>
        /// Length of Directory Record
        /// </summary>
        public byte DirectoryRecordLength { get; set; }

        /// <summary>
        /// Length of the extended attribute record
        /// If no extended attribyute record is used, set to 0x00
        /// </summary>
        public byte ExtendedAttributeRecordLength { get; set; }

        /// <summary>
        /// Logical block number of the first logical block allocated to this extent
        /// </summary>
        public int ExtentLocation { get; set; }

        /// <summary>
        /// Number of logical blocks allocated to this extent
        /// </summary>
        public int ExtentLength { get; set; }
    }
}
