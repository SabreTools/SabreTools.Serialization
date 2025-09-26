namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// See the section describing the Strong Encryption Specification
    /// for details.  Refer to the section in this document entitled
    /// "Incorporating PKWARE Proprietary Technology into Your Product"
    /// for more information.
    /// </summary>
    /// <remarks>Header ID = 0x0017</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class StrongEncryptionHeader : ExtensibleDataField
    {
        /// <summary>
        /// Format definition for this record
        /// </summary>
        public ushort Format { get; set; }

        /// <summary>
        /// Encryption algorithm identifier
        /// </summary>
        public ushort AlgID { get; set; }

        /// <summary>
        /// Bit length of encryption key
        /// </summary>
        public ushort Bitlen { get; set; }

        /// <summary>
        /// Processing flags
        /// </summary>
        public ushort Flags { get; set; }

        /// <summary>
        /// Certificate decryption extra field data
        /// </summary>
        /// <remarks>
        /// Refer to the explanation for CertData
        /// in the section describing the
        /// Certificate Processing Method under
        /// the Strong Encryption Specification
        /// </remarks>
        public byte[]? CertData { get; set; }
    }
}
