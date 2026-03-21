namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// Central directory file header
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/>
    public class CentralDirectoryFileHeader
    {
        /// <summary>
        /// Central file header signature (0x02014B50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Host system on which the file attributes are compatible
        /// </summary>
        public HostSystem HostSystem { get; set; }

        /// <summary>
        /// ZIP specification version
        /// </summary>
        public byte VersionMadeBy { get; set; }

        /// <summary>
        /// Version needed to extract
        /// </summary>
        /// <remarks>TODO: Add mapping of versions</remarks>
        public ushort VersionNeededToExtract { get; set; }

        /// <summary>
        /// General purpose bit flag
        /// </summary>
        public GeneralPurposeBitFlags Flags { get; set; }

        /// <summary>
        /// Compression method
        /// </summary>
        public CompressionMethod CompressionMethod { get; set; }

        /// <summary>
        /// Last modified file time
        /// </summary>
        public ushort LastModifedFileTime { get; set; }

        /// <summary>
        /// Last modified file date
        /// </summary>
        public ushort LastModifiedFileDate { get; set; }

        /// <summary>
        /// CRC-32
        /// </summary>
        public uint CRC32 { get; set; }

        /// <summary>
        /// Compressed size
        /// </summary>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// Uncompressed size
        /// </summary>
        public uint UncompressedSize { get; set; }

        /// <summary>
        /// File name length
        /// </summary>
        public ushort FileNameLength { get; set; }

        /// <summary>
        /// Extra field length
        /// </summary>
        public ushort ExtraFieldLength { get; set; }

        /// <summary>
        /// File comment length
        /// </summary>
        public ushort FileCommentLength { get; set; }

        /// <summary>
        /// Disk number start
        /// </summary>
        public ushort DiskNumberStart { get; set; }

        /// <summary>
        /// Internal file attributes
        /// </summary>
        public InternalFileAttributes InternalFileAttributes { get; set; }

        /// <summary>
        /// External file attributes
        /// </summary>
        public uint ExternalFileAttributes { get; set; }

        /// <summary>
        /// Relative offset of local header
        /// </summary>
        /// <remarks>
        /// If ZIP64 and value is 0xFFFFFFFF, size will be in the
        /// extended information extra field
        /// </remarks>
        public uint RelativeOffsetOfLocalHeader { get; set; }

        /// <summary>
        /// File name (variable size)
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Extra fields (variable size)
        /// </summary>
        public ExtensibleDataField[]? ExtraFields { get; set; }

        /// <summary>
        /// File comment (variable size)
        /// </summary>
        public string? FileComment { get; set; }
    }
}
