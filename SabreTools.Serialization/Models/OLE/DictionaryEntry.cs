namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The DictionaryEntry packet represents a mapping between a property identifier and a
    /// property name
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class DictionaryEntry
    {
        /// <summary>
        /// An unsigned integer representing a property identifier. MUST be a valid
        /// PropertyIdentifier value in the range 0x00000002 to 0x7FFFFFFF, inclusive
        /// (this specifically excludes the property identifiers for any of the special
        /// properties specified in section 2.18).
        /// </summary>
        public PropertyIdentifier PropertyIdentifier { get; set; }

        /// <summary>
        /// If the property set's CodePage property has the value CP_WINUNICODE (0x04B0),
        /// MUST be the length of the Name field in 16-bit Unicode characters, including
        /// the null terminator but not including padding (if any). Otherwise, MUST be the
        /// length of the Name field in 8-bit characters, including the null terminator.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// If the property set's CodePage property has the value CP_WINUNICODE (0x04B0),
        /// MUST be a null-terminated array of 16-bit Unicode characters, followed by zero
        /// padding to a multiple of 4 bytes. Otherwise, MUST be a null-terminated array of
        /// 8-bit characters from the code page identified by the CodePage property and MUST
        /// NOT be padded.
        /// </summary>
        public string Name { get; set; }
    }
}
