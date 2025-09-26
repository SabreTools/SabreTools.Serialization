namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the ZipIt extra block
    /// for Macintosh. The local-header and central-header versions
    /// are identical. This block MUST be present if the file is
    /// stored MacBinary-encoded and it SHOULD NOT be used if the file
    /// is not stored MacBinary-encoded.
    /// </summary>
    /// <remarks>Header ID = 0x2605</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class ZipItMacintoshExtraField : ExtensibleDataField
    {
        /// <summary>
        /// "ZPIT" - extra-field signature
        /// </summary>
        public uint ExtraFieldSignature { get; set; }

        /// <summary>
        /// Length of FileName
        /// </summary>
        public byte FnLen { get; set; }

        /// <summary>
        /// Full Macintosh filename
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Four-byte Mac file type string
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[]? FileType { get; set; }

        /// <summary>
        /// Four-byte Mac creator string
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[]? Creator { get; set; }
    }
}
