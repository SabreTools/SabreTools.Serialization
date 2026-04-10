using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.Metadata;
using SabreTools.Metadata.Filter;
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
            get => _internal.Album;
            set => _internal.Album = value;
        }

        public string? AltRomname
        {
            get => _internal.AltRomname;
            set => _internal.AltRomname = value;
        }

        public string? AltTitle
        {
            get => _internal.AltTitle;
            set => _internal.AltTitle = value;
        }

        public string? Artist
        {
            get => _internal.Artist;
            set => _internal.Artist = value;
        }

        public string? ASRDetectedLang
        {
            get => _internal.ASRDetectedLang;
            set => _internal.ASRDetectedLang = value;
        }

        public string? ASRDetectedLangConf
        {
            get => _internal.ASRDetectedLangConf;
            set => _internal.ASRDetectedLangConf = value;
        }

        public string? ASRTranscribedLang
        {
            get => _internal.ASRTranscribedLang;
            set => _internal.ASRTranscribedLang = value;
        }

        public string? Bios
        {
            get => _internal.Bios;
            set => _internal.Bios = value;
        }

        public string? Bitrate
        {
            get => _internal.Bitrate;
            set => _internal.Bitrate = value;
        }

        public string? BitTorrentMagnetHash
        {
            get => _internal.BitTorrentMagnetHash;
            set => _internal.BitTorrentMagnetHash = value;
        }

        public string? ClothCoverDetectionModuleVersion
        {
            get => _internal.ClothCoverDetectionModuleVersion;
            set => _internal.ClothCoverDetectionModuleVersion = value;
        }

        public string? CollectionCatalogNumber
        {
            get => _internal.CollectionCatalogNumber;
            set => _internal.CollectionCatalogNumber = value;
        }

        public string? Comment
        {
            get => _internal.Comment;
            set => _internal.Comment = value;
        }

        public string? CRC16
        {
            get => _internal.CRC16;
            set => _internal.CRC16 = value;
        }

        public string? CRC32
        {
            get => _internal.CRC32;
            set => _internal.CRC32 = value;
        }

        public string? CRC64
        {
            get => _internal.CRC64;
            set => _internal.CRC64 = value;
        }

        public string? Creator
        {
            get => _internal.Creator;
            set => _internal.Creator = value;
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
            get => _internal.Date;
            set => _internal.Date = value;
        }

        public bool? Dispose
        {
            get => _internal.Dispose;
            set => _internal.Dispose = value;
        }

        public string? Extension
        {
            get => _internal.Extension;
            set => _internal.Extension = value;
        }

        public long? FileCount
        {
            get => _internal.FileCount;
            set => _internal.FileCount = value;
        }

        public bool? FileIsAvailable
        {
            get => _internal.FileIsAvailable;
            set => _internal.FileIsAvailable = value;
        }

        public string? Flags
        {
            get => _internal.Flags;
            set => _internal.Flags = value;
        }

        public string? Format
        {
            get => _internal.Format;
            set => _internal.Format = value;
        }

        public string? Header
        {
            get => _internal.Header;
            set => _internal.Header = value;
        }

        public string? Height
        {
            get => _internal.Height;
            set => _internal.Height = value;
        }

        public string? hOCRCharToWordhOCRVersion
        {
            get => _internal.hOCRCharToWordhOCRVersion;
            set => _internal.hOCRCharToWordhOCRVersion = value;
        }

        public string? hOCRCharToWordModuleVersion
        {
            get => _internal.hOCRCharToWordModuleVersion;
            set => _internal.hOCRCharToWordModuleVersion = value;
        }

        public string? hOCRFtsTexthOCRVersion
        {
            get => _internal.hOCRFtsTexthOCRVersion;
            set => _internal.hOCRFtsTexthOCRVersion = value;
        }

        public string? hOCRFtsTextModuleVersion
        {
            get => _internal.hOCRFtsTextModuleVersion;
            set => _internal.hOCRFtsTextModuleVersion = value;
        }

        public string? hOCRPageIndexhOCRVersion
        {
            get => _internal.hOCRPageIndexhOCRVersion;
            set => _internal.hOCRPageIndexhOCRVersion = value;
        }

        public string? hOCRPageIndexModuleVersion
        {
            get => _internal.hOCRPageIndexModuleVersion;
            set => _internal.hOCRPageIndexModuleVersion = value;
        }

        public bool? Inverted
        {
            get => _internal.Inverted;
            set => _internal.Inverted = value;
        }

        public string? LastModifiedTime
        {
            get => _internal.LastModifiedTime;
            set => _internal.LastModifiedTime = value;
        }

        /// <inheritdoc>/>
        public override ItemType ItemType => ItemType.Rom;

        public string? Length
        {
            get => _internal.Length;
            set => _internal.Length = value;
        }

        public LoadFlag? LoadFlag
        {
            get => _internal.LoadFlag;
            set => _internal.LoadFlag = value;
        }

        public string? MatrixNumber
        {
            get => _internal.MatrixNumber;
            set => _internal.MatrixNumber = value;
        }

        public string? MD2
        {
            get => _internal.MD2;
            set => _internal.MD2 = value;
        }

        public string? MD4
        {
            get => _internal.MD4;
            set => _internal.MD4 = value;
        }

        public string? MD5
        {
            get => _internal.MD5;
            set => _internal.MD5 = value;
        }

        public string? Merge
        {
            get => _internal.Merge;
            set => _internal.Merge = value;
        }

        public bool? MIA
        {
            get => _internal.MIA;
            set => _internal.MIA = value;
        }

        public string? Name
        {
            get => _internal.Name;
            set => _internal.Name = value;
        }

        public string? Offset
        {
            get => _internal.Offset;
            set => _internal.Offset = value;
        }

        public string? OpenMSXType
        {
            get => _internal.OpenMSXType;
            set => _internal.OpenMSXType = value;
        }

        public OpenMSXSubType? OpenMSXMediaType
        {
            get => _internal.OpenMSXMediaType;
            set => _internal.OpenMSXMediaType = value;
        }

        public Original? Original { get; set; }

        public string? OriginalProperty
        {
            get => _internal.Original;
            set => _internal.Original = value;
        }

        [JsonIgnore]
        public bool OriginalSpecified => Original is not null;

        public bool? Optional
        {
            get => _internal.Optional;
            set => _internal.Optional = value;
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
            get => _internal.PDFModuleVersion;
            set => _internal.PDFModuleVersion = value;
        }

        public string? PreviewImage
        {
            get => _internal.PreviewImage;
            set => _internal.PreviewImage = value;
        }

        public string? Publisher
        {
            get => _internal.Publisher;
            set => _internal.Publisher = value;
        }

        public string? Region
        {
            get => _internal.Region;
            set => _internal.Region = value;
        }

        public string? Remark
        {
            get => _internal.Remark;
            set => _internal.Remark = value;
        }

        public string? RIPEMD128
        {
            get => _internal.RIPEMD128;
            set => _internal.RIPEMD128 = value;
        }

        public string? RIPEMD160
        {
            get => _internal.RIPEMD160;
            set => _internal.RIPEMD160 = value;
        }

        public string? Rotation
        {
            get => _internal.Rotation;
            set => _internal.Rotation = value;
        }

        public string? Serial
        {
            get => _internal.Serial;
            set => _internal.Serial = value;
        }

        public string? SHA1
        {
            get => _internal.SHA1;
            set => _internal.SHA1 = value;
        }

        public string? SHA256
        {
            get => _internal.SHA256;
            set => _internal.SHA256 = value;
        }

        public string? SHA384
        {
            get => _internal.SHA384;
            set => _internal.SHA384 = value;
        }

        public string? SHA512
        {
            get => _internal.SHA512;
            set => _internal.SHA512 = value;
        }

        public long? Size
        {
            get => _internal.Size;
            set => _internal.Size = value;
        }

        public bool? SoundOnly
        {
            get => _internal.SoundOnly;
            set => _internal.SoundOnly = value;
        }

        public string? SourceProperty
        {
            get => _internal.Source;
            set => _internal.Source = value;
        }

        public string? SpamSum
        {
            get => _internal.SpamSum;
            set => _internal.SpamSum = value;
        }

        public string? Start
        {
            get => _internal.Start;
            set => _internal.Start = value;
        }

        public ItemStatus? Status
        {
            get => _internal.Status;
            set => _internal.Status = value;
        }

        public string? Summation
        {
            get => _internal.Summation;
            set => _internal.Summation = value;
        }

        public string? TesseractOCR
        {
            get => _internal.TesseractOCR;
            set => _internal.TesseractOCR = value;
        }

        public string? TesseractOCRConverted
        {
            get => _internal.TesseractOCRConverted;
            set => _internal.TesseractOCRConverted = value;
        }

        public string? TesseractOCRDetectedLang
        {
            get => _internal.TesseractOCRDetectedLang;
            set => _internal.TesseractOCRDetectedLang = value;
        }

        public string? TesseractOCRDetectedLangConf
        {
            get => _internal.TesseractOCRDetectedLangConf;
            set => _internal.TesseractOCRDetectedLangConf = value;
        }

        public string? TesseractOCRDetectedScript
        {
            get => _internal.TesseractOCRDetectedScript;
            set => _internal.TesseractOCRDetectedScript = value;
        }

        public string? TesseractOCRDetectedScriptConf
        {
            get => _internal.TesseractOCRDetectedScriptConf;
            set => _internal.TesseractOCRDetectedScriptConf = value;
        }

        public string? TesseractOCRModuleVersion
        {
            get => _internal.TesseractOCRModuleVersion;
            set => _internal.TesseractOCRModuleVersion = value;
        }

        public string? TesseractOCRParameters
        {
            get => _internal.TesseractOCRParameters;
            set => _internal.TesseractOCRParameters = value;
        }

        public string? Title
        {
            get => _internal.Title;
            set => _internal.Title = value;
        }

        public string? Track
        {
            get => _internal.Track;
            set => _internal.Track = value;
        }

        public string? Value
        {
            get => _internal.Value;
            set => _internal.Value = value;
        }

        public string? WhisperASRModuleVersion
        {
            get => _internal.WhisperASRModuleVersion;
            set => _internal.WhisperASRModuleVersion = value;
        }

        public string? WhisperModelHash
        {
            get => _internal.WhisperModelHash;
            set => _internal.WhisperModelHash = value;
        }

        public string? WhisperModelName
        {
            get => _internal.WhisperModelName;
            set => _internal.WhisperModelName = value;
        }

        public string? WhisperVersion
        {
            get => _internal.WhisperVersion;
            set => _internal.WhisperVersion = value;
        }

        public string? Width
        {
            get => _internal.Width;
            set => _internal.Width = value;
        }

        public string? WordConfidenceInterval0To10
        {
            get => _internal.WordConfidenceInterval0To10;
            set => _internal.WordConfidenceInterval0To10 = value;
        }

        public string? WordConfidenceInterval11To20
        {
            get => _internal.WordConfidenceInterval11To20;
            set => _internal.WordConfidenceInterval11To20 = value;
        }

        public string? WordConfidenceInterval21To30
        {
            get => _internal.WordConfidenceInterval21To30;
            set => _internal.WordConfidenceInterval21To30 = value;
        }

        public string? WordConfidenceInterval31To40
        {
            get => _internal.WordConfidenceInterval31To40;
            set => _internal.WordConfidenceInterval31To40 = value;
        }

        public string? WordConfidenceInterval41To50
        {
            get => _internal.WordConfidenceInterval41To50;
            set => _internal.WordConfidenceInterval41To50 = value;
        }

        public string? WordConfidenceInterval51To60
        {
            get => _internal.WordConfidenceInterval51To60;
            set => _internal.WordConfidenceInterval51To60 = value;
        }

        public string? WordConfidenceInterval61To70
        {
            get => _internal.WordConfidenceInterval61To70;
            set => _internal.WordConfidenceInterval61To70 = value;
        }

        public string? WordConfidenceInterval71To80
        {
            get => _internal.WordConfidenceInterval71To80;
            set => _internal.WordConfidenceInterval71To80 = value;
        }

        public string? WordConfidenceInterval81To90
        {
            get => _internal.WordConfidenceInterval81To90;
            set => _internal.WordConfidenceInterval81To90 = value;
        }

        public string? WordConfidenceInterval91To100
        {
            get => _internal.WordConfidenceInterval91To100;
            set => _internal.WordConfidenceInterval91To100 = value;
        }

        public string? xxHash364
        {
            get => _internal.xxHash364;
            set => _internal.xxHash364 = value;
        }

        public string? xxHash3128
        {
            get => _internal.xxHash3128;
            set => _internal.xxHash3128 = value;
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

            string? crc32 = CRC32;
            if (crc32 is not null)
                CRC32 = TextHelper.NormalizeCRC32(crc32);

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

            string? crc32 = CRC32;
            if (crc32 is not null)
                CRC32 = TextHelper.NormalizeCRC32(crc32);

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
            => _internal.Clone() as Data.Models.Metadata.Rom ?? new();

        #endregion

        #region Comparision Methods

        /// <inheritdoc/>
        public override bool Equals(DatItem? other)
        {
            // If the other item is null
            if (other is null)
                return false;

            // If the type matches
            if (other is Rom otherRom)
                return _internal.PartialEquals(otherRom._internal);

            // Everything else fails
            return false;
        }

        /// <summary>
        /// Fill any missing size and hash information from another Rom
        /// </summary>
        /// <param name="other">Rom to fill information from</param>
        public void FillMissingInformation(Rom other)
            => _internal.FillMissingHashes(other._internal);

        /// <summary>
        /// Returns if the Rom contains any hashes
        /// </summary>
        /// <returns>True if any hash exists, false otherwise</returns>
        public bool HasHashes() => _internal.HasHashes();

        /// <summary>
        /// Returns if all of the hashes are set to their 0-byte values
        /// </summary>
        /// <returns>True if any hash matches the 0-byte value, false otherwise</returns>
        public bool HasZeroHash() => _internal.HasZeroHash();

        #endregion

        #region Manipulation

        /// <inheritdoc/>
        public override bool PassesFilter(FilterRunner filterRunner)
        {
            if (Machine is not null && !Machine.PassesFilter(filterRunner))
                return false;

            // TODO: DataArea
            // TODO: Part

            return filterRunner.Run(_internal);
        }

        /// <inheritdoc/>
        public override bool PassesFilterDB(FilterRunner filterRunner)
        {
            // TODO: DataArea
            // TODO: Part

            return filterRunner.Run(_internal);
        }

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

                case ItemKey.CRC32:
                    key = CRC32;
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
