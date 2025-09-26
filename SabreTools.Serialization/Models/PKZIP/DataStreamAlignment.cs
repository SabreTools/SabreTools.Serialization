namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// (per Zbynek Vyskovsky) Defines alignment of data stream of this
    /// entry within the zip archive.  Additionally, indicates whether the
    /// compression method should be kept when re-compressing the zip file.
    /// 
    /// The purpose of this extra field is to align specific resources to
    /// word or page boundaries so they can be easily mapped into memory.
    /// 
    /// The alignment field (lower 15 bits) defines the minimal alignment
    /// required by the data stream.   Bit 15 of alignment field indicates
    /// whether the compression method of this entry can be changed when
    /// recompressing the zip file.  The value 0 means the compression method
    /// should not be changed.  The value 1 indicates  the compression method
    /// may be changed. The padding field contains padding to ensure the correct
    /// alignment.  It can be changed at any time when the offset or required
    /// alignment changes. (see https://issues.apache.org/jira/browse/COMPRESS-391)
    /// </summary>
    /// <remarks>Header ID = 0xa11e</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class DataStreamAlignment : ExtensibleDataField
    {
        /// <summary>
        /// Required alignment and indicator
        /// </summary>
        public ushort Alignment { get; set; }

        /// <summary>
        /// 0x00-padding
        /// </summary>
        public byte[]? Padding { get; set; }
    }
}
