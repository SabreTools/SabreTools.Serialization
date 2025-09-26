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
            if (obj == null)
                return null;

            var files = new Files();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
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
            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms == null)
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
                Name = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Source = item.ReadString(Data.Models.Metadata.Rom.SourceKey),
                BitTorrentMagnetHash = item.ReadString(Data.Models.Metadata.Rom.BitTorrentMagnetHashKey),
                LastModifiedTime = item.ReadString(Data.Models.Metadata.Rom.LastModifiedTimeKey),
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                MD5 = item.ReadString(Data.Models.Metadata.Rom.MD5Key),
                CRC32 = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                FileCount = item.ReadString(Data.Models.Metadata.Rom.FileCountKey),
                Format = item.ReadString(Data.Models.Metadata.Rom.FormatKey),
                Original = item.ReadString(Data.Models.Metadata.Rom.OriginalKey),
                Summation = item.ReadString(Data.Models.Metadata.Rom.SummationKey),
                MatrixNumber = item.ReadString(Data.Models.Metadata.Rom.MatrixNumberKey),
                CollectionCatalogNumber = item.ReadString(Data.Models.Metadata.Rom.CollectionCatalogNumberKey),
                Publisher = item.ReadString(Data.Models.Metadata.Rom.PublisherKey),
                Comment = item.ReadString(Data.Models.Metadata.Rom.CommentKey),

                ASRDetectedLang = item.ReadString(Data.Models.Metadata.Rom.ASRDetectedLangKey),
                ASRDetectedLangConf = item.ReadString(Data.Models.Metadata.Rom.ASRDetectedLangConfKey),
                ASRTranscribedLang = item.ReadString(Data.Models.Metadata.Rom.ASRTranscribedLangKey),
                WhisperASRModuleVersion = item.ReadString(Data.Models.Metadata.Rom.WhisperASRModuleVersionKey),
                WhisperModelHash = item.ReadString(Data.Models.Metadata.Rom.WhisperModelHashKey),
                WhisperModelName = item.ReadString(Data.Models.Metadata.Rom.WhisperModelNameKey),
                WhisperVersion = item.ReadString(Data.Models.Metadata.Rom.WhisperVersionKey),

                ClothCoverDetectionModuleVersion = item.ReadString(Data.Models.Metadata.Rom.ClothCoverDetectionModuleVersionKey),
                hOCRCharToWordhOCRVersion = item.ReadString(Data.Models.Metadata.Rom.hOCRCharToWordhOCRVersionKey),
                hOCRCharToWordModuleVersion = item.ReadString(Data.Models.Metadata.Rom.hOCRCharToWordModuleVersionKey),
                hOCRFtsTexthOCRVersion = item.ReadString(Data.Models.Metadata.Rom.hOCRFtsTexthOCRVersionKey),
                hOCRFtsTextModuleVersion = item.ReadString(Data.Models.Metadata.Rom.hOCRFtsTextModuleVersionKey),
                hOCRPageIndexhOCRVersion = item.ReadString(Data.Models.Metadata.Rom.hOCRPageIndexhOCRVersionKey),
                hOCRPageIndexModuleVersion = item.ReadString(Data.Models.Metadata.Rom.hOCRPageIndexModuleVersionKey),
                TesseractOCR = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRKey),
                TesseractOCRConverted = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRConvertedKey),
                TesseractOCRDetectedLang = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedLangKey),
                TesseractOCRDetectedLangConf = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedLangConfKey),
                TesseractOCRDetectedScript = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedScriptKey),
                TesseractOCRDetectedScriptConf = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRDetectedScriptConfKey),
                TesseractOCRModuleVersion = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRModuleVersionKey),
                TesseractOCRParameters = item.ReadString(Data.Models.Metadata.Rom.TesseractOCRParametersKey),
                PDFModuleVersion = item.ReadString(Data.Models.Metadata.Rom.PDFModuleVersionKey),
                WordConfidenceInterval0To10 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval0To10Key),
                WordConfidenceInterval11To20 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval11To20Key),
                WordConfidenceInterval21To30 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval21To30Key),
                WordConfidenceInterval31To40 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval31To40Key),
                WordConfidenceInterval41To50 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval41To50Key),
                WordConfidenceInterval51To60 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval51To60Key),
                WordConfidenceInterval61To70 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval61To70Key),
                WordConfidenceInterval71To80 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval71To80Key),
                WordConfidenceInterval81To90 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval81To90Key),
                WordConfidenceInterval91To100 = item.ReadString(Data.Models.Metadata.Rom.WordConfidenceInterval91To100Key),

                Album = item.ReadString(Data.Models.Metadata.Rom.AlbumKey),
                Artist = item.ReadString(Data.Models.Metadata.Rom.ArtistKey),
                Bitrate = item.ReadString(Data.Models.Metadata.Rom.BitrateKey),
                Creator = item.ReadString(Data.Models.Metadata.Rom.CreatorKey),
                Height = item.ReadString(Data.Models.Metadata.Rom.HeightKey),
                Length = item.ReadString(Data.Models.Metadata.Rom.LengthKey),
                PreviewImage = item.ReadString(Data.Models.Metadata.Rom.PreviewImageKey),
                Rotation = item.ReadString(Data.Models.Metadata.Rom.RotationKey),
                Title = item.ReadString(Data.Models.Metadata.Rom.TitleKey),
                Track = item.ReadString(Data.Models.Metadata.Rom.TrackKey),
                Width = item.ReadString(Data.Models.Metadata.Rom.WidthKey),
            };
            return file;
        }
    }
}
