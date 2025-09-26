namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of a shortened variant of the
    /// ZipIt extra block for Macintosh used only for directory
    /// entries. This variant is used by ZipIt 1.3.5 and newer to 
    /// save some optional Mac-specific information about directories.
    /// The local-header and central-header versions are identical.
    /// </summary>
    /// <remarks>Header ID = 0x2805</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class ZipItMacintoshShortDirectoryExtraField : ExtensibleDataField
    {
        /// <summary>
        /// "ZPIT" - extra-field signature
        /// </summary>
        public uint ExtraFieldSignature { get; set; }

        /// <summary>
        /// attributes from DInfo.frFlags, MAY be omitted
        /// </summary>
        public ushort? FrFlags { get; set; }

        /// <summary>
        /// ZipIt view flag, MAY be omitted
        /// </summary>
        public ZipItInternalSettings? View { get; set; }
    }
}
