namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The UnicodeString packet represents a Unicode string.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class UnicodeString
    {
        /// <summary>
        /// The length in 16-bit Unicode characters of the <see cref="Characters"/> field, including the null
        /// terminator, but not including padding (if any).
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// If <see cref="Length"/> is zero, this field MUST be zero bytes in length. If <see cref="Length"/> is
        /// nonzero, this field MUST be a null-terminated array of 16-bit Unicode characters, followed by zero
        /// padding to a multiple of 4 bytes. The string represented by this field SHOULD NOT contain
        /// embedded or additional trailing null characters.
        /// </summary>
        public string Characters { get; set; } = string.Empty;
    }
}
