namespace SabreTools.Data.Models.LZ
{
    /// <summary>
    /// LZ variant with variable compression
    /// </summary>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    public sealed class KWAJFile
    {
        /// <summary>
        /// Header
        /// </summary>
        public KWAJHeader Header { get; set; }

        /// <summary>
        /// Optional extensions defined by <see cref="KWAJHeader.HeaderFlags"/>
        /// </summary>
        public KWAJHeaderExtensions HeaderExtensions { get; set; }

        // Followed immediately by compressed data
    }
}
