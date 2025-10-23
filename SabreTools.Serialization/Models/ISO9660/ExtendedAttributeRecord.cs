namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 Extended Attribute Record
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class ExtendedAttributeRecord
    {
        /// <summary>
        /// Owner ID number for this file
        /// 0x0000 if no owner, implies 0x0000 Group ID
        /// </summary>
        public BothEndianInt16 OwnerIdentification { get; set; }

        /// <summary>
        /// Group ID number for the owner of this file
        /// 0x0000 if no group, implies 0x0000 Owner ID
        /// </summary>
        public BothEndianInt16 GroupIdentification { get; set; }

        /// <summary>
        /// 16-flag flag variable where every other bit is set to 1
        /// Flag 1 (Bit 0): 1 if system users may not read, 0 otherwise
        /// Flag 2 (Bit 2): 1 if system users may not execute, 0 otherwise
        /// Flag 3 (Bit 4): 1 if owner may not read, 0 otherwise
        /// Flag 4 (Bit 6): 1 if owner may not execute, 0 otherwise
        /// Flag 5 (Bit 8): 1 if group members may not read, 0 otherwise
        /// Flag 6 (Bit 10): 1 if group members may not execute, 0 otherwise
        /// Flag 7 (Bit 12): 1 if non-group members may not read, 0 if any user can read
        /// Flag 8 (Bit 14): 1 if non-group members may not execute, 0 if any user can execute
        /// </summary>
        public ushort Permissions { get; set; }

        /// <summary>
        /// Datetime of when the file content was created
        /// </summary>
        public DecDateTime FileCreationDateTime

        /// <summary>
        /// Datetime of when the file content was last modified
        /// </summary>
        public DecDateTime FileModificationDateTime

        /// <summary>
        /// Datetime of when the file content expires
        /// </summary>
        public DecDateTime FileExpirationDateTime

        /// <summary>
        /// Datetime of when the file content is effective from
        /// </summary>
        public DecDateTime FileEffectiveDateTime

        /// <summary>
        /// Record type
        /// 0 = Unspecified
        /// 1 = Sequence of fixed-length records
        /// 2 = Sequence of variable-length records, Record Control World is LSB
        /// 3 = Sequence of variable-length records, Record Control World is MSB
        /// 4-127 (0x04-7F): Reserved
        /// 128-255 (0x80-FF): System Use
        /// </summary>
        public byte RecordFormat { get; set; }

        /// <summary>
        /// Record attributes
        /// 0 = Records are preceeded by linefeed and followed by carriage return
        /// 1 = First byte of each record is Fortran-style
        /// 2 = Record contains necessary control information within it
        /// 3-255 (0x03-FF): Reserved
        /// Note: If RecordType is zero, this field is ignored by readers 
        /// </summary>
        public byte RecordAttributes { get; set; }

        /// <summary>
        /// Record Length
        /// If RecordType is 0, this field is 0
        /// If RecordType is 1, this field is length in bytes
        /// If RecordType is 2 or 3, this field is maximum length in bytes of a record in the file
        /// </summary>
        public BothEndianInt16 RecordLength { get; set; }

        /// <summary>
        /// 32-byte name of the intended system
        /// Primary: a-characters or a1-characters only, padded to the right with spaces
        /// </summary>
        public byte[] SystemIdentifier { get; set; }

        /// <summary>
        /// 64-bytes for system use
        /// </summary>
        public byte[] SystemUse { get; set; }

        /// <summary>
        /// Extended Attribyte Record Version
        /// ISO9660 sets this to 0x01
        /// </summary>
        public byte ExtendedAttributeRecordVersion { get; set; }

        /// <summary>
        /// Length of the escape sequences field
        /// </summary>
        public byte EscapeSequencesLength { get; set; }

        /// <summary>
        /// 64-bytes reserved (0x00)
        /// </summary>
        public byte[] Reserved64Bytes { get; set; }

        /// <summary>
        /// Length of the Application use field
        /// </summary>
        public BothEndianInt16 ApplicationLength { get; set; }

        /// <summary>
        /// ApplicationLength-bytes for application use
        /// </summary>
        public byte[] ApplicationUse { get; set; }

        /// <summary>
        /// EscapeSequencesLength-bytes list of escape sequences to interpret this file
        /// Optional, and padded to the right with 0x00
        /// </summary>
        public byte[]? EscapeSequences { get; set; }
    }
}
