using System;

namespace SabreTools.Data.Models.GZIP
{
    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    public enum CompressionMethod : byte
    {
        RESERVED0 = 0,
        RESERVED1 = 1,
        RESERVED2 = 2,
        RESERVED3 = 3,
        RESERVED4 = 4,
        RESERVED5 = 5,
        RESERVED6 = 6,
        RESERVED7 = 7,

        Deflate = 8,
    }

    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    [Flags]
    public enum ExtraFlags : byte
    {
        /// <summary>
        /// Compressor used maximum compression, slowest algorithm
        /// </summary>
        MaximumCompression = 0x02,

        /// <summary>
        /// Compressor used fastest algorithm
        /// </summary>
        FastestAlgorithm = 0x04,
    }

    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    [Flags]
    public enum Flags : byte
    {
        /// <summary>
        /// The file is probably ASCII text
        /// </summary>
        FTEXT = 0x01,

        /// <summary>
        /// CRC-16 for the gzip header is present
        /// </summary>
        FHCRC = 0x02,

        /// <summary>
        /// Optional extra fields are present
        /// </summary>
        FEXTRA = 0x04,

        /// <summary>
        /// Original filename is present
        /// </summary>
        FNAME = 0x08,

        /// <summary>
        /// Zero-terminated file comment is present
        /// </summary>
        FCOMMENT = 0x10,

        RESERVED5 = 0x20,
        RESERVED6 = 0x40,
        RESERVED7 = 0x80,
    }

    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    public enum OperatingSystem : byte
    {
        /// <summary>
        /// FAT filesystem (MS-DOS, OS/2, NT/Win32)
        /// </summary>
        FAT = 0,

        /// <summary>
        /// Amiga
        /// </summary>
        Amiga = 1,

        /// <summary>
        /// VMS (or OpenVMS)
        /// </summary>
        VMS = 2,

        /// <summary>
        /// Unix
        /// </summary>
        Unix = 3,

        /// <summary>
        /// VM/CMS
        /// </summary>
        VMCMS = 4,

        /// <summary>
        /// Atari TOS
        /// </summary>
        AtariTOS = 5,

        /// <summary>
        /// HPFS filesystem (OS/2, NT)
        /// </summary>
        HPFS = 6,

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
        /// TOPS-20
        /// </summary>
        TOPS20 = 10,

        /// <summary>
        /// NTFS filesystem (NT)
        /// </summary>
        NTFS = 11,

        /// <summary>
        /// QDOS
        /// </summary>
        QDOS = 12,

        /// <summary>
        /// Acorn RISCOS
        /// </summary>
        AcornRISCOS = 13,

        /// <summary>
        /// unknown
        /// </summary>
        Unknown = 255,
    }
}