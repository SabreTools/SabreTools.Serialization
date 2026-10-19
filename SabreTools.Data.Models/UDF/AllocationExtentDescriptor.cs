namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Allocation Extent Descriptor (4/14.5), continues the allocation descriptors of a File Entry
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class AllocationExtentDescriptor : Descriptor
    {
        /// <summary>
        /// Logical block of the previous allocation extent
        /// </summary>
        public uint PreviousAllocationExtentLocation { get; set; }

        /// <summary>
        /// Length of the allocation descriptors in bytes
        /// </summary>
        public uint LengthOfAllocationDescriptors { get; set; }

        /// <summary>
        /// Allocation descriptors, of the type given in the owning File Entry's ICB tag flags
        /// </summary>
        public byte[] AllocationDescriptors { get; set; } = [];
    }
}
