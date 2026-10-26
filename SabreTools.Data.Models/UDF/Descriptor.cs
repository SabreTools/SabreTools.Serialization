namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// Abstract UDF tagged descriptor, all start with a Descriptor Tag
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public abstract class Descriptor
    {
        /// <summary>
        /// Descriptor tag
        /// </summary>
        public DescriptorTag DescriptorTag { get; set; } = new();
    }
}
