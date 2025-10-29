using SabreTools.Numerics;

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
        public BothInt16 OwnerIdentification { get; set; }

        /// <summary>
        /// Group ID number for the owner of this file
        /// 0x0000 if no group, implies 0x0000 Owner ID
        /// </summary>
        public BothInt16 GroupIdentification { get; set; }

        /// <summary>
        /// 16-bit flag variable with 8 flags where every other bit is set to 1
        /// i.e. minimum value of 0b1010101010101010 (0xAAAA)
        /// </summary>
        public Permissions Permissions { get; set; }

        /// <summary>
        /// Datetime of when the file content was created
        /// </summary>
        public DecDateTime FileCreationDateTime { get; set; }

        /// <summary>
        /// Datetime of when the file content was last modified
        /// </summary>
        public DecDateTime FileModificationDateTime { get; set; }

        /// <summary>
        /// Datetime of when the file content expires
        /// </summary>
        public DecDateTime FileExpirationDateTime { get; set; }

        /// <summary>
        /// Datetime of when the file content is effective from
        /// </summary>
        public DecDateTime FileEffectiveDateTime { get; set; }

        /// <summary>
        /// Record format type
        /// </summary>
        public RecordFormat RecordFormat { get; set; }

        /// <summary>
        /// Record attributes
        /// Note: If RecordType is zero, this field is ignored by readers
        /// </summary>
        public RecordAttributes RecordAttributes { get; set; }

        /// <summary>
        /// Record Length
        /// If RecordType is 0, this field is 0
        /// If RecordType is 1, this field is length in bytes
        /// If RecordType is 2 or 3, this field is maximum length in bytes of a record in the file
        /// </summary>
        public BothInt16 RecordLength { get; set; }

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
        public BothInt16 ApplicationLength { get; set; }

        /// <summary>
        /// ApplicationLength-bytes for application use
        /// </summary>
        public byte[] ApplicationUse { get; set; }

        /// <summary>
        /// EscapeSequencesLength-bytes list of escape sequences to interpret this file
        /// Optional, and if present, padded to the right with 0x00
        /// </summary>
        public byte[]? EscapeSequences { get; set; }
    }
}
