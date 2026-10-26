using System;

namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// Enum for DescriptorTag.TagIdentifier
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public enum TagIdentifier : ushort
    {
        /// <summary>
        /// Primary Volume Descriptor (3/10.1)
        /// </summary>
        PRIMARY_VOLUME_DESCRIPTOR = 1,

        /// <summary>
        /// Anchor Volume Descriptor Pointer (3/10.2)
        /// </summary>
        ANCHOR_VOLUME_DESCRIPTOR_POINTER = 2,

        /// <summary>
        /// Volume Descriptor Pointer (3/10.3)
        /// </summary>
        VOLUME_DESCRIPTOR_POINTER = 3,

        /// <summary>
        /// Implementation Use Volume Descriptor (3/10.4)
        /// </summary>
        IMPLEMENTATION_USE_VOLUME_DESCRIPTOR = 4,

        /// <summary>
        /// Partition Descriptor (3/10.5)
        /// </summary>
        PARTITION_DESCRIPTOR = 5,

        /// <summary>
        /// Logical Volume Descriptor (3/10.6)
        /// </summary>
        LOGICAL_VOLUME_DESCRIPTOR = 6,

        /// <summary>
        /// Unallocated Space Descriptor (3/10.8)
        /// </summary>
        UNALLOCATED_SPACE_DESCRIPTOR = 7,

        /// <summary>
        /// Terminating Descriptor (3/10.9)
        /// </summary>
        TERMINATING_DESCRIPTOR = 8,

        /// <summary>
        /// Logical Volume Integrity Descriptor (3/10.10)
        /// </summary>
        LOGICAL_VOLUME_INTEGRITY_DESCRIPTOR = 9,

        /// <summary>
        /// File Set Descriptor (4/14.1)
        /// </summary>
        FILE_SET_DESCRIPTOR = 256,

        /// <summary>
        /// File Identifier Descriptor (4/14.4)
        /// </summary>
        FILE_IDENTIFIER_DESCRIPTOR = 257,

        /// <summary>
        /// Allocation Extent Descriptor (4/14.5)
        /// </summary>
        ALLOCATION_EXTENT_DESCRIPTOR = 258,

        /// <summary>
        /// Indirect Entry (4/14.7)
        /// </summary>
        INDIRECT_ENTRY = 259,

        /// <summary>
        /// Terminal Entry (4/14.8)
        /// </summary>
        TERMINAL_ENTRY = 260,

        /// <summary>
        /// File Entry (4/14.9)
        /// </summary>
        FILE_ENTRY = 261,

        /// <summary>
        /// Extended Attribute Header Descriptor (4/14.10.1)
        /// </summary>
        EXTENDED_ATTRIBUTE_HEADER_DESCRIPTOR = 262,

        /// <summary>
        /// Unallocated Space Entry (4/14.11)
        /// </summary>
        UNALLOCATED_SPACE_ENTRY = 263,

        /// <summary>
        /// Space Bitmap Descriptor (4/14.12)
        /// </summary>
        SPACE_BITMAP_DESCRIPTOR = 264,

        /// <summary>
        /// Partition Integrity Entry (4/14.13)
        /// </summary>
        PARTITION_INTEGRITY_ENTRY = 265,

        /// <summary>
        /// Extended File Entry (4/14.17)
        /// </summary>
        EXTENDED_FILE_ENTRY = 266,
    }

    /// <summary>
    /// Enum for FileIdentifierDescriptor.FileCharacteristics (4/14.4.3)
    /// Bits 5-7: Reserved: 0
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    [Flags]
    public enum FileCharacteristics : byte
    {
        /// <summary>
        /// No characteristics set
        /// </summary>
        NONE = 0x00,

        /// <summary>
        /// The existence of the file shall not be made known to the user (hidden)
        /// </summary>
        EXISTENCE = 0x01,

        /// <summary>
        /// The file is a directory
        /// </summary>
        DIRECTORY = 0x02,

        /// <summary>
        /// The descriptor shall be ignored (deleted)
        /// </summary>
        DELETED = 0x04,

        /// <summary>
        /// The ICB identifies the parent directory
        /// </summary>
        PARENT = 0x08,

        /// <summary>
        /// The file contains implementation use data (UDF: stream directory metadata)
        /// </summary>
        METADATA = 0x10,
    }

    /// <summary>
    /// Enum for ICBTag.FileType (4/14.6.6, UDF 2.3.5.4)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public enum FileType : byte
    {
        /// <summary>
        /// Not specified
        /// </summary>
        UNSPECIFIED = 0,

        /// <summary>
        /// Unallocated Space Entry
        /// </summary>
        UNALLOCATED_SPACE_ENTRY = 1,

        /// <summary>
        /// Partition Integrity Entry
        /// </summary>
        PARTITION_INTEGRITY_ENTRY = 2,

        /// <summary>
        /// Indirect Entry
        /// </summary>
        INDIRECT_ENTRY = 3,

        /// <summary>
        /// Directory
        /// </summary>
        DIRECTORY = 4,

        /// <summary>
        /// Sequence of bytes (a regular file)
        /// </summary>
        FILE = 5,

        /// <summary>
        /// Block special device
        /// </summary>
        BLOCK_DEVICE = 6,

        /// <summary>
        /// Character special device
        /// </summary>
        CHARACTER_DEVICE = 7,

        /// <summary>
        /// Extended attributes
        /// </summary>
        EXTENDED_ATTRIBUTES = 8,

        /// <summary>
        /// FIFO file
        /// </summary>
        FIFO = 9,

        /// <summary>
        /// Socket
        /// </summary>
        SOCKET = 10,

        /// <summary>
        /// Terminal Entry
        /// </summary>
        TERMINAL_ENTRY = 11,

        /// <summary>
        /// Symbolic link
        /// </summary>
        SYMBOLIC_LINK = 12,

        /// <summary>
        /// Stream directory
        /// </summary>
        STREAM_DIRECTORY = 13,

        /// <summary>
        /// UDF Virtual Allocation Table
        /// </summary>
        VIRTUAL_ALLOCATION_TABLE = 248,

        /// <summary>
        /// UDF Real-Time file
        /// </summary>
        REAL_TIME_FILE = 249,

        /// <summary>
        /// UDF Metadata File
        /// </summary>
        METADATA_FILE = 250,

        /// <summary>
        /// UDF Metadata Mirror File
        /// </summary>
        METADATA_MIRROR_FILE = 251,

        /// <summary>
        /// UDF Metadata Bitmap File
        /// </summary>
        METADATA_BITMAP_FILE = 252,
    }

    /// <summary>
    /// Enum for the allocation descriptor type in ICBTag.Flags (4/14.6.8)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public enum AllocationDescriptorType : byte
    {
        /// <summary>
        /// Short Allocation Descriptors (4/14.14.1)
        /// </summary>
        SHORT = 0,

        /// <summary>
        /// Long Allocation Descriptors (4/14.14.2)
        /// </summary>
        LONG = 1,

        /// <summary>
        /// Extended Allocation Descriptors (4/14.14.3)
        /// </summary>
        EXTENDED = 2,

        /// <summary>
        /// The file data is embedded in the allocation descriptors field
        /// </summary>
        EMBEDDED = 3,
    }

    /// <summary>
    /// Enum for the extent type in the top two bits of an allocation descriptor's extent length (4/14.14.1.1)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public enum ExtentType : byte
    {
        /// <summary>
        /// Extent recorded and allocated
        /// </summary>
        RECORDED_AND_ALLOCATED = 0,

        /// <summary>
        /// Extent not recorded but allocated
        /// </summary>
        NOT_RECORDED_BUT_ALLOCATED = 1,

        /// <summary>
        /// Extent not recorded and not allocated
        /// </summary>
        NOT_RECORDED_NOT_ALLOCATED = 2,

        /// <summary>
        /// The extent is the next extent of allocation descriptors
        /// </summary>
        NEXT_EXTENT = 3,
    }
}
