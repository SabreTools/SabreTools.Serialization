namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Virtual Partition Map (UDF 2.2.8), a partition on write-once media whose logical blocks are mapped
    /// to logical blocks of the underlying partition by the Virtual Allocation Table
    /// </summary>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class VirtualPartitionMap : Type2PartitionMap
    {
        /// <summary>
        /// Reserved, 24 bytes of 0x00
        /// </summary>
        public byte[] Reserved2 { get; set; } = new byte[24];
    }
}
