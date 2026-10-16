namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Partition Descriptor (3/10.5), the physical location of a partition
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class PartitionDescriptor : Descriptor
    {
        /// <summary>
        /// Volume Descriptor Sequence Number, the highest number prevails
        /// </summary>
        public uint VolumeDescriptorSequenceNumber { get; set; }

        /// <summary>
        /// Partition flags (bit 0: space is allocated)
        /// </summary>
        public ushort PartitionFlags { get; set; }

        /// <summary>
        /// Partition number, referenced by the partition maps
        /// </summary>
        public ushort PartitionNumber { get; set; }

        /// <summary>
        /// Partition contents ("+NSR02" or "+NSR03" for UDF)
        /// </summary>
        public EntityIdentifier PartitionContents { get; set; } = new();

        /// <summary>
        /// Partition contents use (Partition Header Descriptor for NSR), 128 bytes
        /// </summary>
        public byte[] PartitionContentsUse { get; set; } = new byte[128];

        /// <summary>
        /// Access type (1 = read-only, 2 = write-once, 3 = rewritable, 4 = overwritable)
        /// </summary>
        public uint AccessType { get; set; }

        /// <summary>
        /// Logical sector number of the first sector of the partition
        /// </summary>
        public uint PartitionStartingLocation { get; set; }

        /// <summary>
        /// Length of the partition in logical sectors
        /// </summary>
        public uint PartitionLength { get; set; }

        /// <summary>
        /// Implementation identifier
        /// </summary>
        public EntityIdentifier ImplementationIdentifier { get; set; } = new();

        /// <summary>
        /// Implementation use, 128 bytes
        /// </summary>
        public byte[] ImplementationUse { get; set; } = new byte[128];

        /// <summary>
        /// Reserved, 156 bytes of 0x00
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[156];
    }
}
