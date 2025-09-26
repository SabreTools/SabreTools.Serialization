using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.AttractMode;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.HeaderKey] = item.Header,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine? ConvertMachineToInternalModel(Row? item)
        {
            if (item == null)
                return null;

            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.Name,
                [Serialization.Models.Metadata.Machine.EmulatorKey] = item.Emulator,
                [Serialization.Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Serialization.Models.Metadata.Machine.YearKey] = item.Year,
                [Serialization.Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Serialization.Models.Metadata.Machine.CategoryKey] = item.Category,
                [Serialization.Models.Metadata.Machine.PlayersKey] = item.Players,
                [Serialization.Models.Metadata.Machine.RotationKey] = item.Rotation,
                [Serialization.Models.Metadata.Machine.ControlKey] = item.Control,
                [Serialization.Models.Metadata.Machine.StatusKey] = item.Status,
                [Serialization.Models.Metadata.Machine.DisplayCountKey] = item.DisplayCount,
                [Serialization.Models.Metadata.Machine.DisplayTypeKey] = item.DisplayType,
                [Serialization.Models.Metadata.Machine.ExtraKey] = item.Extra,
                [Serialization.Models.Metadata.Machine.ButtonsKey] = item.Buttons,
                [Serialization.Models.Metadata.Machine.FavoriteKey] = item.Favorite,
                [Serialization.Models.Metadata.Machine.TagsKey] = item.Tags,
                [Serialization.Models.Metadata.Machine.PlayedCountKey] = item.PlayedCount,
                [Serialization.Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Serialization.Models.Metadata.Machine.PlayedTimeKey] = item.PlayedTime,
                [Serialization.Models.Metadata.Machine.RomKey] = new Serialization.Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };
            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.Title,
                [Serialization.Models.Metadata.Rom.AltRomnameKey] = item.AltRomname,
                [Serialization.Models.Metadata.Rom.AltTitleKey] = item.AltTitle,
                [Serialization.Models.Metadata.Rom.FileIsAvailableKey] = item.FileIsAvailable,
            };
            return rom;
        }
    }
}
