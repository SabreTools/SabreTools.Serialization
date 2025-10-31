using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH#NCCH_Header"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class NCCHHeader
    {
        /// <summary>
        /// RSA-2048 signature of the NCCH header, using SHA-256.
        /// </summary>
        /// <remarks>0x100 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] RSA2048Signature = new byte[0x100];

        /// <summary>
        /// Magic ID, always 'NCCH'
        /// </summary>
        /// <remarks>4 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string? MagicID;

        /// <summary>
        /// Content size, in media units (1 media unit = 0x200 bytes)
        /// </summary>
        public uint ContentSizeInMediaUnits;

        /// <summary>
        /// Partition ID
        /// </summary>
        public ulong PartitionId;

        /// <summary>
        /// Maker code
        /// </summary>
        public ushort MakerCode;

        /// <summary>
        /// Version
        /// </summary>
        public ushort Version;

        /// <summary>
        /// When ncchflag[7] = 0x20 starting with FIRM 9.6.0-X, this is compared with the first output u32 from a
        /// SHA256 hash. The data used for that hash is 0x18-bytes: [0x10-long title-unique content lock seed]
        /// [programID from NCCH + 0x118]. This hash is only used for verification of the content lock seed, and
        /// is not the actual keyY.
        /// </summary>
        public uint VerificationHash;

        /// <summary>
        /// Program ID
        /// </summary>
        /// <remarks>8 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] ProgramId = new byte[8];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Reserved1 = new byte[0x10];

        /// <summary>
        /// Logo Region SHA-256 hash. (For applications built with SDK 5+) (Supported from firmware: 5.0.0-11)
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] LogoRegionHash = new byte[0x20];

        /// <summary>
        /// Product code
        /// </summary>
        /// <remarks>0x10 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
        public string? ProductCode;

        /// <summary>
        /// Extended header SHA-256 hash (SHA256 of 2x Alignment Size, beginning at 0x0 of ExHeader)
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] ExtendedHeaderHash = new byte[0x20];

        /// <summary>
        /// Extended header size, in bytes
        /// </summary>
        public uint ExtendedHeaderSizeInBytes;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved2;

        /// <summary>
        /// Flags
        /// </summary>
        public NCCHHeaderFlags? Flags;

        /// <summary>
        /// Plain region offset, in media units
        /// </summary>
        public uint PlainRegionOffsetInMediaUnits;

        /// <summary>
        /// Plain region size, in media units
        /// </summary>
        public uint PlainRegionSizeInMediaUnits;

        /// <summary>
        /// Logo Region offset, in media units (For applications built with SDK 5+) (Supported from firmware: 5.0.0-11)
        /// </summary>
        public uint LogoRegionOffsetInMediaUnits;

        /// <summary>
        /// Logo Region size, in media units (For applications built with SDK 5+) (Supported from firmware: 5.0.0-11)
        /// </summary>
        public uint LogoRegionSizeInMediaUnits;

        /// <summary>
        /// ExeFS offset, in media units
        /// </summary>
        public uint ExeFSOffsetInMediaUnits;

        /// <summary>
        /// ExeFS size, in media units
        /// </summary>
        public uint ExeFSSizeInMediaUnits;

        /// <summary>
        /// ExeFS hash region size, in media units
        /// </summary>
        public uint ExeFSHashRegionSizeInMediaUnits;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved3;

        /// <summary>
        /// RomFS offset, in media units
        /// </summary>
        public uint RomFSOffsetInMediaUnits;

        /// <summary>
        /// RomFS size, in media units
        /// </summary>
        public uint RomFSSizeInMediaUnits;

        /// <summary>
        /// RomFS hash region size, in media units
        /// </summary>
        public uint RomFSHashRegionSizeInMediaUnits;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved4;

        /// <summary>
        /// ExeFS superblock SHA-256 hash - (SHA-256 hash, starting at 0x0 of the ExeFS over the number of
        /// media units specified in the ExeFS hash region size)
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] ExeFSSuperblockHash = new byte[0x20];

        /// <summary>
        /// RomFS superblock SHA-256 hash - (SHA-256 hash, starting at 0x0 of the RomFS over the number
        /// of media units specified in the RomFS hash region size)
        /// </summary>
        /// <remarks>0x20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] RomFSSuperblockHash = new byte[0x20];
    }
}
