using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.ArchiveDotOrg;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : IModelSerializer<Files, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(Files? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.File != null && item.File.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.File, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Files"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(Files item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.NameKey] = "archive.org",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(File? item)
        {
            var machine = new Serialization.Models.Metadata.Machine();

            var rom = ConvertToInternalModel(item);
            if (rom != null)
                machine[Serialization.Models.Metadata.Machine.RomKey] = new Serialization.Models.Metadata.Rom[] { rom };

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom? ConvertToInternalModel(File? item)
        {
            if (item == null)
                return null;

            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                [Serialization.Models.Metadata.Rom.SourceKey] = item.Source,
                [Serialization.Models.Metadata.Rom.BitTorrentMagnetHashKey] = item.BitTorrentMagnetHash,
                [Serialization.Models.Metadata.Rom.LastModifiedTimeKey] = item.LastModifiedTime,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                [Serialization.Models.Metadata.Rom.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Rom.FileCountKey] = item.FileCount,
                [Serialization.Models.Metadata.Rom.FormatKey] = item.Format,
                [Serialization.Models.Metadata.Rom.OriginalKey] = item.Original,
                [Serialization.Models.Metadata.Rom.SummationKey] = item.Summation,
                [Serialization.Models.Metadata.Rom.MatrixNumberKey] = item.MatrixNumber,
                [Serialization.Models.Metadata.Rom.CollectionCatalogNumberKey] = item.CollectionCatalogNumber,
                [Serialization.Models.Metadata.Rom.PublisherKey] = item.Publisher,
                [Serialization.Models.Metadata.Rom.CommentKey] = item.Comment,

                [Serialization.Models.Metadata.Rom.ASRDetectedLangKey] = item.ASRDetectedLang,
                [Serialization.Models.Metadata.Rom.ASRDetectedLangConfKey] = item.ASRDetectedLangConf,
                [Serialization.Models.Metadata.Rom.ASRTranscribedLangKey] = item.ASRTranscribedLang,
                [Serialization.Models.Metadata.Rom.WhisperASRModuleVersionKey] = item.WhisperASRModuleVersion,
                [Serialization.Models.Metadata.Rom.WhisperModelHashKey] = item.WhisperModelHash,
                [Serialization.Models.Metadata.Rom.WhisperModelNameKey] = item.WhisperModelName,
                [Serialization.Models.Metadata.Rom.WhisperVersionKey] = item.WhisperVersion,

                [Serialization.Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey] = item.ClothCoverDetectionModuleVersion,
                [Serialization.Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey] = item.hOCRCharToWordhOCRVersion,
                [Serialization.Models.Metadata.Rom.hOCRCharToWordModuleVersionKey] = item.hOCRCharToWordModuleVersion,
                [Serialization.Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey] = item.hOCRFtsTexthOCRVersion,
                [Serialization.Models.Metadata.Rom.hOCRFtsTextModuleVersionKey] = item.hOCRFtsTextModuleVersion,
                [Serialization.Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey] = item.hOCRPageIndexhOCRVersion,
                [Serialization.Models.Metadata.Rom.hOCRPageIndexModuleVersionKey] = item.hOCRPageIndexModuleVersion,
                [Serialization.Models.Metadata.Rom.TesseractOCRKey] = item.TesseractOCR,
                [Serialization.Models.Metadata.Rom.TesseractOCRConvertedKey] = item.TesseractOCRConverted,
                [Serialization.Models.Metadata.Rom.TesseractOCRDetectedLangKey] = item.TesseractOCRDetectedLang,
                [Serialization.Models.Metadata.Rom.TesseractOCRDetectedLangConfKey] = item.TesseractOCRDetectedLangConf,
                [Serialization.Models.Metadata.Rom.TesseractOCRDetectedScriptKey] = item.TesseractOCRDetectedScript,
                [Serialization.Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey] = item.TesseractOCRDetectedScriptConf,
                [Serialization.Models.Metadata.Rom.TesseractOCRModuleVersionKey] = item.TesseractOCRModuleVersion,
                [Serialization.Models.Metadata.Rom.TesseractOCRParametersKey] = item.TesseractOCRParameters,
                [Serialization.Models.Metadata.Rom.PDFModuleVersionKey] = item.PDFModuleVersion,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval0To10Key] = item.WordConfidenceInterval0To10,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval11To20Key] = item.WordConfidenceInterval11To20,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval21To30Key] = item.WordConfidenceInterval21To30,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval31To40Key] = item.WordConfidenceInterval31To40,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval41To50Key] = item.WordConfidenceInterval41To50,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval51To60Key] = item.WordConfidenceInterval51To60,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval61To70Key] = item.WordConfidenceInterval61To70,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval71To80Key] = item.WordConfidenceInterval71To80,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval81To90Key] = item.WordConfidenceInterval81To90,
                [Serialization.Models.Metadata.Rom.WordConfidenceInterval91To100Key] = item.WordConfidenceInterval91To100,

                [Serialization.Models.Metadata.Rom.AlbumKey] = item.Album,
                [Serialization.Models.Metadata.Rom.ArtistKey] = item.Artist,
                [Serialization.Models.Metadata.Rom.BitrateKey] = item.Bitrate,
                [Serialization.Models.Metadata.Rom.CreatorKey] = item.Creator,
                [Serialization.Models.Metadata.Rom.HeightKey] = item.Height,
                [Serialization.Models.Metadata.Rom.LengthKey] = item.Length,
                [Serialization.Models.Metadata.Rom.PreviewImageKey] = item.PreviewImage,
                [Serialization.Models.Metadata.Rom.RotationKey] = item.Rotation,
                [Serialization.Models.Metadata.Rom.TitleKey] = item.Title,
                [Serialization.Models.Metadata.Rom.TrackKey] = item.Track,
                [Serialization.Models.Metadata.Rom.WidthKey] = item.Width,
            };
            return rom;
        }
    }
}
