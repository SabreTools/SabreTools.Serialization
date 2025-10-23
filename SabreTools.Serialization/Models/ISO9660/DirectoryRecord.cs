namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory Record (represents either a directory or a file in the filesystem)
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DirectoryRecord
    {
        /// <summary>
        /// Datetime format used by DirectoryRecord
        /// </summary>
        public sealed class DirectoryRecordDateTime
        {
            /// <summary>
            /// Number of years since 1900
            /// </summary>
            public byte YearsSince1990 { get; set; }

            /// <summary>
            /// Month of the year, 1-12
            /// </summary>
            public byte Month { get; set; }

            /// <summary>
            /// Day of the month, 1-31
            /// </summary>
            public byte Day { get; set; }

            /// <summary>
            /// Hour of the day, 0-23
            /// </summary>
            public byte Hour { get; set; }

            /// <summary>
            /// Minute of the hour, 0-59
            /// </summary>
            public byte Minute { get; set; }

            /// <summary>
            /// Second of the minute, 0-59
            /// </summary>
            public byte Second { get; set; }

            /// <summary>
            /// Time zone offset (from GMT = UTC 0), represented by a single byte
            /// Unit = 15min offset
            /// 0 = offset of -12 hours (UTC-12)
            /// 100 = offset of +13 hours (UTC+13)
            /// </summary>
            public byte TimezoneOffset { get; set; }
        }

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
        public BothEndianInt32 ExtentLocation { get; set; }

        /// <summary>
        /// Number of logical blocks allocated to this extent
        /// </summary>
        public BothEndianInt32 ExtentLength { get; set; }

        /// <summary>
        /// Datetime of recording for the Directory Record
        /// If not specified, all values are 0x00
        /// </summary>
        public DirectoryRecordDateTime RecordingDateTime { get; set; }

        /// <summary>
        /// File flags:
        /// Flag 1 (Bit 0 LSB): Existence: 1 if file should be hidden from the user upon inquiry, 0 otherwise
        /// Flag 2 (Bit 1 LSB): Directory: 1 if the directory Record identifies a directory, 0 otherwise
        /// Flag 3 (Bit 2 LSB): Associated File: 1 if the file is an associated file, 0 otherwise
        /// Flag 4 (Bit 3 LSB): Record: 1 if file has record format specified by non-zero record format of extended attribute record
        /// Flag 5 (Bit 4 LSB): Protection: 1 if owner/group ID is set for the file and permissions field is set properly, 0 otherwise
        /// Flag 6 (Bit 5 LSB): Reserved: 0
        /// Flag 7 (Bit 5 LSB): Reserved: 0
        /// Flag 8 (Bit 5 LSB): Multi-extent: 1 if Directory Extent is not the final record for the file, 0 otherwise
        /// </summary>
        public byte FileFlags { get; set; }

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
        public BothEndianInt16 VolumeSequenceNumber { get; set; }

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
        public byte[] FileIdentifier { get; set; }

        /// <summary>
        /// If FileIdentifierLength is odd, the FileIdentifier is followed by a single padding byte (0x00)
        /// </summary>
        public byte? PaddingField { get; set; }

        /// <summary>
        /// Optional bytes at the end of a directory record for system use
        /// Must be an even number of bytes long (pad with a single 0x00 to make it even)
        /// Note: This is where SUSP extensions are located, including Rock Ridge
        /// </summary>
        public byte[]? SystemUse { get; set; }
    }
}
