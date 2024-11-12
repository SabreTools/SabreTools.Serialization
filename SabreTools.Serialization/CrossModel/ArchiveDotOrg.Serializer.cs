using System.Linq;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : IModelSerializer<Models.ArchiveDotOrg.Files, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(Models.ArchiveDotOrg.Files? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.File != null && item.File.Length > 0)
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.File
                    .Where(f => f != null)
                    .Select(ConvertMachineToInternalModel)
                    .Where(m => m != null)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.Files"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.ArchiveDotOrg.Files item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "archive.org",
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Models.ArchiveDotOrg.File? item)
        {
            var machine = new Models.Metadata.Machine();

            var rom = ConvertToInternalModel(item);
            if (rom != null)
                machine[Models.Metadata.Machine.RomKey] = new Models.Metadata.Rom[] { rom };

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.ArchiveDotOrg.File"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom? ConvertToInternalModel(Models.ArchiveDotOrg.File? item)
        {
            if (item == null)
                return null;

            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SourceKey] = item.Source,
                [Models.Metadata.Rom.BitTorrentMagnetHashKey] = item.BitTorrentMagnetHash,
                [Models.Metadata.Rom.LastModifiedTimeKey] = item.LastModifiedTime,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.MD5Key] = item.MD5,
                [Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.FileCountKey] = item.FileCount,
                [Models.Metadata.Rom.FormatKey] = item.Format,
                [Models.Metadata.Rom.OriginalKey] = item.Original,
                [Models.Metadata.Rom.SummationKey] = item.Summation,
                [Models.Metadata.Rom.MatrixNumberKey] = item.MatrixNumber,
                [Models.Metadata.Rom.CollectionCatalogNumberKey] = item.CollectionCatalogNumber,
                [Models.Metadata.Rom.PublisherKey] = item.Publisher,
                [Models.Metadata.Rom.CommentKey] = item.Comment,

                [Models.Metadata.Rom.ASRDetectedLangKey] = item.ASRDetectedLang,
                [Models.Metadata.Rom.ASRDetectedLangConfKey] = item.ASRDetectedLangConf,
                [Models.Metadata.Rom.ASRTranscribedLangKey] = item.ASRTranscribedLang,
                [Models.Metadata.Rom.WhisperASRModuleVersionKey] = item.WhisperASRModuleVersion,
                [Models.Metadata.Rom.WhisperModelHashKey] = item.WhisperModelHash,
                [Models.Metadata.Rom.WhisperModelNameKey] = item.WhisperModelName,
                [Models.Metadata.Rom.WhisperVersionKey] = item.WhisperVersion,

                [Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey] = item.ClothCoverDetectionModuleVersion,
                [Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey] = item.hOCRCharToWordhOCRVersion,
                [Models.Metadata.Rom.hOCRCharToWordModuleVersionKey] = item.hOCRCharToWordModuleVersion,
                [Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey] = item.hOCRFtsTexthOCRVersion,
                [Models.Metadata.Rom.hOCRFtsTextModuleVersionKey] = item.hOCRFtsTextModuleVersion,
                [Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey] = item.hOCRPageIndexhOCRVersion,
                [Models.Metadata.Rom.hOCRPageIndexModuleVersionKey] = item.hOCRPageIndexModuleVersion,
                [Models.Metadata.Rom.TesseractOCRKey] = item.TesseractOCR,
                [Models.Metadata.Rom.TesseractOCRConvertedKey] = item.TesseractOCRConverted,
                [Models.Metadata.Rom.TesseractOCRDetectedLangKey] = item.TesseractOCRDetectedLang,
                [Models.Metadata.Rom.TesseractOCRDetectedLangConfKey] = item.TesseractOCRDetectedLangConf,
                [Models.Metadata.Rom.TesseractOCRDetectedScriptKey] = item.TesseractOCRDetectedScript,
                [Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey] = item.TesseractOCRDetectedScriptConf,
                [Models.Metadata.Rom.TesseractOCRModuleVersionKey] = item.TesseractOCRModuleVersion,
                [Models.Metadata.Rom.TesseractOCRParametersKey] = item.TesseractOCRParameters,
                [Models.Metadata.Rom.PDFModuleVersionKey] = item.PDFModuleVersion,
                [Models.Metadata.Rom.WordConfidenceInterval0To10Key] = item.WordConfidenceInterval0To10,
                [Models.Metadata.Rom.WordConfidenceInterval11To20Key] = item.WordConfidenceInterval11To20,
                [Models.Metadata.Rom.WordConfidenceInterval21To30Key] = item.WordConfidenceInterval21To30,
                [Models.Metadata.Rom.WordConfidenceInterval31To40Key] = item.WordConfidenceInterval31To40,
                [Models.Metadata.Rom.WordConfidenceInterval41To50Key] = item.WordConfidenceInterval41To50,
                [Models.Metadata.Rom.WordConfidenceInterval51To60Key] = item.WordConfidenceInterval51To60,
                [Models.Metadata.Rom.WordConfidenceInterval61To70Key] = item.WordConfidenceInterval61To70,
                [Models.Metadata.Rom.WordConfidenceInterval71To80Key] = item.WordConfidenceInterval71To80,
                [Models.Metadata.Rom.WordConfidenceInterval81To90Key] = item.WordConfidenceInterval81To90,
                [Models.Metadata.Rom.WordConfidenceInterval91To100Key] = item.WordConfidenceInterval91To100,

                [Models.Metadata.Rom.AlbumKey] = item.Album,
                [Models.Metadata.Rom.ArtistKey] = item.Artist,
                [Models.Metadata.Rom.BitrateKey] = item.Bitrate,
                [Models.Metadata.Rom.CreatorKey] = item.Creator,
                [Models.Metadata.Rom.HeightKey] = item.Height,
                [Models.Metadata.Rom.LengthKey] = item.Length,
                [Models.Metadata.Rom.PreviewImageKey] = item.PreviewImage,
                [Models.Metadata.Rom.RotationKey] = item.Rotation,
                [Models.Metadata.Rom.TitleKey] = item.Title,
                [Models.Metadata.Rom.TrackKey] = item.Track,
                [Models.Metadata.Rom.WidthKey] = item.Width,
            };
            return rom;
        }
    }
}