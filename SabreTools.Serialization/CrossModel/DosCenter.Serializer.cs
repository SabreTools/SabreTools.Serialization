using System;
using SabreTools.Data.Models.DosCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class DosCenter : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile();

            if (obj?.DosCenter != null)
                metadataFile[Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.DosCenter);

            if (obj?.Game != null && obj.Game.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.DosCenter.DosCenter"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Data.Models.DosCenter.DosCenter item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = item.Name,
                [Data.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Data.Models.Metadata.Header.VersionKey] = item.Version,
                [Data.Models.Metadata.Header.DateKey] = item.Date,
                [Data.Models.Metadata.Header.AuthorKey] = item.Author,
                [Data.Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Data.Models.Metadata.Header.CommentKey] = item.Comment,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Game"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.NameKey] = item.Name,
            };

            if (item.File != null && item.File.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.File, ConvertToInternalModel);
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(File item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.NameKey] = item.Name,
                [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.DateKey] = item.Date,
            };
            return rom;
        }
    }
}
