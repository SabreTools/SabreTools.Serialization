using System;
using SabreTools.Data.Models.ArchiveDotOrg;
using SabreTools.Text.Extensions;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : BaseMetadataSerializer<Files>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(Files? item)
        {
            if (item is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                Header = ConvertHeaderToInternalModel(),
            };

            if (item?.File is not null && item.File.Length > 0)
                metadataFile.Machine = Array.ConvertAll(item.File, ConvertMachineToInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Files"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Data.Models.Metadata.Header
            {
                Name =  "archive.org",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(File? item)
        {
            var machine = new Data.Models.Metadata.Machine();

            var rom = ConvertToInternalModel(item);
            if (rom is not null)
                machine.Rom = new Data.Models.Metadata.Rom[] { rom };

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom? ConvertToInternalModel(File? item)
        {
            if (item is null)
                return null;

            var rom = new Data.Models.Metadata.Rom
            {
                Name = item.Name,
                Source = item.Source,
                BitTorrentMagnetHash = item.BitTorrentMagnetHash,
                LastModifiedTime = item.LastModifiedTime,
                Size = NumberHelper.ConvertToInt64(item.Size),
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
            return rom;
        }
    }
}
