namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// Represents a single ZIP/ZIP64 archive
    /// </summary>
    /// <remarks>PKZIP archives are meant to be read from the end</remarks>
    public class Archive
    {
        /// <summary>
        /// Local file entries
        /// </summary>
        public LocalFile[]? LocalFiles { get; set; }

        /// <summary>
        /// Optional archive decryption header, appears after all entries
        /// </summary>
        /// TODO: Determine the model
        public byte[] ArchiveDecryptionHeader { get; set; } = [];

        /// <summary>
        /// Optional archive extra data record, appears after either
        /// the archive decryption header or after all entries
        /// </summary>
        public ArchiveExtraDataRecord? ArchiveExtraDataRecord { get; set; }

        /// <summary>
        /// Central directory headers in sequential order
        /// </summary>
        public CentralDirectoryFileHeader[]? CentralDirectoryHeaders { get; set; }

        /// <summary>
        /// Optional ZIP64 end of central directory record
        /// </summary>
        public EndOfCentralDirectoryRecord64? ZIP64EndOfCentralDirectoryRecord { get; set; }

        /// <summary>
        /// Optional ZIP64 end of central directory locator
        /// </summary>
        public EndOfCentralDirectoryLocator64? ZIP64EndOfCentralDirectoryLocator { get; set; }

        /// <summary>
        /// Required end of central directory record, always must be last
        /// </summary>
        public EndOfCentralDirectoryRecord? EndOfCentralDirectoryRecord { get; set; }
    }
}
