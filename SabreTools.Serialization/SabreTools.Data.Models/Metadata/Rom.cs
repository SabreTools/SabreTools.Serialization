using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Data.Models.Metadata
{
    [JsonObject("rom"), XmlRoot("rom")]
    public class Rom : DatItem
    {
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

        /// <remarks>(yes|no) "no"</remarks>
        public const string DisposeKey = "dispose";

        /// <remarks>string; OfflineList.FileRomCRC</remarks>
        public const string ExtensionKey = "extension";

        /// <remarks>long; ArchiveDotOrg.File</remarks>
        public const string FileCountKey = "filecount";

        /// <remarks>bool; AttractMode.Row</remarks>
        public const string FileIsAvailableKey = "file_is_available";

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

        /// <remarks>(yes|no) "no"</remarks>
        public const string InvertedKey = "inverted";

        /// <remarks>long; ArchiveDotOrg.File</remarks>
        public const string LastModifiedTimeKey = "mtime";

        /// <remarks>string, possibly long; Also in ArchiveDotOrg.File</remarks>
        public const string LengthKey = "length";

        /// <remarks>(load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore)</remarks>
        public const string LoadFlagKey = "loadflag";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string MatrixNumberKey = "matrix_number";

        /// <remarks>string</remarks>
        public const string MD2Key = "md2";

        /// <remarks>string</remarks>
        public const string MD4Key = "md4";

        /// <remarks>string</remarks>
        public const string MD5Key = "md5";

        /// <remarks>string; OpenMSX.RomBase</remarks>
        public const string OpenMSXMediaType = "mediatype";

        /// <remarks>string</remarks>
        public const string MergeKey = "merge";

        /// <remarks>(yes|no) "no"</remarks>
        public const string MIAKey = "mia";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

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

        /// <remarks>(yes|no) "no"</remarks>
        public const string OptionalKey = "optional";

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

        /// <remarks>long</remarks>
        public const string SizeKey = "size";

        /// <remarks>(yes|no) "no"</remarks>
        public const string SoundOnlyKey = "soundonly";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string SourceKey = "source";

        /// <remarks>string</remarks>
        public const string SpamSumKey = "spamsum";

        /// <remarks>string, possibly long; OpenMSX.RomBase</remarks>
        public const string StartKey = "start";

        /// <remarks>(baddump|nodump|good|verified) "good"</remarks>
        public const string StatusKey = "status";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string SummationKey = "summation";

        /// <remarks>string; ArchiveDotOrg.File</remarks>
        public const string TitleKey = "title";

        /// <remarks>string, possibly long; ArchiveDotOrg.File</remarks>
        public const string TrackKey = "track";

        /// <remarks>string; OpenMSX.RomBase</remarks>
        public const string OpenMSXType = "type";

        /// <remarks>string</remarks>
        public const string ValueKey = "value";

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

        public Rom() => Type = ItemType.Rom;
    }
}
