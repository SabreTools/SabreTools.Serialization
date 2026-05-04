namespace SabreTools.Data.Models.XRD
{
    /// <summary>
    /// Xbox Rebuild/Recovery/Redump Data
    /// Custom file format containing descriptive metadata about Xbox / Xbox 360 disc images
    /// </summary>
    public class File
    {
        /// <summary>
        /// "XRD\xFF\x00"
        /// </summary>
        public byte[] Magic { get; set; } = new byte[5];

        /// <summary>
        /// XRD File Format Version
        /// 0x01 = Standard
        /// 0x02 = Also contains Wiped Video ISO size/hashes, and Video ISO file size/hashes
        /// Intended use is Version 0x02 only for XGD3 discs, but not mandated
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// XGD Type: XGD1, XGD2, XGD3, 0xFF = Unknown
        /// </summary>
        public byte XGDType { get; set; }

        /// <summary>
        /// XGD SubType
        /// XGD1:
        ///  subtype 0 = XGD1 Beta (XB00104M)
        ///  subtype 1 = Standard
        /// XGD2:
        ///  subtype 0 = XGD2 Beta "Wave 0" (3CFB91D5)
        ///  subtype 1-20 = Wave 1-20, Standard
        ///  subtype 0x81 = Hybrid XGD2 / DVD-Video (65472451)
        /// XGD3:
        ///  subtype 0 = XGD3 Beta (152C2978, FD91511A, FFFFFDEB, FFFFFDE3)
        ///  subtype 1 = Standard
        /// 0xFF = Unknown
        /// Others undefined
        /// </summary>
        public byte XGDSubtype { get; set; }

        /// <summary>
        /// 8-character serial in disc ringcode
        /// e.g. XGD1: e.g. "MS00101A"
        /// e.g. XGD2/3: e.g. "1A2B3C4D"
        /// Can be parsed from XBE/XEX Certificate
        /// </summary>
        public byte[] Ringcode { get; set; } = new byte[8];

        /// <summary>
        /// Size of the redump ISO in bytes
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ulong RedumpSize { get; set; }

        /// <summary>
        /// CRC-32 hash of the redump ISO
        /// </summary>
        public byte[] RedumpCRC { get; set; } = new byte[4];

        /// <summary>
        /// MD5 hash of the redump ISO
        /// </summary>
        public byte[] RedumpMD5 { get; set; } = new byte[16];

        /// <summary>
        /// SHA-1 hash of the redump ISO
        /// </summary>
        public byte[] RedumpSHA1 { get; set; } = new byte[20];

        /// <summary>
        /// Size of the Raw XISO in bytes
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ulong RawXISOSize { get; set; }

        /// <summary>
        /// CRC-32 hash of the Raw XISO
        /// </summary>
        public byte[] RawXISOCRC { get; set; } = new byte[4];

        /// <summary>
        /// MD5 hash of the Raw XISO
        /// </summary>
        public byte[] RawXISOMD5 { get; set; } = new byte[16];

        /// <summary>
        /// SHA-1 hash of the Raw XISO
        /// </summary>
        public byte[] RawXISOSHA1 { get; set; } = new byte[20];

        /// <summary>
        /// Size of the Wiped/Trimmed XISO in bytes
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ulong CookedXISOSize { get; set; }

        /// <summary>
        /// CRC-32 hash of the Wiped/Trimmed XISO
        /// </summary>
        public byte[] CookedXISOCRC { get; set; } = new byte[4];

        /// <summary>
        /// MD5 hash of the Wiped/Trimmed XISO
        /// </summary>
        public byte[] CookedXISOMD5 { get; set; } = new byte[16];

        /// <summary>
        /// SHA-1 hash of the Wiped/Trimmed XISO
        /// </summary>
        public byte[] CookedXISOSHA1 { get; set; } = new byte[20];

        /// <summary>
        /// Size of the Video ISO in bytes
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ulong VideoISOSize { get; set; }

        /// <summary>
        /// CRC-32 hash of the Video ISO
        /// </summary>
        public byte[] VideoISOCRC { get; set; } = new byte[4];

        /// <summary>
        /// MD5 hash of the Video ISO
        /// </summary>
        public byte[] VideoISOMD5 { get; set; } = new byte[16];

        /// <summary>
        /// SHA-1 hash of the Video ISO
        /// </summary>
        public byte[] VideoISOSHA1 { get; set; } = new byte[20];

        /// <summary>
        /// Size of the wiped Video ISO in bytes
        /// Field does not exist for Version = 1
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ulong? WipedVideoISOSize { get; set; }

        /// <summary>
        /// CRC-32 hash of the wiped Video ISO
        /// Field does not exist for Version = 1
        /// </summary>
        public byte[]? WipedVideoISOCRC { get; set; } = new byte[4];

        /// <summary>
        /// MD5 hash of the wiped Video ISO
        /// Field does not exist for Version = 1
        /// </summary>
        public byte[]? WipedVideoISOMD5 { get; set; } = new byte[16];

        /// <summary>
        /// SHA-1 hash of the wiped Video ISO
        /// Field does not exist for Version = 1
        /// </summary>
        public byte[]? WipedVideoISOSHA1 { get; set; } = new byte[20];

        /// <summary>
        /// Size of the filler data that has been hashed, in bytes
        /// Should always be 2048
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ulong FillerSize { get; set; }

        /// <summary>
        /// CRC-32 Hash of the first sector of the XDVDFS filesystem (filler data)
        /// The hash is used to identify filler data or brute force the seed
        /// </summary>
        public byte[] FillerCRC { get; set; } = new byte[4];

        /// <summary>
        /// MD5 Hash of the first sector of the XDVDFS filesystem (filler data)
        /// The hash is used to identify filler data or brute force the seed
        /// </summary>
        public byte[] FillerMD5 { get; set; } = new byte[16];

        /// <summary>
        /// SHA-1 Hash of the first sector of the XDVDFS filesystem (filler data)
        /// The hash is used to identify filler data or brute force the seed
        /// </summary>
        public byte[] FillerSHA1 { get; set; } = new byte[20];

        /// <summary>
        /// The starting sector offset for each security sector range
        /// Security sector ranges are 4096-sectors long
        /// XGD1: 16 sector offsets
        /// XGD2/3: 2 sector offsets
        public uint[]? SecuritySectors { get; set; } = [];

        /// <summary>
        /// XBE Certificate, starts with length of structure
        /// </summary>
        /// <remarks>XGD1 only, Little-endian</remarks>
        public XboxExecutable.Certificate? XboxCertificate { get; set; }

        /// <summary>
        /// XEX Certificate, starts with length of structure
        /// </summary>
        /// <remarks>XGD2/3 only, Big-endian</remarks>
        public XenonExecutable.Certificate? Xbox360Certificate { get; set; }

        /// <summary>
        /// Number of files in the XISO
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public int FileCount { get; set; }

        /// <summary>
        /// File offsets and hashes in the XISO
        /// Length of array equal to FileCount
        /// </summary>
        public FileEntry[] FileInfo { get; set; } = [];

        /// <summary>
        /// XISO Volume Descriptor
        /// </summary>
        /// <remarks>2048 bytes</remarks>
        public XDVDFS.VolumeDescriptor VolumeDescriptor { get; set; } = new();

        /// <summary>
        /// Xbox DVD Layout Descriptor, immediately follows Volume Descriptor
        /// XGD1: Contains version numbers and signature bytes (always present)
        /// XGD2: Zeroed apart from initial signature bytes (always present?)
        /// XGD3: Sector not present (always not present?)
        /// Always check if this is present by reading LayoutDescriptor magic bytes
        /// </summary>
        /// <remarks>2048 bytes</remarks>
        public XDVDFS.LayoutDescriptor? LayoutDescriptor { get; set; }

        /// <summary>
        /// Number of directory records in the XISO
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public int DirectoryCount { get; set; }

        /// <summary>
        /// List of XISO descriptors and their sector offsets and sizes
        /// The root directory descriptor is not guaranteed to be the first
        /// </summary>
        public DirectoryEntry[] DirectoryInfo { get; set; } = [];

        /// <summary>
        /// Number of files in Video ISO
        /// Field does not exist for Version = 1
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public int? VideoISOFileCount { get; set; }

        /// <summary>
        /// File offsets and hashes in the Video ISO
        /// Length of array equal to VideoISOFileCount
        /// Field does not exist for Version = 1
        /// </summary>
        public FileEntry[]? VideoISOFileInfo { get; set; }

        /// <summary>
        /// Size of all data in XRD file prior to this field
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public uint XRDSize { get; set; }

        /// <summary>
        /// SHA-1 Hash of the XRD file prior to XRDSize field
        /// </summary>
        public byte[] XRDSHA1 { get; set; } = new byte[20];
    }
}
