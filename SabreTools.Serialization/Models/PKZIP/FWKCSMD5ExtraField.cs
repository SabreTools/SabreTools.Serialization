namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The FWKCS Contents_Signature System, used in
    /// automatically identifying files independent of file name,
    /// optionally adds and uses an extra field to support the
    /// rapid creation of an enhanced contents_signature.
    ///
    /// When FWKCS revises a .ZIP file central directory to add
    /// this extra field for a file, it also replaces the
    /// central directory entry for that file's uncompressed
    /// file length with a measured value.
    ///
    /// FWKCS provides an option to strip this extra field, if
    /// present, from a .ZIP file central directory. In adding
    /// this extra field, FWKCS preserves .ZIP file Authenticity
    /// Verification; if stripping this extra field, FWKCS
    /// preserves all versions of AV through PKZIP version 2.04g.
    ///
    /// FWKCS, and FWKCS Contents_Signature System, are
    /// trademarks of Frederick W. Kantor.
    /// </summary>
    /// <remarks>Header ID = 0x4b46</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class FWKCSMD5ExtraField : ExtensibleDataField
    {
        /// <summary>
        /// "MD5"
        /// </summary>
        /// <remarks>3 bytes</remarks>
        public byte[] Preface { get; set; } = new byte[3];

        /// <summary>
        /// Uncompressed file's MD5 hash, low byte first
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] MD5 { get; set; } = new byte[16];
    }
}
