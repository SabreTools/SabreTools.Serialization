using System.Collections.Generic;

namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM disc image, made up of multiple data tracks, ISO 10149 / ECMA-130
    /// Specifically not a mixed-mode CD disc image, pure CD-ROM disc
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class CDROM
    {
        /// <summary>
        /// CD-ROM data tracks
        /// </summary>
        public DataTrack[] Tracks { get; set; } = [];
    }
}
