namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The TypedPropertyValue structure represents the typed value of a property in a property set
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class TypedPropertyValue
    {
        /// <summary>
        /// MUST be a value from the PropertyType enumeration, indicating the type of
        /// property represented.
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        /// MUST be set to zero, and any nonzero value SHOULD be rejected
        /// </summary>
        public ushort Padding { get; set; }

        /// <summary>
        /// MUST be the value of the property represented and serialized according to
        /// the value of Type as follows.
        /// </summary>
        /// <remarks>See documentation for required lengths</remarks>
        public byte[]? Value { get; set; }
    }
}
