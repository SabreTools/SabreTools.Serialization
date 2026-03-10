using System;

namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD2 security sector data
    /// </summary>
    /// <see href="https://github.com/Deterous/ParseXboxMetadata/blob/main/docs/Structure%20of%20XGD2%20SS%20and%20DMI.pdf"/>
    public class SecuritySectorXGD2 : SecuritySector
    {
        // Common PFI data in base class

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>239 bytes, all 0x00</remarks>
        public byte[] Reserved0011 { get; set; } = new byte[239];

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Should be 0x00, 0x00, 0x00, 0x30</remarks>
        public byte[] Unknown0100 { get; set; } = new byte[4];

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Should be 0x00, 0x00, 0x06, 0xE0</remarks>
        public byte[] Unknown0104 { get; set; } = new byte[4];

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>SHA-1? Unique for each ISO</remarks>
        public byte[] Unknown0108 { get; set; } = new byte[20];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>228 bytes, all 0x00</remarks>
        public byte[] Reserved011C { get; set; } = new byte[228];

        /// <summary>
        /// 23 Challege Response data (9 bytes each)
        /// </summary>
        public ChallengeResponseDataXGD23[] ChallengeResponses { get; set; } = new ChallengeResponseDataXGD23[23];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>Should be 0x00</remarks>
        public byte Reserved02CF { get; set; }

        /// <summary>
        /// CPR_MAI key
        /// </summary>
        /// <remarks>Unknown endianness</remarks>
        public uint CprMaiKey { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>44 bytes, all 0x00</remarks>
        public byte[] Reserved02D4 { get; set; } = new byte[44];

        /// <summary>
        /// Encrypted challenge response table - version
        /// </summary>
        /// <remarks>Should be 0x01</remarks>
        public byte EncryptedChallengeResponseTableVersion { get; set; }

        /// <summary>
        /// Encrypted challenge response table - entries
        /// </summary>
        /// <remarks>Should be 0x17</remarks>
        public byte EncryptedChallengeResponseTableEntryCount { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>2 bytes, all 0x00</remarks>
        public byte[] Reserved0302 { get; set; } = new byte[2];

        /// <summary>
        /// Encrypted challenge responses
        /// </summary>
        /// <remarks>21 entries</remarks>
        public EncryptedChallengeResponseXGD23[] EncryptedChallengeResponses { get; set; } = new EncryptedChallengeResponseXGD23[21];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>96 bytes, all 0x00</remarks>
        public byte[] Reserved0400 { get; set; } = new byte[96];

        /// <summary>
        /// Media ID
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] MediaID { get; set; } = new byte[16];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>46 bytes, all 0x00</remarks>
        public byte[] Reserved0470 { get; set; } = new byte[46];

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Should be 0x04</remarks>
        public byte Unknown049E { get; set; }

        /// <summary>
        /// FILETIME - Authoring Timestamp
        /// </summary>
        public ulong AuthoringTimestamp { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>19 bytes, all 0x00</remarks>
        public byte[] Reserved04A7 { get; set; } = new byte[19];

        /// <summary>
        /// Version?
        /// </summary>
        /// <remarks>Should be 0x02</remarks>
        public byte Version { get; set; }

        /// <summary>
        /// Unknown 16 bytes (ISO Mastering GUID?)
        /// </summary>
        public Guid ISOMasteringGuid { get; set; }

        /// <summary>
        /// SHA-1 hash A
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] SHA1HashA { get; set; } = new byte[20];

        /// <summary>
        /// Signature A
        /// </summary>
        /// <remarks>256 bytes</remarks>
        public byte[] SignatureA { get; set; } = new byte[256];

        /// <summary>
        /// FILETIME - Disc Mastering Timestamp
        /// </summary>
        public ulong DiscMasteringTimestamp { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>4 bytes, all 0x00; meant to be a time_t?</remarks>
        public byte[] Reserved05E7 { get; set; } = new byte[4];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>15 bytes, all 0x00</remarks>
        public byte[] Reserved05EB { get; set; } = new byte[15];

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>Should be 0x02</remarks>
        public byte Unknown05FA { get; set; }

        /// <summary>
        /// Unknown 16 bytes (Disc Mastering GUID?)
        /// </summary>
        public Guid DiscMasteringGuid { get; set; }

        /// <summary>
        /// SHA-1 hash B
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] SHA1HashB { get; set; } = new byte[20];

        /// <summary>
        /// Signature B
        /// </summary>
        /// <remarks>256 bytes</remarks>
        public byte[] SignatureB { get; set; } = new byte[256];

        /// <summary>
        /// SS Version
        /// </summary>
        /// <remarks>Should be 0x02</remarks>
        public byte SSVersion { get; set; }

        /// <summary>
        /// Number of security sector ranges
        /// </summary>
        /// <remarks>Should be 0x15</remarks>
        public byte NumberOfSecuritySectorRanges { get; set; }

        /// <summary>
        /// 21 Security Sector Ranges (9 bytes each)
        /// </summary>
        /// <remarks>Actually 23 entries</remarks>
        public DriveEntryData[] DriveEntries { get; set; } = new DriveEntryData[23];

        /// <summary>
        /// Duplicated 21 Security Sector Ranges (9 bytes each)
        /// </summary>
        /// <remarks>Actually 23 entries</remarks>
        public DriveEntryData[] DuplicatedDriveEntries { get; set; } = new DriveEntryData[23];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>Should be 0x00</remarks>
        public byte Reserved07FF { get; set; }
    }
}
