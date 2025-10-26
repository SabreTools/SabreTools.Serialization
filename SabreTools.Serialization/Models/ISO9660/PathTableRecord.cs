namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Path Table Record
    /// Each path table record is numbered (starting from 1), which corresponds to the ordinal number of the corresponding directory
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class PathTableRecord
    {
        /// <summary>
        /// Length of Directory Identifier
        /// </summary>
        public byte DirectoryIdentifierLength { get; set; }

        /// <summary>
        /// Length of the the extended attribute record
        /// </summary>
        public byte ExtendedAttributeRecordLength { get; set; }

        /// <summary>
        /// Location of the first logical block number of the first logical block allocated to the extent
        /// </summary>
        public int ExtentLocation { get; set; }

        /// <summary>
        /// Location of the first logical block number of the first logical block allocated to the extent
        /// </summary>
        public short ParentDirectoryNumber { get; set; }

        /// <summary>
        /// Directory name
        /// Either d-characters or d1-characters, or a single 0x00 byte
        /// </summary>
        public byte DirectoryIdentifier { get; set; }

        /// <summary>
        /// If DirectoryIdentifierLength is odd, the DirectoryIdentifier is followed by a single padding byte (0x00)
        /// Optional field
        /// </summary>
        public byte? PaddingField { get; set; }
    }
}
