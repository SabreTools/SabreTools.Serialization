using System;
using System.Collections.Generic;
using SabreTools.Data.Models.ArchiveDotOrg;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : BaseMetadataSerializer<Files>
    {
        /// <inheritdoc/>
        public override Files? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var files = new Files();

            var machines = obj.Machine;
            var items = new List<File>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertFromInternalModel(machine));
            }

            files.File = [.. items];
            return files;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="File"/>
        /// </summary>
        private static File[] ConvertFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var roms = item.Rom;
            if (roms is null)
                return [];

            return Array.ConvertAll(roms, ConvertFromInternalModel);
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="File"/>
        /// </summary>
        private static File ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var file = new File
            {
                Name = item.Name,
                Source = item.Source,
                BitTorrentMagnetHash = item.BitTorrentMagnetHash,
                LastModifiedTime = item.LastModifiedTime,
                Size = item.Size?.ToString(),
                MD5 = item.MD5,
                CRC32 = item.CRC32,
                SHA1 = item.SHA1,
                FileCount = item.FileCount,
                Format = item.Format,
                Original = item.Original,
                Summation = item.Summation,
                MatrixNumber = item.MatrixNumber,
                CollectionCatalogNumber = item.CollectionCatalogNumber,
                Publisher = item.Publisher,
                Comment = item.Comment,

                ASRDetectedLang = item.ASRDetectedLang,
                ASRDetectedLangConf = item.ASRDetectedLangConf,
                ASRTranscribedLang = item.ASRTranscribedLang,
                WhisperASRModuleVersion = item.WhisperASRModuleVersion,
                WhisperModelHash = item.WhisperModelHash,
                WhisperModelName = item.WhisperModelName,
                WhisperVersion = item.WhisperVersion,

                ClothCoverDetectionModuleVersion = item.ClothCoverDetectionModuleVersion,
                hOCRCharToWordhOCRVersion = item.hOCRCharToWordhOCRVersion,
                hOCRCharToWordModuleVersion = item.hOCRCharToWordModuleVersion,
                hOCRFtsTexthOCRVersion = item.hOCRFtsTexthOCRVersion,
                hOCRFtsTextModuleVersion = item.hOCRFtsTextModuleVersion,
                hOCRPageIndexhOCRVersion = item.hOCRPageIndexhOCRVersion,
                hOCRPageIndexModuleVersion = item.hOCRPageIndexModuleVersion,
                TesseractOCR = item.TesseractOCR,
                TesseractOCRConverted = item.TesseractOCRConverted,
                TesseractOCRDetectedLang = item.TesseractOCRDetectedLang,
                TesseractOCRDetectedLangConf = item.TesseractOCRDetectedLangConf,
                TesseractOCRDetectedScript = item.TesseractOCRDetectedScript,
                TesseractOCRDetectedScriptConf = item.TesseractOCRDetectedScriptConf,
                TesseractOCRModuleVersion = item.TesseractOCRModuleVersion,
                TesseractOCRParameters = item.TesseractOCRParameters,
                PDFModuleVersion = item.PDFModuleVersion,
                WordConfidenceInterval0To10 = item.WordConfidenceInterval0To10,
                WordConfidenceInterval11To20 = item.WordConfidenceInterval11To20,
                WordConfidenceInterval21To30 = item.WordConfidenceInterval21To30,
                WordConfidenceInterval31To40 = item.WordConfidenceInterval31To40,
                WordConfidenceInterval41To50 = item.WordConfidenceInterval41To50,
                WordConfidenceInterval51To60 = item.WordConfidenceInterval51To60,
                WordConfidenceInterval61To70 = item.WordConfidenceInterval61To70,
                WordConfidenceInterval71To80 = item.WordConfidenceInterval71To80,
                WordConfidenceInterval81To90 = item.WordConfidenceInterval81To90,
                WordConfidenceInterval91To100 = item.WordConfidenceInterval91To100,

                Album = item.Album,
                Artist = item.Artist,
                Bitrate = item.Bitrate,
                Creator = item.Creator,
                Height = item.Height,
                Length = item.Length,
                PreviewImage = item.PreviewImage,
                Rotation = item.Rotation,
                Title = item.Title,
                Track = item.Track,
                Width = item.Width,
            };
            return file;
        }
    }
}
