namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the Policy Decryption Key "extra" block.
    /// TData is a variable length, variable content field.  It holds
    /// information about encryptions and/or encryption key sources.
    /// Contact PKWARE for information on current TData structures.
    /// Information in this "extra" block may aternatively be placed
    /// within comment fields.  Refer to the section in this document
    /// entitled "Incorporating PKWARE Proprietary Technology into Your
    /// Product" for more information.
    /// </summary>
    /// <remarks>Header ID = 0x0021</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class PolicyDecryptionKeyRecordExtraField : ExtensibleDataField
    {
        /// <summary>
        /// Data about the key
        /// </summary>
        public byte[] TData { get; set; }
    }
}
