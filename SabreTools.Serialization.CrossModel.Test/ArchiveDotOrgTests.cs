using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class ArchiveDotOrgTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new ArchiveDotOrg();

            // Build the data
            Data.Models.ArchiveDotOrg.Files files = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(files);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.ArchiveDotOrg.Files? newFiles = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newFiles);
            Assert.NotNull(newFiles.File);
            var newFile = Assert.Single(newFiles.File);
            Validate(newFile);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.ArchiveDotOrg.Files Build()
        {
            var file = new Data.Models.ArchiveDotOrg.File
            {
                Name = "name",
                Source = "source",
                BitTorrentMagnetHash = "btih",
                LastModifiedTime = "mtime",
                Size = "12345",
                MD5 = "md5",
                CRC32 = "crc32",
                SHA1 = "sha1",
                FileCount = 12345,
                Format = "format",
                Original = "original",
                Summation = "summation",
                MatrixNumber = "matrix_number",
                CollectionCatalogNumber = "collection-catalog-number",
                Publisher = "publisher",
                Comment = "comment",
                ASRDetectedLang = "asr_detected_lang",
                ASRDetectedLangConf = "asr_detected_lang_conf",
                ASRTranscribedLang = "asr_transcribed_lang",
                WhisperASRModuleVersion = "whisper_asr_module_version",
                WhisperModelHash = "whisper_model_hash",
                WhisperModelName = "whisper_model_name",
                WhisperVersion = "whisper_version",
                ClothCoverDetectionModuleVersion = "cloth_cover_detection_module_version",
                hOCRCharToWordhOCRVersion = "hocr_char_to_word_hocr_version",
                hOCRCharToWordModuleVersion = "hocr_char_to_word_module_version",
                hOCRFtsTexthOCRVersion = "hocr_fts_text_hocr_version",
                hOCRFtsTextModuleVersion = "hocr_fts_text_module_version",
                hOCRPageIndexhOCRVersion = "hocr_pageindex_hocr_version",
                hOCRPageIndexModuleVersion = "hocr_pageindex_module_version",
                TesseractOCR = "ocr",
                TesseractOCRConverted = "ocr_converted",
                TesseractOCRDetectedLang = "ocr_detected_lang",
                TesseractOCRDetectedLangConf = "ocr_detected_lang_conf",
                TesseractOCRDetectedScript = "ocr_detected_script",
                TesseractOCRDetectedScriptConf = "ocr_detected_script_conf",
                TesseractOCRModuleVersion = "ocr_module_version",
                TesseractOCRParameters = "ocr_parameters",
                PDFModuleVersion = "pdf_module_version",
                WordConfidenceInterval0To10 = "word_conf_0_10",
                WordConfidenceInterval11To20 = "word_conf_11_20",
                WordConfidenceInterval21To30 = "word_conf_21_30",
                WordConfidenceInterval31To40 = "word_conf_31_40",
                WordConfidenceInterval41To50 = "word_conf_41_50",
                WordConfidenceInterval51To60 = "word_conf_51_60",
                WordConfidenceInterval61To70 = "word_conf_61_70",
                WordConfidenceInterval71To80 = "word_conf_71_80",
                WordConfidenceInterval81To90 = "word_conf_81_90",
                WordConfidenceInterval91To100 = "word_conf_91_100",
                Album = "album",
                Artist = "artist",
                Bitrate = "bitrate",
                Creator = "creator",
                Height = "height",
                Length = "length",
                PreviewImage = "preview-image",
                Rotation = "rotation",
                Title = "title",
                Track = "track",
                Width = "width",
            };

            return new Data.Models.ArchiveDotOrg.Files
            {
                File = [file]
            };
        }

        /// <summary>
        /// Validate a File
        /// </summary>
        private static void Validate(Data.Models.ArchiveDotOrg.File? file)
        {
            Assert.NotNull(file);
            Assert.Equal("name", file.Name);
            Assert.Equal("source", file.Source);
            Assert.Equal("btih", file.BitTorrentMagnetHash);
            Assert.Equal("mtime", file.LastModifiedTime);
            Assert.Equal("12345", file.Size);
            Assert.Equal("md5", file.MD5);
            Assert.Equal("crc32", file.CRC32);
            Assert.Equal("sha1", file.SHA1);
            Assert.Equal(12345, file.FileCount);
            Assert.Equal("format", file.Format);
            Assert.Equal("original", file.Original);
            Assert.Equal("summation", file.Summation);
            Assert.Equal("matrix_number", file.MatrixNumber);
            Assert.Equal("collection-catalog-number", file.CollectionCatalogNumber);
            Assert.Equal("publisher", file.Publisher);
            Assert.Equal("comment", file.Comment);
            Assert.Equal("asr_detected_lang", file.ASRDetectedLang);
            Assert.Equal("asr_detected_lang_conf", file.ASRDetectedLangConf);
            Assert.Equal("asr_transcribed_lang", file.ASRTranscribedLang);
            Assert.Equal("whisper_asr_module_version", file.WhisperASRModuleVersion);
            Assert.Equal("whisper_model_hash", file.WhisperModelHash);
            Assert.Equal("whisper_model_name", file.WhisperModelName);
            Assert.Equal("whisper_version", file.WhisperVersion);
            Assert.Equal("cloth_cover_detection_module_version", file.ClothCoverDetectionModuleVersion);
            Assert.Equal("hocr_char_to_word_hocr_version", file.hOCRCharToWordhOCRVersion);
            Assert.Equal("hocr_char_to_word_module_version", file.hOCRCharToWordModuleVersion);
            Assert.Equal("hocr_fts_text_hocr_version", file.hOCRFtsTexthOCRVersion);
            Assert.Equal("hocr_fts_text_module_version", file.hOCRFtsTextModuleVersion);
            Assert.Equal("hocr_pageindex_hocr_version", file.hOCRPageIndexhOCRVersion);
            Assert.Equal("hocr_pageindex_module_version", file.hOCRPageIndexModuleVersion);
            Assert.Equal("ocr", file.TesseractOCR);
            Assert.Equal("ocr_converted", file.TesseractOCRConverted);
            Assert.Equal("ocr_detected_lang", file.TesseractOCRDetectedLang);
            Assert.Equal("ocr_detected_lang_conf", file.TesseractOCRDetectedLangConf);
            Assert.Equal("ocr_detected_script", file.TesseractOCRDetectedScript);
            Assert.Equal("ocr_detected_script_conf", file.TesseractOCRDetectedScriptConf);
            Assert.Equal("ocr_module_version", file.TesseractOCRModuleVersion);
            Assert.Equal("ocr_parameters", file.TesseractOCRParameters);
            Assert.Equal("pdf_module_version", file.PDFModuleVersion);
            Assert.Equal("word_conf_0_10", file.WordConfidenceInterval0To10);
            Assert.Equal("word_conf_11_20", file.WordConfidenceInterval11To20);
            Assert.Equal("word_conf_21_30", file.WordConfidenceInterval21To30);
            Assert.Equal("word_conf_31_40", file.WordConfidenceInterval31To40);
            Assert.Equal("word_conf_41_50", file.WordConfidenceInterval41To50);
            Assert.Equal("word_conf_51_60", file.WordConfidenceInterval51To60);
            Assert.Equal("word_conf_61_70", file.WordConfidenceInterval61To70);
            Assert.Equal("word_conf_71_80", file.WordConfidenceInterval71To80);
            Assert.Equal("word_conf_81_90", file.WordConfidenceInterval81To90);
            Assert.Equal("word_conf_91_100", file.WordConfidenceInterval91To100);
            Assert.Equal("album", file.Album);
            Assert.Equal("artist", file.Artist);
            Assert.Equal("bitrate", file.Bitrate);
            Assert.Equal("creator", actual: file.Creator);
            Assert.Equal("height", file.Height);
            Assert.Equal("length", file.Length);
            Assert.Equal("preview-image", file.PreviewImage);
            Assert.Equal("rotation", file.Rotation);
            Assert.Equal("title", file.Title);
            Assert.Equal("track", file.Track);
            Assert.Equal("width", file.Width);
        }
    }
}
