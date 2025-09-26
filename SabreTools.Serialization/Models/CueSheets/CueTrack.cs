/// <remarks>
/// Information sourced from http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf
/// </remarks>
namespace SabreTools.Data.Models.CueSheets
{
    /// <summary>
    /// Represents a single TRACK in a FILE
    /// </summary>
    public class CueTrack
    {
        /// <summary>
        /// Track number. The range is 1 to 99.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Track datatype
        /// </summary>
        public CueTrackDataType DataType { get; set; }

        /// <summary>
        /// FLAGS
        /// </summary>
        public CueTrackFlag Flags { get; set; }

        /// <summary>
        /// ISRC
        /// </summary>
        /// <remarks>12 characters in length</remarks>
        public string? ISRC { get; set; }

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
        /// PREGAP
        /// </summary>
        public PreGap? PreGap { get; set; }

        /// <summary>
        /// List of INDEX in TRACK
        /// </summary>
        /// <remarks>Must start with 0 or 1 and then sequential</remarks>
        public CueIndex[]? Indices { get; set; }

        /// <summary>
        /// POSTGAP
        /// </summary>
        public PostGap? PostGap { get; set; }
    }
}
