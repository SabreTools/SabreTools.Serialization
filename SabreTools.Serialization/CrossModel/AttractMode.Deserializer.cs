using System;
using System.Collections.Generic;
using SabreTools.Data.Models.AttractMode;

namespace SabreTools.Serialization.CrossModel
{
    public partial class AttractMode : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            var items = new List<Row>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Row = [.. items];
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile
            {
                Header = item.ReadStringArray(Data.Models.Metadata.Header.HeaderKey),
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms == null || roms.Length == 0)
                return [];

            return Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item));
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Rom item, Data.Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                Name = parent.ReadString(Data.Models.Metadata.Machine.NameKey),
                Title = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Emulator = parent.ReadString(Data.Models.Metadata.Machine.EmulatorKey),
                CloneOf = parent.ReadString(Data.Models.Metadata.Machine.CloneOfKey),
                Year = parent.ReadString(Data.Models.Metadata.Machine.YearKey),
                Manufacturer = parent.ReadString(Data.Models.Metadata.Machine.ManufacturerKey),
                Category = parent.ReadString(Data.Models.Metadata.Machine.CategoryKey),
                Players = parent.ReadString(Data.Models.Metadata.Machine.PlayersKey),
                Rotation = parent.ReadString(Data.Models.Metadata.Machine.RotationKey),
                Control = parent.ReadString(Data.Models.Metadata.Machine.ControlKey),
                Status = parent.ReadString(Data.Models.Metadata.Machine.StatusKey),
                DisplayCount = parent.ReadString(Data.Models.Metadata.Machine.DisplayCountKey),
                DisplayType = parent.ReadString(Data.Models.Metadata.Machine.DisplayTypeKey),
                AltRomname = item.ReadString(Data.Models.Metadata.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Data.Models.Metadata.Rom.AltTitleKey),
                Extra = parent.ReadString(Data.Models.Metadata.Machine.ExtraKey),
                Buttons = parent.ReadString(Data.Models.Metadata.Machine.ButtonsKey),
                Favorite = parent.ReadString(Data.Models.Metadata.Machine.FavoriteKey),
                Tags = parent.ReadString(Data.Models.Metadata.Machine.TagsKey),
                PlayedCount = parent.ReadString(Data.Models.Metadata.Machine.PlayedCountKey),
                PlayedTime = parent.ReadString(Data.Models.Metadata.Machine.PlayedTimeKey),
                FileIsAvailable = item.ReadString(Data.Models.Metadata.Rom.FileIsAvailableKey),
            };
            return row;
        }
    }
}
