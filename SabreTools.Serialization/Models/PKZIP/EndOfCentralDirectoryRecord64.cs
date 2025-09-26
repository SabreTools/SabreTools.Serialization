namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// Zip64 end of central directory record
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class EndOfCentralDirectoryRecord64
    {
        /// <summary>
        /// ZIP64 end of central directory signature (0x06064B50)
        /// </summary>
        public uint Signature { get; set; }

        /// <summary>
        /// Size of ZIP64 end of central directory record
        /// </summary>
        /// <remarks>SizeOfFixedFields + SizeOfVariableData - 12</remarks>
        public ulong DirectoryRecordSize { get; set; }

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
        /// Number of this disk
        /// </summary>
        public uint DiskNumber { get; set; }

        /// <summary>
        /// Number of the disk with the start of the central directory
        /// </summary>
        public uint StartDiskNumber { get; set; }

        /// <summary>
        /// Total number of entries in the central directory on this disk
        /// </summary>
        public ulong TotalEntriesOnDisk { get; set; }

        /// <summary>
        /// Total number of entries in the central directory
        /// </summary>
        public ulong TotalEntries { get; set; }

        /// <summary>
        /// Size of the central directory
        /// </summary>
        public ulong CentralDirectorySize { get; set; }

        /// <summary>
        /// Offset of start of central directory with respect to the
        /// starting disk number
        /// </summary>
        public ulong CentralDirectoryOffset { get; set; }

        /// <summary>
        /// ZIP64 extensible data sector
        /// </summary>
        public byte[]? ExtensibleDataSector { get; set; }
    }
}
