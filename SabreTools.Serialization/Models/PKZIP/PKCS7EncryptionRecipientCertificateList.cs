namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// This field MAY contain information about each of the certificates
    /// used in encryption processing and it can be used to identify who is
    /// allowed to decrypt encrypted files.  This field SHOULD only appear
    /// in the archive extra data record. This field is not required and
    /// serves only to aid archive modifications by preserving public
    /// encryption key data. Individual security requirements may dictate
    /// that this data be omitted to deter information exposure.
    /// 
    /// See the section describing the Strong Encryption Specification
    /// for details.  Refer to the section in this document entitled
    /// "Incorporating PKWARE Proprietary Technology into Your Product"
    /// for more information.
    /// </summary>
    /// <remarks>Header ID = 0x0019</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class PKCS7EncryptionRecipientCertificateList : ExtensibleDataField
    {
        /// <summary>
        /// Format version number - MUST be 0x0001 at this time
        /// </summary>
        public ushort Version { get; set; }

        /// <summary>
        /// PKCS#7 data blob
        /// </summary>
        public byte[]? CStore { get; set; }
    }
}
