namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// A CD-ROM sector
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public abstract class Sector
    {
        /// <summary>
        /// Sync pattern, 12 bytes
        /// </summary>
        public byte[] SyncPattern { get; set; } = new byte[12];

        /// <summary>
        /// Sector Address, 3 bytes
        /// </summary>
        public byte[] Address { get; set; } = new byte[3];

        /// <summary>
        /// CD-ROM mode
        /// </summary>
        public byte Mode { get; set; }
    }
}
