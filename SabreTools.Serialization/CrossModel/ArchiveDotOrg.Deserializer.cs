using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : IModelSerializer<Models.ArchiveDotOrg.Files, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.ArchiveDotOrg.Files? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var files = new Models.ArchiveDotOrg.Files();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            var items = new List<Models.ArchiveDotOrg.File>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertFromInternalModel(machine));
            }

            files.File = [.. items];
            return files;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to an array of <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        private static Models.ArchiveDotOrg.File[] ConvertFromInternalModel(Models.Metadata.Machine item)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null)
                return [];

            return Array.ConvertAll(roms, ConvertFromInternalModel);
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.ArchiveDotOrg.File"/>
        /// </summary>
        private static Models.ArchiveDotOrg.File ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var file = new Models.ArchiveDotOrg.File
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Source = item.ReadString(Models.Metadata.Rom.SourceKey),
                BitTorrentMagnetHash = item.ReadString(Models.Metadata.Rom.BitTorrentMagnetHashKey),
                LastModifiedTime = item.ReadString(Models.Metadata.Rom.LastModifiedTimeKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                MD5 = item.ReadString(Models.Metadata.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                FileCount = item.ReadString(Models.Metadata.Rom.FileCountKey),
                Format = item.ReadString(Models.Metadata.Rom.FormatKey),
                Original = item.ReadString(Models.Metadata.Rom.OriginalKey),
                Summation = item.ReadString(Models.Metadata.Rom.SummationKey),
                MatrixNumber = item.ReadString(Models.Metadata.Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.ReadString(Models.Metadata.Rom.CollectionCatalogNumberKey),
                Publisher = item.ReadString(Models.Metadata.Rom.PublisherKey),
                Comment = item.ReadString(Models.Metadata.Rom.CommentKey),

                ASRDetectedLang = item.ReadString(Models.Metadata.Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.ReadString(Models.Metadata.Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.ReadString(Models.Metadata.Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.ReadString(Models.Metadata.Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.ReadString(Models.Metadata.Rom.WhisperModelHashKey),
                WhisperModelName = item.ReadString(Models.Metadata.Rom.WhisperModelNameKey),
                WhisperVersion = item.ReadString(Models.Metadata.Rom.WhisperVersionKey),

                ClothCoverDetectionModuleVersion = item.ReadString(Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.ReadString(Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.ReadString(Models.Metadata.Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.ReadString(Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.ReadString(Models.Metadata.Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.ReadString(Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.ReadString(Models.Metadata.Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.ReadString(Models.Metadata.Rom.TesseractOCRKey),
                TesseractOCRConverted = item.ReadString(Models.Metadata.Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.ReadString(Models.Metadata.Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.ReadString(Models.Metadata.Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.ReadString(Models.Metadata.Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.ReadString(Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRModuleVersion = item.ReadString(Models.Metadata.Rom.TesseractOCRModuleVersionKey),
                TesseractOCRParameters = item.ReadString(Models.Metadata.Rom.TesseractOCRParametersKey),
                PDFModuleVersion = item.ReadString(Models.Metadata.Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.ReadString(Models.Metadata.Rom.WordConfidenceInterval91To100Key),

                Album = item.ReadString(Models.Metadata.Rom.AlbumKey),
                Artist = item.ReadString(Models.Metadata.Rom.ArtistKey),
                Bitrate = item.ReadString(Models.Metadata.Rom.BitrateKey),
                Creator = item.ReadString(Models.Metadata.Rom.CreatorKey),
                Height = item.ReadString(Models.Metadata.Rom.HeightKey),
                Length = item.ReadString(Models.Metadata.Rom.LengthKey),
                PreviewImage = item.ReadString(Models.Metadata.Rom.PreviewImageKey),
                Rotation = item.ReadString(Models.Metadata.Rom.RotationKey),
                Title = item.ReadString(Models.Metadata.Rom.TitleKey),
                Track = item.ReadString(Models.Metadata.Rom.TrackKey),
                Width = item.ReadString(Models.Metadata.Rom.WidthKey),
            };
            return file;
        }
    }
}