namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Sparing Table (UDF 2.2.12), the relocations of defective packets of a Sparable Partition
    /// </summary>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class SparingTable : Descriptor
    {
        /// <summary>
        /// Sparing identifier, "*UDF Sparing Table"
        /// </summary>
        public EntityIdentifier SparingIdentifier { get; set; } = new();

        /// <summary>
        /// Number of map entries
        /// </summary>
        public ushort ReallocationTableLength { get; set; }

        /// <summary>
        /// Reserved, 2 bytes of 0x00
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// Sequence number, incremented each time the table is updated
        /// </summary>
        public uint SequenceNumber { get; set; }

        /// <summary>
        /// Map entries, sorted by original location
        /// </summary>
        public SparingEntry[] MapEntries { get; set; } = [];
    }
}
