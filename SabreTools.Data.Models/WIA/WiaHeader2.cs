namespace SabreTools.Data.Models.WIA
{
    /// <summary>
    /// WIA / RVZ second header (0xDC bytes).
    /// Immediately follows WiaHeader1 in the file.
    /// All multi-byte fields are big-endian on disk; the reader converts to host order.
    /// </summary>
    public sealed class WiaHeader2
    {
        /// <summary>
        /// Disc type: 1 = GameCube, 2 = Wii
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public WiaDiscType DiscType { get; set; }

        /// <summary>
        /// Compression algorithm applied to group data
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public WiaRvzCompressionType CompressionType { get; set; }

        /// <summary>
        /// Informational compression level used when writing (1-9)
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int CompressionLevel { get; set; }

        /// <summary>
        /// Group / chunk size in bytes.
        /// WIA requires exactly 2 MiB; RVZ accepts powers of 2 between 32 KiB and 2 MiB.
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint ChunkSize { get; set; }

        /// <summary>
        /// First 0x80 bytes of the disc image (unencrypted disc header)
        /// </summary>
        /// <remarks>0x80 bytes</remarks>
        public byte[] DiscHeader { get; set; } = new byte[0x80];

        /// <summary>
        /// Number of PartitionEntry structures that follow the raw-data entries
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NumberOfPartitionEntries { get; set; }

        /// <summary>
        /// Size of each PartitionEntry in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint PartitionEntrySize { get; set; }

        /// <summary>
        /// File offset of the PartitionEntry array
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong PartitionEntriesOffset { get; set; }

        /// <summary>
        /// SHA-1 hash of all PartitionEntry data
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] PartitionEntriesHash { get; set; } = new byte[20];

        /// <summary>
        /// Number of RawDataEntry structures
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NumberOfRawDataEntries { get; set; }

        /// <summary>
        /// File offset of the RawDataEntry array
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong RawDataEntriesOffset { get; set; }

        /// <summary>
        /// Total size in bytes of all RawDataEntry structures
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint RawDataEntriesSize { get; set; }

        /// <summary>
        /// Number of group entries (WiaGroupEntry or RvzGroupEntry)
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint NumberOfGroupEntries { get; set; }

        /// <summary>
        /// File offset of the group-entry array
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public ulong GroupEntriesOffset { get; set; }

        /// <summary>
        /// Total size in bytes of all group entries
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint GroupEntriesSize { get; set; }

        /// <summary>
        /// Number of valid bytes in <see cref="CompressorData"/>
        /// </summary>
        public byte CompressorDataSize { get; set; }

        /// <summary>
        /// Algorithm-specific compressor parameters (up to 7 bytes).
        /// LZMA: 5-byte prop block. LZMA2: 1-byte dict-size code. Others: unused.
        /// </summary>
        /// <remarks>7 bytes</remarks>
        public byte[] CompressorData { get; set; } = new byte[7];
    }
}
