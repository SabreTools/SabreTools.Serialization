using System;
using System.Collections.Generic;
using System.Xml;
using SabreTools.Data.Models.ArchiveDotOrg;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class ArchiveDotOrg : BaseBinaryReader<Files>
    {
        /// <inheritdoc/>
        public override Files? Deserialize(System.IO.Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                // Create the XmlTextReader
                var reader = new XmlTextReader(data);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                // Parse the XML, if possible
                Files? files = null;
                while (reader.Read())
                {
                    // Only process starting elements
                    if (!reader.IsStartElement())
                        continue;

                    switch (reader.Name)
                    {
                        case "files":
                            if (files is not null && Debug)
                                Console.WriteLine($"'{reader.Name}' element already found, overwriting");

                            files = ParseFiles(reader);
                            break;

                        default:
                            if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                            break;
                    }
                }

                return files;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse from an XmlTextReader into a Files
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled Files on success, null on error</returns>
        public Files ParseFiles(XmlTextReader reader)
        {
            var obj = new Files();

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            List<File> files = [];
            while (reader.Read())
            {
                // Only process starting elements
                if (!reader.IsStartElement())
                    continue;

                switch (reader.Name)
                {
                    case "file":
                        var file = ParseFile(reader);
                        if (file is not null)
                            files.Add(file);

                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            obj.File = [.. files];

            return obj;
        }

        /// <summary>
        /// Parse from an XmlTextReader into a File
        /// </summary>
        /// <param name="reader">XmlTextReader to read from</param>
        /// <returns>Filled File on success, null on error</returns>
        public File ParseFile(XmlTextReader reader)
        {
            var obj = new File();

            obj.Name = reader.GetAttribute("name");
            obj.Source = reader.GetAttribute("source");

            // Handle empty elements
            if (reader.IsEmptyElement)
                return obj;

            reader.Read();
            while (!reader.EOF)
            {
                // An ending element means exit
                if (reader.NodeType == XmlNodeType.EndElement)
                    break;

                // Only process starting elements
                if (!reader.IsStartElement())
                {
                    reader.Skip();
                    continue;
                }

                switch (reader.Name)
                {
                    case "btih":
                        obj.BitTorrentMagnetHash = reader.ReadElementContentAsString();
                        break;
                    case "mtime":
                        obj.LastModifiedTime = reader.ReadElementContentAsString();
                        break;
                    case "size":
                        obj.Size = reader.ReadElementContentAsString();
                        break;
                    case "md5":
                        obj.MD5 = reader.ReadElementContentAsString();
                        break;
                    case "crc32":
                        obj.CRC32 = reader.ReadElementContentAsString();
                        break;
                    case "sha1":
                        obj.SHA1 = reader.ReadElementContentAsString();
                        break;
                    case "filecount":
                        obj.FileCount = NumberHelper.ConvertToInt64(reader.ReadElementContentAsString());
                        break;
                    case "format":
                        obj.Format = reader.ReadElementContentAsString();
                        break;
                    case "original":
                        obj.Original = reader.ReadElementContentAsString();
                        break;
                    case "summation":
                        obj.Summation = reader.ReadElementContentAsString();
                        break;
                    case "matrix_number":
                        obj.MatrixNumber = reader.ReadElementContentAsString();
                        break;
                    case "collection-catalog-number":
                        obj.CollectionCatalogNumber = reader.ReadElementContentAsString();
                        break;
                    case "publisher":
                        obj.Publisher = reader.ReadElementContentAsString();
                        break;
                    case "comment":
                        obj.Comment = reader.ReadElementContentAsString();
                        break;

                    // ASR-Related
                    case "asr_detected_lang":
                        obj.ASRDetectedLang = reader.ReadElementContentAsString();
                        break;
                    case "asr_detected_lang_conf":
                        obj.ASRDetectedLangConf = reader.ReadElementContentAsString();
                        break;
                    case "asr_transcribed_lang":
                        obj.ASRTranscribedLang = reader.ReadElementContentAsString();
                        break;
                    case "whisper_asr_module_version":
                        obj.WhisperASRModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "whisper_model_hash":
                        obj.WhisperModelHash = reader.ReadElementContentAsString();
                        break;
                    case "whisper_model_name":
                        obj.WhisperModelName = reader.ReadElementContentAsString();
                        break;
                    case "whisper_version":
                        obj.WhisperVersion = reader.ReadElementContentAsString();
                        break;

                    // OCR-Related
                    case "cloth_cover_detection_module_version":
                        obj.ClothCoverDetectionModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "hocr_char_to_word_hocr_version":
                        obj.hOCRCharToWordhOCRVersion = reader.ReadElementContentAsString();
                        break;
                    case "hocr_char_to_word_module_version":
                        obj.hOCRCharToWordModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "hocr_fts_text_hocr_version":
                        obj.hOCRFtsTexthOCRVersion = reader.ReadElementContentAsString();
                        break;
                    case "hocr_fts_text_module_version":
                        obj.hOCRFtsTextModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "hocr_pageindex_hocr_version":
                        obj.hOCRPageIndexhOCRVersion = reader.ReadElementContentAsString();
                        break;
                    case "hocr_pageindex_module_version":
                        obj.hOCRPageIndexModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "ocr":
                        obj.TesseractOCR = reader.ReadElementContentAsString();
                        break;
                    case "ocr_converted":
                        obj.TesseractOCRConverted = reader.ReadElementContentAsString();
                        break;
                    case "ocr_detected_lang":
                        obj.TesseractOCRDetectedLang = reader.ReadElementContentAsString();
                        break;
                    case "ocr_detected_lang_conf":
                        obj.TesseractOCRDetectedLangConf = reader.ReadElementContentAsString();
                        break;
                    case "ocr_detected_script":
                        obj.TesseractOCRDetectedScript = reader.ReadElementContentAsString();
                        break;
                    case "ocr_detected_script_conf":
                        obj.TesseractOCRDetectedScriptConf = reader.ReadElementContentAsString();
                        break;
                    case "ocr_module_version":
                        obj.TesseractOCRModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "ocr_parameters":
                        obj.TesseractOCRParameters = reader.ReadElementContentAsString();
                        break;
                    case "pdf_module_version":
                        obj.PDFModuleVersion = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_0_10":
                        obj.WordConfidenceInterval0To10 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_11_20":
                        obj.WordConfidenceInterval11To20 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_21_30":
                        obj.WordConfidenceInterval21To30 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_31_40":
                        obj.WordConfidenceInterval31To40 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_41_50":
                        obj.WordConfidenceInterval41To50 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_51_60":
                        obj.WordConfidenceInterval51To60 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_61_70":
                        obj.WordConfidenceInterval61To70 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_71_80":
                        obj.WordConfidenceInterval71To80 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_81_90":
                        obj.WordConfidenceInterval81To90 = reader.ReadElementContentAsString();
                        break;
                    case "word_conf_91_100":
                        obj.WordConfidenceInterval91To100 = reader.ReadElementContentAsString();
                        break;

                    // Media-Related
                    case "album":
                        obj.Album = reader.ReadElementContentAsString();
                        break;
                    case "artist":
                        obj.Artist = reader.ReadElementContentAsString();
                        break;
                    case "bitrate":
                        obj.Bitrate = reader.ReadElementContentAsString();
                        break;
                    case "creator":
                        obj.Creator = reader.ReadElementContentAsString();
                        break;
                    case "height":
                        obj.Height = reader.ReadElementContentAsString();
                        break;
                    case "length":
                        obj.Length = reader.ReadElementContentAsString();
                        break;
                    case "preview-image":
                        obj.PreviewImage = reader.ReadElementContentAsString();
                        break;
                    case "rotation":
                        obj.Rotation = reader.ReadElementContentAsString();
                        break;
                    case "title":
                        obj.Title = reader.ReadElementContentAsString();
                        break;
                    case "track":
                        obj.Track = reader.ReadElementContentAsString();
                        break;
                    case "width":
                        obj.Width = reader.ReadElementContentAsString();
                        break;

                    default:
                        if (Debug) Console.Error.WriteLine($"Element '{reader.Name}' is not recognized");
                        reader.Skip();
                        break;
                }
            }

            return obj;
        }
    }
}
