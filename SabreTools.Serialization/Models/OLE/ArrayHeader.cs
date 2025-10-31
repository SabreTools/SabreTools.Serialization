namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The ArrayHeader packet represents the type and dimensions of an array property type.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class ArrayHeader
    {
        /// <summary>
        /// MUST be set to the value obtained by clearing the VT_ARRAY (0x2000) bit of
        /// this array property's PropertyType value.
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        /// An unsigned integer representing the number of dimensions in the array
        /// property. MUST be at least 1 and at most 31.
        /// </summary>
        public uint NumDimensions { get; set; }

        /// <summary>
        /// MUST be a sequence of ArrayDimension packets
        ///
        /// The number of scalar values in an array property can be calculated from the ArrayHeader packet
        /// as the product of the Size fields of each of the ArrayDimension packets.
        /// </summary>
        public ArrayDimension[] Dimensions { get; set; }
    }
}
