namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// Special purpose data for ZIP64 extensible data field
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class SpecialPurposeDataHeader
    {
        /// <summary>
        /// Header ID (data type)
        /// </summary>
        public HeaderID HeaderID { get; set; }

        /// <summary>
        /// Data size
        /// </summary>
        public uint DataSize { get; set; }

        /// <summary>
        /// Data (variable size)
        /// </summary>
        /// TODO: Implement models for all extra field types - 4.5.3
        public byte[] Data { get; set; }
    }
}
