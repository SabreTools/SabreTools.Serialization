using System.IO;
using System.Text;
using System.Xml;
using SabreTools.Data.Models.ArchiveDotOrg;
using SabreTools.IO.Extensions;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class ArchiveDotOrg : BaseBinaryWriter<Files>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(Files? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                IndentChar = '\t',
                Indentation = 1
            };
            writer.Settings?.CheckCharacters = false;
            writer.Settings?.NewLineChars = "\n";

            // Write document start
            writer.WriteStartDocument();

            // Write the files, if they exist
            WriteFiles(obj, writer);
            writer.Flush();

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write a File to an XmlTextWriter
        /// </summary>
        /// <param name="obj">File to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFile(Data.Models.ArchiveDotOrg.File obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("file");

            writer.WriteOptionalAttributeString("name", obj.Name);
            writer.WriteOptionalAttributeString("source", obj.Source);

            writer.WriteOptionalElementString("btih", obj.BitTorrentMagnetHash);
            writer.WriteOptionalElementString("mtime", obj.LastModifiedTime);
            writer.WriteOptionalElementString("size", obj.Size);
            writer.WriteOptionalElementString("md5", obj.MD5);
            writer.WriteOptionalElementString("crc32", obj.CRC32);
            writer.WriteOptionalElementString("sha1", obj.SHA1);
            writer.WriteOptionalElementString("filecount", obj.FileCount);
            writer.WriteOptionalElementString("format", obj.Format);
            writer.WriteOptionalElementString("original", obj.Original);
            writer.WriteOptionalElementString("summation", obj.Summation);
            writer.WriteOptionalElementString("matrix_number", obj.MatrixNumber);
            writer.WriteOptionalElementString("collection-catalog-number", obj.CollectionCatalogNumber);
            writer.WriteOptionalElementString("publisher", obj.Publisher);
            writer.WriteOptionalElementString("comment", obj.Comment);

            // ASR-Related
            writer.WriteOptionalElementString("asr_detected_lang", obj.ASRDetectedLang);
            writer.WriteOptionalElementString("asr_detected_lang_conf", obj.ASRDetectedLangConf);
            writer.WriteOptionalElementString("asr_transcribed_lang", obj.ASRTranscribedLang);
            writer.WriteOptionalElementString("whisper_asr_module_version", obj.WhisperASRModuleVersion);
            writer.WriteOptionalElementString("whisper_model_hash", obj.WhisperModelHash);
            writer.WriteOptionalElementString("whisper_model_name", obj.WhisperModelName);
            writer.WriteOptionalElementString("whisper_version", obj.WhisperVersion);

            // OCR-Related
            writer.WriteOptionalElementString("cloth_cover_detection_module_version", obj.ClothCoverDetectionModuleVersion);
            writer.WriteOptionalElementString("hocr_char_to_word_hocr_version", obj.hOCRCharToWordhOCRVersion);
            writer.WriteOptionalElementString("hocr_char_to_word_module_version", obj.hOCRCharToWordModuleVersion);
            writer.WriteOptionalElementString("hocr_fts_text_hocr_version", obj.hOCRFtsTexthOCRVersion);
            writer.WriteOptionalElementString("hocr_fts_text_module_version", obj.hOCRFtsTextModuleVersion);
            writer.WriteOptionalElementString("hocr_pageindex_hocr_version", obj.hOCRPageIndexhOCRVersion);
            writer.WriteOptionalElementString("hocr_pageindex_module_version", obj.hOCRPageIndexModuleVersion);
            writer.WriteOptionalElementString("ocr", obj.TesseractOCR);
            writer.WriteOptionalElementString("ocr_converted", obj.TesseractOCRConverted);
            writer.WriteOptionalElementString("ocr_detected_lang", obj.TesseractOCRDetectedLang);
            writer.WriteOptionalElementString("ocr_detected_lang_conf", obj.TesseractOCRDetectedLangConf);
            writer.WriteOptionalElementString("ocr_detected_script", obj.TesseractOCRDetectedScript);
            writer.WriteOptionalElementString("ocr_detected_script_conf", obj.TesseractOCRDetectedScriptConf);
            writer.WriteOptionalElementString("ocr_module_version", obj.TesseractOCRModuleVersion);
            writer.WriteOptionalElementString("ocr_parameters", obj.TesseractOCRParameters);
            writer.WriteOptionalElementString("pdf_module_version", obj.PDFModuleVersion);
            writer.WriteOptionalElementString("word_conf_0_10", obj.WordConfidenceInterval0To10);
            writer.WriteOptionalElementString("word_conf_11_20", obj.WordConfidenceInterval11To20);
            writer.WriteOptionalElementString("word_conf_21_30", obj.WordConfidenceInterval21To30);
            writer.WriteOptionalElementString("word_conf_31_40", obj.WordConfidenceInterval31To40);
            writer.WriteOptionalElementString("word_conf_41_50", obj.WordConfidenceInterval41To50);
            writer.WriteOptionalElementString("word_conf_51_60", obj.WordConfidenceInterval51To60);
            writer.WriteOptionalElementString("word_conf_61_70", obj.WordConfidenceInterval61To70);
            writer.WriteOptionalElementString("word_conf_71_80", obj.WordConfidenceInterval71To80);
            writer.WriteOptionalElementString("word_conf_81_90", obj.WordConfidenceInterval81To90);
            writer.WriteOptionalElementString("word_conf_91_100", obj.WordConfidenceInterval91To100);

            // Media-Related
            writer.WriteOptionalElementString("album", obj.Album);
            writer.WriteOptionalElementString("artist", obj.Artist);
            writer.WriteOptionalElementString("bitrate", obj.Bitrate);
            writer.WriteOptionalElementString("creator", obj.Creator);
            writer.WriteOptionalElementString("height", obj.Height);
            writer.WriteOptionalElementString("length", obj.Length);
            writer.WriteOptionalElementString("preview-image", obj.PreviewImage);
            writer.WriteOptionalElementString("rotation", obj.Rotation);
            writer.WriteOptionalElementString("title", obj.Title);
            writer.WriteOptionalElementString("track", obj.Track);
            writer.WriteOptionalElementString("width", obj.Width);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write a Files to an XmlTextWriter
        /// </summary>
        /// <param name="obj">Files to write</param>
        /// <param name="writer">XmlTextReader to write to</param>
        private static void WriteFiles(Files obj, XmlTextWriter writer)
        {
            writer.WriteStartElement("files");

            if (obj.File is not null && obj.File.Length > 0)
            {
                foreach (var file in obj.File)
                {
                    WriteFile(file, writer);
                }
            }

            writer.WriteEndElement();
        }
    }
}
