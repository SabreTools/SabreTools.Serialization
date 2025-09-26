namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// This field contains the information about which certificate in
    /// the PKCS#7 store was used to sign a particular file. It also
    /// contains the signature data. This field can appear multiple
    /// times, but can only appear once per certificate.
    /// </summary>
    /// <remarks>Header ID = 0x0015</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class X509IndividualFile : ExtensibleDataField
    {
        /// <summary>
        /// Signature Data
        /// </summary>
        public byte[]? TData { get; set; }
    }
}
