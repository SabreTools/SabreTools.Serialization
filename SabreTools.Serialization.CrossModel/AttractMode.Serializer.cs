using System;
using SabreTools.Data.Models.AttractMode;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                Header = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row is not null && obj.Row.Length > 0)
                metadataFile.Machine = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Data.Models.Metadata.Header
            {
                HeaderRow = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                Name = item.Name,
                Emulator = item.Emulator,
                CloneOf = item.CloneOf,
                Year = item.Year,
                Manufacturer = item.Manufacturer,
                Category = item.Category is null ? null : [item.Category],
                Players = item.Players,
                Rotation = item.Rotation,
                Control = item.Control,
                Status = item.Status,
                DisplayCount = item.DisplayCount,
                DisplayType = item.DisplayType,
                Extra = item.Extra,
                Buttons = item.Buttons,
                Favorite = item.Favorite,
                Tags = item.Tags,
                PlayedCount = item.PlayedCount,
                PlayedTime = item.PlayedTime,
                Rom = [ConvertToInternalModel(item)],
            };
            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                Name = item.Title,
                [Data.Models.Metadata.Rom.AltRomnameKey] = item.AltRomname,
                [Data.Models.Metadata.Rom.AltTitleKey] = item.AltTitle,
                FileIsAvailable = item.FileIsAvailable,
            };
            return rom;
        }
    }
}
