namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Short Allocation Descriptor (short_ad, 4/14.14.1), an extent within the partition of the descriptor
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class ShortAllocationDescriptor
    {
        /// <summary>
        /// Length of the extent in bytes (lower 30 bits) and the ExtentType (upper 2 bits)
        /// A length of 0 terminates a sequence of allocation descriptors
        /// </summary>
        public uint ExtentLength { get; set; }

        /// <summary>
        /// Logical block number of the start of the extent
        /// </summary>
        public uint ExtentPosition { get; set; }
    }
}
