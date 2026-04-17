namespace SabreTools.Wrappers
{
    /// <summary>
    /// Represents each of the IWrapper implementations
    /// </summary>
    public enum WrapperType
    {
        /// <summary>
        /// Unknown or unsupported
        /// </summary>
        UNKNOWN,

        /// <summary>
        /// AACS media key block
        /// </summary>
        AACSMediaKeyBlock,

        /// <summary>
        /// Atari 7800 cart image
        /// </summary>
        Atari7800Cart,

        /// <summary>
        /// Atari Lynx cart image
        /// </summary>
        AtariLynxCart,

        /// <summary>
        /// BD+ SVM
        /// </summary>
        BDPlusSVM,

        /// <summary>
        /// BFPK custom archive
        /// </summary>
        BFPK,

        /// <summary>
        /// Half-Life Level
        /// </summary>
        BSP,

        /// <summary>
        /// bzip2 archive
        /// </summary>
        BZip2,

        /// <summary>
        /// CD-ROM bin file
        /// </summary>
        CDROM,

        /// <summary>
        /// Compound File Binary
        /// </summary>
        CFB,

        /// <summary>
        /// MAME Compressed Hunks of Data
        /// </summary>
        CHD,

        /// <summary>
        /// CTR Importable Archive
        /// </summary>
        CIA,

        /// <summary>
        /// Executable or library
        /// </summary>
        /// <remarks>Includes MZ, NE, LE/LX, and PE</remarks>
        Executable,

        /// <summary>
        /// fwNES FDS file
        /// </summary>
        FDS,

        /// <summary>
        /// Half-Life Game Cache File
        /// </summary>
        GCF,

        /// <summary>
        /// GCZ compressed GameCube / Wii disc image
        /// </summary>
        GCZ,

        /// <summary>
        /// gzip archive
        /// </summary>
        GZip,

        /// <summary>
        /// Key-value pair INI file
        /// </summary>
        /// <remarks>Currently has no IWrapper implementation</remarks>
        IniFile,

        /// <summary>
        /// InstallShield archive v3
        /// </summary>
        InstallShieldArchiveV3,

        /// <summary>
        /// InstallShield cabinet file
        /// </summary>
        InstallShieldCAB,

        /// <summary>
        /// PS3 ISO Rebuild Data
        /// </summary>
        IRD,

        /// <summary>
        /// ISO 9660 Volume (Disc image)
        /// </summary>
        ISO9660,

        /// <summary>
        /// Link Data Security encrypted file
        /// </summary>
        LDSCRYPT,

        /// <summary>
        /// LZ-compressed file, KWAJ variant
        /// </summary>
        LZKWAJ,

        /// <summary>
        /// LZ-compressed file, QBasic variant
        /// </summary>
        LZQBasic,

        /// <summary>
        /// LZ-compressed file, SZDD variant
        /// </summary>
        LZSZDD,

        /// <summary>
        /// Microsoft cabinet file
        /// </summary>
        MicrosoftCAB,

        /// <summary>
        /// MPQ game data archive
        /// </summary>
        MoPaQ,

        /// <summary>
        /// Nintendo 3DS cart image
        /// </summary>
        N3DS,

        /// <summary>
        /// Half-Life No Cache File
        /// </summary>
        NCF,

        /// <summary>
        /// Nintendo Entertainment System cart image
        /// </summary>
        NESCart,

        /// <summary>
        /// Nintendo DS/DSi cart image
        /// </summary>
        Nitro,

        /// <summary>
        /// Nintendo GameCube / Wii disc image
        /// </summary>
        NintendoDisc,

        /// <summary>
        /// Half-Life Package File
        /// </summary>
        PAK,

        /// <summary>
        /// NovaLogic Game Archive Format
        /// </summary>
        PFF,

        /// <summary>
        /// PIC data object
        /// </summary>
        PIC,

        /// <summary>
        /// PKWARE ZIP archive and derivatives
        /// </summary>
        PKZIP,

        /// <summary>
        /// PlayJ audio file
        /// </summary>
        PlayJAudioFile,

        /// <summary>
        /// PlayJ playlist file
        /// </summary>
        PlayJPlaylist,

        /// <summary>
        /// Quantum archive
        /// </summary>
        Quantum,

        /// <summary>
        /// Quick Disk Famicom Disk System image
        /// </summary>
        QD,

        /// <summary>
        /// RAR archive
        /// </summary>
        RAR,

        /// <summary>
        /// RealArcade Installer
        /// </summary>
        RealArcadeInstaller,

        /// <summary>
        /// RealArcade Mezzanine
        /// </summary>
        RealArcadeMezzanine,

        /// <summary>
        /// SecuROM DFA File
        /// </summary>
        SecuROMDFA,

        /// <summary>
        /// 7-zip archive
        /// </summary>
        SevenZip,

        /// <summary>
        /// StarForce FileSystem file
        /// </summary>
        SFFS,

        /// <summary>
        /// SGA
        /// </summary>
        SGA,

        /// <summary>
        /// Redumper skeleton (Wiped ISO9660 disc image)
        /// </summary>
        Skeleton,

        /// <summary>
        /// Steam SKU sis file
        /// </summary>
        SkuSis,

        /// <summary>
        /// Secure Transacted File System
        /// </summary>
        STFS,

        /// <summary>
        /// Tape archive
        /// </summary>
        TapeArchive,

        /// <summary>
        /// Various generic textfile formats
        /// </summary>
        /// <remarks>Currently has no IWrapper implementation</remarks>
        Textfile,

        /// <summary>
        /// Half-Life 2 Level
        /// </summary>
        VBSP,

        /// <summary>
        /// Valve Package File
        /// </summary>
        VPK,

        /// <summary>
        /// Half-Life Texture Package File
        /// </summary>
        WAD,

        /// <summary>
        /// Wise Installer Overlay Header
        /// </summary>
        WiseOverlayHeader,

        /// <summary>
        /// Wise Installer Script File
        /// </summary>
        WiseScript,

        /// <summary>
        /// WIA / RVZ compressed GameCube / Wii disc image
        /// </summary>
        WIA,

        /// <summary>
        /// XBox Executable
        /// </summary>
        XboxExecutable,

        /// <summary>
        /// Xbox DVD Filesystem
        /// </summary>
        XDVDFS,

        /// <summary>
        /// Xenon (Xbox 360) Executable
        /// </summary>
        XenonExecutable,

        /// <summary>
        /// xz archive
        /// </summary>
        XZ,

        /// <summary>
        /// Xbox Package File
        /// </summary>
        XZP,

        /// <summary>
        /// ZArchive archive
        /// </summary>
        ZArchive,

        /// <summary>
        /// ZStandard compressed file
        /// </summary>
        ZSTD,
    }
}
