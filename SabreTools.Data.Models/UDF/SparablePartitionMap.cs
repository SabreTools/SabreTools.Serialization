namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Sparable Partition Map (UDF 2.2.9), a partition on rewritable media whose defective packets
    /// are relocated to spare packets listed in a Sparing Table
    /// </summary>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class SparablePartitionMap : Type2PartitionMap
    {
        /// <summary>
        /// Packet length in logical blocks, the unit of relocation
        /// </summary>
        public ushort PacketLength { get; set; }

        /// <summary>
        /// Number of copies of the Sparing Table (1 to 4)
        /// </summary>
        public byte NumberOfSparingTables { get; set; }

        /// <summary>
        /// Reserved, 1 byte of 0x00
        /// </summary>
        public byte Reserved2 { get; set; }

        /// <summary>
        /// Size of each Sparing Table in bytes
        /// </summary>
        public uint SizeOfEachSparingTable { get; set; }

        /// <summary>
        /// Logical sectors of the Sparing Tables, the first NumberOfSparingTables are used
        /// </summary>
        public uint[] LocationsOfSparingTables { get; set; } = new uint[4];
    }
}
