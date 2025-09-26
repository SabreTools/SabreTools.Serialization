using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.ArchiveDotOrg;

namespace SabreTools.Serialization.CrossModel
{
    public partial class ArchiveDotOrg : IModelSerializer<Files, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Files? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var files = new Files();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            var items = new List<File>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertFromInternalModel(machine));
            }

            files.File = [.. items];
            return files;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to an array of <see cref="File"/>
        /// </summary>
        private static File[] ConvertFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms == null)
                return [];

            return Array.ConvertAll(roms, ConvertFromInternalModel);
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="File"/>
        /// </summary>
        private static File ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var file = new File
            {
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Source = item.ReadString(Serialization.Models.Metadata.Rom.SourceKey),
                BitTorrentMagnetHash = item.ReadString(Serialization.Models.Metadata.Rom.BitTorrentMagnetHashKey),
                LastModifiedTime = item.ReadString(Serialization.Models.Metadata.Rom.LastModifiedTimeKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Rom.MD5Key),
                CRC32 = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                FileCount = item.ReadString(Serialization.Models.Metadata.Rom.FileCountKey),
                Format = item.ReadString(Serialization.Models.Metadata.Rom.FormatKey),
                Original = item.ReadString(Serialization.Models.Metadata.Rom.OriginalKey),
                Summation = item.ReadString(Serialization.Models.Metadata.Rom.SummationKey),
                MatrixNumber = item.ReadString(Serialization.Models.Metadata.Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.ReadString(Serialization.Models.Metadata.Rom.CollectionCatalogNumberKey),
                Publisher = item.ReadString(Serialization.Models.Metadata.Rom.PublisherKey),
                Comment = item.ReadString(Serialization.Models.Metadata.Rom.CommentKey),

                ASRDetectedLang = item.ReadString(Serialization.Models.Metadata.Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.ReadString(Serialization.Models.Metadata.Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.ReadString(Serialization.Models.Metadata.Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.ReadString(Serialization.Models.Metadata.Rom.WhisperModelHashKey),
                WhisperModelName = item.ReadString(Serialization.Models.Metadata.Rom.WhisperModelNameKey),
                WhisperVersion = item.ReadString(Serialization.Models.Metadata.Rom.WhisperVersionKey),

                ClothCoverDetectionModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.ReadString(Serialization.Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.ReadString(Serialization.Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.ReadString(Serialization.Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRKey),
                TesseractOCRConverted = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRModuleVersionKey),
                TesseractOCRParameters = item.ReadString(Serialization.Models.Metadata.Rom.TesseractOCRParametersKey),
                PDFModuleVersion = item.ReadString(Serialization.Models.Metadata.Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.ReadString(Serialization.Models.Metadata.Rom.WordConfidenceInterval91To100Key),

                Album = item.ReadString(Serialization.Models.Metadata.Rom.AlbumKey),
                Artist = item.ReadString(Serialization.Models.Metadata.Rom.ArtistKey),
                Bitrate = item.ReadString(Serialization.Models.Metadata.Rom.BitrateKey),
                Creator = item.ReadString(Serialization.Models.Metadata.Rom.CreatorKey),
                Height = item.ReadString(Serialization.Models.Metadata.Rom.HeightKey),
                Length = item.ReadString(Serialization.Models.Metadata.Rom.LengthKey),
                PreviewImage = item.ReadString(Serialization.Models.Metadata.Rom.PreviewImageKey),
                Rotation = item.ReadString(Serialization.Models.Metadata.Rom.RotationKey),
                Title = item.ReadString(Serialization.Models.Metadata.Rom.TitleKey),
                Track = item.ReadString(Serialization.Models.Metadata.Rom.TrackKey),
                Width = item.ReadString(Serialization.Models.Metadata.Rom.WidthKey),
            };
            return file;
        }
    }
}
