using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;
using SabreTools.Text.Extensions;

namespace SabreTools.Metadata.DatItems.Formats
{
    /// <summary>
    /// Represents a generic file within a set
    /// </summary>
    [JsonObject("rom"), XmlRoot("rom")]
    public sealed class Rom : DatItem<Data.Models.Metadata.Rom>
    {
        #region Properties

        public string? Album
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Album;
            set => (_internal as Data.Models.Metadata.Rom)?.Album = value;
        }

        public string? AltRomname
        {
            get => (_internal as Data.Models.Metadata.Rom)?.AltRomname;
            set => (_internal as Data.Models.Metadata.Rom)?.AltRomname = value;
        }

        public string? AltTitle
        {
            get => (_internal as Data.Models.Metadata.Rom)?.AltTitle;
            set => (_internal as Data.Models.Metadata.Rom)?.AltTitle = value;
        }

        public string? Artist
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Artist;
            set => (_internal as Data.Models.Metadata.Rom)?.Artist = value;
        }

        public string? ASRDetectedLang
        {
            get => (_internal as Data.Models.Metadata.Rom)?.ASRDetectedLang;
            set => (_internal as Data.Models.Metadata.Rom)?.ASRDetectedLang = value;
        }

        public string? ASRDetectedLangConf
        {
            get => (_internal as Data.Models.Metadata.Rom)?.ASRDetectedLangConf;
            set => (_internal as Data.Models.Metadata.Rom)?.ASRDetectedLangConf = value;
        }

        public string? ASRTranscribedLang
        {
            get => (_internal as Data.Models.Metadata.Rom)?.ASRTranscribedLang;
            set => (_internal as Data.Models.Metadata.Rom)?.ASRTranscribedLang = value;
        }

        public string? Bios
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Bios;
            set => (_internal as Data.Models.Metadata.Rom)?.Bios = value;
        }

        public string? Bitrate
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Bitrate;
            set => (_internal as Data.Models.Metadata.Rom)?.Bitrate = value;
        }

        public string? BitTorrentMagnetHash
        {
            get => (_internal as Data.Models.Metadata.Rom)?.BitTorrentMagnetHash;
            set => (_internal as Data.Models.Metadata.Rom)?.BitTorrentMagnetHash = value;
        }

        public string? ClothCoverDetectionModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.ClothCoverDetectionModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.ClothCoverDetectionModuleVersion = value;
        }

        public string? CollectionCatalogNumber
        {
            get => (_internal as Data.Models.Metadata.Rom)?.CollectionCatalogNumber;
            set => (_internal as Data.Models.Metadata.Rom)?.CollectionCatalogNumber = value;
        }

        public string? Comment
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Comment;
            set => (_internal as Data.Models.Metadata.Rom)?.Comment = value;
        }

        public string? CRC
        {
            get => (_internal as Data.Models.Metadata.Rom)?.CRC;
            set => (_internal as Data.Models.Metadata.Rom)?.CRC = value;
        }

        public string? CRC16
        {
            get => (_internal as Data.Models.Metadata.Rom)?.CRC16;
            set => (_internal as Data.Models.Metadata.Rom)?.CRC16 = value;
        }

        public string? CRC64
        {
            get => (_internal as Data.Models.Metadata.Rom)?.CRC64;
            set => (_internal as Data.Models.Metadata.Rom)?.CRC64 = value;
        }

        public string? Creator
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Creator;
            set => (_internal as Data.Models.Metadata.Rom)?.Creator = value;
        }

        public DataArea? DataArea { get; set; }

        [JsonIgnore]
        public bool DataAreaSpecified
        {
            get
            {
                return DataArea is not null
                    && (!string.IsNullOrEmpty(DataArea.Name)
                        || DataArea.Size is not null
                        || DataArea.Width is not null
                        || DataArea.Endianness is not null);
            }
        }

        public string? Date
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Date;
            set => (_internal as Data.Models.Metadata.Rom)?.Date = value;
        }

        public bool? Dispose
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Dispose;
            set => (_internal as Data.Models.Metadata.Rom)?.Dispose = value;
        }

        public string? Extension
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Extension;
            set => (_internal as Data.Models.Metadata.Rom)?.Extension = value;
        }

        public long? FileCount
        {
            get => (_internal as Data.Models.Metadata.Rom)?.FileCount;
            set => (_internal as Data.Models.Metadata.Rom)?.FileCount = value;
        }

        public bool? FileIsAvailable
        {
            get => (_internal as Data.Models.Metadata.Rom)?.FileIsAvailable;
            set => (_internal as Data.Models.Metadata.Rom)?.FileIsAvailable = value;
        }

        public string? Flags
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Flags;
            set => (_internal as Data.Models.Metadata.Rom)?.Flags = value;
        }

        public string? Format
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Format;
            set => (_internal as Data.Models.Metadata.Rom)?.Format = value;
        }

        public string? Header
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Header;
            set => (_internal as Data.Models.Metadata.Rom)?.Header = value;
        }

        public string? Height
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Height;
            set => (_internal as Data.Models.Metadata.Rom)?.Height = value;
        }

        public string? hOCRCharToWordhOCRVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.hOCRCharToWordhOCRVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.hOCRCharToWordhOCRVersion = value;
        }

        public string? hOCRCharToWordModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.hOCRCharToWordModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.hOCRCharToWordModuleVersion = value;
        }

        public string? hOCRFtsTexthOCRVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.hOCRFtsTexthOCRVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.hOCRFtsTexthOCRVersion = value;
        }

        public string? hOCRFtsTextModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.hOCRFtsTextModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.hOCRFtsTextModuleVersion = value;
        }

        public string? hOCRPageIndexhOCRVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.hOCRPageIndexhOCRVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.hOCRPageIndexhOCRVersion = value;
        }

        public string? hOCRPageIndexModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.hOCRPageIndexModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.hOCRPageIndexModuleVersion = value;
        }

        public bool? Inverted
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Inverted;
            set => (_internal as Data.Models.Metadata.Rom)?.Inverted = value;
        }

        public string? LastModifiedTime
        {
            get => (_internal as Data.Models.Metadata.Rom)?.LastModifiedTime;
            set => (_internal as Data.Models.Metadata.Rom)?.LastModifiedTime = value;
        }

        /// <inheritdoc>/>
        public override ItemType ItemType => ItemType.Rom;

        public string? Length
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Length;
            set => (_internal as Data.Models.Metadata.Rom)?.Length = value;
        }

        public LoadFlag? LoadFlag
        {
            get => (_internal as Data.Models.Metadata.Rom)?.LoadFlag;
            set => (_internal as Data.Models.Metadata.Rom)?.LoadFlag = value;
        }

        public string? MatrixNumber
        {
            get => (_internal as Data.Models.Metadata.Rom)?.MatrixNumber;
            set => (_internal as Data.Models.Metadata.Rom)?.MatrixNumber = value;
        }

        public string? MD2
        {
            get => (_internal as Data.Models.Metadata.Rom)?.MD2;
            set => (_internal as Data.Models.Metadata.Rom)?.MD2 = value;
        }

        public string? MD4
        {
            get => (_internal as Data.Models.Metadata.Rom)?.MD4;
            set => (_internal as Data.Models.Metadata.Rom)?.MD4 = value;
        }

        public string? MD5
        {
            get => (_internal as Data.Models.Metadata.Rom)?.MD5;
            set => (_internal as Data.Models.Metadata.Rom)?.MD5 = value;
        }

        public string? Merge
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Merge;
            set => (_internal as Data.Models.Metadata.Rom)?.Merge = value;
        }

        public bool? MIA
        {
            get => (_internal as Data.Models.Metadata.Rom)?.MIA;
            set => (_internal as Data.Models.Metadata.Rom)?.MIA = value;
        }

        public string? Name
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Name;
            set => (_internal as Data.Models.Metadata.Rom)?.Name = value;
        }

        public string? Offset
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Offset;
            set => (_internal as Data.Models.Metadata.Rom)?.Offset = value;
        }

        public string? OpenMSXType
        {
            get => (_internal as Data.Models.Metadata.Rom)?.OpenMSXType;
            set => (_internal as Data.Models.Metadata.Rom)?.OpenMSXType = value;
        }

        public OpenMSXSubType? OpenMSXMediaType
        {
            get => (_internal as Data.Models.Metadata.Rom)?.OpenMSXMediaType;
            set => (_internal as Data.Models.Metadata.Rom)?.OpenMSXMediaType = value;
        }

        public Original? Original { get; set; }

        public string? OriginalProperty
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Original;
            set => (_internal as Data.Models.Metadata.Rom)?.Original = value;
        }

        [JsonIgnore]
        public bool OriginalSpecified => Original is not null;

        public bool? Optional
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Optional;
            set => (_internal as Data.Models.Metadata.Rom)?.Optional = value;
        }

        public Part? Part { get; set; }

        [JsonIgnore]
        public bool PartSpecified
        {
            get
            {
                return Part is not null
                    && (!string.IsNullOrEmpty(Part.Name)
                        || !string.IsNullOrEmpty(Part.Interface));
            }
        }

        public string? PDFModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.PDFModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.PDFModuleVersion = value;
        }

        public string? PreviewImage
        {
            get => (_internal as Data.Models.Metadata.Rom)?.PreviewImage;
            set => (_internal as Data.Models.Metadata.Rom)?.PreviewImage = value;
        }

        public string? Publisher
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Publisher;
            set => (_internal as Data.Models.Metadata.Rom)?.Publisher = value;
        }

        public string? Region
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Region;
            set => (_internal as Data.Models.Metadata.Rom)?.Region = value;
        }

        public string? Remark
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Remark;
            set => (_internal as Data.Models.Metadata.Rom)?.Remark = value;
        }

        public string? RIPEMD128
        {
            get => (_internal as Data.Models.Metadata.Rom)?.RIPEMD128;
            set => (_internal as Data.Models.Metadata.Rom)?.RIPEMD128 = value;
        }

        public string? RIPEMD160
        {
            get => (_internal as Data.Models.Metadata.Rom)?.RIPEMD160;
            set => (_internal as Data.Models.Metadata.Rom)?.RIPEMD160 = value;
        }

        public string? Rotation
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Rotation;
            set => (_internal as Data.Models.Metadata.Rom)?.Rotation = value;
        }

        public string? Serial
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Serial;
            set => (_internal as Data.Models.Metadata.Rom)?.Serial = value;
        }

        public string? SHA1
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SHA1;
            set => (_internal as Data.Models.Metadata.Rom)?.SHA1 = value;
        }

        public string? SHA256
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SHA256;
            set => (_internal as Data.Models.Metadata.Rom)?.SHA256 = value;
        }

        public string? SHA384
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SHA384;
            set => (_internal as Data.Models.Metadata.Rom)?.SHA384 = value;
        }

        public string? SHA512
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SHA512;
            set => (_internal as Data.Models.Metadata.Rom)?.SHA512 = value;
        }

        public long? Size
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Size;
            set => (_internal as Data.Models.Metadata.Rom)?.Size = value;
        }

        public bool? SoundOnly
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SoundOnly;
            set => (_internal as Data.Models.Metadata.Rom)?.SoundOnly = value;
        }

        public string? SourceProperty
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Source;
            set => (_internal as Data.Models.Metadata.Rom)?.Source = value;
        }

        public string? SpamSum
        {
            get => (_internal as Data.Models.Metadata.Rom)?.SpamSum;
            set => (_internal as Data.Models.Metadata.Rom)?.SpamSum = value;
        }

        public string? Start
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Start;
            set => (_internal as Data.Models.Metadata.Rom)?.Start = value;
        }

        public ItemStatus? Status
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Status;
            set => (_internal as Data.Models.Metadata.Rom)?.Status = value;
        }

        public string? Summation
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Summation;
            set => (_internal as Data.Models.Metadata.Rom)?.Summation = value;
        }

        public string? TesseractOCR
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCR;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCR = value;
        }

        public string? TesseractOCRConverted
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRConverted;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRConverted = value;
        }

        public string? TesseractOCRDetectedLang
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedLang;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedLang = value;
        }

        public string? TesseractOCRDetectedLangConf
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedLangConf;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedLangConf = value;
        }

        public string? TesseractOCRDetectedScript
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedScript;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedScript = value;
        }

        public string? TesseractOCRDetectedScriptConf
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedScriptConf;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRDetectedScriptConf = value;
        }

        public string? TesseractOCRModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRModuleVersion = value;
        }

        public string? TesseractOCRParameters
        {
            get => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRParameters;
            set => (_internal as Data.Models.Metadata.Rom)?.TesseractOCRParameters = value;
        }

        public string? Title
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Title;
            set => (_internal as Data.Models.Metadata.Rom)?.Title = value;
        }

        public string? Track
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Track;
            set => (_internal as Data.Models.Metadata.Rom)?.Track = value;
        }

        public string? Value
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Value;
            set => (_internal as Data.Models.Metadata.Rom)?.Value = value;
        }

        public string? WhisperASRModuleVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WhisperASRModuleVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.WhisperASRModuleVersion = value;
        }

        public string? WhisperModelHash
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WhisperModelHash;
            set => (_internal as Data.Models.Metadata.Rom)?.WhisperModelHash = value;
        }

        public string? WhisperModelName
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WhisperModelName;
            set => (_internal as Data.Models.Metadata.Rom)?.WhisperModelName = value;
        }

        public string? WhisperVersion
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WhisperVersion;
            set => (_internal as Data.Models.Metadata.Rom)?.WhisperVersion = value;
        }

        public string? Width
        {
            get => (_internal as Data.Models.Metadata.Rom)?.Width;
            set => (_internal as Data.Models.Metadata.Rom)?.Width = value;
        }

        public string? WordConfidenceInterval0To10
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval0To10;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval0To10 = value;
        }

        public string? WordConfidenceInterval11To20
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval11To20;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval11To20 = value;
        }

        public string? WordConfidenceInterval21To30
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval21To30;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval21To30 = value;
        }

        public string? WordConfidenceInterval31To40
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval31To40;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval31To40 = value;
        }

        public string? WordConfidenceInterval41To50
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval41To50;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval41To50 = value;
        }

        public string? WordConfidenceInterval51To60
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval51To60;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval51To60 = value;
        }

        public string? WordConfidenceInterval61To70
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval61To70;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval61To70 = value;
        }

        public string? WordConfidenceInterval71To80
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval71To80;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval71To80 = value;
        }

        public string? WordConfidenceInterval81To90
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval81To90;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval81To90 = value;
        }

        public string? WordConfidenceInterval91To100
        {
            get => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval91To100;
            set => (_internal as Data.Models.Metadata.Rom)?.WordConfidenceInterval91To100 = value;
        }

        public string? xxHash364
        {
            get => (_internal as Data.Models.Metadata.Rom)?.xxHash364;
            set => (_internal as Data.Models.Metadata.Rom)?.xxHash364 = value;
        }

        public string? xxHash3128
        {
            get => (_internal as Data.Models.Metadata.Rom)?.xxHash3128;
            set => (_internal as Data.Models.Metadata.Rom)?.xxHash3128 = value;
        }

        #endregion

        #region Constructors

        public Rom() : base()
        {
            DupeType = 0x00;
            Status = ItemStatus.None;
        }

        public Rom(Dump item, Machine machine, Source source, int index)
        {
            // If we don't have rom data, we can't do anything
            Data.Models.Metadata.Rom? rom = null;
            OpenMSXSubType? subType = null;

            if (item.Rom is not null)
            {
                rom = item.Rom;
                subType = OpenMSXSubType.Rom;
            }
            else if (item.MegaRom is not null)
            {
                rom = item.MegaRom;
                subType = OpenMSXSubType.MegaRom;
            }
            else if (item.SCCPlusCart is not null)
            {
                rom = item.SCCPlusCart;
                subType = OpenMSXSubType.SCCPlusCart;
            }

            // Just return if nothing valid was found
            if (rom is null)
                return;

            string name = $"{machine.Name}_{index++}{(!string.IsNullOrEmpty(rom!.Remark) ? $" {rom.Remark}" : string.Empty)}";

            Name = name;
            Offset = rom.Start;
            OpenMSXMediaType = subType;
            OpenMSXType = rom.ItemType.ToString();
            Remark = rom.Remark;
            SHA1 = rom.SHA1;
            Start = rom.Start;
            Source = source;

            var original = item.Original;
            if (original is not null)
            {
                Original = new Original
                {
                    Value = original.Value,
                    Content = original.Content,
                };
            }

            CopyMachineInformation(machine);

            // Process hash values
            // TODO: This should be normalized to CRC-16
            string? crc16 = CRC16;
            if (crc16 is not null)
                CRC16 = NormalizeHashData(crc16, 4);

            string? crc = CRC;
            if (crc is not null)
                CRC = TextHelper.NormalizeCRC32(crc);

            // TODO: This should be normalized to CRC-64
            string? crc64 = CRC64;
            if (crc64 is not null)
                CRC64 = NormalizeHashData(crc64, 16);

            string? md2 = MD2;
            if (md2 is not null)
                MD2 = TextHelper.NormalizeMD2(md2);

            string? md4 = MD4;
            if (md4 is not null)
                MD4 = TextHelper.NormalizeMD5(md4);

            string? md5 = MD5;
            if (md5 is not null)
                MD5 = TextHelper.NormalizeMD5(md5);

            string? ripemd128 = RIPEMD128;
            if (ripemd128 is not null)
                RIPEMD128 = TextHelper.NormalizeRIPEMD128(ripemd128);

            string? ripemd160 = RIPEMD160;
            if (ripemd160 is not null)
                RIPEMD160 = TextHelper.NormalizeRIPEMD160(ripemd160);

            string? sha1 = SHA1;
            if (sha1 is not null)
                SHA1 = TextHelper.NormalizeSHA1(sha1);

            string? sha256 = SHA256;
            if (sha256 is not null)
                SHA256 = TextHelper.NormalizeSHA256(sha256);

            string? sha384 = SHA384;
            if (sha384 is not null)
                SHA384 = TextHelper.NormalizeSHA384(sha384);

            string? sha512 = SHA512;
            if (sha512 is not null)
                SHA512 = TextHelper.NormalizeSHA512(sha512);
        }

        public Rom(Data.Models.Metadata.Rom item) : base(item)
        {
            DupeType = 0x00;

            // Process hash values
            // TODO: This should be normalized to CRC-16
            string? crc16 = CRC16;
            if (crc16 is not null)
                CRC16 = NormalizeHashData(crc16, 4);

            string? crc = CRC;
            if (crc is not null)
                CRC = TextHelper.NormalizeCRC32(crc);

            // TODO: This should be normalized to CRC-64
            string? crc64 = CRC64;
            if (crc64 is not null)
                CRC64 = NormalizeHashData(crc64, 16);

            string? md2 = MD2;
            if (md2 is not null)
                MD2 = TextHelper.NormalizeMD2(md2);

            string? md4 = MD4;
            if (md4 is not null)
                MD4 = TextHelper.NormalizeMD5(md4);

            string? md5 = MD5;
            if (md5 is not null)
                MD5 = TextHelper.NormalizeMD5(md5);

            string? ripemd128 = RIPEMD128;
            if (ripemd128 is not null)
                RIPEMD128 = TextHelper.NormalizeRIPEMD128(ripemd128);

            string? ripemd160 = RIPEMD160;
            if (ripemd160 is not null)
                RIPEMD160 = TextHelper.NormalizeRIPEMD160(ripemd160);

            string? sha1 = SHA1;
            if (sha1 is not null)
                SHA1 = TextHelper.NormalizeSHA1(sha1);

            string? sha256 = SHA256;
            if (sha256 is not null)
                SHA256 = TextHelper.NormalizeSHA256(sha256);

            string? sha384 = SHA384;
            if (sha384 is not null)
                SHA384 = TextHelper.NormalizeSHA384(sha384);

            string? sha512 = SHA512;
            if (sha512 is not null)
                SHA512 = TextHelper.NormalizeSHA512(sha512);
        }

        public Rom(Data.Models.Metadata.Rom item, Machine machine, Source source) : this(item)
        {
            Source = source;
            CopyMachineInformation(machine);
        }

        /// <summary>
        /// Normalize a hash string and pad to the correct size
        /// </summary>
        /// TODO: Remove when IO is updated
        private static string? NormalizeHashData(string? hash, int expectedLength)
        {
            // If we have a known blank hash, return blank
            if (hash is null)
                return null;
            else if (hash == string.Empty || hash == "-" || hash == "_")
                return string.Empty;

            // Check to see if it's a "hex" hash
            hash = hash!.Trim().Replace("0x", string.Empty);

            // If we have a blank hash now, return blank
            if (string.IsNullOrEmpty(hash))
                return string.Empty;

            // If the hash shorter than the required length, pad it
            if (hash.Length < expectedLength)
                hash = hash.PadLeft(expectedLength, '0');

            // If the hash is longer than the required length, it's invalid
            else if (hash.Length > expectedLength)
                return string.Empty;

            // Now normalize the hash
            hash = hash.ToLowerInvariant();

            // Otherwise, make sure that every character is a proper match
            for (int i = 0; i < hash.Length; i++)
            {
                char c = hash[i];
#if NET7_0_OR_GREATER
                if (!char.IsAsciiHexDigit(c))
#else
                if (!IsAsciiHexDigit(c))
#endif
                {
                    hash = string.Empty;
                    break;
                }
            }

            return hash;
        }

#if NETFRAMEWORK || NETCOREAPP3_1 || NET5_0 || NET6_0 || NETSTANDARD2_0_OR_GREATER
        /// <summary>
        /// Indicates whether a character is categorized as an ASCII hexademical digit.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>true if c is a hexademical digit; otherwise, false.</returns>
        /// <remarks>This method determines whether the character is in the range '0' through '9', inclusive, 'A' through 'F', inclusive, or 'a' through 'f', inclusive.</remarks>
        /// TODO: Remove when IO is updated
        internal static bool IsAsciiHexDigit(char c)
        {
            return char.ToLowerInvariant(c) switch
            {
                '0' => true,
                '1' => true,
                '2' => true,
                '3' => true,
                '4' => true,
                '5' => true,
                '6' => true,
                '7' => true,
                '8' => true,
                '9' => true,
                'a' => true,
                'b' => true,
                'c' => true,
                'd' => true,
                'e' => true,
                'f' => true,
                _ => false,
            };
        }
#endif

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone() => new Rom(GetInternalClone());

        /// <inheritdoc/>
        public override Data.Models.Metadata.Rom GetInternalClone()
            => (_internal as Data.Models.Metadata.Rom)?.Clone() as Data.Models.Metadata.Rom ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Rom otherRom)
                return ((Data.Models.Metadata.Rom)_internal).Equals((Data.Models.Metadata.Rom)otherRom._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(ModelBackedItem<Data.Models.Metadata.DatItem>? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Rom otherRom)
                return ((Data.Models.Metadata.Rom)_internal).Equals((Data.Models.Metadata.Rom)otherRom._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Rom otherRom)
                return ((Data.Models.Metadata.Rom)_internal).PartialEquals((Data.Models.Metadata.Rom)otherRom._internal);

            // Everything else fails
            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(DatItem<Data.Models.Metadata.Rom>? other)
        {
            // If the other value is invalid
            if (other is null)
                return false;

            // If the type matches
            if (other is Rom otherRom)
                return ((Data.Models.Metadata.Rom)_internal).PartialEquals((Data.Models.Metadata.Rom)otherRom._internal);

            // Everything else fails
            return false;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other)
            => (_internal as Data.Models.Metadata.Rom).FillMissingHashes(other._internal as Data.Models.Metadata.Rom);

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes() => (_internal as Data.Models.Metadata.Rom)?.HasHashes() ?? false;

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash() => (_internal as Data.Models.Metadata.Rom)?.HasZeroHash() ?? false;

        #endregion

        #region Sorting and Merging

        /// <inheritdoc/>
        public override string GetKey(ItemKey bucketedBy, Machine? machine, Source? source, bool lower = true, bool norename = true)
        {
            // Set the output key as the default blank string
            string? key;

#pragma warning disable IDE0010
            // Now determine what the key should be based on the bucketedBy value
            switch (bucketedBy)
            {
                case ItemKey.CRC16:
                    key = CRC16;
                    break;

                case ItemKey.CRC:
                    key = CRC;
                    break;

                case ItemKey.CRC64:
                    key = CRC64;
                    break;

                case ItemKey.MD2:
                    key = MD2;
                    break;

                case ItemKey.MD4:
                    key = MD4;
                    break;

                case ItemKey.MD5:
                    key = MD5;
                    break;

                case ItemKey.RIPEMD128:
                    key = RIPEMD128;
                    break;

                case ItemKey.RIPEMD160:
                    key = RIPEMD160;
                    break;

                case ItemKey.SHA1:
                    key = SHA1;
                    break;

                case ItemKey.SHA256:
                    key = SHA256;
                    break;

                case ItemKey.SHA384:
                    key = SHA384;
                    break;

                case ItemKey.SHA512:
                    key = SHA512;
                    break;

                case ItemKey.SpamSum:
                    key = SpamSum;
                    break;

                // Let the base handle generic stuff
                default:
                    return base.GetKey(bucketedBy, machine, source, lower, norename);
            }
#pragma warning restore IDE0010

            // Double and triple check the key for corner cases
            key ??= string.Empty;
            if (lower)
                key = key.ToLowerInvariant();

            return key;
        }

        #endregion
    }
}
