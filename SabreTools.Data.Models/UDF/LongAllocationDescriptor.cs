namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Long Allocation Descriptor (long_ad, 4/14.14.2), an extent within any partition of the logical volume
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class LongAllocationDescriptor
    {
        /// <summary>
        /// Length of the extent in bytes (lower 30 bits) and the ExtentType (upper 2 bits)
        /// A length of 0 terminates a sequence of allocation descriptors
        /// </summary>
        public uint ExtentLength { get; set; }

        /// <summary>
        /// Logical block address of the start of the extent
        /// </summary>
        public LogicalBlockAddress ExtentLocation { get; set; } = new();

        /// <summary>
        /// Implementation use (UDF: flags and unique ID bits)
        /// </summary>
        public byte[] ImplementationUse { get; set; } = new byte[6];
    }
}
