using System;
using System.Collections.Generic;
using SabreTools.Data.Models.RomCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : ICrossModel<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
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

            if (item.ContainsKey(Data.Models.Metadata.Header.AuthorKey)
                || item.ContainsKey(Data.Models.Metadata.Header.VersionKey)
                || item.ContainsKey(Data.Models.Metadata.Header.EmailKey)
                || item.ContainsKey(Data.Models.Metadata.Header.HomepageKey)
                || item.ContainsKey(Data.Models.Metadata.Header.UrlKey)
                || item.ContainsKey(Data.Models.Metadata.Header.DateKey)
                || item.ContainsKey(Data.Models.Metadata.Header.CommentKey))
            {
                metadataFile.Credits = new Credits
                {
                    Author = item.ReadString(Data.Models.Metadata.Header.AuthorKey),
                    Version = item.ReadString(Data.Models.Metadata.Header.VersionKey),
                    Email = item.ReadString(Data.Models.Metadata.Header.EmailKey),
                    Homepage = item.ReadString(Data.Models.Metadata.Header.HomepageKey),
                    Url = item.ReadString(Data.Models.Metadata.Header.UrlKey),
                    Date = item.ReadString(Data.Models.Metadata.Header.DateKey),
                    Comment = item.ReadString(Data.Models.Metadata.Header.CommentKey),
                };
            }

            if (item.ContainsKey(Data.Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Data.Models.Metadata.Header.PluginKey)
                || item.ContainsKey(Data.Models.Metadata.Header.ForceMergingKey))
            {
                metadataFile.Dat = new Dat
                {
                    Version = item.ReadString(Data.Models.Metadata.Header.DatVersionKey),
                    Plugin = item.ReadString(Data.Models.Metadata.Header.PluginKey),
                    Split = item.ReadString(Data.Models.Metadata.Header.ForceMergingKey) == "split" ? "yes" : "no",
                    Merge = item.ReadString(Data.Models.Metadata.Header.ForceMergingKey) == "merge" ? "yes" : "no",
                };
            }

            if (item.ContainsKey(Data.Models.Metadata.Header.RefNameKey)
                || item.ContainsKey(Data.Models.Metadata.Header.EmulatorVersionKey))
            {
                metadataFile.Emulator = new Emulator
                {
                    RefName = item.ReadString(Data.Models.Metadata.Header.RefNameKey),
                    Version = item.ReadString(Data.Models.Metadata.Header.EmulatorVersionKey),
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Rom"/>
        /// </summary>
        private static Rom[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms == null)
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
                ParentName = parent.ReadString(Data.Models.Metadata.Machine.CloneOfKey),
                //ParentDescription = parent.ReadString(Data.Models.Metadata.Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent.ReadString(Data.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Data.Models.Metadata.Machine.DescriptionKey),
                RomName = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                RomCRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                RomSize = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                RomOf = parent.ReadString(Data.Models.Metadata.Machine.RomOfKey),
                MergeName = item.ReadString(Data.Models.Metadata.Rom.MergeKey),
            };
            return row;
        }
    }
}
