namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// Enum for a CD-ROM's sector mode
    /// Explicitly does not contain non-CD-ROM modes like AUDIO, CDG, CDI, and length-specific modes
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
    public enum SectorMode
    {
        /// <summary>
        /// CD-ROM Unknown Mode
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// CD-ROM Mode 0 (All bytes after header are 0x00)
        /// </summary>
        MODE0,

        /// <summary>
        /// CD-ROM Mode 1
        /// </summary>
        MODE1,

        /// <summary>
        /// CD-ROM Mode 2 (Formless)
        /// </summary>
        MODE2,

        /// <summary>
        /// CD-ROM XA Mode 2 Form 1
        /// </summary>
        MODE2_FORM1,

        /// <summary>
        /// CD-ROM XA Mode 2 Form 2
        /// </summary>
        MODE2_FORM2,
    }
}
