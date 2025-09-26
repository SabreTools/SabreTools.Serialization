using System;

/// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
/// <see href="https://github.com/adamhathcock/sharpcompress/blob/master/src/SharpCompress/Common/Zip/Headers/LocalEntryHeaderExtraFactory.cs"/> 
namespace SabreTools.Data.Models.PKZIP
{
    [Flags]
    public enum ActionsReactions : uint
    {
        /// <summary>
        /// Use for auto detection
        /// </summary>
        AutoDetection = 0b0000_0000_0000_0000_0000_0000_0000_0001,

        /// <summary>
        /// Treat as a self-patch
        /// </summary>
        SelfPatch = 0b0000_0000_0000_0000_0000_0000_0000_0010,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved2 = 0b0000_0000_0000_0000_0000_0000_0000_0100,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved3 = 0b0000_0000_0000_0000_0000_0000_0000_1000,

        /// <summary>
        /// Action (None)
        /// </summary>
        ActionNone = 0b0000_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// Action (Add)
        /// </summary>
        ActionAdd = 0b0000_0000_0000_0000_0000_0000_0001_0000,

        /// <summary>
        /// Action (Delete)
        /// </summary>
        ActionDelete = 0b0000_0000_0000_0000_0000_0000_0010_0000,

        /// <summary>
        /// Action (Patch)
        /// </summary>
        ActionPatch = 0b0000_0000_0000_0000_0000_0000_0011_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved6 = 0b0000_0000_0000_0000_0000_0000_0100_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved7 = 0b0000_0000_0000_0000_0000_0000_1000_0000,

        /// <summary>
        /// Absent file reaction (Ask)
        /// </summary>
        ReactionAbsentAsk = 0b0000_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// Absent file reaction (Skip)
        /// </summary>
        ReactionAbsentSkip = 0b0000_0000_0000_0000_0000_0001_0000_0000,

        /// <summary>
        /// Absent file reaction (Ignore)
        /// </summary>
        ReactionAbsentIgnore = 0b0000_0000_0000_0000_0000_0010_0000_0000,

        /// <summary>
        /// Absent file reaction (Fail)
        /// </summary>
        ReactionAbsentFail = 0b0000_0000_0000_0000_0000_0011_0000_0000,

        /// <summary>
        /// Newer file reaction (Ask)
        /// </summary>
        ReactionNewerAsk = 0b0000_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// Newer file reaction (Skip)
        /// </summary>
        ReactionNewerSkip = 0b0000_0000_0000_0000_0000_0100_0000_0000,

        /// <summary>
        /// Newer file reaction (Ignore)
        /// </summary>
        ReactionNewerIgnore = 0b0000_0000_0000_0000_0000_1000_0000_0000,

        /// <summary>
        /// Newer file reaction (Fail)
        /// </summary>
        ReactionNewerFail = 0b0000_0000_0000_0000_0000_1100_0000_0000,

        /// <summary>
        /// Unknown file reaction (Ask)
        /// </summary>
        ReactionUnknownAsk = 0b0000_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// Unknown file reaction (Skip)
        /// </summary>
        ReactionUnknownSkip = 0b0000_0000_0000_0000_0001_0000_0000_0000,

        /// <summary>
        /// Unknown file reaction (Ignore)
        /// </summary>
        ReactionUnknownIgnore = 0b0000_0000_0000_0000_0010_0000_0000_0000,

        /// <summary>
        /// Unknown file reaction (Fail)
        /// </summary>
        ReactionUnknownFail = 0b0000_0000_0000_0000_0011_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved14 = 0b0000_0000_0000_0000_0100_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved15 = 0b0000_0000_0000_0000_1000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved16 = 0b0000_0000_0000_0001_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved17 = 0b0000_0000_0000_0010_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved18 = 0b0000_0000_0000_0100_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved19 = 0b0000_0000_0000_1000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved20 = 0b0000_0000_0001_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved21 = 0b0000_0000_0010_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved22 = 0b0000_0000_0100_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved23 = 0b0000_0000_1000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved24 = 0b0000_0001_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved25 = 0b0000_0010_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved26 = 0b0000_0100_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved27 = 0b0000_1000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved28 = 0b0001_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved29 = 0b0010_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved30 = 0b0100_0000_0000_0000_0000_0000_0000_0000,

        /// <summary>
        /// RESERVED
        /// </summary>
        Reserved31 = 0b1000_0000_0000_0000_0000_0000_0000_0000,
    }

    public enum AS400ExtraFieldAttributeFieldCode : ushort
    {
        /// <summary>
        /// Source type i.e. CLP etc
        /// </summary>
        SourceType = 0x4001,

        /// <summary>
        /// The text description of the library
        /// </summary>
        LibraryTextDescription = 0x4002,

        /// <summary>
        /// The text description of the file
        /// </summary>
        FileTextDescription = 0x4003,

        /// <summary>
        /// The text description of the member
        /// </summary>
        MemberTextDescription = 0x4004,

        /// <summary>
        /// x'F0' or 0 is PF-DTA,  x'F1' or 1 is PF_SRC
        /// </summary>
        PFType = 0x4005,

        /// <summary>
        /// Database Type Code
        /// </summary>
        /// <remarks>1 byte</remarks>
        DatabaseTypeCode = 0x4007,

        /// <summary>
        /// Database file and fields definition
        /// </summary>
        DatabaseFileAndFieldsDefinition = 0x4008,

        /// <summary>
        /// GZIP file type
        /// </summary>
        /// <remarks>2 bytes</remarks>
        GZIPFileType = 0x4009,

        /// <summary>
        /// IFS code page
        /// </summary>
        /// <remarks>2 bytes</remarks>
        IFSCodePage = 0x400B,

        /// <summary>
        /// IFS Time of last file status change
        /// </summary>
        /// <remarks>4 bytes</remarks>
        IFSTimeOfLastFileStatusChange = 0x400C,

        /// <summary>
        /// IFS Access Time
        /// </summary>
        /// <remarks>4 bytes</remarks>
        IFSAccessTime = 0x4001,

        /// <summary>
        /// IFS Modification time
        /// </summary>
        /// <remarks>4 bytes</remarks>
        IFSModificationTime = 0x4001,

        /// <summary>
        /// Length of the records in the file
        /// </summary>
        /// <remarks>2 bytes</remarks>
        RecordsLength = 0x4001,

        /// <summary>
        /// GZIP two words
        /// </summary>
        /// <remarks>8 bytes</remarks>
        GZIPTwoWords = 0x4001,
    }

    public enum CompressionMethod : ushort
    {
        /// <summary>
        /// The file is stored (no compression)
        /// </summary>
        Stored = 0,

        /// <summary>
        /// The file is Shrunk
        /// </summary>
        Shrunk = 1,

        /// <summary>
        /// The file is Reduced with compression factor 1
        /// </summary>
        ReducedCompressionFactor1 = 2,

        /// <summary>
        /// The file is Reduced with compression factor 2
        /// </summary>
        ReducedCompressionFactor2 = 3,

        /// <summary>
        /// The file is Reduced with compression factor 3
        /// </summary>
        ReducedCompressionFactor3 = 4,

        /// <summary>
        /// The file is Reduced with compression factor 4
        /// </summary>
        ReducedCompressionFactor4 = 5,

        /// <summary>
        /// The file is Imploded
        /// </summary>
        Implode = 6,

        /// <summary>
        /// Reserved for Tokenizing compression algorithm
        /// </summary>
        TokenizingCompressionAlgorithm = 7,

        /// <summary>
        /// The file is Deflated
        /// </summary>
        Deflate = 8,

        /// <summary>
        /// Enhanced Deflating using Deflate64(tm)
        /// </summary>
        Deflate64 = 9,

        /// <summary>
        /// PKWARE Data Compression Library Imploding (old IBM TERSE)
        /// </summary>
        PKWAREDataCompressionLibraryImplode = 10,

        /// <summary>
        /// Reserved by PKWARE
        /// </summary>
        CompressionMethod11 = 11,

        /// <summary>
        /// File is compressed using BZIP2 algorithm
        /// </summary>
        BZIP2 = 12,

        /// <summary>
        /// Reserved by PKWARE
        /// </summary>
        CompressionMethod13 = 13,

        /// <summary>
        /// LZMA
        /// </summary>
        LZMA = 14,

        /// <summary>
        /// Reserved by PKWARE
        /// </summary>
        CompressionMethod15 = 15,

        /// <summary>
        /// IBM z/OS CMPSC Compression
        /// </summary>
        IBMzOSCMPSC = 16,

        /// <summary>
        /// Reserved by PKWARE
        /// </summary>
        CompressionMethod17 = 17,

        /// <summary>
        /// File is compressed using IBM TERSE (new)
        /// </summary>
        IBMTERSE = 18,

        /// <summary>
        /// IBM LZ77 z Architecture
        /// </summary>
        IBMLZ77 = 19,

        /// <summary>
        /// deprecated (use method 93 for zstd)
        /// </summary>
        [Obsolete]
        OldZstandard = 20,

        /// <summary>
        /// Zstandard (zstd) Compression
        /// </summary>
        Zstandard = 93,

        /// <summary>
        /// MP3 Compression
        /// </summary>
        MP3 = 94,

        /// <summary>
        /// XZ Compression
        /// </summary>
        XZ = 95,

        /// <summary>
        /// JPEG variant
        /// </summary>
        JPEGVariant = 96,

        /// <summary>
        /// WavPack compressed data
        /// </summary>
        WavPack = 97,

        /// <summary>
        /// PPMd version I, Rev 1
        /// </summary>
        PPMdVersionIRev1 = 98,

        /// <summary>
        /// AE-x encryption marker
        /// </summary>
        AExEncryption = 99,
    }

    [Flags]
    public enum GeneralPurposeBitFlags : ushort
    {
        /// <summary>
        /// Indicates that the file is encrypted
        /// </summary>
        FileEncrypted = 0b0000_0000_0000_0001,

        #region Compression Method 6 (Implode)

        /// <summary>
        /// Set   - 8K sliding dictionary
        /// Clear - 4K sliding dictionary
        /// </summary>
        LargeSlidingDictionary = 0b0000_0000_0000_0010,

        /// <summary>
        /// Set   - 3 Shannon-Fano trees were used
        /// Clear - 2 Shannon-Fano trees were used
        /// </summary>
        ThreeTrees = 0b0000_0000_0000_0100,

        #endregion

        #region Compression Method 8 (Deflate)

        /// <summary>
        /// Normal (-en) compression
        /// </summary>
        NormalCompression = 0b0000_0000_0000_0000,

        /// <summary>
        /// Maximum (-ex) compression
        /// </summary>
        MaximumCompression = 0b0000_0000_0000_0010,

        /// <summary>
        /// Fast (-ef) compression
        /// </summary>
        FastCompression = 0b0000_0000_0000_0100,

        /// <summary>
        /// Super Fast (-es) compression
        /// </summary>
        SuperFastCompression = 0b0000_0000_0000_0110,

        #endregion

        #region Compression Method 14 (LZMA)

        /// <summary>
        /// Set   - indicates an end-of-stream (EOS) marker is used to
        ///         mark the end of the compressed data stream.
        /// Clear - an EOS marker is not present and the compressed data
        ///         size must be known to extract.
        /// </summary>
        EndOfStreamMarker = 0b0000_0000_0000_0010,

        #endregion

        /// <summary>
        /// set - the fields crc-32, compressed size and
        ///       uncompressed size are set to zero in the
        ///       local header. The correct values are put
        ///       in the data descriptor immediately
        ///       following the compressed data.
        /// </summary>
        NoCRC = 0b0000_0000_0000_1000,

        /// <summary>
        /// Reserved for use with method 8, for enhanced deflating. 
        /// </summary>
        EnhancedDeflateReserved = 0b0000_0000_0001_0000,

        /// <summary>
        /// Indicates that the file is compressed patched data.
        /// </summary>
        /// <remarks>Requires PKZIP version 2.70 or greater</remarks>
        CompressedPatchedData = 0b0000_0000_0010_0000,

        /// <summary>
        /// Set   - you MUST set the version needed to extract value
        ///         to at least 50 and you MUST also set bit 0. If AES
        ///         encryption is used, the version needed to extract
        ///         value MUST be at least 51.
        /// </summary>
        StrongEncryption = 0b0000_0000_0100_0000,

        /// <summary>
        /// Currently unused
        /// </summary>
        Bit7 = 0b0000_0000_1000_0000,

        /// <summary>
        /// Currently unused
        /// </summary>
        Bit8 = 0b0000_0001_0000_0000,

        /// <summary>
        /// Currently unused
        /// </summary>
        Bit9 = 0b0000_0010_0000_0000,

        /// <summary>
        /// Currently unused
        /// </summary>
        Bit10 = 0b0000_0100_0000_0000,

        /// <summary>
        /// Set   - the filename and comment fields for this
        ///         file MUST be encoded using UTF-8.
        /// </summary>
        LanguageEncodingFlag = 0b0000_1000_0000_0000,

        /// <summary>
        /// Reserved by PKWARE for enhanced compression
        /// </summary>
        PKWAREEnhancedCompression = 0b0001_0000_0000_0000,

        /// <summary>
        /// Set   - Set when encrypting the Central Directory to
        ///         indicate selected data values in the Local
        ///         Header are masked to hide their actual values
        /// </summary>
        LocalHeaderValuesMasked = 0b0010_0000_0000_0000,

        /// <summary>
        /// Reserved by PKWARE for alternate streams
        /// </summary>
        PKWAREAlternateStreams = 0b0100_0000_0000_0000,

        /// <summary>
        /// Reserved by PKWARE
        /// </summary>
        PKWAREReserved = 0b1000_0000_0000_0000,
    }

    public enum HeaderID : ushort
    {
        /// <summary>
        /// Zip64 extended information extra field
        /// </summary>
        Zip64ExtendedInformation = 0x0001,

        /// <summary>
        /// AV Info
        /// </summary>
        AVInfo = 0x0007,

        /// <summary>
        /// Reserved for extended language encoding data (PFS)
        /// </summary>
        ExtendedLanguageEncodingData = 0x0008,

        /// <summary>
        /// OS/2
        /// </summary>
        OS2 = 0x0009,

        /// <summary>
        /// NTFS
        /// </summary>
        NTFS = 0x000A,

        /// <summary>
        /// OpenVMS
        /// </summary>
        OpenVMS = 0x000C,

        /// <summary>
        /// UNIX
        /// </summary>
        UNIX = 0x000D,

        /// <summary>
        /// Reserved for file stream and fork descriptors
        /// </summary>
        FileStreamFork = 0x000E,

        /// <summary>
        /// Patch Descriptor
        /// </summary>
        PatchDescriptor = 0x000F,

        /// <summary>
        /// PKCS#7 Store for X.509 Certificates
        /// </summary>
        PKCSStore = 0x0014,

        /// <summary>
        /// X.509 Certificate ID and Signature for individual file
        /// </summary>
        X509IndividualFile = 0x0015,

        /// <summary>
        /// X.509 Certificate ID for Central Directory
        /// </summary>
        X509CentralDirectory = 0x0016,

        /// <summary>
        /// Strong Encryption Header
        /// </summary>
        StrongEncryptionHeader = 0x0017,

        /// <summary>
        /// Record Management Controls
        /// </summary>
        RecordManagementControls = 0x0018,

        /// <summary>
        /// PKCS#7 Encryption Recipient Certificate List
        /// </summary>
        PKCSCertificateList = 0x0019,

        /// <summary>
        /// Reserved for Timestamp record
        /// </summary>
        Timestamp = 0x0020,

        /// <summary>
        /// Policy Decryption Key Record
        /// </summary>
        PolicyDecryptionKey = 0x0021,

        /// <summary>
        /// Smartcrypt Key Provider Record
        /// </summary>
        SmartcryptKeyProvider = 0x0022,

        /// <summary>
        /// Smartcrypt Policy Key Data Record
        /// </summary>
        SmartcryptPolicyKeyData = 0x0023,

        /// <summary>
        /// IBM S/390 (Z390), AS/400 (I400) attributes - uncompressed
        /// </summary>
        IBMS390AttributesUncompressed = 0x0065,

        /// <summary>
        /// Reserved for IBM S/390 (Z390), AS/400 (I400) attributes - compressed
        /// </summary>
        IBMS390AttributesCompressed = 0x0066,

        /// <summary>
        /// POSZIP 4690 (reserved)
        /// </summary>
        POSZIP4690 = 0x4690,

        #region Third-Party

        /// <summary>
        /// Macintosh
        /// </summary>
        Macintosh = 0x07C8,

        /// <summary>
        /// Pixar USD header ID
        /// </summary>
        PixarUSD = 0x1986,

        /// <summary>
        /// ZipIt Macintosh
        /// </summary>
        ZipItMacintosh = 0x2605,

        /// <summary>
        /// ZipIt Macintosh 1.3.5+
        /// </summary>
        ZipItMacintosh135Plus = 0x2705,

        /// <summary>
        /// ZipIt Macintosh 1.3.5+
        /// </summary>
        ZipItMacintosh135PlusAlt = 0x2805,

        /// <summary>
        /// Info-ZIP Macintosh
        /// </summary>
        InfoZIPMacintosh = 0x334D,

        /// <summary>
        /// Acorn/SparkFS
        /// </summary>
        AcornSparkFS = 0x4341,

        /// <summary>
        /// Windows NT security descriptor (binary ACL)
        /// </summary>
        WindowsNTSecurityDescriptor = 0x4453,

        /// <summary>
        /// VM/CMS
        /// </summary>
        VMCMS = 0x4704,

        /// <summary>
        /// MVS
        /// </summary>
        MVS = 0x470F,

        /// <summary>
        /// THEOS (old?)
        /// </summary>
        THEOSold = 0x4854,

        /// <summary>
        /// FWKCS MD5
        /// </summary>
        FWKCSMD5 = 0x4B46,

        /// <summary>
        /// OS/2 access control list (text ACL)
        /// </summary>
        OS2AccessControlList = 0x4C41,

        /// <summary>
        /// Info-ZIP OpenVMS
        /// </summary>
        InfoZIPOpenVMS = 0x4D49,

        /// <summary>
        /// Macintosh Smartzip (??)
        /// </summary>
        MacintoshSmartzip = 0x4D63,

        /// <summary>
        /// Xceed original location extra field
        /// </summary>
        XceedOriginalLocation = 0x4F4C,

        /// <summary>
        /// AOS/VS (ACL)
        /// </summary>
        ADSVS = 0x5356,

        /// <summary>
        /// extended timestamp
        /// </summary>
        ExtendedTimestamp = 0x5455,

        /// <summary>
        /// Xceed unicode extra field
        /// </summary>
        XceedUnicode = 0x554E,

        /// <summary>
        /// Info-ZIP UNIX (original, also OS/2, NT, etc)
        /// </summary>
        InfoZIPUNIX = 0x5855,

        /// <summary>
        /// Info-ZIP Unicode Comment Extra Field
        /// </summary>
        InfoZIPUnicodeComment = 0x6375,

        /// <summary>
        /// BeOS/BeBox
        /// </summary>
        BeOSBeBox = 0x6542,

        /// <summary>
        /// THEOS
        /// </summary>
        THEOS = 0x6854,

        /// <summary>
        /// Info-ZIP Unicode Path Extra Field
        /// </summary>
        InfoZIPUnicodePath = 0x7075,

        /// <summary>
        /// AtheOS/Syllable
        /// </summary>
        AtheOSSyllable = 0x7441,

        /// <summary>
        /// ASi UNIX
        /// </summary>
        ASiUNIX = 0x756E,

        /// <summary>
        /// Info-ZIP UNIX (new)
        /// </summary>
        InfoZIPUNIXNew = 0x7855,

        /// <summary>
        /// Info-ZIP UNIX (newer UID/GID)
        /// </summary>
        InfoZIPUNIXNewer = 0x7875,

        /// <summary>
        /// Data Stream Alignment (Apache Commons-Compress)
        /// </summary>
        DataStreamAlignment = 0xA11E,

        /// <summary>
        /// Microsoft Open Packaging Growth Hint
        /// </summary>
        MicrosoftOpenPackagingGrowthHint = 0xA220,

        /// <summary>
        /// Java JAR file Extra Field Header ID
        /// </summary>
        JavaJAR = 0xCAFE,

        /// <summary>
        /// Android ZIP Alignment Extra Field
        /// </summary>
        AndroidZIPAlignment = 0xD935,

        /// <summary>
        /// Korean ZIP code page info
        /// </summary>
        KoreanZIPCodePage = 0xE57A,

        /// <summary>
        /// SMS/QDOS
        /// </summary>
        SMSQDOS = 0xFD4A,

        /// <summary>
        /// AE-x encryption structure
        /// </summary>
        AExEncryptionStructure = 0x9901,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0x9902,

        #endregion
    }

    public enum HostSystem : byte
    {
        /// <summary>
        /// MS-DOS and OS/2 (FAT / VFAT / FAT32 file systems)
        /// </summary>
        MSDOS = 0,

        /// <summary>
        /// Amiga
        /// </summary>
        Amiga = 1,

        /// <summary>
        /// OpenVMS
        /// </summary>
        OpenVMS = 2,

        /// <summary>
        /// UNIX
        /// </summary>
        UNIX = 3,

        /// <summary>
        /// VM/CMS
        /// </summary>
        VMCMS = 4,

        /// <summary>
        /// Atari ST
        /// </summary>
        AtariST = 5,

        /// <summary>
        /// OS/2 H.P.F.S.
        /// </summary>
        OS2HPFS = 6,

        /// <summary>
        /// Macintosh
        /// </summary>
        Macintosh = 7,

        /// <summary>
        /// Z-System
        /// </summary>
        ZSystem = 8,

        /// <summary>
        /// CP/M
        /// </summary>
        CPM = 9,

        /// <summary>
        /// Windows NTFS
        /// </summary>
        WindowsNTFS = 10,

        /// <summary>
        /// MVS (OS/390 - Z/OS)
        /// </summary>
        MVS = 11,

        /// <summary>
        /// VSE
        /// </summary>
        VSE = 12,

        /// <summary>
        /// Acorn Risc
        /// </summary>
        AcornRisc = 13,

        /// <summary>
        /// VFAT
        /// </summary>
        VFAT = 14,

        /// <summary>
        /// alternate MVS
        /// </summary>
        AlternateVMS = 15,

        /// <summary>
        /// BeOS
        /// </summary>
        BeOS = 16,

        /// <summary>
        /// Tandem
        /// </summary>
        Tandem = 17,

        /// <summary>
        /// OS/400
        /// </summary>
        OS400 = 18,

        /// <summary>
        /// OS X (Darwin)
        /// </summary>
        OSX = 19,

        // 20 thru 255 - unused
    }

    [Flags]
    public enum InternalFileAttributes : ushort
    {
        /// <summary>
        /// Set   - The file is apparently an ASCII or text file
        /// Clear - The file is apparently binary data
        /// </summary>
        ASCII = 0b0000_0000_0000_0001,

        /// <summary>
        /// Reserved for use by PKWARE
        /// </summary>
        Bit1 = 0b0000_0000_0000_0010,

        /// <summary>
        /// Reserved for use by PKWARE
        /// </summary>
        Bit2 = 0b0000_0000_0000_0100,

        /*
         4.4.14.2 The 0x0002 bit of this field indicates, if set, that 
         a 4 byte variable record length control field precedes each 
         logical record indicating the length of the record. The 
         record length control field is stored in little-endian byte
         order.  This flag is independent of text control characters, 
         and if used in conjunction with text data, includes any 
         control characters in the total length of the record. This 
         value is provided for mainframe data transfer support.
        */
    }

    [Flags]
    public enum RecordedTimeFlag : byte
    {
        None = 0,

        LastModified = 1,

        LastAccessed = 2,

        Created = 4,
    }

    [Flags]
    public enum ZipItInternalSettings : ushort
    {
        /// <summary>
        /// If set, the folder is shown expanded (open)
        /// when the archive contents are viewed in ZipIt.
        /// </summary>
        ShowExpanded = 0b0000_0000_0000_0001,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved1 = 0b0000_0000_0000_0010,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved2 = 0b0000_0000_0000_0100,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved3 = 0b0000_0000_0000_1000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved4 = 0b0000_0000_0001_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved5 = 0b0000_0000_0010_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved6 = 0b0000_0000_0100_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved7 = 0b0000_0000_1000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved8 = 0b0000_0001_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved9 = 0b0000_0010_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved10 = 0b0000_0100_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved11 = 0b0000_1000_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved12 = 0b0001_0000_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved13 = 0b0010_0000_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved14 = 0b0100_0000_0000_0000,

        /// <summary>
        /// Reserved, zero
        /// </summary>
        Reserved15 = 0b1000_0000_0000_0000,
    }

    public enum ZOSExtraFieldAttributeFieldCode : ushort
    {
        /// <summary>
        /// File Type
        /// <summary>
        /// </remarks>2 bytes</remarks>
        FileType = 0x0001,

        /// <summary>
        /// NonVSAM Record Format
        /// <summary>
        /// </remarks>1 byte</remarks>
        NonVSAMRecordFormat = 0x0002,

        /// <summary>
        /// Reserved
        /// <summary>
        /// </remarks></remarks>
        Reserved03 = 0x0003,

        /// <summary>
        /// NonVSAM Block Size
        /// <summary>
        /// </remarks>2 bytes Big Endian</remarks>
        NonVSAMBlockSize = 0x0004,

        /// <summary>
        /// Primary Space Allocation
        /// <summary>
        /// </remarks>3 bytes Big Endian</remarks>
        PrimarySpaceAllocation = 0x0005,

        /// <summary>
        /// Secondary Space Allocation
        /// <summary>
        /// </remarks>3 bytes Big Endian</remarks>
        SecondarySpaceAllocation = 0x0006,

        /// <summary>
        /// Space Allocation Type1 byte flag
        /// <summary>
        /// </remarks>1 byte</remarks>
        SpaceAllocationType1ByteFlag = 0x0007,

        /// <summary>
        /// Modification Date
        /// <summary>
        /// </remarks>Retired with PKZIP 5.0 +</remarks>
        ModificationDate = 0x0008,

        /// <summary>
        /// Expiration Date
        /// <summary>
        /// </remarks>Retired with PKZIP 5.0 +</remarks>
        ExpirationDate = 0x0009,

        /// <summary>
        /// PDS Directory Block Allocation
        /// <summary>
        /// </remarks>3 bytes Big Endian binary value</remarks>
        PDSDirectoryBlockAllocation = 0x000A,

        /// <summary>
        /// NonVSAM Volume List
        /// <summary>
        /// </remarks>variable</remarks>
        NonVSAMVolumeList = 0x000B,

        /// <summary>
        /// UNIT Reference
        /// <summary>
        /// </remarks>Retired with PKZIP 5.0 +</remarks>
        UNITReference = 0x000C,

        /// <summary>
        /// DF/SMS Management Class
        /// <summary>
        /// </remarks>8 bytes EBCDIC Text Value</remarks>
        DFSMSManagementClass = 0x000D,

        /// <summary>
        /// DF/SMS Storage Class
        /// <summary>
        /// </remarks>8 bytes EBCDIC Text Value</remarks>
        DFSMSStorageClass = 0x000E,

        /// <summary>
        /// DF/SMS Data Class
        /// <summary>
        /// </remarks>8 bytes EBCDIC Text Value</remarks>
        DFSMSDataClass = 0x000F,

        /// <summary>
        /// PDS/PDSE Member Info
        /// <summary>
        /// </remarks>30 bytes</remarks>
        PDSPDSEMemberInfo = 0x0010,

        /// <summary>
        /// VSAM sub-filetype
        /// <summary>
        /// </remarks>2 bytes</remarks>
        VSAMSubFiletype = 0x0011,

        /// <summary>
        /// VSAM LRECL
        /// <summary>
        /// </remarks>13 bytes EBCDIC "(num_avg num_max)"</remarks>
        VSAMLRECL = 0x0012,

        /// <summary>
        /// VSAM Cluster Name
        /// <summary>
        /// </remarks>Retired with PKZIP 5.0 +</remarks>
        VSAMClusterName = 0x0013,

        /// <summary>
        /// VSAM KSDS Key Information
        /// <summary>
        /// </remarks>13 bytes EBCDIC "(num_length num_position)"</remarks>
        VSAMKSDSKeyInformation = 0x0014,

        /// <summary>
        /// VSAM Average LRECL
        /// <summary>
        /// </remarks>5 bytes EBCDIC num_value padded with blanks</remarks>
        VSAMAverageLRECL = 0x0015,

        /// <summary>
        /// VSAM Maximum LRECL
        /// <summary>
        /// </remarks>5 bytes EBCDIC num_value padded with blanks</remarks>
        VSAMMaximumLRECL = 0x0016,

        /// <summary>
        /// VSAM KSDS Key Length
        /// <summary>
        /// </remarks>5 bytes EBCDIC num_value padded with blanks</remarks>
        VSAMKSDSKeyLength = 0x0017,

        /// <summary>
        /// VSAM KSDS Key Position
        /// <summary>
        /// </remarks>5 bytes EBCDIC num_value padded with blanks</remarks>
        VSAMKSDSKeyPosition = 0x0018,

        /// <summary>
        /// VSAM Data Name
        /// <summary>
        /// </remarks>1-44 bytes EBCDIC text string</remarks>
        VSAMDataName = 0x0019,

        /// <summary>
        /// VSAM KSDS Index Name
        /// <summary>
        /// </remarks>1-44 bytes EBCDIC text string</remarks>
        VSAMKSDSIndexName = 0x001A,

        /// <summary>
        /// VSAM Catalog Name
        /// <summary>
        /// </remarks>1-44 bytes EBCDIC text string</remarks>
        VSAMCatalogName = 0x001B,

        /// <summary>
        /// VSAM Data Space Type
        /// <summary>
        /// </remarks>9 bytes EBCDIC text string</remarks>
        VSAMDataSpaceType = 0x001C,

        /// <summary>
        /// VSAM Data Space Primary
        /// <summary>
        /// </remarks>9 bytes EBCDIC num_value left-justified</remarks>
        VSAMDataSpacePrimary = 0x001D,

        /// <summary>
        /// VSAM Data Space Secondary
        /// <summary>
        /// </remarks>9 bytes EBCDIC num_value left-justified</remarks>
        VSAMDataSpaceSecondary = 0x001E,

        /// <summary>
        /// VSAM Data Volume List
        /// <summary>
        /// </remarks>variable EBCDIC text list of 6-character Volume IDs</remarks>
        VSAMDataVolumeList = 0x001F,

        /// <summary>
        /// VSAM Data Buffer Space
        /// <summary>
        /// </remarks>8 bytes EBCDIC num_value left-justified</remarks>
        VSAMDataBufferSpace = 0x0020,

        /// <summary>
        /// VSAM Data CISIZE
        /// <summary>
        /// </remarks>5 bytes EBCDIC num_value left-justified</remarks>
        VSAMDataCISIZE = 0x0021,

        /// <summary>
        /// VSAM Erase Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMEraseFlag = 0x0022,

        /// <summary>
        /// VSAM Free CI %
        /// <summary>
        /// </remarks>3 bytes EBCDIC num_value left-justified</remarks>
        VSAMFreeCIPercentage = 0x0023,

        /// <summary>
        /// VSAM Free CA %
        /// <summary>
        /// </remarks>3 bytes EBCDIC num_value left-justified</remarks>
        VSAMFreeCAPercentage = 0x0024,

        /// <summary>
        /// VSAM Index Volume List
        /// <summary>
        /// </remarks>variable EBCDIC text list of 6-character Volume IDs</remarks>
        VSAMIndexVolumeList = 0x0025,

        /// <summary>
        /// VSAM Ordered Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMOrderedFlag = 0x0026,

        /// <summary>
        /// VSAM REUSE Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMREUSEFlag = 0x0027,

        /// <summary>
        /// VSAM SPANNED Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMSPANNEDFlag = 0x0028,

        /// <summary>
        /// VSAM Recovery Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMRecoveryFlag = 0x0029,

        /// <summary>
        /// VSAM  WRITECHK  Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMWRITECHKFlag = 0x002A,

        /// <summary>
        /// VSAM Cluster/Data SHROPTS
        /// <summary>
        /// </remarks>3 bytes EBCDIC "n,y"</remarks>
        CSAMClusterDataSHROPTS = 0x002B,

        /// <summary>
        /// VSAM Index SHROPTS
        /// <summary>
        /// </remarks>3 bytes EBCDIC "n,y"</remarks>
        VSAMIndexSHROPTS = 0x002C,

        /// <summary>
        /// VSAM Index Space Type
        /// <summary>
        /// </remarks>9 bytes EBCDIC text string</remarks>
        VSAMIndexSpaceType = 0x002D,

        /// <summary>
        /// VSAM Index Space Primary
        /// <summary>
        /// </remarks>9 bytes EBCDIC num_value left-justified</remarks>
        VSAMIndexSpacePrimary = 0x002E,

        /// <summary>
        /// VSAM Index Space Secondary
        /// <summary>
        /// </remarks>9 bytes EBCDIC num_value left-justified</remarks>
        VSAMIndexSpaceSecondary = 0x002F,

        /// <summary>
        /// VSAM Index CISIZE
        /// <summary>
        /// </remarks>v</remarks>
        VSAMIndexCISIZE = 0x0030,

        /// <summary>
        /// VSAM Index IMBED
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMIndexIMBED = 0x0031,

        /// <summary>
        /// VSAM Index Ordered Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMIndexOrderedFlag = 0x0032,

        /// <summary>
        /// VSAM REPLICATE Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMREPLICATEFlag = 0x0033,

        /// <summary>
        /// VSAM Index REUSE Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        VSAMIndexREUSEFlag = 0x0034,

        /// <summary>
        /// VSAM Index WRITECHK Flag
        /// <summary>
        /// </remarks>1 byte flag Retired with PKZIP 5.0 +</remarks>
        VSAMIndexWRITECHKFlag = 0x0035,

        /// <summary>
        /// VSAM Owner
        /// <summary>
        /// </remarks>8 bytes EBCDIC text string</remarks>
        VSAMOwner = 0x0036,

        /// <summary>
        /// VSAM Index Owner
        /// <summary>
        /// </remarks>8 bytes EBCDIC text string</remarks>
        VSAMIndexOwner = 0x0037,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved38 = 0x0038,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved39 = 0x0039,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved3A = 0x003A,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved3B = 0x003B,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved3C = 0x003C,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved3D = 0x003D,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved3E = 0x003E,

        /// <summary>
        /// Reserved
        /// <summary>
        ReservedeF = 0x003F,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved40 = 0x0040,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved41 = 0x0041,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved42 = 0x0042,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved43 = 0x0043,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved44 = 0x0044,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved45 = 0x0045,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved46 = 0x0046,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved47 = 0x0047,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved48 = 0x0048,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved49 = 0x0049,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved4A = 0x004A,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved4B = 0x004B,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved4C = 0x004C,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved4D = 0x004D,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved4E = 0x004E,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved4F = 0x004F,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved50 = 0x0050,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved51 = 0x0051,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved52 = 0x0052,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved53 = 0x0053,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved54 = 0x0054,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved55 = 0x0055,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved56 = 0x0056,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved57 = 0x0057,

        /// <summary>
        /// PDS/PDSE Member TTR Info
        /// <summary>
        /// </remarks>6 bytes  Big Endian</remarks>
        PDSPDSEMemberTTRInfo = 0x0058,

        /// <summary>
        /// PDS 1st LMOD Text TTR
        /// <summary>
        /// </remarks>3 bytes  Big Endian</remarks>
        PDSFirstLMODTextTTR = 0x0059,

        /// <summary>
        /// PDS LMOD EP Rec #
        /// <summary>
        /// </remarks>4 bytes  Big Endian</remarks>
        PDSLMODEPRecNum = 0x005A,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved5B = 0x005B,

        /// <summary>
        /// Max Length of records
        /// <summary>
        /// </remarks>2 bytes  Big Endian</remarks>
        MaxLengthOfRecords = 0x005C,

        /// <summary>
        /// PDSE Flag
        /// <summary>
        /// </remarks>1 byte flag</remarks>
        PDSEFlag = 0x005D,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved5E = 0x005E,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved5F = 0x005F,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved60 = 0x0060,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved61 = 0x0061,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved62 = 0x0062,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved63 = 0x0063,

        /// <summary>
        /// Reserved
        /// <summary>
        Reserved64 = 0x0064,

        /// <summary>
        /// Last Date Referenced
        /// <summary>
        /// </remarks>4 bytes  Packed Hex "yyyymmdd"</remarks>
        LastDateReferenced = 0x0065,

        /// <summary>
        /// Date Created
        /// <summary>
        /// </remarks>4 bytes  Packed Hex "yyyymmdd"</remarks>
        DateCreated = 0x0066,

        /// <summary>
        /// GZIP two words
        /// <summary>
        /// </remarks>8 bytes</remarks>
        GZIPTwoWords = 0x0068,

        /// <summary>
        /// Extended NOTE Location
        /// <summary>
        /// </remarks>12 bytes Big Endian</remarks>
        ExtendedNOTELocation = 0x0071,

        /// <summary>
        /// Archive device UNIT
        /// <summary>
        /// </remarks>6 bytes  EBCDIC</remarks>
        ArchiveDeviceUNIT = 0x0072,

        /// <summary>
        /// Archive 1st Volume
        /// <summary>
        /// </remarks>6 bytes  EBCDIC</remarks>
        ArchiveFirstVolume = 0x0073,

        /// <summary>
        /// Archive 1st VOL File Seq#
        /// <summary>
        /// </remarks>2 bytes  Binary</remarks>
        ArchiveFirstVolFileSeqNum = 0x0074,

        /// <summary>
        /// Native I/O Flags
        /// <summary>
        /// </remarks>2 bytes</remarks>
        NativeIOFlags = 0x0075,

        /// <summary>
        /// Unix File Type
        /// <summary>
        /// </remarks>1 byte   enumerated</remarks>
        UnixFileType = 0x0081,

        /// <summary>
        /// Unix File Format
        /// <summary>
        /// </remarks>1 byte   enumerated</remarks>
        UnixFileFormat = 0x0082,

        /// <summary>
        /// Unix File Character Set Tag Info
        /// <summary>
        /// </remarks>4 bytes</remarks>
        UnixFileCharacterSetTagInfo = 0x0083,

        /// <summary>
        /// ZIP Environmental Processing Info
        /// <summary>
        /// </remarks>4 bytes</remarks>
        ZIPEnvironmentalProcessingInfo = 0x0090,

        /// <summary>
        /// EAV EATTR Flags
        /// <summary>
        /// </remarks>1 byte</remarks>
        EAVEATTRFlags = 0x0091,

        /// <summary>
        /// DSNTYPE Flags
        /// <summary>
        /// </remarks>1 byte</remarks>
        DSNTYPEFlags = 0x0092,

        /// <summary>
        /// Total Space Allocation (Cyls)
        /// <summary>
        /// </remarks>4 bytes  Big Endian</remarks>
        TotalSpaceAllocationCyls = 0x0093,

        /// <summary>
        /// NONVSAM DSORG
        /// <summary>
        /// </remarks>2 bytes</remarks>
        NONVSAMDSORG = 0x009D,

        /// <summary>
        /// Program Virtual Object Info
        /// <summary>
        /// </remarks>3 bytes</remarks>
        ProgramVirtualObjectInfo = 0x009E,

        /// <summary>
        /// Encapsulated file Info
        /// <summary>
        /// </remarks>9 bytes</remarks>
        EncapsulatedFileInfo = 0x009F,

        /// <summary>
        /// Cluster Log
        /// <summary>
        /// </remarks>4 bytes  Binary</remarks>
        ClusterLog = 0x00A2,

        /// <summary>
        /// Cluster LSID Length
        /// <summary>
        /// </remarks>4 bytes  Binary</remarks>
        ClusterLSIDLength = 0x00A3,

        /// <summary>
        /// Cluster LSID
        /// <summary>
        /// </remarks>26 bytes  EBCDIC</remarks>
        ClusterLSID = 0x00A4,

        /// <summary>
        /// Unix File Creation Time
        /// <summary>
        /// </remarks>4 bytes</remarks>
        UnixFileCreationTime = 0x400C,

        /// <summary>
        /// Unix File Access Time
        /// <summary>
        /// </remarks>4 bytes</remarks>
        UnixFileAccessTime = 0x400D,

        /// <summary>
        /// Unix File Modification time
        /// <summary>
        /// </remarks>4 bytes</remarks>
        UnixFileModificationTime = 0x400E,

        /// <summary>
        /// IBMCMPSC Compression Info
        /// <summary>
        /// </remarks>variable</remarks>
        IBMCMPSCCompressionInfo = 0x4101,

        /// <summary>
        /// IBMCMPSC Compression Size
        /// <summary>
        /// </remarks>8 bytes  Big Endian</remarks>
        IBMCMPSCCompressionSize = 0x4102,
    }
}
