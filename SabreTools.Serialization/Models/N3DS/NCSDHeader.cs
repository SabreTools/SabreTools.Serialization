namespace SabreTools.Data.Models.N3DS
{
    /// <summary>
    /// There are two known specialisations of the NCSD container format:
    /// - The CTR Cart Image (CCI) format, the 3DS' raw NAND format
    /// - CCI is the format of game ROM images.
    ///
    /// CTR System Update (CSU) is a variant of CCI, where the only difference
    /// is in the file extension.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/NCSD"/>
    public sealed class NCSDHeader
    {
        #region Common to all NCSD files

        /// <summary>
        /// RSA-2048 SHA-256 signature of the NCSD header
        /// </summary>
        /// <remarks>0x100 bytes</remarks>
        public byte[] RSA2048Signature { get; set; } = new byte[0x100];

        /// <summary>
        /// Magic Number 'NCSD'
        /// </summary>
        public string MagicNumber { get; set; } = string.Empty;

        /// <summary>
        /// Size of the NCSD image, in media units (1 media unit = 0x200 bytes)
        /// </summary>
        public uint ImageSizeInMediaUnits { get; set; }

        /// <summary>
        /// Media ID
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public byte[] MediaId { get; set; } = new byte[8];

        /// <summary>
        /// Partitions FS type (0=None, 1=Normal, 3=FIRM, 4=AGB_FIRM save)
        /// </summary>
        public FilesystemType PartitionsFSType { get; set; }

        /// <summary>
        /// Partitions crypt type (each byte corresponds to a partition in the partition table)
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public byte[] PartitionsCryptType { get; set; } = new byte[8];

        /// <summary>
        /// Offset & Length partition table, in media units
        /// </summary>
        public PartitionTableEntry[] PartitionsTable { get; set; } = [];

        #endregion

        #region CTR Cart Image (CCI) Specific

        /// <summary>
        /// Exheader SHA-256 hash
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        public byte[] ExheaderHash { get; set; } = new byte[0x200];

        /// <summary>
        /// Additional header size
        /// </summary>
        public uint AdditionalHeaderSize { get; set; }

        /// <summary>
        /// Sector zero offset
        /// </summary>
        public uint SectorZeroOffset { get; set; }

        /// <summary>
        /// Partition Flags
        /// </summary>
        public byte[] PartitionFlags { get; set; } = [];

        /// <summary>
        /// Partition ID table
        /// </summary>
        public ulong[]? PartitionIdTable { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        public byte[] Reserved1 { get; set; } = new byte[0x20];

        /// <summary>
        /// Reserved?
        /// </summary>
        /// <remarks>0x0E bytes</remarks>
        public byte[] Reserved2 { get; set; } = new byte[0x0E];

        /// <summary>
        /// Support for this was implemented with 9.6.0-X FIRM. Bit0=1 enables using bits 1-2, it's unknown
        /// what these two bits are actually used for(the value of these two bits get compared with some other
        /// value during NCSD verification/loading). This appears to enable a new, likely hardware-based,
        /// antipiracy check on cartridges.
        /// </summary>
        public byte FirmUpdateByte1 { get; set; }

        /// <summary>
        /// Support for this was implemented with 9.6.0-X FIRM, see below regarding save crypto.
        /// </summary>
        public byte FirmUpdateByte2 { get; set; }

        #endregion

        #region Raw NAND Format Specific

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>0x5E bytes</remarks>
        public byte[] Unknown { get; set; } = new byte[0x5E];

        /// <summary>
        /// Encrypted MBR partition-table, for the TWL partitions(key-data used for this keyslot is console-unique).
        /// </summary>
        /// <remarks>0x42 bytes</remarks>
        public byte[] EncryptedMBR { get; set; } = new byte[0x42];

        #endregion
    }
}
