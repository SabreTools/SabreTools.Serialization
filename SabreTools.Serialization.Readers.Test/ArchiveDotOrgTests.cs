using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class ArchiveDotOrgTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new ArchiveDotOrg();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new ArchiveDotOrg();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new ArchiveDotOrg();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new ArchiveDotOrg();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new ArchiveDotOrg();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new ArchiveDotOrg();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ArchiveDotOrg();
            var serializer = new Writers.ArchiveDotOrg();

            // Build the data
            Data.Models.ArchiveDotOrg.Files files = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(files);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.ArchiveDotOrg.Files? newFiles = deserializer.Deserialize(actual);

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
                BitTorrentMagnetHash = "bittorrentmagnethash",
                LastModifiedTime = "lastmodifiedtime",
                Size = "size",
                MD5 = "md5",
                CRC32 = "crc32",
                SHA1 = "sha1",
                FileCount = 12345,
                Format = "format",
                Original = "original",
                Summation = "summation",
                MatrixNumber = "matrixnumber",
                CollectionCatalogNumber = "collectioncatalognumber",
                Publisher = "publisher",
                Comment = "comment",
                ASRDetectedLang = "asrdetectedlang",
                ASRDetectedLangConf = "asrdetectedlangconf",
                ASRTranscribedLang = "asrtranscribedlang",
                WhisperASRModuleVersion = "whisperasrmoduleversion",
                WhisperModelHash = "whispermodelhash",
                WhisperModelName = "whispermodelname",
                WhisperVersion = "whisperversion",
                ClothCoverDetectionModuleVersion = "clothcoverdetectionmoduleversion",
                hOCRCharToWordhOCRVersion = "hocrchartowordhocrversion",
                hOCRCharToWordModuleVersion = "hocrchartowordmoduleversion",
                hOCRFtsTexthOCRVersion = "hocrftstexthocrversion",
                hOCRFtsTextModuleVersion = "hocrftstextmoduleversion",
                hOCRPageIndexhOCRVersion = "hocrpageindexhocrversion",
                hOCRPageIndexModuleVersion = "hocrpageindexmoduleversion",
                TesseractOCR = "tesseractocr",
                TesseractOCRConverted = "tesseractocrconverted",
                TesseractOCRDetectedLang = "tesseractocrdetectedlang",
                TesseractOCRDetectedLangConf = "tesseractocrdetectedlangconf",
                TesseractOCRDetectedScript = "tesseractocrdetectedscript",
                TesseractOCRDetectedScriptConf = "tesseractocrdetectedscriptconf",
                TesseractOCRModuleVersion = "tesseractocrmoduleversion",
                TesseractOCRParameters = "tesseractocrparameters",
                PDFModuleVersion = "pdfmoduleversion",
                WordConfidenceInterval0To10 = "wordconfidenceinterval0to10",
                WordConfidenceInterval11To20 = "wordconfidenceinterval11to20",
                WordConfidenceInterval21To30 = "wordconfidenceinterval21to30",
                WordConfidenceInterval31To40 = "wordconfidenceinterval31to40",
                WordConfidenceInterval41To50 = "wordconfidenceinterval41to50",
                WordConfidenceInterval51To60 = "wordconfidenceinterval51to60",
                WordConfidenceInterval61To70 = "wordconfidenceinterval61to70",
                WordConfidenceInterval71To80 = "wordconfidenceinterval71to80",
                WordConfidenceInterval81To90 = "wordconfidenceinterval81to90",
                WordConfidenceInterval91To100 = "wordconfidenceinterval91to100",
                Album = "album",
                Artist = "artist",
                Bitrate = "bitrate",
                Creator = "creator",
                Height = "height",
                Length = "length",
                PreviewImage = "previewimage",
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
            Assert.Equal("bittorrentmagnethash", file.BitTorrentMagnetHash);
            Assert.Equal("lastmodifiedtime", file.LastModifiedTime);
            Assert.Equal("size", file.Size);
            Assert.Equal("md5", file.MD5);
            Assert.Equal("crc32", file.CRC32);
            Assert.Equal("sha1", file.SHA1);
            Assert.Equal(12345, file.FileCount);
            Assert.Equal("format", file.Format);
            Assert.Equal("original", file.Original);
            Assert.Equal("summation", file.Summation);
            Assert.Equal("matrixnumber", file.MatrixNumber);
            Assert.Equal("collectioncatalognumber", file.CollectionCatalogNumber);
            Assert.Equal("publisher", file.Publisher);
            Assert.Equal("comment", file.Comment);
            Assert.Equal("asrdetectedlang", file.ASRDetectedLang);
            Assert.Equal("asrdetectedlangconf", file.ASRDetectedLangConf);
            Assert.Equal("asrtranscribedlang", file.ASRTranscribedLang);
            Assert.Equal("whisperasrmoduleversion", file.WhisperASRModuleVersion);
            Assert.Equal("whispermodelhash", file.WhisperModelHash);
            Assert.Equal("whispermodelname", file.WhisperModelName);
            Assert.Equal("whisperversion", file.WhisperVersion);
            Assert.Equal("clothcoverdetectionmoduleversion", file.ClothCoverDetectionModuleVersion);
            Assert.Equal("hocrchartowordhocrversion", file.hOCRCharToWordhOCRVersion);
            Assert.Equal("hocrchartowordmoduleversion", file.hOCRCharToWordModuleVersion);
            Assert.Equal("hocrftstexthocrversion", file.hOCRFtsTexthOCRVersion);
            Assert.Equal("hocrftstextmoduleversion", file.hOCRFtsTextModuleVersion);
            Assert.Equal("hocrpageindexhocrversion", file.hOCRPageIndexhOCRVersion);
            Assert.Equal("hocrpageindexmoduleversion", file.hOCRPageIndexModuleVersion);
            Assert.Equal("tesseractocr", file.TesseractOCR);
            Assert.Equal("tesseractocrconverted", file.TesseractOCRConverted);
            Assert.Equal("tesseractocrdetectedlang", file.TesseractOCRDetectedLang);
            Assert.Equal("tesseractocrdetectedlangconf", file.TesseractOCRDetectedLangConf);
            Assert.Equal("tesseractocrdetectedscript", file.TesseractOCRDetectedScript);
            Assert.Equal("tesseractocrdetectedscriptconf", file.TesseractOCRDetectedScriptConf);
            Assert.Equal("tesseractocrmoduleversion", file.TesseractOCRModuleVersion);
            Assert.Equal("tesseractocrparameters", file.TesseractOCRParameters);
            Assert.Equal("pdfmoduleversion", file.PDFModuleVersion);
            Assert.Equal("wordconfidenceinterval0to10", file.WordConfidenceInterval0To10);
            Assert.Equal("wordconfidenceinterval11to20", file.WordConfidenceInterval11To20);
            Assert.Equal("wordconfidenceinterval21to30", file.WordConfidenceInterval21To30);
            Assert.Equal("wordconfidenceinterval31to40", file.WordConfidenceInterval31To40);
            Assert.Equal("wordconfidenceinterval41to50", file.WordConfidenceInterval41To50);
            Assert.Equal("wordconfidenceinterval51to60", file.WordConfidenceInterval51To60);
            Assert.Equal("wordconfidenceinterval61to70", file.WordConfidenceInterval61To70);
            Assert.Equal("wordconfidenceinterval71to80", file.WordConfidenceInterval71To80);
            Assert.Equal("wordconfidenceinterval81to90", file.WordConfidenceInterval81To90);
            Assert.Equal("wordconfidenceinterval91to100", file.WordConfidenceInterval91To100);
            Assert.Equal("album", file.Album);
            Assert.Equal("artist", file.Artist);
            Assert.Equal("bitrate", file.Bitrate);
            Assert.Equal("creator", file.Creator);
            Assert.Equal("height", file.Height);
            Assert.Equal("length", file.Length);
            Assert.Equal("previewimage", file.PreviewImage);
            Assert.Equal("rotation", file.Rotation);
            Assert.Equal("title", file.Title);
            Assert.Equal("track", file.Track);
            Assert.Equal("width", file.Width);
        }
    }
}
