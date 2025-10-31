using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.Nitro
{
    /// <summary>
    /// Nintendo DSi extended cart header
    /// </summary>
    /// <see href="https://dsibrew.org/wiki/DSi_cartridge_header"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ExtendedDSiHeader
    {
        /// <summary>
        /// Global MBK1..MBK5 Settings
        /// </summary>
        /// <remarks>5 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] GlobalMBK15Settings = new uint[5];

        /// <summary>
        ///	Local MBK6..MBK8 Settings for ARM9
        /// </summary>
        /// <remarks>3 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] LocalMBK68SettingsARM9 = new uint[3];

        /// <summary>
        /// Local MBK6..MBK8 Settings for ARM7
        /// </summary>
        /// <remarks>3 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] LocalMBK68SettingsARM7 = new uint[3];

        /// <summary>
        /// Global MBK9 Setting
        /// </summary>
        public uint GlobalMBK9Setting;

        /// <summary>
        /// Region Flags
        /// </summary>
        public uint RegionFlags;

        /// <summary>
        /// Access control
        /// </summary>
        public uint AccessControl;

        /// <summary>
        /// ARM7 SCFG EXT mask (controls which devices to enable)
        /// </summary>
        public uint ARM7SCFGEXTMask;

        /// <summary>
        /// Reserved/flags? When bit2 of byte 0x1bf is set, usage of banner.sav from the title data dir is enabled.(additional banner data)
        /// </summary>
        public uint ReservedFlags;

        /// <summary>
        /// ARM9i rom offset
        /// </summary>
        public uint ARM9iRomOffset;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved3;

        /// <summary>
        /// ARM9i load address
        /// </summary>
        public uint ARM9iLoadAddress;

        /// <summary>
        /// ARM9i size;
        /// </summary>
        public uint ARM9iSize;

        /// <summary>
        /// ARM7i rom offset
        /// </summary>
        public uint ARM7iRomOffset;

        /// <summary>
        /// Pointer to base address where various structures and parameters are passed to the title - what is that???
        /// </summary>
        public uint Reserved4;

        /// <summary>
        /// ARM7i load address
        /// </summary>
        public uint ARM7iLoadAddress;

        /// <summary>
        /// ARM7i size;
        /// </summary>
        public uint ARM7iSize;

        /// <summary>
        /// Digest NTR region offset
        /// </summary>
        public uint DigestNTRRegionOffset;

        /// <summary>
        /// Digest NTR region length
        /// </summary>
        public uint DigestNTRRegionLength;

        // <summary>
        /// Digest TWL region offset
        /// </summary>
        public uint DigestTWLRegionOffset;

        /// <summary>
        /// Digest TWL region length
        /// </summary>
        public uint DigestTWLRegionLength;

        // <summary>
        /// Digest Sector Hashtable region offset
        /// </summary>
        public uint DigestSectorHashtableRegionOffset;

        /// <summary>
        /// Digest Sector Hashtable region length
        /// </summary>
        public uint DigestSectorHashtableRegionLength;

        // <summary>
        /// Digest Block Hashtable region offset
        /// </summary>
        public uint DigestBlockHashtableRegionOffset;

        /// <summary>
        /// Digest Block Hashtable region length
        /// </summary>
        public uint DigestBlockHashtableRegionLength;

        /// <summary>
        /// Digest Sector size
        /// </summary>
        public uint DigestSectorSize;

        /// <summary>
        /// Digeset Block Sectorount
        /// </summary>
        public uint DigestBlockSectorCount;

        /// <summary>
        /// Icon Banner Size (usually 0x23C0)
        /// </summary>
        public uint IconBannerSize;

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public uint Unknown1;

        /// <summary>
        /// NTR+TWL region ROM size (total size including DSi area)
        /// </summary>
        public uint NTRTWLRegionRomSize;

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        /// <remarks>12 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] Unknown2 = new byte[12];

        /// <summary>
        /// Modcrypt area 1 offset
        /// </summary>
        public uint ModcryptArea1Offset;

        /// <summary>
        /// Modcrypt area 1 size
        /// </summary>
        public uint ModcryptArea1Size;

        /// <summary>
        /// Modcrypt area 2 offset
        /// </summary>
        public uint ModcryptArea2Offset;

        /// <summary>
        /// Modcrypt area 2 size
        /// </summary>
        public uint ModcryptArea2Size;

        /// <summary>
        /// Title ID
        /// </summary>
        /// <remarks>8 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] TitleID = new byte[8];

        /// <summary>
        /// DSiWare: "public.sav" size
        /// </summary>
        public uint DSiWarePublicSavSize;

        /// <summary>
        /// DSiWare: "private.sav" size
        /// </summary>
        public uint DSiWarePrivateSavSize;

        /// <summary>
        /// Reserved (zero)
        /// </summary>
        /// <remarks>176 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 176)]
        public byte[] ReservedZero = new byte[176];

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        /// <remarks>16 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Unknown3 = new byte[16];

        /// <summary>
        /// ARM9 (with encrypted secure area) SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ARM9WithSecureAreaSHA1HMACHash = new byte[20];

        /// <summary>
        /// ARM7 SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ARM7SHA1HMACHash = new byte[20];

        /// <summary>
        /// Digest master SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] DigestMasterSHA1HMACHash = new byte[20];

        /// <summary>
        /// Banner SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] BannerSHA1HMACHash = new byte[20];

        /// <summary>
        /// ARM9i (decrypted) SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ARM9iDecryptedSHA1HMACHash = new byte[20];

        /// <summary>
        /// ARM7i (decrypted) SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ARM7iDecryptedSHA1HMACHash = new byte[20];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>40 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] Reserved5 = new byte[40];

        /// <summary>
        /// ARM9 (without secure area) SHA1 HMAC hash
        /// </summary>
        /// <remarks>20 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] ARM9NoSecureAreaSHA1HMACHash = new byte[20];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>2636 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2636)]
        public byte[] Reserved6 = new byte[2636];

        /// <summary>
        /// Reserved and unchecked region, always zero. Used for passing arguments in debug environment.
        /// </summary>
        /// <remarks>0x180 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x180)]
        public byte[] ReservedAndUnchecked = new byte[0x180];

        /// <summary>
        /// RSA signature (the first 0xE00 bytes of the header are signed with an 1024-bit RSA signature).
        /// </summary>
        /// <remarks>0x80 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
        public byte[] RSASignature = new byte[0x80];
    }
}
