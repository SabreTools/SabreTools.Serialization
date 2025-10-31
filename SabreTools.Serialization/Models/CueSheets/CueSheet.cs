/// <remarks>
/// Information sourced from http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf
/// </remarks>
namespace SabreTools.Data.Models.CueSheets
{
    /// <summary>
    /// Represents a single cuesheet
    /// </summary>
    public class CueSheet
    {
        /// <summary>
        /// CATALOG
        /// </summary>
        public string? Catalog { get; set; }

        /// <summary>
        /// CDTEXTFILE
        /// </summary>
        public string? CdTextFile { get; set; }

        /// <summary>
        /// PERFORMER
        /// </summary>
        public string? Performer { get; set; }

        /// <summary>
        /// SONGWRITER
        /// </summary>
        public string? Songwriter { get; set; }

        /// <summary>
        /// TITLE
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// List of FILE in cuesheet
        /// </summary>
        public CueFile[] Files { get; set; }
    }
}
