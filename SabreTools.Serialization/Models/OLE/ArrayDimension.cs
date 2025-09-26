namespace SabreTools.Serialization.Models.OLE
{
    /// <summary>
    /// The ArrayDimension packet represents the size and index offset of a dimension of an array
    /// property type.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class ArrayDimension
    {
        /// <summary>
        /// An unsigned integer representing the size of the dimension.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// A signed integer representing the index offset of the dimension. For
        /// example, an array dimension that is to be accessed with a 0-based index would have the value
        /// zero, whereas an array dimension that is to be accessed with a 1-based index would have the
        /// value 0x00000001.
        /// </summary>
        public int Value { get; set; }
    }
}
