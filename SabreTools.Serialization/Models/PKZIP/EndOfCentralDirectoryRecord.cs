namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// End of central directory record
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class EndOfCentralDirectoryRecord
    {
        /// <summary>
        /// End of central directory signature (0x06054B50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Number of this disk
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public ushort DiskNumber { get; set; }

        /// <summary>
        /// Number of the disk with the start of the central directory
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public ushort StartDiskNumber { get; set; }

        /// <summary>
        /// Total number of entries in the central directory on this disk
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public ushort TotalEntriesOnDisk { get; set; }

        /// <summary>
        /// Total number of entries in the central directory
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public ushort TotalEntries { get; set; }

        /// <summary>
        /// Size of the central directory
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFFFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public uint CentralDirectorySize { get; set; }

        /// <summary>
        /// Offset of start of central directory with respect to the
        /// starting disk number
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFFFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public uint CentralDirectoryOffset { get; set; }

        /// <summary>
        /// .ZIP file comment length
        /// </summary>
        public ushort FileCommentLength { get; set; }

        /// <summary>
        /// .ZIP file comment
        /// </summary>
        public string? FileComment { get; set; }
    }
}
