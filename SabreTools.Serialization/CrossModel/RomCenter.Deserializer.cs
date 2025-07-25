using System;
using System.Collections.Generic;
using SabreTools.Models.RomCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            var items = new List<Rom>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Games = new Games { Rom = [.. items] };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.RomCenter.MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile();

            if (item.ContainsKey(Models.Metadata.Header.AuthorKey)
                || item.ContainsKey(Models.Metadata.Header.VersionKey)
                || item.ContainsKey(Models.Metadata.Header.EmailKey)
                || item.ContainsKey(Models.Metadata.Header.HomepageKey)
                || item.ContainsKey(Models.Metadata.Header.UrlKey)
                || item.ContainsKey(Models.Metadata.Header.DateKey)
                || item.ContainsKey(Models.Metadata.Header.CommentKey))
            {
                metadataFile.Credits = new Credits
                {
                    Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                    Version = item.ReadString(Models.Metadata.Header.VersionKey),
                    Email = item.ReadString(Models.Metadata.Header.EmailKey),
                    Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                    Url = item.ReadString(Models.Metadata.Header.UrlKey),
                    Date = item.ReadString(Models.Metadata.Header.DateKey),
                    Comment = item.ReadString(Models.Metadata.Header.CommentKey),
                };
            }

            if (item.ContainsKey(Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Models.Metadata.Header.PluginKey)
                || item.ContainsKey(Models.Metadata.Header.ForceMergingKey))
            {
                metadataFile.Dat = new Dat
                {
                    Version = item.ReadString(Models.Metadata.Header.DatVersionKey),
                    Plugin = item.ReadString(Models.Metadata.Header.PluginKey),
                    Split = item.ReadString(Models.Metadata.Header.ForceMergingKey) == "split" ? "yes" : "no",
                    Merge = item.ReadString(Models.Metadata.Header.ForceMergingKey) == "merge" ? "yes" : "no",
                };
            }

            if (item.ContainsKey(Models.Metadata.Header.RefNameKey)
                || item.ContainsKey(Models.Metadata.Header.EmulatorVersionKey))
            {
                metadataFile.Emulator = new Emulator
                {
                    RefName = item.ReadString(Models.Metadata.Header.RefNameKey),
                    Version = item.ReadString(Models.Metadata.Header.EmulatorVersionKey),
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Models.RomCenter.Rom"/>
        /// </summary>
        private static Rom[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null)
                return [];

            return Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item));
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Models.RomCenter.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item, Models.Metadata.Machine parent)
        {
            var row = new Rom
            {
                ParentName = parent.ReadString(Models.Metadata.Machine.CloneOfKey),
                //ParentDescription = parent.ReadString(Models.Metadata.Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                RomName = item.ReadString(Models.Metadata.Rom.NameKey),
                RomCRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                RomSize = item.ReadString(Models.Metadata.Rom.SizeKey),
                RomOf = parent.ReadString(Models.Metadata.Machine.RomOfKey),
                MergeName = item.ReadString(Models.Metadata.Rom.MergeKey),
            };
            return row;
        }
    }
}
