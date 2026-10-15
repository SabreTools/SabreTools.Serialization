namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Recorded Address (lb_addr, 4/7.1), a logical block within a partition
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class LogicalBlockAddress
    {
        /// <summary>
        /// Logical block number, relative to the start of the partition
        /// </summary>
        public uint LogicalBlockNumber { get; set; }

        /// <summary>
        /// Index of the partition map in the Logical Volume Descriptor
        /// </summary>
        public ushort PartitionReferenceNumber { get; set; }
    }
}
