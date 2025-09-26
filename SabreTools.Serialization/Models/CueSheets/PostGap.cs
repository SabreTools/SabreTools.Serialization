/// <remarks>
/// Information sourced from http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf
/// </remarks>
namespace SabreTools.Data.Models.CueSheets
{
    /// <summary>
    /// Represents POSTGAP information of a track
    /// </summary>
    public class PostGap
    {
        /// <summary>
        /// Length of POSTGAP in minutes
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Length of POSTGAP in seconds
        /// </summary>
        /// <remarks>There are 60 seconds in a minute</remarks>
        public int Seconds { get; set; }

        /// <summary>
        /// Length of POSTGAP in frames.
        /// </summary>
        /// <remarks>There are 75 frames per second</remarks>
        public int Frames { get; set; }
    }
}
