using System;

namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// XGD1 security sector data
    /// </summary>
    /// <see href="https://github.com/Deterous/ParseXboxMetadata/blob/main/docs/Structure%20of%20XGD1%20SS%20and%20DMI.pdf"/>
    public class SecuritySectorXGD1 : SecuritySector
    {
        // Common PFI data in base class

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>703 bytes, all 0x00</remarks>
        public byte[] Reserved0011 { get; set; } = new byte[703];

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
        /// Encrypted challenge responses
        /// </summary>
        /// <remarks>23 entries</remarks>
        public EncryptedChallengeResponseXGD1[] EncryptedChallengeResponses { get; set; } = new EncryptedChallengeResponseXGD1[23];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>32 bytes, all 0x00</remarks>
        public byte[] Reserved03FF { get; set; } = new byte[32];

        /// <summary>
        /// FILETIME - Creation Timestamp?
        /// </summary>
        public ulong CreationTimestamp { get; set; }

        /// <summary>
        /// Unknown 16 bytes (Certificate GUID?)
        /// </summary>
        public Guid CertificateGuid { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>4 bytes, all 0x00</remarks>
        public byte[] Reserved0437 { get; set; } = new byte[4];

        /// <summary>
        /// Unknown 16 bytes (Authoring GUID?)
        /// </summary>
        public Guid AuthoringGuid { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>84 bytes, all 0x00</remarks>
        public byte[] Reserved044B { get; set; } = new byte[84];

        /// <summary>
        /// FILETIME - Authoring Timestamp
        /// </summary>
        public ulong AuthoringTimestamp { get; set; }

        /// <summary>
        /// time_t - Certificate Timestamp
        /// </summary>
        /// <remarks>Early discs are all 0x00, if so then 0x427 is also 0x00</remarks>
        public uint CertificateTimestamp { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>15 bytes, all 0x00</remarks>
        public byte[] Reserved04AB { get; set; } = new byte[15];

        /// <summary>
        /// Version?
        /// </summary>
        /// <remarks>Should be 0x01</remarks>
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
        /// <remarks>0xFF (XGD1 discs from Mar-2007 onwards are 0x02 like XGD2)</remarks>
        public byte Unknown05FA { get; set; }

        /// <summary>
        /// Unknown 16 bytes (Disc Mastering GUID? 26 known variations for XGD1)
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
        /// <remarks>Should be 0x01</remarks>
        public byte SSVersion { get; set; }

        /// <summary>
        /// Number of security sector ranges
        /// </summary>
        /// <remarks>Should be 0x17</remarks>
        public byte NumberOfSecuritySectorRanges { get; set; }

        /// <summary>
        /// 23 Challenge Response Data (9 bytes each, includes Security Sector Ranges)
        /// </summary>
        public DriveEntryData[] ChallengeResponses { get; set; } = new DriveEntryData[23];

        /// <summary>
        /// Duplicated 23 Challenge Response Data (9 bytes each)
        /// </summary>
        public DriveEntryData[] DuplicatedChallengeResponses { get; set; } = new DriveEntryData[23];

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>Should be 0x00</remarks>
        public byte Reserved07FF { get; set; }
    }
}
