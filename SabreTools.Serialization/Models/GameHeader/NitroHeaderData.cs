namespace SabreTools.Serialization.Models.GameHeader
{
    /// <summary>
    /// Header Data section for an NDS cart image
    /// </summary>
    public sealed class NitroHeaderData
    {
        public string? GameTitle { get; set; }

        public string? GameSerial { get; set; }

        public string? MakerCode { get; set; }

        public byte UnitCode { get; set; }

        public byte EncryptionSeed { get; set; }

        public byte DeviceSize { get; set; }

        public string? DeviceSizeInfo { get; set; }

        public byte AsianRegion { get; set; }

        // Hex string, prefixed
        public string? Reserved1 { get; set; }

        public byte Version { get; set; }

        public string? VersionInfo { get; set; }

        public byte Autostart { get; set; }

        public string? AutostartInfo { get; set; }

        public uint ARM9ROMOffset { get; set; }

        public uint ARM9EntryAddress { get; set; }

        public uint ARM9RAMOffset { get; set; }

        public uint ARM7ROMOffset { get; set; }

        public uint ARM7EntryAddress { get; set; }

        public uint ARM7RAMOffset { get; set; }

        public uint FNTOffset { get; set; }

        public uint FNTSize { get; set; }

        public uint FATOffset { get; set; }

        public uint FATSize { get; set; }

        public uint ARM9OverlayOffset { get; set; }

        public uint ARM9OverlaySize { get; set; }

        public uint ARM7OverlayOffset { get; set; }

        public uint ARM7OverlaySize { get; set; }

        public uint NormalCMDSetting { get; set; }

        public uint Key1CMDSetting { get; set; }

        public uint IconAddress { get; set; }

        public ushort SecureCRC16 { get; set; }

        public string? SecureCRC16Info { get; set; }

        public ushort SecureTimeout { get; set; }

        public uint ARM9AutoloadAddress { get; set; }

        public uint ARM7AutoloadAddress { get; set; }

        public ulong SecureDisable { get; set; }

        public uint UsedRomSize { get; set; }

        public string? UsedRomSizeInfo { get; set; }

        public uint HeaderSize { get; set; }

        public string? HeaderSizeInfo { get; set; }

        // Hex string, no prefix
        public string? Reserved2 { get; set; }

        // Hex string, no prefix
        public string? NintendoLogo { get; set; }

        public ushort LogoCRC16 { get; set; }

        public string? LogoCRC16Info { get; set; }

        public ushort HeaderCRC16 { get; set; }

        public string? HeaderCRC16Info { get; set; }

        // Hex string, prefixed
        public string? Reserved3 { get; set; }

        // Hex string, prefixed
        public string? ConfigSettings { get; set; }

        public uint DsiRegionMask { get; set; }

        public uint AccessControl { get; set; }

        public uint ARM7SCFG { get; set; }

        public uint DSiAppFlags { get; set; }

        public uint DSi9RomOffset { get; set; }

        public uint DSi9EntryAddress { get; set; }

        public uint DSi9RamAddress { get; set; }

        public uint DSi9Size { get; set; }

        public uint DSi7RomOffset { get; set; }

        public uint DSi7EntryAddress { get; set; }

        public uint DSi7RamAddress { get; set; }

        public uint DSi7Size { get; set; }

        public uint DigestNTROffset { get; set; }

        public uint DigestNTRSize { get; set; }

        public uint DigestTWLOffset { get; set; }

        public uint DigestTWLSize { get; set; }

        public uint DigestSectorHashTableOffset { get; set; }

        public uint DigestSectorHashTableSize { get; set; }

        public uint DigestBlockHashTableOffset { get; set; }

        public uint DigestBlockHashTableLength { get; set; }

        public uint DigestSectorSize { get; set; }

        public uint DigestBlockSectorCount { get; set; }

        // Hex string, prefixed
        public string? Reserved4 { get; set; }

        public uint Modcrypt1Offset { get; set; }

        public uint Modcrypt1Size { get; set; }

        public uint Modcrypt2Offset { get; set; }

        public uint Modcrypt2Size { get; set; }

        public ulong TitleID { get; set; }

        // Hex string, prefixed
        public string? Reserved5 { get; set; }

        // Hex string, prefixed
        public string? ARM9SHA1HMAC { get; set; }

        // Hex string, prefixed
        public string? ARM7SHA1HMAC { get; set; }

        // Hex string, prefixed
        public string? DigestMasterSHA1HMAC { get; set; }

        // Hex string, prefixed
        public string? BannerSHA1HMAC { get; set; }

        // Hex string, prefixed
        public string? ARM9iSHA1HMAC { get; set; }

        // Hex string, prefixed
        public string? ARM7iSHA1HMAC { get; set; }

        // Hex string, prefixed
        public string? Reserved6 { get; set; }

        // Hex string, prefixed
        public string? UnknownHash { get; set; }

        // Hex string, prefixed
        public string? Reserved7 { get; set; }

        // Hex string, prefixed
        public string? Reserved8 { get; set; }

        // Hex string, prefixed
        public string? RSASignature { get; set; }
    }
}