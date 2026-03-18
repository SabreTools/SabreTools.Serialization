namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of a shortened variant of the
    /// ZipIt extra block for Macintosh (without "full name" entry).
    /// This variant is used by ZipIt 1.3.5 and newer for entries of
    /// files (not directories) that do not have a MacBinary encoded
    /// file. The local-header and central-header versions are identical.
    /// </summary>
    /// <remarks>Header ID = 0x2705</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class ZipItMacintoshShortFileExtraField : ExtensibleDataField
    {
        /// <summary>
        /// "ZPIT" - extra-field signature
        /// </summary>
        public uint ExtraFieldSignature { get; set; }

        /// <summary>
        /// Four-byte Mac file type string
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] FileType { get; set; } = new byte[4];

        /// <summary>
        /// Four-byte Mac creator string
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] Creator { get; set; } = new byte[4];

        /// <summary>
        /// Attributes from FInfo.frFlags, MAY be omitted
        /// </summary>
        public ushort? FdFlags { get; set; }

        /// <summary>
        /// Reserved, MAY be omitted
        /// </summary>
        public ushort? Reserved { get; set; }
    }
}
