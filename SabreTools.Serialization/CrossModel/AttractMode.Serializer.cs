using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.AttractMode;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.HeaderKey] = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine? ConvertMachineToInternalModel(Row? item)
        {
            if (item == null)
                return null;

            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
                [Models.Metadata.Machine.EmulatorKey] = item.Emulator,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Metadata.Machine.CategoryKey] = item.Category,
                [Models.Metadata.Machine.PlayersKey] = item.Players,
                [Models.Metadata.Machine.RotationKey] = item.Rotation,
                [Models.Metadata.Machine.ControlKey] = item.Control,
                [Models.Metadata.Machine.StatusKey] = item.Status,
                [Models.Metadata.Machine.DisplayCountKey] = item.DisplayCount,
                [Models.Metadata.Machine.DisplayTypeKey] = item.DisplayType,
                [Models.Metadata.Machine.ExtraKey] = item.Extra,
                [Models.Metadata.Machine.ButtonsKey] = item.Buttons,
                [Models.Metadata.Machine.FavoriteKey] = item.Favorite,
                [Models.Metadata.Machine.TagsKey] = item.Tags,
                [Models.Metadata.Machine.PlayedCountKey] = item.PlayedCount,
                [Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Models.Metadata.Machine.RomKey] = new Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };
            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Title,
                [Models.Metadata.Rom.AltRomnameKey] = item.AltRomname,
                [Models.Metadata.Rom.AltTitleKey] = item.AltTitle,
                [Models.Metadata.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }
    }
}
