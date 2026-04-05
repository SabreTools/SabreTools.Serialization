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
            if (obj is null)
                return null;

            var header = obj.Header;
            var metadataFile = header is not null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Machine;
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
                Header = item.HeaderRow,
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var roms = item.Rom;
            if (roms is null || roms.Length == 0)
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
                Name = parent.Name,
                Title = item.Name,
                Emulator = parent.Emulator,
                CloneOf = parent.CloneOf,
                Year = parent.Year,
                Manufacturer = parent.Manufacturer,
                Category = parent.Category is null ? null : string.Join(", ", parent.Category),
                Players = parent.Players,
                Rotation = parent.Rotation,
                Control = parent.Control,
                Status = parent.Status,
                DisplayCount = parent.DisplayCount,
                DisplayType = parent.DisplayType,
                AltRomname = item.ReadString(Data.Models.Metadata.Rom.AltRomnameKey),
                AltTitle = item.ReadString(Data.Models.Metadata.Rom.AltTitleKey),
                Extra = parent.Extra,
                Buttons = parent.Buttons,
                Favorite = parent.Favorite,
                Tags = parent.Tags,
                PlayedCount = parent.PlayedCount,
                PlayedTime = parent.PlayedTime,
                FileIsAvailable = item.FileIsAvailable,
            };
            return row;
        }
    }
}
