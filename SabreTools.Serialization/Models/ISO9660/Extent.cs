namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 filesystem extent
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public abstract class Extent
    {
        /// <summary>
        /// Entry is either a DirectoryExtent or a FileExtent
        /// </summary>
        public byte Type { get; set; }
    }
}
