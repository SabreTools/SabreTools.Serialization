namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// PKZIP local file
    /// </summary>
    /// <see href="https://petlibrary.tripod.com/ZIP.HTM"/>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class LocalFile
    {
        /// <summary>
        /// Local file header
        /// </summary>
        public LocalFileHeader LocalFileHeader { get; set; }

        /// <summary>
        /// Encryption header
        /// </summary>
        /// TODO: Determine the model for the encryption headers
        public byte[] EncryptionHeaders { get; set; }

        /// <summary>
        /// File data, appears after the encryption header
        /// if it exists or after the local file header otherwise
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Data descriptors, appears after the file data
        /// </summary>
        /// <remarks>Cannot exist if <see cref="ZIP64DataDescriptor"/> is populated</remarks>
        public DataDescriptor? DataDescriptor { get; set; }

        /// <summary>
        /// ZIP64 Data descriptors, appears after the file data
        /// </summary>
        /// <remarks>Cannot exist if <see cref="DataDescriptor"/> is populated</remarks>
        public DataDescriptor64? ZIP64DataDescriptor { get; set; }

    }
}
