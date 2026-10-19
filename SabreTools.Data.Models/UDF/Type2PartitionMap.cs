namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Type 2 Partition Map (3/10.7.3, UDF 2.2.8-2.2.10), a partition identified by its partition type identifier
    /// (e.g. "*UDF Virtual Partition", "*UDF Sparable Partition", "*UDF Metadata Partition")
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public class Type2PartitionMap : PartitionMap
    {
        /// <summary>
        /// Reserved, 2 bytes of 0x00
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[2];

        /// <summary>
        /// Partition type identifier
        /// </summary>
        public EntityIdentifier PartitionTypeIdentifier { get; set; } = new();

        /// <summary>
        /// Volume sequence number of the volume containing the partition
        /// </summary>
        public ushort VolumeSequenceNumber { get; set; }

        /// <summary>
        /// Partition number of the underlying Partition Descriptor
        /// </summary>
        public ushort PartitionNumber { get; set; }

        /// <summary>
        /// Partition type specific data after the partition number, 24 bytes
        /// Empty for partition types that are parsed further (MetadataPartitionMap)
        /// </summary>
        public byte[] PartitionTypeData { get; set; } = [];
    }
}
