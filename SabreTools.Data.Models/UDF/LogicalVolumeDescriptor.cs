namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Logical Volume Descriptor (3/10.6), block size, partition maps and the location of the File Set Descriptor
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class LogicalVolumeDescriptor : Descriptor
    {
        /// <summary>
        /// Volume Descriptor Sequence Number, the highest number prevails
        /// </summary>
        public uint VolumeDescriptorSequenceNumber { get; set; }

        /// <summary>
        /// Descriptor character set
        /// </summary>
        public CharacterSetSpecification DescriptorCharacterSet { get; set; } = new();

        /// <summary>
        /// Logical volume identifier, dstring (last byte = length)
        /// </summary>
        public byte[] LogicalVolumeIdentifier { get; set; } = new byte[128];

        /// <summary>
        /// Size of a logical block in bytes
        /// </summary>
        public uint LogicalBlockSize { get; set; }

        /// <summary>
        /// Domain identifier ("*OSTA UDF Compliant", suffix holds the UDF revision)
        /// </summary>
        public EntityIdentifier DomainIdentifier { get; set; } = new();

        /// <summary>
        /// Logical volume contents use (UDF: extent of the File Set Descriptor)
        /// </summary>
        public LongAllocationDescriptor LogicalVolumeContentsUse { get; set; } = new();

        /// <summary>
        /// Length of the partition maps in bytes
        /// </summary>
        public uint MapTableLength { get; set; }

        /// <summary>
        /// Number of partition maps
        /// </summary>
        public uint NumberOfPartitionMaps { get; set; }

        /// <summary>
        /// Implementation identifier
        /// </summary>
        public EntityIdentifier ImplementationIdentifier { get; set; } = new();

        /// <summary>
        /// Implementation use, 128 bytes
        /// </summary>
        public byte[] ImplementationUse { get; set; } = new byte[128];

        /// <summary>
        /// Extent of the Logical Volume Integrity Sequence
        /// </summary>
        public ExtentDescriptor IntegritySequenceExtent { get; set; } = new();

        /// <summary>
        /// Partition maps, a partition reference number is the index into this array
        /// </summary>
        public PartitionMap[] PartitionMaps { get; set; } = [];
    }
}
