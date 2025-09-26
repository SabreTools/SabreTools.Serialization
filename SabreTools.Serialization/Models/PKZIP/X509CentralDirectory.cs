namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// This field contains the information about which certificate in
    /// the PKCS#7 store was used to sign the central directory structure.
    /// When the Central Directory Encryption feature is enabled for a
    /// ZIP file, this record will appear in the Archive Extra Data Record,
    /// otherwise it will appear in the first central directory record.
    /// </summary>
    /// <remarks>Header ID = 0x0016</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class X509CentralDirectory : ExtensibleDataField
    {
        /// <summary>
        /// Data
        /// </summary>
        public byte[]? TData { get; set; }
    }
}
