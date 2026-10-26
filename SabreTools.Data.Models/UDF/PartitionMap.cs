namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// Abstract UDF Partition Map (3/10.7)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public abstract class PartitionMap
    {
        /// <summary>
        /// Partition map type (1 or 2)
        /// </summary>
        public byte PartitionMapType { get; set; }

        /// <summary>
        /// Length of the partition map in bytes
        /// </summary>
        public byte PartitionMapLength { get; set; }
    }
}
