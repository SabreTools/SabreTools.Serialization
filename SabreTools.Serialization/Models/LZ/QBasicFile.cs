namespace SabreTools.Data.Models.LZ
{
    /// <summary>
    /// LZ variant used in QBasic 4.5 installer
    /// </summary>
    /// <see href="https://www.cabextract.org.uk/libmspack/doc/szdd_kwaj_format.html"/>
    public sealed class QBasicFile
    {
        /// <summary>
        /// Header
        /// </summary>
        public QBasicHeader? Header { get; set; }

        // Followed immediately by compressed data
    }
}