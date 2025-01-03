using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class ArchiveDotOrgTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Serialization.CrossModel.ArchiveDotOrg();

            // Build the data
            Models.ArchiveDotOrg.Files files = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(files);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.ArchiveDotOrg.Files? newFiles = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newFiles);
            Assert.NotNull(newFiles.File);
            var newFile = Assert.Single(newFiles.File);
            Validate(newFile);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.ArchiveDotOrg.Files Build()
        {
            var file = new Models.ArchiveDotOrg.File
            {
                Name = "XXXXXX",
                Source = "XXXXXX",
                BitTorrentMagnetHash = "XXXXXX",
                LastModifiedTime = "XXXXXX",
                Size = "XXXXXX",
                MD5 = "XXXXXX",
                CRC32 = "XXXXXX",
                SHA1 = "XXXXXX",
                FileCount = "XXXXXX",
                Format = "XXXXXX",
                Original = "XXXXXX",
                Summation = "XXXXXX",
                MatrixNumber = "XXXXXX",
                CollectionCatalogNumber = "XXXXXX",
                Publisher = "XXXXXX",
                Comment = "XXXXXX",
                ASRDetectedLang = "XXXXXX",
                ASRDetectedLangConf = "XXXXXX",
                ASRTranscribedLang = "XXXXXX",
                WhisperASRModuleVersion = "XXXXXX",
                WhisperModelHash = "XXXXXX",
                WhisperModelName = "XXXXXX",
                WhisperVersion = "XXXXXX",
                ClothCoverDetectionModuleVersion = "XXXXXX",
                hOCRCharToWordhOCRVersion = "XXXXXX",
                hOCRCharToWordModuleVersion = "XXXXXX",
                hOCRFtsTexthOCRVersion = "XXXXXX",
                hOCRFtsTextModuleVersion = "XXXXXX",
                hOCRPageIndexhOCRVersion = "XXXXXX",
                hOCRPageIndexModuleVersion = "XXXXXX",
                TesseractOCR = "XXXXXX",
                TesseractOCRConverted = "XXXXXX",
                TesseractOCRDetectedLang = "XXXXXX",
                TesseractOCRDetectedLangConf = "XXXXXX",
                TesseractOCRDetectedScript = "XXXXXX",
                TesseractOCRDetectedScriptConf = "XXXXXX",
                TesseractOCRModuleVersion = "XXXXXX",
                TesseractOCRParameters = "XXXXXX",
                PDFModuleVersion = "XXXXXX",
                WordConfidenceInterval0To10 = "XXXXXX",
                WordConfidenceInterval11To20 = "XXXXXX",
                WordConfidenceInterval21To30 = "XXXXXX",
                WordConfidenceInterval31To40 = "XXXXXX",
                WordConfidenceInterval41To50 = "XXXXXX",
                WordConfidenceInterval51To60 = "XXXXXX",
                WordConfidenceInterval61To70 = "XXXXXX",
                WordConfidenceInterval71To80 = "XXXXXX",
                WordConfidenceInterval81To90 = "XXXXXX",
                WordConfidenceInterval91To100 = "XXXXXX",
                Album = "XXXXXX",
                Artist = "XXXXXX",
                Bitrate = "XXXXXX",
                Creator = "XXXXXX",
                Height = "XXXXXX",
                Length = "XXXXXX",
                PreviewImage = "XXXXXX",
                Rotation = "XXXXXX",
                Title = "XXXXXX",
                Track = "XXXXXX",
                Width = "XXXXXX",
            };

            return new Models.ArchiveDotOrg.Files
            {
                File = [file]
            };
        }

        /// <summary>
        /// Validate a File
        /// </summary>
        private static void Validate(Models.ArchiveDotOrg.File? file)
        {
            Assert.NotNull(file);
            Assert.Equal("XXXXXX", file.Name);
            Assert.Equal("XXXXXX", file.Source);
            Assert.Equal("XXXXXX", file.BitTorrentMagnetHash);
            Assert.Equal("XXXXXX", file.LastModifiedTime);
            Assert.Equal("XXXXXX", file.Size);
            Assert.Equal("XXXXXX", file.MD5);
            Assert.Equal("XXXXXX", file.CRC32);
            Assert.Equal("XXXXXX", file.SHA1);
            Assert.Equal("XXXXXX", file.FileCount);
            Assert.Equal("XXXXXX", file.Format);
            Assert.Equal("XXXXXX", file.Original);
            Assert.Equal("XXXXXX", file.Summation);
            Assert.Equal("XXXXXX", file.MatrixNumber);
            Assert.Equal("XXXXXX", file.CollectionCatalogNumber);
            Assert.Equal("XXXXXX", file.Publisher);
            Assert.Equal("XXXXXX", file.Comment);
            Assert.Equal("XXXXXX", file.ASRDetectedLang);
            Assert.Equal("XXXXXX", file.ASRDetectedLangConf);
            Assert.Equal("XXXXXX", file.ASRTranscribedLang);
            Assert.Equal("XXXXXX", file.WhisperASRModuleVersion);
            Assert.Equal("XXXXXX", file.WhisperModelHash);
            Assert.Equal("XXXXXX", file.WhisperModelName);
            Assert.Equal("XXXXXX", file.WhisperVersion);
            Assert.Equal("XXXXXX", file.ClothCoverDetectionModuleVersion);
            Assert.Equal("XXXXXX", file.hOCRCharToWordhOCRVersion);
            Assert.Equal("XXXXXX", file.hOCRCharToWordModuleVersion);
            Assert.Equal("XXXXXX", file.hOCRFtsTexthOCRVersion);
            Assert.Equal("XXXXXX", file.hOCRFtsTextModuleVersion);
            Assert.Equal("XXXXXX", file.hOCRPageIndexhOCRVersion);
            Assert.Equal("XXXXXX", file.hOCRPageIndexModuleVersion);
            Assert.Equal("XXXXXX", file.TesseractOCR);
            Assert.Equal("XXXXXX", file.TesseractOCRConverted);
            Assert.Equal("XXXXXX", file.TesseractOCRDetectedLang);
            Assert.Equal("XXXXXX", file.TesseractOCRDetectedLangConf);
            Assert.Equal("XXXXXX", file.TesseractOCRDetectedScript);
            Assert.Equal("XXXXXX", file.TesseractOCRDetectedScriptConf);
            Assert.Equal("XXXXXX", file.TesseractOCRModuleVersion);
            Assert.Equal("XXXXXX", file.TesseractOCRParameters);
            Assert.Equal("XXXXXX", file.PDFModuleVersion);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval0To10);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval11To20);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval21To30);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval31To40);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval41To50);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval51To60);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval61To70);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval71To80);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval81To90);
            Assert.Equal("XXXXXX", file.WordConfidenceInterval91To100);
            Assert.Equal("XXXXXX", file.Album);
            Assert.Equal("XXXXXX", file.Artist);
            Assert.Equal("XXXXXX", file.Bitrate);
            Assert.Equal("XXXXXX", file.Creator);
            Assert.Equal("XXXXXX", file.Height);
            Assert.Equal("XXXXXX", file.Length);
            Assert.Equal("XXXXXX", file.PreviewImage);
            Assert.Equal("XXXXXX", file.Rotation);
            Assert.Equal("XXXXXX", file.Title);
            Assert.Equal("XXXXXX", file.Track);
            Assert.Equal("XXXXXX", file.Width);
        }
    }
}