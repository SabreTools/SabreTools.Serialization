using System;
using System.Collections.Generic;
using SabreTools.Data.Models.RomCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var header = obj.Header;
            var metadataFile = header is not null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Machine;
            var items = new List<Rom>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Games = new Games { Rom = [.. items] };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile();

            if (item.Author != null
                || item.Version != null
                || item.Email != null
                || item.Homepage != null
                || item.Url != null
                || item.Date != null
                || item.Comment != null)
            {
                metadataFile.Credits = new Credits
                {
                    Author = item.Author,
                    Version = item.Version,
                    Email = item.Email,
                    Homepage = item.Homepage,
                    Url = item.Url,
                    Date = item.Date,
                    Comment = item.Comment,
                };
            }

            if (item.DatVersion != null
                || item.Plugin != null
                || item.ForceMerging != Data.Models.Metadata.MergingFlag.None)
            {
                metadataFile.Dat = new Dat
                {
                    Version = item.DatVersion,
                    Plugin = item.Plugin,
                    Split = item.ForceMerging == Data.Models.Metadata.MergingFlag.Split ? "yes" : "no",
                    Merge = item.ForceMerging == Data.Models.Metadata.MergingFlag.Merged ? "yes" : "no",
                };
            }

            if (item.RefName != null || item.EmulatorVersion != null)
            {
                metadataFile.Emulator = new Emulator
                {
                    RefName = item.RefName,
                    Version = item.EmulatorVersion,
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Rom"/>
        /// </summary>
        private static Rom[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var roms = item.Rom;
            if (roms is null)
                return [];

            return Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item));
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Data.Models.Metadata.Rom item, Data.Models.Metadata.Machine parent)
        {
            var row = new Rom
            {
                ParentName = parent.CloneOf,
                //ParentDescription = parent.ReadString(Data.Models.Metadata.Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent.Name,
                GameDescription = parent.Description,
                RomName = item.Name,
                RomCRC = item.CRC,
                RomSize = item.Size,
                RomOf = parent.RomOf,
                MergeName = item.Merge,
            };
            return row;
        }
    }
}
