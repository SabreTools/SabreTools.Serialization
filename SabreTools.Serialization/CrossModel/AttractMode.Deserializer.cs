using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.AttractMode;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            var items = new List<Row>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Row = [.. items];
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile
            {
                Header = item.ReadStringArray(Serialization.Models.Metadata.Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to an array of <see cref="Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms == null || roms.Length == 0)
                return [];

            return Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item));
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Rom item, Serialization.Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                Name = parent.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                Title = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Emulator = parent.ReadString(Serialization.Models.Metadata.Machine.EmulatorKey),
                CloneOf = parent.ReadString(Serialization.Models.Metadata.Machine.CloneOfKey),
                Year = parent.ReadString(Serialization.Models.Metadata.Machine.YearKey),
                Manufacturer = parent.ReadString(Serialization.Models.Metadata.Machine.ManufacturerKey),
                Category = parent.ReadString(Serialization.Models.Metadata.Machine.CategoryKey),
                Players = parent.ReadString(Serialization.Models.Metadata.Machine.PlayersKey),
                Rotation = parent.ReadString(Serialization.Models.Metadata.Machine.RotationKey),
                Control = parent.ReadString(Serialization.Models.Metadata.Machine.ControlKey),
                Status = parent.ReadString(Serialization.Models.Metadata.Machine.StatusKey),
                DisplayCount = parent.ReadString(Serialization.Models.Metadata.Machine.DisplayCountKey),
                DisplayType = parent.ReadString(Serialization.Models.Metadata.Machine.DisplayTypeKey),
                AltRomname = item.ReadString(Serialization.Models.Metadata.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Serialization.Models.Metadata.Rom.AltTitleKey),
                Extra = parent.ReadString(Serialization.Models.Metadata.Machine.ExtraKey),
                Buttons = parent.ReadString(Serialization.Models.Metadata.Machine.ButtonsKey),
                Favorite = parent.ReadString(Serialization.Models.Metadata.Machine.FavoriteKey),
                Tags = parent.ReadString(Serialization.Models.Metadata.Machine.TagsKey),
                PlayedCount = parent.ReadString(Serialization.Models.Metadata.Machine.PlayedCountKey),
                PlayedTime = parent.ReadString(Serialization.Models.Metadata.Machine.PlayedTimeKey),
                FileIsAvailable = item.ReadString(Serialization.Models.Metadata.Rom.FileIsAvailableKey),
            };
            return row;
        }
    }
}
