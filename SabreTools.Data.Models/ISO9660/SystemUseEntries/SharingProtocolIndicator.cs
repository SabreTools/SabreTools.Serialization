namespace SabreTools.Data.Models.ISO9660.SystemUseEntries
{
    /// <summary>
    /// "SP" System Use Sharing Protocol Indicator, shows that the directory hierarchy uses SUSP
    /// </summary>
    /// <remarks>
    /// Recorded at the start of the System Use field of the "." record of the root directory,
    /// or after the CD-ROM XA system use information (byte 15) on a CD-ROM XA disc
    /// </remarks>
    /// <see href="http://www.o3one.org/hwdocs/cdrom_formats/susp112.doc"/>
    public sealed class SharingProtocolIndicator : BaseEntry
    {
        /// <summary>
        /// Check bytes, 0xBE 0xEF
        /// </summary>
        /// <remarks>2 bytes</remarks>
        public byte[] CheckBytes { get; set; } = new byte[2];

        /// <summary>
        /// Number of bytes skipped at the start of the System Use field of every other directory record
        /// before its System Use Entries (LEN_SKP)
        /// </summary>
        public byte BytesSkipped { get; set; }
    }
}
