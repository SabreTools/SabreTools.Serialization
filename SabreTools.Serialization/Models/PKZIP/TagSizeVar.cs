namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// This represents a common tag/size/data combination
    /// structure that is used by multiple extra field types.
    /// </summary>
    public class TagSizeVar
    {
        /// <summary>
        /// Attribute tag
        /// </summary>
        public ushort Tag { get; set; }

        /// <summary>
        /// Attribute size
        /// </summary>
        public ushort Size { get; set; }

        /// <summary>
        /// Variable-length data
        /// </summary>
        public byte[]? Var { get; set; }
    }
}
