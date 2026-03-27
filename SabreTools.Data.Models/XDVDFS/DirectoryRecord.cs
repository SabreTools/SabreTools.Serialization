namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// Xbox DVD Filesystem Directory Record
    /// Padded with 0xFF to be a multiple of 4 bytes
    /// </summary>
    /// <see href="https://multimedia.cx/xdvdfs.html"/>
    /// <see href="https://github.dev/Deterous/XboxKit/"/>
    public class DirectoryRecord
    {
        /// <summary>
        /// Offset of left child directory record
        /// Unit is number of uints from start of directory descriptor
        /// If zero, no left child directory record exists
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort LeftChildOffset { get; set; }

        /// <summary>
        /// Offset of right child directory record
        /// Unit is number of uints from start of directory descriptor
        /// If zero, no right child directory record exists
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort RightChildOffset { get; set; }

        /// <summary>
        /// Sector offset into filesystem for the current record
        /// If record is a file, points to the first sector of file
        /// If record is a directory, points to first sector of directory descriptor
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public uint ExtentOffset { get; set; }

        /// <summary>
        /// Extent size into filesystem for the current record
        /// If record is a file, size of file in bytes
        /// If record is a directory, number of bytes in directory descriptor
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public uint ExtentSize { get; set; }

        /// <summary>
        /// File attributes of current record
        /// </summary>
        public FileFlags FileFlags { get; set; }

        /// <summary>
        /// Length in bytes of the following filename
        /// </summary>
        public byte FilenameLength { get; set; }

        /// <summary>
        /// Name of the record, encoded in single-byte per character
        /// </summary>
        public byte[] Filename { get; set; } = [];

        /// <summary>
        /// Padding to fill up remainder of uint32
        /// Not present if prior data is a multiple of 4 bytes
        /// <remarks>0-3 bytes, all 0xFF</remarks>
        public byte[]? Padding { get; set; }
    }
}
