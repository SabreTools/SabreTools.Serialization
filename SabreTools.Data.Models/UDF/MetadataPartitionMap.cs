namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Metadata Partition Map (UDF 2.2.10, UDF 2.50 and later), a partition whose logical blocks
    /// are the contents of the Metadata File recorded in the underlying partition
    /// </summary>
    /// <see href="http://www.osta.org/specs/pdf/udf260.pdf"/>
    public sealed class MetadataPartitionMap : Type2PartitionMap
    {
        /// <summary>
        /// Logical block of the Metadata File's File Entry in the underlying partition
        /// </summary>
        public uint MetadataFileLocation { get; set; }

        /// <summary>
        /// Logical block of the Metadata Mirror File's File Entry in the underlying partition
        /// </summary>
        public uint MetadataMirrorFileLocation { get; set; }

        /// <summary>
        /// Logical block of the Metadata Bitmap File's File Entry, 0xFFFFFFFF if not recorded
        /// </summary>
        public uint MetadataBitmapFileLocation { get; set; }

        /// <summary>
        /// Allocation unit size in logical blocks
        /// </summary>
        public uint AllocationUnitSize { get; set; }

        /// <summary>
        /// Alignment unit size in logical blocks
        /// </summary>
        public ushort AlignmentUnitSize { get; set; }

        /// <summary>
        /// Flags (bit 0: duplicate metadata flag)
        /// </summary>
        public byte Flags { get; set; }

        /// <summary>
        /// Reserved, 5 bytes of 0x00
        /// </summary>
        public byte[] Reserved2 { get; set; } = new byte[5];
    }
}
