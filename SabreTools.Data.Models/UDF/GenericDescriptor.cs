namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF tagged descriptor of a type that is not parsed further
    /// (e.g. Implementation Use Volume Descriptor, Unallocated Space Descriptor, Terminating Descriptor)
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class GenericDescriptor : Descriptor
    {
        /// <summary>
        /// Contents of the descriptor after the tag, up to the end of the logical sector
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
