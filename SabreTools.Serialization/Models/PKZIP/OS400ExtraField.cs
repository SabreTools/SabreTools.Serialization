namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// The following is the layout of the OS/400 "extra" block.
    /// </summary>
    /// <remarks>Header ID = 0x0065</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class OS400ExtraField : ExtensibleDataField
    {
        /// <summary>
        /// EBCDIC "Z390" 0xE9F3F9F0
        /// or "T4MV" for TargetFour
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// Attribute data (see APPENDIX A)
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
