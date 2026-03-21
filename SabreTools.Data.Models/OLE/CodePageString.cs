namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The CodePageString packet represents a string whose encoding depends on the
    /// value of the property set's CodePage property.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class CodePageString
    {
        /// <summary>
        /// The size in bytes of the <see cref="Characters"/> field, including the null terminator,
        /// but not including padding (if any). If the property set's CodePage property
        /// has the value CP_WINUNICODE (0x04B0), then the value MUST be a multiple of 2
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// If <see cref="Size"/> is zero, this field MUST be zero bytes in length. If <see cref="Size"/>
        /// is nonzero and the CodePage property set's CodePage property has the value CP_WINUNICODE (0x04B0),
        /// then the value MUST be a null-terminated array of 16-bit Unicode characters, followed by zero
        /// padding to a multiple of 4 bytes. If <see cref="Size"/> is nonzero and the property set's CodePage
        /// property has any other value, it MUST be a null-terminated array of 8-bit characters from the code
        /// page identified by the CodePage property, followed by zero padding to a multiple of 4 bytes. The
        /// string represented by this field MAY contain embedded or additional trailing null characters and
        /// an OLEPS implementation MUST be able to handle such strings. However, the manner in which
        /// strings with embedded or additional trailing null characters are presented by the implementation
        /// to an application is implementation-specific. For maximum interoperability, an OLEPS
        /// implementation SHOULD NOT write strings with embedded or trailing null characters unless
        /// specifically requested to do so by an application.
        /// </summary>
        public string Characters { get; set; } = string.Empty;
    }
}
