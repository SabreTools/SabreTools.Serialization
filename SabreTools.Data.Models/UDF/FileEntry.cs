namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF File Entry (4/14.9), the ICB describing a file or directory and where its data is recorded
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public class FileEntry : Descriptor
    {
        /// <summary>
        /// ICB tag
        /// </summary>
        public ICBTag ICBTag { get; set; } = new();

        /// <summary>
        /// User ID
        /// </summary>
        public uint Uid { get; set; }

        /// <summary>
        /// Group ID
        /// </summary>
        public uint Gid { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        public uint Permissions { get; set; }

        /// <summary>
        /// Number of File Identifier Descriptors identifying this ICB
        /// </summary>
        public ushort FileLinkCount { get; set; }

        /// <summary>
        /// Record format
        /// </summary>
        public byte RecordFormat { get; set; }

        /// <summary>
        /// Record display attributes
        /// </summary>
        public byte RecordDisplayAttributes { get; set; }

        /// <summary>
        /// Record length
        /// </summary>
        public uint RecordLength { get; set; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public ulong InformationLength { get; set; }

        /// <summary>
        /// Number of recorded logical blocks
        /// </summary>
        public ulong LogicalBlocksRecorded { get; set; }

        /// <summary>
        /// Access date and time
        /// </summary>
        public Timestamp AccessDateAndTime { get; set; } = new();

        /// <summary>
        /// Modification date and time
        /// </summary>
        public Timestamp ModificationDateAndTime { get; set; } = new();

        /// <summary>
        /// Attribute date and time
        /// </summary>
        public Timestamp AttributeDateAndTime { get; set; } = new();

        /// <summary>
        /// Checkpoint
        /// </summary>
        public uint Checkpoint { get; set; }

        /// <summary>
        /// ICB of the extended attributes, length 0 if none
        /// </summary>
        public LongAllocationDescriptor ExtendedAttributeICB { get; set; } = new();

        /// <summary>
        /// Implementation identifier
        /// </summary>
        public EntityIdentifier ImplementationIdentifier { get; set; } = new();

        /// <summary>
        /// Unique ID
        /// </summary>
        public ulong UniqueID { get; set; }

        /// <summary>
        /// Length of the extended attributes in bytes
        /// </summary>
        public uint LengthOfExtendedAttributes { get; set; }

        /// <summary>
        /// Length of the allocation descriptors in bytes
        /// </summary>
        public uint LengthOfAllocationDescriptors { get; set; }

        /// <summary>
        /// Extended attributes
        /// </summary>
        public byte[] ExtendedAttributes { get; set; } = [];

        /// <summary>
        /// Allocation descriptors of the type given in the ICB tag flags,
        /// or the file data itself if the type is AllocationDescriptorType.EMBEDDED
        /// </summary>
        public byte[] AllocationDescriptors { get; set; } = [];
    }
}
