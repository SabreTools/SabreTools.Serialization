using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    // TODO: IEquatable<Rom>
    [JsonObject("rom"), XmlRoot("rom")]
    public class Rom : DatItem, ICloneable
    {
        #region Properties

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Dispose { get; set; }

        /// <remarks>ArchiveDotOrg.File</remarks>
        public long? FileCount { get; set; }

        /// <remarks>bool; AttractMode.Row</remarks>
        public bool? FileIsAvailable { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Inverted { get; set; }

        /// <remarks>(load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore)</remarks>
        public LoadFlag? LoadFlag { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? MIA { get; set; }

        public string? Name { get; set; }

        /// <remarks>string; OpenMSX.RomBase</remarks>
        public OpenMSXSubType? OpenMSXMediaType { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? Optional { get; set; }

        public long? Size { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        public bool? SoundOnly { get; set; }

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        public ItemStatus? Status { get; set; }

        public string? Value { get; set; }

        #endregion

        #region Keys

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string AlbumKey = "album";

        /// <remarks>string; AttractMode.Row</remarks>
        public const string AltRomnameKey = "alt_romname";

        /// <remarks>string; AttractMode.Row</remarks>
        public const string AltTitleKey = "alt_title";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string ArtistKey = "artist";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string ASRDetectedLangKey = "asr_detected_lang";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string ASRDetectedLangConfKey = "asr_detected_lang_conf";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string ASRTranscribedLangKey = "asr_transcribed_lang";

        /// <remarks>string</remarks>
        public const string BiosKey = "bios";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string BitrateKey = "bitrate";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string BitTorrentMagnetHashKey = "btih";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string ClothCoverDetectionModuleVersionKey = "cloth_cover_detection_module_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string CollectionCatalogNumberKey = "collection-catalog-number";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string CommentKey = "comment";

        /// <remarks>string; Also "crc32" in ArchiveDotOrg.File</remarks>
        public const string CRCKey = "crc";

        /// <remarks>string</remarks>
        public const string CRC16Key = "crc16";

        /// <remarks>string</remarks>
        public const string CRC64Key = "crc64";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string CreatorKey = "creator";

        /// <remarks>string</remarks>
        public const string DateKey = "date";

        /// <remarks>string; OfflineList.FileRomCRC</remarks>
        public const string ExtensionKey = "extension";

        /// <remarks>string</remarks>
        public const string FlagsKey = "flags";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string FormatKey = "format";

        /// <remarks>string</remarks>
        public const string HeaderKey = "header";

        /// <remarks>string, possibly long; ArchiveDotOrg.File</remarks>
        public const string HeightKey = "height";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string hOCRCharToWordhOCRVersionKey = "hocr_char_to_word_hocr_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string hOCRCharToWordModuleVersionKey = "hocr_char_to_word_module_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string hOCRFtsTexthOCRVersionKey = "hocr_fts_text_hocr_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string hOCRFtsTextModuleVersionKey = "hocr_fts_text_module_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string hOCRPageIndexhOCRVersionKey = "hocr_pageindex_hocr_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string hOCRPageIndexModuleVersionKey = "hocr_pageindex_module_version";

        /// <remarks>long; ArchiveDotOrg.File</remarks>
        public const string LastModifiedTimeKey = "mtime";

        /// <remarks>string, possibly long; Also in ArchiveDotOrg.File</remarks>
        public const string LengthKey = "length";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string MatrixNumberKey = "matrix_number";

        /// <remarks>string</remarks>
        public const string MD2Key = "md2";

        /// <remarks>string</remarks>
        public const string MD4Key = "md4";

        /// <remarks>string</remarks>
        public const string MD5Key = "md5";

        /// <remarks>string</remarks>
        public const string MergeKey = "merge";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRKey = "ocr";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRConvertedKey = "ocr_converted";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRDetectedLangKey = "ocr_detected_lang";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRDetectedLangConfKey = "ocr_detected_lang_conf";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRDetectedScriptKey = "ocr_detected_script";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRDetectedScriptConfKey = "ocr_detected_script_conf";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRModuleVersionKey = "ocr_module_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TesseractOCRParametersKey = "ocr_parameters";

        /// <remarks>string, possibly long; Originally "offs"</remarks>
        public const string OffsetKey = "offset";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string OriginalKey = "original";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string PDFModuleVersionKey = "pdf_module_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string PreviewImageKey = "preview-image";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string PublisherKey = "publisher";

        /// <remarks>string</remarks>
        public const string RegionKey = "region";

        /// <remarks>string; OpenMSX.RomBase</remarks>
        public const string RemarkKey = "remark";

        /// <remarks>string</remarks>
        public const string RIPEMD128Key = "ripemd128";

        /// <remarks>string</remarks>
        public const string RIPEMD160Key = "ripemd160";

        /// <remarks>string, possibly long; ArchiveDotOrg.File</remarks>
        public const string RotationKey = "rotation";

        /// <remarks>string</remarks>
        public const string SerialKey = "serial";

        /// <remarks>string</remarks>
        public const string SHA1Key = "sha1";

        /// <remarks>string</remarks>
        public const string SHA256Key = "sha256";

        /// <remarks>string</remarks>
        public const string SHA384Key = "sha384";

        /// <remarks>string</remarks>
        public const string SHA512Key = "sha512";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string SourceKey = "source";

        /// <remarks>string</remarks>
        public const string SpamSumKey = "spamsum";

        /// <remarks>string, possibly long; OpenMSX.RomBase</remarks>
        public const string StartKey = "start";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string SummationKey = "summation";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TitleKey = "title";

        /// <remarks>string, possibly long; ArchiveDotOrg.File</remarks>
        public const string TrackKey = "track";

        /// <remarks>string; OpenMSX.RomBase</remarks>
        public const string OpenMSXType = "type";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WhisperASRModuleVersionKey = "whisper_asr_module_version";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WhisperModelHashKey = "whisper_model_hash";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WhisperModelNameKey = "whisper_model_name";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WhisperVersionKey = "whisper_version";

        /// <remarks>string, possibly long; ArchiveDotOrg.File</remarks>
        public const string WidthKey = "width";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval0To10Key = "word_conf_0_10";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval11To20Key = "word_conf_11_20";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval21To30Key = "word_conf_21_30";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval31To40Key = "word_conf_31_40";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval41To50Key = "word_conf_41_50";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval51To60Key = "word_conf_51_60";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval61To70Key = "word_conf_61_70";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval71To80Key = "word_conf_71_80";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval81To90Key = "word_conf_81_90";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string WordConfidenceInterval91To100Key = "word_conf_91_100";

        /// <remarks>string</remarks>
        public const string xxHash364Key = "xxh3_64";

        /// <remarks>string</remarks>
        public const string xxHash3128Key = "xxh3_128";

        #endregion

        public Rom() => ItemType = ItemType.Rom;

        /// <inheritdoc/>
        public object Clone()
        {
            var obj = new Rom();

            obj.Dispose = Dispose;
            obj.FileCount = FileCount;
            obj.FileIsAvailable = FileIsAvailable;
            obj.Inverted = Inverted;
            obj.LoadFlag = LoadFlag;
            obj.MIA = MIA;
            obj.Name = Name;
            obj.OpenMSXMediaType = OpenMSXMediaType;
            obj.Optional = Optional;
            obj.Size = Size;
            obj.SoundOnly = SoundOnly;
            obj.Status = Status;
            obj.Value = Value;

            // TODO: Replace these when moving away from keys
            if (TryGetValue(AlbumKey, out var Album))
                obj[AlbumKey] = Album;
            if (TryGetValue(AltRomnameKey, out var AltRomname))
                obj[AltRomnameKey] = AltRomname;
            if (TryGetValue(AltTitleKey, out var AltTitle))
                obj[AltTitleKey] = AltTitle;
            if (TryGetValue(ArtistKey, out var Artist))
                obj[ArtistKey] = Artist;
            if (TryGetValue(ASRDetectedLangKey, out var ASRDetectedLang))
                obj[ASRDetectedLangKey] = ASRDetectedLang;
            if (TryGetValue(ASRDetectedLangConfKey, out var ASRDetectedLangConf))
                obj[ASRDetectedLangConfKey] = ASRDetectedLangConf;
            if (TryGetValue(ASRTranscribedLangKey, out var ASRTranscribedLang))
                obj[ASRTranscribedLangKey] = ASRTranscribedLang;
            if (TryGetValue(BiosKey, out var Bios))
                obj[BiosKey] = Bios;
            if (TryGetValue(BitrateKey, out var Bitrate))
                obj[BitrateKey] = Bitrate;
            if (TryGetValue(BitTorrentMagnetHashKey, out var BitTorrentMagnetHash))
                obj[BitTorrentMagnetHashKey] = BitTorrentMagnetHash;
            if (TryGetValue(ClothCoverDetectionModuleVersionKey, out var ClothCoverDetectionModuleVersion))
                obj[ClothCoverDetectionModuleVersionKey] = ClothCoverDetectionModuleVersion;
            if (TryGetValue(CollectionCatalogNumberKey, out var CollectionCatalogNumber))
                obj[CollectionCatalogNumberKey] = CollectionCatalogNumber;
            if (TryGetValue(CommentKey, out var Comment))
                obj[CommentKey] = Comment;
            if (TryGetValue(CRCKey, out var CRC))
                obj[CRCKey] = CRC;
            if (TryGetValue(CRC16Key, out var CRC16))
                obj[CRC16Key] = CRC16;
            if (TryGetValue(CRC64Key, out var CRC64))
                obj[CRC64Key] = CRC64;
            if (TryGetValue(CreatorKey, out var Creator))
                obj[CreatorKey] = Creator;
            if (TryGetValue(DateKey, out var Date))
                obj[DateKey] = Date;
            if (TryGetValue(ExtensionKey, out var Extension))
                obj[ExtensionKey] = Extension;
            if (TryGetValue(FlagsKey, out var Flags))
                obj[FlagsKey] = Flags;
            if (TryGetValue(FormatKey, out var Format))
                obj[FormatKey] = Format;
            if (TryGetValue(HeaderKey, out var Header))
                obj[HeaderKey] = Header;
            if (TryGetValue(HeightKey, out var Height))
                obj[HeightKey] = Height;
            if (TryGetValue(hOCRCharToWordhOCRVersionKey, out var hOCRCharToWordhOCRVersion))
                obj[hOCRCharToWordhOCRVersionKey] = hOCRCharToWordhOCRVersion;
            if (TryGetValue(hOCRCharToWordModuleVersionKey, out var hOCRCharToWordModuleVersion))
                obj[hOCRCharToWordModuleVersionKey] = hOCRCharToWordModuleVersion;
            if (TryGetValue(hOCRFtsTexthOCRVersionKey, out var hOCRFtsTexthOCRVersion))
                obj[hOCRFtsTexthOCRVersionKey] = hOCRFtsTexthOCRVersion;
            if (TryGetValue(hOCRFtsTextModuleVersionKey, out var hOCRFtsTextModuleVersion))
                obj[hOCRFtsTextModuleVersionKey] = hOCRFtsTextModuleVersion;
            if (TryGetValue(hOCRPageIndexhOCRVersionKey, out var hOCRPageIndexhOCRVersion))
                obj[hOCRPageIndexhOCRVersionKey] = hOCRPageIndexhOCRVersion;
            if (TryGetValue(hOCRPageIndexModuleVersionKey, out var hOCRPageIndexModuleVersion))
                obj[hOCRPageIndexModuleVersionKey] = hOCRPageIndexModuleVersion;
            if (TryGetValue(LastModifiedTimeKey, out var LastModifiedTime))
                obj[LastModifiedTimeKey] = LastModifiedTime;
            if (TryGetValue(LengthKey, out var Length))
                obj[LengthKey] = Length;
            if (TryGetValue(MatrixNumberKey, out var MatrixNumber))
                obj[MatrixNumberKey] = MatrixNumber;
            if (TryGetValue(MD2Key, out var MD2))
                obj[MD2Key] = MD2;
            if (TryGetValue(MD4Key, out var MD4))
                obj[MD4Key] = MD4;
            if (TryGetValue(MD5Key, out var MD5))
                obj[MD5Key] = MD5;
            if (TryGetValue(MergeKey, out var Merge))
                obj[MergeKey] = Merge;
            if (TryGetValue(TesseractOCRKey, out var TesseractOCR))
                obj[TesseractOCRKey] = TesseractOCR;
            if (TryGetValue(TesseractOCRConvertedKey, out var TesseractOCRConverted))
                obj[TesseractOCRConvertedKey] = TesseractOCRConverted;
            if (TryGetValue(TesseractOCRDetectedLangKey, out var TesseractOCRDetectedLang))
                obj[TesseractOCRDetectedLangKey] = TesseractOCRDetectedLang;
            if (TryGetValue(TesseractOCRDetectedLangConfKey, out var TesseractOCRDetectedLangConf))
                obj[TesseractOCRDetectedLangConfKey] = TesseractOCRDetectedLangConf;
            if (TryGetValue(TesseractOCRDetectedScriptKey, out var TesseractOCRDetectedScript))
                obj[TesseractOCRDetectedScriptKey] = TesseractOCRDetectedScript;
            if (TryGetValue(TesseractOCRDetectedScriptConfKey, out var TesseractOCRDetectedScriptConf))
                obj[TesseractOCRDetectedScriptConfKey] = TesseractOCRDetectedScriptConf;
            if (TryGetValue(TesseractOCRModuleVersionKey, out var TesseractOCRModuleVersion))
                obj[TesseractOCRModuleVersionKey] = TesseractOCRModuleVersion;
            if (TryGetValue(TesseractOCRParametersKey, out var TesseractOCRParameters))
                obj[TesseractOCRParametersKey] = TesseractOCRParameters;
            if (TryGetValue(OffsetKey, out var Offset))
                obj[OffsetKey] = Offset;
            if (TryGetValue(OriginalKey, out var Original))
                obj[OriginalKey] = Original;
            if (TryGetValue(PDFModuleVersionKey, out var PDFModuleVersion))
                obj[PDFModuleVersionKey] = PDFModuleVersion;
            if (TryGetValue(PreviewImageKey, out var PreviewImage))
                obj[PreviewImageKey] = PreviewImage;
            if (TryGetValue(PublisherKey, out var Publisher))
                obj[PublisherKey] = Publisher;
            if (TryGetValue(RegionKey, out var Region))
                obj[RegionKey] = Region;
            if (TryGetValue(RemarkKey, out var Remark))
                obj[RemarkKey] = Remark;
            if (TryGetValue(RIPEMD128Key, out var RIPEMD128))
                obj[RIPEMD128Key] = RIPEMD128;
            if (TryGetValue(RIPEMD160Key, out var RIPEMD160))
                obj[RIPEMD160Key] = RIPEMD160;
            if (TryGetValue(RotationKey, out var Rotation))
                obj[RotationKey] = Rotation;
            if (TryGetValue(SerialKey, out var Serial))
                obj[SerialKey] = Serial;
            if (TryGetValue(SHA1Key, out var SHA1))
                obj[SHA1Key] = SHA1;
            if (TryGetValue(SHA256Key, out var SHA256))
                obj[SHA256Key] = SHA256;
            if (TryGetValue(SHA384Key, out var SHA384))
                obj[SHA384Key] = SHA384;
            if (TryGetValue(SHA512Key, out var SHA512))
                obj[SHA512Key] = SHA512;
            if (TryGetValue(SourceKey, out var Source))
                obj[SourceKey] = Source;
            if (TryGetValue(SpamSumKey, out var SpamSum))
                obj[SpamSumKey] = SpamSum;
            if (TryGetValue(StartKey, out var Start))
                obj[StartKey] = Start;
            if (TryGetValue(SummationKey, out var Summation))
                obj[SummationKey] = Summation;
            if (TryGetValue(TitleKey, out var Title))
                obj[TitleKey] = Title;
            if (TryGetValue(TrackKey, out var Track))
                obj[TrackKey] = Track;
            if (TryGetValue(OpenMSXType, out var OpenMSXTypeValue))
                obj[OpenMSXType] = OpenMSXTypeValue;
            if (TryGetValue(WhisperASRModuleVersionKey, out var WhisperASRModuleVersion))
                obj[WhisperASRModuleVersionKey] = WhisperASRModuleVersion;
            if (TryGetValue(WhisperModelHashKey, out var WhisperModelHash))
                obj[WhisperModelHashKey] = WhisperModelHash;
            if (TryGetValue(WhisperModelNameKey, out var WhisperModelName))
                obj[WhisperModelNameKey] = WhisperModelName;
            if (TryGetValue(WhisperVersionKey, out var WhisperVersion))
                obj[WhisperVersionKey] = WhisperVersion;
            if (TryGetValue(WidthKey, out var Width))
                obj[WidthKey] = Width;
            if (TryGetValue(WordConfidenceInterval0To10Key, out var WordConfidenceInterval0To10))
                obj[WordConfidenceInterval0To10Key] = WordConfidenceInterval0To10;
            if (TryGetValue(WordConfidenceInterval11To20Key, out var WordConfidenceInterval11To20))
                obj[WordConfidenceInterval11To20Key] = WordConfidenceInterval11To20;
            if (TryGetValue(WordConfidenceInterval21To30Key, out var WordConfidenceInterval21To30))
                obj[WordConfidenceInterval21To30Key] = WordConfidenceInterval21To30;
            if (TryGetValue(WordConfidenceInterval31To40Key, out var WordConfidenceInterval31To40))
                obj[WordConfidenceInterval31To40Key] = WordConfidenceInterval31To40;
            if (TryGetValue(WordConfidenceInterval41To50Key, out var WordConfidenceInterval41To50))
                obj[WordConfidenceInterval41To50Key] = WordConfidenceInterval41To50;
            if (TryGetValue(WordConfidenceInterval51To60Key, out var WordConfidenceInterval51To60))
                obj[WordConfidenceInterval51To60Key] = WordConfidenceInterval51To60;
            if (TryGetValue(WordConfidenceInterval61To70Key, out var WordConfidenceInterval61To70))
                obj[WordConfidenceInterval61To70Key] = WordConfidenceInterval61To70;
            if (TryGetValue(WordConfidenceInterval71To80Key, out var WordConfidenceInterval71To80))
                obj[WordConfidenceInterval71To80Key] = WordConfidenceInterval71To80;
            if (TryGetValue(WordConfidenceInterval81To90Key, out var WordConfidenceInterval81To90))
                obj[WordConfidenceInterval81To90Key] = WordConfidenceInterval81To90;
            if (TryGetValue(WordConfidenceInterval91To100Key, out var WordConfidenceInterval91To100))
                obj[WordConfidenceInterval91To100Key] = WordConfidenceInterval91To100;
            if (TryGetValue(xxHash364Key, out var xxHash364))
                obj[xxHash364Key] = xxHash364;
            if (TryGetValue(xxHash3128Key, out var xxHash3128))
                obj[xxHash3128Key] = xxHash3128;

            return obj;
        }
    }
}
