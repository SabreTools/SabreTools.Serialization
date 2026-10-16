namespace SabreTools.Data.Models.UDF
{
    /// <summary>
    /// UDF Extent Descriptor (extent_ad, 3/7.1), an extent of logical sectors
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-167_3rd_edition_june_1997.pdf"/>
    public sealed class ExtentDescriptor
    {
        /// <summary>
        /// Length of the extent in bytes
        /// </summary>
        public uint ExtentLength { get; set; }

        /// <summary>
        /// Logical sector number of the start of the extent
        /// </summary>
        public uint ExtentLocation { get; set; }
    }
}
