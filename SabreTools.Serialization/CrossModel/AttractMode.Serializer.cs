using System;
using SabreTools.Data.Models.AttractMode;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.HeaderKey] = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine? ConvertMachineToInternalModel(Row? item)
        {
            if (item == null)
                return null;

            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.NameKey] = item.Name,
                [Data.Models.Metadata.Machine.EmulatorKey] = item.Emulator,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Data.Models.Metadata.Machine.YearKey] = item.Year,
                [Data.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Data.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Data.Models.Metadata.Machine.PlayersKey] = item.Players,
                [Data.Models.Metadata.Machine.RotationKey] = item.Rotation,
                [Data.Models.Metadata.Machine.ControlKey] = item.Control,
                [Data.Models.Metadata.Machine.StatusKey] = item.Status,
                [Data.Models.Metadata.Machine.DisplayCountKey] = item.DisplayCount,
                [Data.Models.Metadata.Machine.DisplayTypeKey] = item.DisplayType,
                [Data.Models.Metadata.Machine.ExtraKey] = item.Extra,
                [Data.Models.Metadata.Machine.ButtonsKey] = item.Buttons,
                [Data.Models.Metadata.Machine.FavoriteKey] = item.Favorite,
                [Data.Models.Metadata.Machine.TagsKey] = item.Tags,
                [Data.Models.Metadata.Machine.PlayedCountKey] = item.PlayedCount,
                [Data.Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { ConvertToInternalModel(item) },
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
                [Data.Models.Metadata.Rom.NameKey] = item.Title,
                [Data.Models.Metadata.Rom.AltRomnameKey] = item.AltRomname,
                [Data.Models.Metadata.Rom.AltTitleKey] = item.AltTitle,
                [Data.Models.Metadata.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }
    }
}
