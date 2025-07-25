using System;
using System.Collections.Generic;
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
            var items = new List<Row>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Row = [.. items];
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.AttractMode.MetadataFile"/>
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
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Models.AttractMode.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null || roms.Length == 0)
                return [];

            return Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item));
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Models.AttractMode.Row"/>
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
