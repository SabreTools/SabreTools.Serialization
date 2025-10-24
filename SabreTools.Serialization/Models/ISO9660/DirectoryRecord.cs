namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory Record (represents either a directory or a file in the filesystem)
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
        public BothEndianInt32? ExtentLocation { get; set; }

        /// <summary>
        /// Number of logical blocks allocated to this extent
        /// </summary>
        public BothEndianInt32? ExtentLength { get; set; }

        /// <summary>
        /// Datetime of recording for the Directory Record
        /// If not specified, all values are 0x00
        /// </summary>
        public DirectoryRecordDateTime? RecordingDateTime { get; set; }

        /// <summary>
        /// Flags for indicating attribytes of the directory record
        /// </summary>
        public FileFlags? FileFlags { get; set; }

        /// <summary>
        /// Assigned file unit size for the file section (interleaved mode)
        /// 0x00 if the file is not recorded in interleaved mode
        /// </summary>
        public byte FileUnitSize { get; set; }

        /// <summary>
        /// Assigned interleave gap size for the file section (interleaved mode)
        /// 0x00 if the file is not recorded in interleaved mode
        /// </summary>
        public byte InterleaveGapSize { get; set; }

        /// <summary>
        /// Volume sequence ordinal number of the volume in the volume set on which the record extent is recorded
        /// </summary>
        public BothEndianInt16? VolumeSequenceNumber { get; set; }

        /// <summary>
        /// Length of the FileIdentifier field in bytes
        /// </summary>
        public byte FileIdentifierLength { get; set; }

        /// <summary>
        /// If FileFlags.Directory is 1, this is the name of the directory
        /// If FileFlags.Directory is 0, this is the name of the file
        /// File: Uses either d-characters or d1-characters and Separator1 and Separator2
        /// Directory: Uses either d-characters or d1-characters, or:
        ///            Is exactly Constants.CurrentDirectory (0x00) or Constants.ParentDirectory (0x01)
        /// </summary>
        public byte[]? FileIdentifier { get; set; }

        /// <summary>
        /// If FileIdentifierLength is odd, the FileIdentifier is followed by a single padding byte (0x00)
        /// Optional field
        /// </summary>
        public byte? PaddingField { get; set; }

        /// <summary>
        /// Optional bytes at the end of a directory record for system use
        /// Must be an even number of bytes long (pad with a single 0x00 to make it even)
        /// Note: This is where SUSP extensions are located, including Rock Ridge
        /// Optional field
        /// </summary>
        public byte[]? SystemUse { get; set; }
    }
}
