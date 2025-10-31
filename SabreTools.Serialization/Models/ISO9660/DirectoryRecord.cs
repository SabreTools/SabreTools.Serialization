using SabreTools.Numerics;

namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Directory Record, a directory descriptor that points to an extent representing a file or directory
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class DirectoryRecord
    {
        /// <summary>
        /// Length of Directory Record
        /// </summary>
        public byte DirectoryRecordLength { get; set; }

        /// <summary>
        /// Length of the extended attribute record
        /// If no extended attribute record is used, set to 0x00
        /// </summary>
        public byte ExtendedAttributeRecordLength { get; set; }

        /// <summary>
        /// Logical block number of the first logical block allocated to this extent
        /// </summary>
        public BothInt32 ExtentLocation { get; set; }

        /// <summary>
        /// Number of bytes allocated to this extent
        /// </summary>
        public BothInt32 ExtentLength { get; set; }

        /// <summary>
        /// Datetime of recording for the Directory Record
        /// If not specified, all values are 0x00
        /// </summary>
        public DirectoryRecordDateTime RecordingDateTime { get; set; }

        /// <summary>
        /// Flags for indicating attributes of the directory record
        /// </summary>
        public FileFlags FileFlags { get; set; }

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
        public BothInt16 VolumeSequenceNumber { get; set; }

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
        /// If record length prior to this is odd, the FileIdentifier is followed by a single padding byte (0x00)
        /// Optional field
        /// </summary>
        public byte? PaddingField { get; set; }

        /// <summary>
        /// Optional bytes at the end of a directory record for system use
        /// Must be an even number of bytes long (pad with a single 0x00 to make it even)
        /// Note: This is where SUSP contents are located, including Rock Ridge extension
        /// Optional field
        /// </summary>
        public byte[] SystemUse { get; set; }
    }
}
