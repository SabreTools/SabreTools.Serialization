namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// Xbox DVD Filesystem Directory Descriptor
    /// The descriptor is stored as a binary tree, left being alphabetically smaller, right larger
    /// Padded with 0xFF to be a multiple of 2048 bytes
    /// </summary>
    /// <see href="https://multimedia.cx/xdvdfs.html"/>
    /// <see href="https://github.com/Deterous/XboxKit/"/>
    public class DirectoryDescriptor
    {
        /// <summary>
        /// List of directory records
        /// </summary>
        public DirectoryRecord[] DirectoryRecords { get; set; } = [];

        /// <summary>
        /// Padding to fill up remainder of sector
        /// Not present if prior data is a multiple of 2048 bytes
        /// <remarks>All 0xFF</remarks>
        public byte[]? Padding { get; set; }
    }
}
