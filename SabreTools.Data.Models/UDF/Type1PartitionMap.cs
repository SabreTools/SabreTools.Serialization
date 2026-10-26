namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Type 1 Partition Map (3/10.7.2), logical blocks map 1:1 onto a Partition Descriptor's sectors
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class Type1PartitionMap : PartitionMap
    {
        /// <summary>
        /// Volume sequence number of the volume containing the partition
        /// </summary>
        public ushort VolumeSequenceNumber { get; set; }

        /// <summary>
        /// Partition number of the Partition Descriptor
        /// </summary>
        public ushort PartitionNumber { get; set; }
    }
}
