namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// In order to allow different programs and different types
    /// of information to be stored in the 'extra' field in .ZIP
    /// files, the following structure MUST be used for all
    /// programs storing data in this field
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public abstract class ExtensibleDataField
    {
        /// <summary>
        /// Header ID
        /// </summary>
        public HeaderID HeaderID { get; set; }

        /// <summary>
        /// Data Size
        /// </summary>
        public ushort DataSize { get; set; }
    }
}
