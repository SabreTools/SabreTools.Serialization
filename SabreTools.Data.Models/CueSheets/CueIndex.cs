/// <remarks>
/// Information sourced from http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf
/// </remarks>
namespace SabreTools.Data.Models.CueSheets
{
    /// <summary>
    /// Represents a single INDEX in a TRACK
    /// </summary>
    public class CueIndex
    {
        /// <summary>
        /// INDEX number, between 0 and 99
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Starting time of INDEX in minutes
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Starting time of INDEX in seconds
        /// </summary>
        /// <remarks>There are 60 seconds in a minute</remarks>
        public int Seconds { get; set; }

        /// <summary>
        /// Starting time of INDEX in frames.
        /// </summary>
        /// <remarks>There are 75 frames per second</remarks>
        public int Frames { get; set; }
    }
}
