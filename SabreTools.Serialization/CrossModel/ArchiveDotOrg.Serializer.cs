using System;
using SabreTools.Data.Models.ArchiveDotOrg;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : IModelSerializer<Files, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(Files? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.File != null && item.File.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.File, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Files"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Files item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = "archive.org",
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
            if (rom != null)
                machine[Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { rom };

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom? ConvertToInternalModel(File? item)
        {
            if (item == null)
                return null;

            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.NameKey] = item.Name,
                [Data.Models.Metadata.Rom.SourceKey] = item.Source,
                [Data.Models.Metadata.Rom.BitTorrentMagnetHashKey] = item.BitTorrentMagnetHash,
                [Data.Models.Metadata.Rom.LastModifiedTimeKey] = item.LastModifiedTime,
                [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                [Data.Models.Metadata.Rom.MD5Key] = item.MD5,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.FileCountKey] = item.FileCount,
                [Data.Models.Metadata.Rom.FormatKey] = item.Format,
                [Data.Models.Metadata.Rom.OriginalKey] = item.Original,
                [Data.Models.Metadata.Rom.SummationKey] = item.Summation,
                [Data.Models.Metadata.Rom.MatrixNumberKey] = item.MatrixNumber,
                [Data.Models.Metadata.Rom.CollectionCatalogNumberKey] = item.CollectionCatalogNumber,
                [Data.Models.Metadata.Rom.PublisherKey] = item.Publisher,
                [Data.Models.Metadata.Rom.CommentKey] = item.Comment,

                [Data.Models.Metadata.Rom.ASRDetectedLangKey] = item.ASRDetectedLang,
                [Data.Models.Metadata.Rom.ASRDetectedLangConfKey] = item.ASRDetectedLangConf,
                [Data.Models.Metadata.Rom.ASRTranscribedLangKey] = item.ASRTranscribedLang,
                [Data.Models.Metadata.Rom.WhisperASRModuleVersionKey] = item.WhisperASRModuleVersion,
                [Data.Models.Metadata.Rom.WhisperModelHashKey] = item.WhisperModelHash,
                [Data.Models.Metadata.Rom.WhisperModelNameKey] = item.WhisperModelName,
                [Data.Models.Metadata.Rom.WhisperVersionKey] = item.WhisperVersion,

                [Data.Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey] = item.ClothCoverDetectionModuleVersion,
                [Data.Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey] = item.hOCRCharToWordhOCRVersion,
                [Data.Models.Metadata.Rom.hOCRCharToWordModuleVersionKey] = item.hOCRCharToWordModuleVersion,
                [Data.Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey] = item.hOCRFtsTexthOCRVersion,
                [Data.Models.Metadata.Rom.hOCRFtsTextModuleVersionKey] = item.hOCRFtsTextModuleVersion,
                [Data.Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey] = item.hOCRPageIndexhOCRVersion,
                [Data.Models.Metadata.Rom.hOCRPageIndexModuleVersionKey] = item.hOCRPageIndexModuleVersion,
                [Data.Models.Metadata.Rom.TesseractOCRKey] = item.TesseractOCR,
                [Data.Models.Metadata.Rom.TesseractOCRConvertedKey] = item.TesseractOCRConverted,
                [Data.Models.Metadata.Rom.TesseractOCRDetectedLangKey] = item.TesseractOCRDetectedLang,
                [Data.Models.Metadata.Rom.TesseractOCRDetectedLangConfKey] = item.TesseractOCRDetectedLangConf,
                [Data.Models.Metadata.Rom.TesseractOCRDetectedScriptKey] = item.TesseractOCRDetectedScript,
                [Data.Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey] = item.TesseractOCRDetectedScriptConf,
                [Data.Models.Metadata.Rom.TesseractOCRModuleVersionKey] = item.TesseractOCRModuleVersion,
                [Data.Models.Metadata.Rom.TesseractOCRParametersKey] = item.TesseractOCRParameters,
                [Data.Models.Metadata.Rom.PDFModuleVersionKey] = item.PDFModuleVersion,
                [Data.Models.Metadata.Rom.WordConfidenceInterval0To10Key] = item.WordConfidenceInterval0To10,
                [Data.Models.Metadata.Rom.WordConfidenceInterval11To20Key] = item.WordConfidenceInterval11To20,
                [Data.Models.Metadata.Rom.WordConfidenceInterval21To30Key] = item.WordConfidenceInterval21To30,
                [Data.Models.Metadata.Rom.WordConfidenceInterval31To40Key] = item.WordConfidenceInterval31To40,
                [Data.Models.Metadata.Rom.WordConfidenceInterval41To50Key] = item.WordConfidenceInterval41To50,
                [Data.Models.Metadata.Rom.WordConfidenceInterval51To60Key] = item.WordConfidenceInterval51To60,
                [Data.Models.Metadata.Rom.WordConfidenceInterval61To70Key] = item.WordConfidenceInterval61To70,
                [Data.Models.Metadata.Rom.WordConfidenceInterval71To80Key] = item.WordConfidenceInterval71To80,
                [Data.Models.Metadata.Rom.WordConfidenceInterval81To90Key] = item.WordConfidenceInterval81To90,
                [Data.Models.Metadata.Rom.WordConfidenceInterval91To100Key] = item.WordConfidenceInterval91To100,

                [Data.Models.Metadata.Rom.AlbumKey] = item.Album,
                [Data.Models.Metadata.Rom.ArtistKey] = item.Artist,
                [Data.Models.Metadata.Rom.BitrateKey] = item.Bitrate,
                [Data.Models.Metadata.Rom.CreatorKey] = item.Creator,
                [Data.Models.Metadata.Rom.HeightKey] = item.Height,
                [Data.Models.Metadata.Rom.LengthKey] = item.Length,
                [Data.Models.Metadata.Rom.PreviewImageKey] = item.PreviewImage,
                [Data.Models.Metadata.Rom.RotationKey] = item.Rotation,
                [Data.Models.Metadata.Rom.TitleKey] = item.Title,
                [Data.Models.Metadata.Rom.TrackKey] = item.Track,
                [Data.Models.Metadata.Rom.WidthKey] = item.Width,
            };
            return rom;
        }
    }
}
