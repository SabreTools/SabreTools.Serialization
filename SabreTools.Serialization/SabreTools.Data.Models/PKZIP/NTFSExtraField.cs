namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the NTFS attributes
    /// "extra" block. (Note: At this time the Mtime, Atime
    /// and Ctime values MAY be used on any WIN32 system.)
    /// </summary>
    /// <remarks>Header ID = 0x000A</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class NTFSExtraField : ExtensibleDataField
    {
        /// <summary>
        /// Reserved for future use
        /// </summary>
        public uint Reserved { get; set; }

        /// <summary>
        /// NTFS attribute tags
        /// </summary>
        public TagSizeVar[] TagSizeVars { get; set; } = [];
    }
}
