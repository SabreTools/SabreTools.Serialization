using System;
using System.Linq;
using SabreTools.Models.AttractMode;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Row = machines
                    .Where(m => m != null)
                    .SelectMany(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.AttractMode.MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile
            {
                Header = item.ReadStringArray(Models.Metadata.Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to an array of <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null || !roms.Any())
                return Array.Empty<Row>();

            return roms
                .Where(r => r != null)
                .Select(rom => ConvertFromInternalModel(rom, item))
                .ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                Name = parent.ReadString(Models.Metadata.Machine.NameKey),
                Title = item.ReadString(Models.Metadata.Rom.NameKey),
                Emulator = parent.ReadString(Models.Metadata.Machine.EmulatorKey),
                CloneOf = parent.ReadString(Models.Metadata.Machine.CloneOfKey),
                Year = parent.ReadString(Models.Metadata.Machine.YearKey),
                Manufacturer = parent.ReadString(Models.Metadata.Machine.ManufacturerKey),
                Category = parent.ReadString(Models.Metadata.Machine.CategoryKey),
                Players = parent.ReadString(Models.Metadata.Machine.PlayersKey),
                Rotation = parent.ReadString(Models.Metadata.Machine.RotationKey),
                Control = parent.ReadString(Models.Metadata.Machine.ControlKey),
                Status = parent.ReadString(Models.Metadata.Machine.StatusKey),
                DisplayCount = parent.ReadString(Models.Metadata.Machine.DisplayCountKey),
                DisplayType = parent.ReadString(Models.Metadata.Machine.DisplayTypeKey),
                AltRomname = item.ReadString(Models.Metadata.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Models.Metadata.Rom.AltTitleKey),
                Extra = parent.ReadString(Models.Metadata.Machine.ExtraKey),
                Buttons = parent.ReadString(Models.Metadata.Machine.ButtonsKey),
                Favorite = parent.ReadString(Models.Metadata.Machine.FavoriteKey),
                Tags = parent.ReadString(Models.Metadata.Machine.TagsKey),
                PlayedCount = parent.ReadString(Models.Metadata.Machine.PlayedCountKey),
                PlayedTime = parent.ReadString(Models.Metadata.Machine.PlayedTimeKey),
                FileIsAvailable = item.ReadString(Models.Metadata.Rom.FileIsAvailableKey),
            };
            return row;
        }
    }
}