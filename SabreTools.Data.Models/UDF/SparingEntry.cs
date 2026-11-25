namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Sparing Table map entry (UDF 2.2.12)
    /// </summary>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class SparingEntry
    {
        /// <summary>
        /// Logical block of the first block of the relocated packet in the partition;
        /// 0xFFFFFFFF for an available spare packet, 0xFFFFFFF0 for a defective one
        /// </summary>
        public uint OriginalLocation { get; set; }

        /// <summary>
        /// Logical sector of the first block of the spare packet
        /// </summary>
        public uint MappedLocation { get; set; }
    }
}
