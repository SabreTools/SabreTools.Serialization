using System;

namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// Enum for VolumeDescriptor.Type
    /// All values 4-254 are reserved
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public enum VolumeDescriptorType : byte
    {
        /// <summary>
        /// Primary Volume Descriptor
        /// </summary>
        BOOT_RECORD_VOLUME_DESCRIPTOR = 0,

        /// <summary>
        /// Primary Volume Descriptor
        /// </summary>
        PRIMARY_VOLUME_DESCRIPTOR = 1,

        /// <summary>
        /// Supplementary Volume Descriptor
        /// </summary>
        SUPPLEMENTARY_VOLUME_DESCRIPTOR = 2,

        /// <summary>
        /// Enhanced Volume Descriptor (including Joliet extensions)
        /// </summary>
        ENHANCED_VOLUME_DESCRIPTOR = SUPPLEMENTARY_VOLUME_DESCRIPTOR,

        /// <summary>
        /// Volume Partition Descriptor
        /// </summary>
        VOLUME_PARTITION_DESCRIPTOR = 3,

        /// <summary>
        /// Volume Descriptor Set Terminator
        /// </summary>
        VOLUME_DESCRIPTOR_SET_TERMINATOR = 255,
    }

    /// <summary>
    /// Enum for DirectoryRecord.FileFlags
    /// Flag 6 (Bit 5 LSB): Reserved: 0
    /// Flag 7 (Bit 5 LSB): Reserved: 0
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    [Flags]
    public enum FileFlags : byte
    {
        /// <summary>
        /// Flag 1 (Bit 0 LSB): Existence
        /// 1 if file should be hidden from the user upon inquiry
        /// 0 otherwise
        /// </summary>
        EXISTENCE = 0x01,

        /// <summary>
        /// Flag 2 (Bit 1 LSB): Directory
        /// 1 if the directory Record identifies a directory
        /// 0 otherwise
        /// </summary>
        DIRECTORY = 0x02,

        /// <summary>
        /// Flag 3 (Bit 2 LSB): Associated File
        /// 1 if the file is an associated file
        /// 0 otherwise
        /// </summary>
        ASSOCIATED_FILE = 0x04,

        /// <summary>
        /// Flag 4 (Bit 3 LSB): Record
        /// 1 if file has record format specified by non-zero record format of extended attribute record
        /// 0 otherwise
        /// </summary>
        RECORD = 0x08,

        /// <summary>
        /// Flag 5 (Bit 4 LSB): Protection
        /// 1 if owner/group ID is set for the file and permissions field is set properly
        /// 0 otherwise
        /// </summary>
        PROTECTION = 0x10,

        /// <summary>
        /// Flag 6 (Bit 5 LSB): Reserved by ISO9660
        /// </summary>
        RESERVED_BIT5 = 0x20,

        /// <summary>
        /// Flag 7 (Bit 6 LSB): Reserved by ISO9660
        /// </summary>
        RESERVED_BIT6 = 0x40,

        /// <summary>
        /// Flag 8 (Bit 7 LSB): Multi-extent
        /// 1 if Directory Extent is not the final record for the file
        /// 0 otherwise
        /// </summary>
        MULTI_EXTENT = 0x80,
    }

    /// <summary>
    /// Enum for SupplementaryVolumeDescriptor.VolumeFlags
    /// Flag 1 (Bit 0, LSB) is used
    /// All other flags/bits are reserved (0x00)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    [Flags]
    public enum VolumeFlags : byte
    {
        /// <summary>
        /// Flag 1 (Bit 0, LSB):
        /// 1 if SupplementaryVolumeDescriptor.EscapeSequences has at least 1 unregistered escape sequence
        /// 0 if all escape sequences in SupplementaryVolumeDescriptor.EscapeSequences are registered
        /// </summary>
        UNREGISTERED_ESCAPE_SEQUENCES = 0x01,
    }

    /// <summary>
    /// Enum for ExtendedAttributeRecord.Permissions
    /// Every 2nd bit is fixed to 1 (i.e. minimum value of 0xAAAA)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    [Flags]
    public enum Permissions : ushort
    {
        /// <summary>
        /// Flag 1 (Bit 0): 1 if system users may not read, 0 otherwise
        /// </summary>
        SYSTEM_USER_CANNOT_READ = 0b0000_0000_0000_0001,

        /// <summary>
        /// Flag 2 (Bit 2): 1 if system users may not execute, 0 otherwise
        /// </summary>
        SYSTEM_USER_CANNOT_EXECUTE = 0b0000_0000_0000_0100,

        /// <summary>
        /// Flag 3 (Bit 4): 1 if owner may not read, 0 otherwise
        /// </summary>
        OWNER_CANNOT_READ = 0b0000_0000_0001_0000,

        /// <summary>
        /// Flag 4 (Bit 6): 1 if owner may not execute, 0 otherwise
        /// </summary>
        OWNER_CANNOT_EXECUTE = 0b0000_0000_0100_0000,

        /// <summary>
        /// Flag 5 (Bit 8): 1 if group members may not read, 0 otherwise
        /// </summary>
        GROUP_MEMBER_CANNOT_READ = 0b0000_0001_0000_0000,

        /// <summary>
        /// Flag 6 (Bit 10): 1 if group members may not execute, 0 otherwise
        /// </summary>
        GROUP_MEMBER_CANNOT_EXECUTE = 0b0000_0100_0000_0000,

        /// <summary>
        /// Flag 7 (Bit 12): 1 if non-group members may not read, 0 if any user can read
        /// </summary>
        NON_GROUP_MEMBER_CANNOT_READ = 0b0001_0000_0000_0000,

        /// <summary>
        /// Flag 8 (Bit 14): 1 if non-group members may not execute, 0 if any user can execute
        /// </summary>
        NON_GROUP_MEMBER_CANNOT_EXECUTE = 0b0100_0000_0000_0000,

        /// <summary>
        /// Fixed values in the enum, every other bit set to 1
        /// </summary>
        PERMISSIONS_MASK = 0b1010_1010_1010_1010,
    }

    /// <summary>
    /// Enum for ExtendedAttributeRecord.RecordFormat
    /// 4-127 (0x04-7F): Reserved
    /// 128-255 (0x80-FF): System Use
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public enum RecordFormat : byte
    {
        /// <summary>
        /// Record format unspecified by this type
        /// </summary>
        UNSPECIFIED = 0x00,

        /// <summary>
        /// Sequence of fixed-length records
        /// </summary>
        FIXED_LENGTH_RECORDS = 0x01,

        /// <summary>
        /// Sequence of variable-length records, Record Control World is LSB
        /// </summary>
        VARIABLE_LENGTH_RECORDS_LSB = 0x02,

        /// <summary>
        /// Sequence of variable-length records, Record Control World is MSB
        /// </summary>
        VARIABLE_LENGTH_RECORDS_MSG = 0x03,
    }

    /// <summary>
    /// Enum for ExtendedAttributeRecord.RecordAttributes
    /// 3-255 (0x03-FF): Reserved by ISO9660
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public enum RecordAttributes : byte
    {
        /// <summary>
        /// Records are preceeded by linefeed and followed by carriage return
        /// </summary>
        LINEFEED_CARRIAGE_RETURN = 0x00,

        /// <summary>
        /// First byte of each record is Fortran-style
        /// </summary>
        FORTRAN_STYLE = 0x01,

        /// <summary>
        /// Record contains necessary control information within itself
        /// </summary>
        SELF_DEFINED = 0x02,
    }
}
