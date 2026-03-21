namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The PropertyIdentifierAndOffset packet is used in the PropertySet packet to represent a
    /// property identifier and the byte offset of the property in the PropertySet packet
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class PropertyIdentifierAndOffset
    {
        /// <summary>
        /// An unsigned integer representing the property identifier of a property
        /// in the property set. MUST be a valid PropertyIdentifier value.
        /// </summary>
        public PropertyIdentifier PropertyIdentifier { get; set; }

        /// <summary>
        /// An unsigned integer representing the offset in bytes from the beginning
        /// of the PropertySet packet to the beginning of the Property field for the
        /// property represented. MUST be a multiple of 4 bytes.
        /// </summary>
        public uint Offset { get; set; }
    }
}
