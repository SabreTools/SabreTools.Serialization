namespace SabreTools.Data.Models.AdvancedInstaller
{
    /// <summary>
    /// Represents the structure at the end of a Caphyon
    /// Advanced Installer SFX file. These SFX files store
    /// all files uncompressed sequentially in the overlay
    /// of an executable.
    /// 
    /// The design is similar to the end of central directory
    /// in a PKZIP file. The footer needs to be read before
    /// the entry table as both the pointer to the start of
    /// the table as well as the entry count are included there.
    /// 
    /// The layout of this is derived from the layout in the
    /// physical file.
    /// </summary>
    public class SFX
    {
        /// <summary>
        /// Set of file entries
        /// </summary>
        public FileEntry[]? Entries { get; set; }

        /// <summary>
        /// Footer representing the central directory
        /// </summary>
        public Footer? Footer { get; set; }
    }
}
