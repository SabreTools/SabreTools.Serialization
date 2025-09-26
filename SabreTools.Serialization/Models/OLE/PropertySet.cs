namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The PropertySet packet represents a property set.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class PropertySet
    {
        /// <summary>
        /// MUST be the total size in bytes of the PropertySet packet
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// An unsigned integer representing the number of properties in the
        /// property set.
        /// </summary>
        public uint NumProperties { get; set; }

        /// <summary>
        /// All PropertyIdentifierAndOffset fields MUST be a sequence of
        /// PropertyIdentifierAndOffset packets. The sequence MUST be in order of
        /// increasing value of the Offset field. Packets are not required to be
        /// in any particular order with regard to the value of the PropertyIdentifier
        /// field.
        /// </summary>
        public PropertyIdentifierAndOffset[]? PropertyIdentifierAndOffsets { get; set; }

        /// <summary>
        /// Each Property field is a sequence of property values, each of which MUST
        /// be represented by a TypedPropertyValue packet or a Dictionary packet in
        /// the special case of the Dictionary property.
        /// </summary>
        public object[]? Properties { get; set; }
    }
}
