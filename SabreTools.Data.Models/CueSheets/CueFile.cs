/// <remarks>
/// Information sourced from http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf
/// </remarks>
namespace SabreTools.Data.Models.CueSheets
{
    /// <summary>
    /// Represents a single FILE in a cuesheet
    /// </summary>
    public class CueFile
    {
        /// <summary>
        /// filename
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// filetype
        /// </summary>
        public CueFileType FileType { get; set; }

        /// <summary>
        /// Text after the REM keyword of each REM line between the FILE line and its first INDEX or TRACK
        /// </summary>
        public string[] Remarks { get; set; } = [];

        /// <summary>
        /// List of INDEX between the FILE line and its first TRACK, which belong to the last TRACK of the previous FILE
        /// </summary>
        /// <remarks>Written by audio rippers that put the gap before a track at the end of the previous file</remarks>
        public CueIndex[] Indices { get; set; } = [];

        /// <summary>
        /// List of TRACK in FILE
        /// </summary>
        public CueTrack[] Tracks { get; set; } = [];
    }
}
