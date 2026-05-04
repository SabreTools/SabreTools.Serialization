namespace SabreTools.Data.Models.XRD
{
    public class DirectoryEntry
    {
        /// <summary>
        /// Sector offset of descriptor in XDVDFS filesystem
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Size of descriptor in sectors
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// SHA-1 Hash of file content
        /// </summary>
        public XDVDFS.DirectoryDescriptor DirectoryDescriptor { get; set; } = new();
    }
}
