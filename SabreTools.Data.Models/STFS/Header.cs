using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// Secure Transacted File System, Header format
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class Header
    {
        /// <summary>
        /// Magic bytes indicating the format
        /// Possible values are "LIVE", "PIRS, and "CON "
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] MagicBytes { get; set; } = new byte[4];

        /// <summary>
        /// Signature Block
        /// Not optional, but abstract class
        /// </summary>
        /// <remarks>552 bytes</remarks>
        public Signature? Signature { get; set; }

        /// <summary>
        /// Used to check package owner
        /// 16 license entries, 16 bytes each
        /// </summary>
        /// <remarks>256 bytes</remarks>
        public LicenseEntry[] LicensingData { get; set; } = new LicenseEntry[16];

        /// <summary>
        /// SHA-1 Integrity Hash of the header (from ContentType/0x344 to first hash table) 
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] HeaderHash { get; set; } = new byte[20];

        /// <summary>
        /// Size of the header, in bytes (from ??? to ???)
        /// The actual end of header is padded and zeroed up until next multiple of 4096 bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint HeaderSize { get; set; }

        /// <summary>
        /// Indication of the content in the STFS
        /// See Enum.ContentType
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int ContentType { get; set; }

        /// <summary>
        /// Intended meaning of some of the below fields
        /// Known values are 0, 1 and 2
        /// 0 = A bunch of fields will be zeroed (Seen in system updates)
        /// 1 = All but the below fields are set
        /// 2 = The following new fields are set:
        /// SeriesID, SeasonID, SeasonNumber, EpisodeNumber,
        /// AdditionalDisplayNames, AdditionalDisplayDescriptions
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int MetadataVersion { get; set; }

        /// <summary>
        /// Size of content in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public long ContentSize { get; set; }

        /// <summary>
        /// Media ID
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint MediaID { get; set; }

        /// <summary>
        /// Version of System/Title Updates
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int Version { get; set; }

        /// <summary>
        /// Base Version of System/Title Updates
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int BaseVersion { get; set; }

        /// <summary>
        /// Title ID
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint TitleID { get; set; }

        /// <summary>
        /// Intended platform for content
        /// 0 = ???, 2 = Xbox 360, 4 = PC
        /// </summary>
        public byte Platform { get; set; }

        /// <summary>
        /// Intended platform for content
        /// Xbox 360 = 2, PC = 4
        /// </summary>
        public byte ExecutableType { get; set; }

        /// <summary>
        /// Disc Number
        /// </summary>
        public byte DiscNumber { get; set; }

        /// <summary>
        /// Disc In Set
        /// </summary>
        public byte DiscInSet { get; set; }

        /// <summary>
        /// Save Game ID
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint SaveGameID { get; set; }

        /// <summary>
        /// Console ID
        /// </summary>
        /// <remarks>5 bytes</remarks>
        public byte[] ConsoleID { get; set; } = new byte[5];

        /// <summary>
        /// Profile ID
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public byte[] ProfileID { get; set; } = new byte[8];

        /// <summary>
        /// STFS Volume Descriptor
        /// Not optional, but abstract class
        /// </summary>
        public VolumeDescriptor? VolumeDescriptor { get; set; }

        /// <summary>
        /// Data File Count
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int DataFileCount { get; set; }

        /// <summary>
        /// Data File Combined Size, in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public long DataFileCombinedSize { get; set; }

        /// <summary>
        /// Descriptor Type
        /// 0 = STFS, 1 = SVOD
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint DescriptorType { get; set; }

        /// <summary>
        /// Reserved bytes, should be all zeroed
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Reserved { get; set; }

        /// <summary>
        /// Series ID
        /// Zeroed for MetadataVersion = 1
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[]? SeriesID { get; set; } = new byte[16];

        /// <summary>
        /// Season ID
        /// Zeroed for MetadataVersion = 1
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[]? SeasonID { get; set; } = new byte[16];

        /// <summary>
        /// Season Number
        /// Zeroed for MetadataVersion = 1
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public short? SeasonNumber { get; set; }

        /// <summary>
        /// Season Number
        /// Zeroed for MetadataVersion = 1
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public short? EpisodeNumber { get; set; }

        /// <summary>
        /// Padding bytes
        /// If MetadataVersion is 2, there are 40 bytes
        /// Otherwise, there are 76 bytes
        /// </summary>
        /// <remarks>40 bytes</remarks>
        public byte[] Padding { get; set; } = [];

        /// <summary>
        /// Device ID
        /// </summary>
        /// <remarks>20 bytes</remarks>
        public byte[] DeviceID { get; set; } = new byte[20];

        /// <summary>
        /// Display Name, UTF-8 string
        /// 128 bytes per locale, 18 different locales
        /// </summary>
        /// <remarks>2304 bytes, UTF-8 string</remarks>
        public byte[] DisplayName { get; set; } = new byte[2304];

        /// <summary>
        /// Display Description, UTF-8 string
        /// 128 bytes per locale, 18 different locales
        /// </summary>
        /// <remarks>2304 bytes, UTF-8 string</remarks>
        public byte[] DisplayDescription { get; set; } = new byte[2304];

        /// <summary>
        /// Publisher Name, UTF-8 string
        /// </summary>
        /// <remarks>128 bytes, UTF-8 string</remarks>
        public byte[] PublisherName { get; set; } = new byte[128];

        /// <summary>
        /// Title Name, UTF-8 string
        /// </summary>
        /// <remarks>128 bytes, UTF-8 string</remarks>
        public byte[] TitleName { get; set; } = new byte[128];

        /// <summary>
        /// Transfer Flags, see Constants.TransferFlags
        /// </summary>
        public byte TransferFlags { get; set; }

        /// <summary>
        /// Size of the thumbnail image, in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int ThumbnailImageSize { get; set; }

        /// <summary>
        /// Size of the title thumbnail image, in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public int TitleThumbnailImageSize { get; set; }

        /// <summary>
        /// Thumbnail image
        /// If Metadata version = 1, 0x4000 bytes allocated, padded with zeroes
        /// If Metadata version = 2, 0x3D00 bytes allocated, padded with zeroes
        /// </summary>
        public byte[] ThumbnailImage { get; set; } = [];

        /// <summary>
        /// Additional Display Names, UTF-8 string
        /// 128 bytes per locale, 6 different locales
        /// Only present if MetadataVersion = 2
        /// </summary>
        /// <remarks>If present, 768 bytes, UTF-8 string</remarks>
        public byte[]? AdditionalDisplayNames { get; set; }

        /// <summary>
        /// Title thumbnail image
        /// If Metadata version = 1, 0x4000 bytes allocated, padded with zeroes
        /// If Metadata version = 2, 0x3D00 bytes allocated, padded with zeroes
        /// </summary>
        public byte[] TitleThumbnailImage { get; set; } = [];

        /// <summary>
        /// Additional Display Descriptions, UTF-8 string
        /// 128 bytes per locale, 6 different locales
        /// Only present if MetadataVersion = 2
        /// </summary>
        /// <remarks>If present, 768 bytes, UTF-8 string</remarks>
        public byte[]? AdditionalDisplayDescriptions { get; set; }
    }
}
