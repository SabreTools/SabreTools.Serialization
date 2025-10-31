namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// Archive extra data record
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class ArchiveExtraDataRecord
    {
        /// <summary>
        /// Archive extra data signature (0x08064B50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Extra field length
        /// </summary>
        public uint ExtraFieldLength { get; set; }

        /// <summary>
        /// Extra field data (variable size)
        /// </summary>
        public byte[] ExtraFieldData { get; set; }
    }
}
