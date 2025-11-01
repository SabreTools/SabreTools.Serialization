namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// This field MUST contain information about each of the certificates
    /// files MAY be signed with. When the Central Directory Encryption
    /// feature is enabled for a ZIP file, this record will appear in
    /// the Archive Extra Data Record, otherwise it will appear in the
    /// first central directory record and will be ignored in any
    /// other record.
    /// </summary>
    /// <remarks>Header ID = 0x0014</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class PKCS7Store : ExtensibleDataField
    {
        /// <summary>
        /// Data about the store
        /// </summary>
        public byte[] TData { get; set; } = [];
    }
}
