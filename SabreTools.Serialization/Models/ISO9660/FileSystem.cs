namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 / EMCA-119 file system composed of a set of volumes (set of disc images)
    /// Files may be spread across volumes (disc images), or be contained entirely within a single volume (disc image)
    /// </summary>
    /// <see cref="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public sealed class FileSystem
    {
        /// <summary>
        /// Set of volumes (disc images) that make up an ISO9660 file system
        /// </summary>
        public Volume[]? VolumeSet { get; set; }
    }
}
