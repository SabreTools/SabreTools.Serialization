using SabreTools.Data.Models.ISO9660;

namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM data track containing a ISO9660 / ECMA-119 filesystem
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public sealed class DataTrack
    {
        /// <summary>
        /// CD-ROM data sectors
        /// </summary>
        public Sector[] Sectors { get; set; } = [];

        /// <summary>
        /// ISO9660 volume within the data track
        /// </summary>
        public Volume Volume { get; set; }
    }
}
