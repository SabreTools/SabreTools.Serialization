using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.RomCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            var items = new List<Rom>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Games = new Games { Rom = [.. items] };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile();

            if (item.ContainsKey(Serialization.Models.Metadata.Header.AuthorKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.VersionKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.EmailKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.HomepageKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.UrlKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.DateKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.CommentKey))
            {
                metadataFile.Credits = new Credits
                {
                    Author = item.ReadString(Serialization.Models.Metadata.Header.AuthorKey),
                    Version = item.ReadString(Serialization.Models.Metadata.Header.VersionKey),
                    Email = item.ReadString(Serialization.Models.Metadata.Header.EmailKey),
                    Homepage = item.ReadString(Serialization.Models.Metadata.Header.HomepageKey),
                    Url = item.ReadString(Serialization.Models.Metadata.Header.UrlKey),
                    Date = item.ReadString(Serialization.Models.Metadata.Header.DateKey),
                    Comment = item.ReadString(Serialization.Models.Metadata.Header.CommentKey),
                };
            }

            if (item.ContainsKey(Serialization.Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.PluginKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.ForceMergingKey))
            {
                metadataFile.Dat = new Dat
                {
                    Version = item.ReadString(Serialization.Models.Metadata.Header.DatVersionKey),
                    Plugin = item.ReadString(Serialization.Models.Metadata.Header.PluginKey),
                    Split = item.ReadString(Serialization.Models.Metadata.Header.ForceMergingKey) == "split" ? "yes" : "no",
                    Merge = item.ReadString(Serialization.Models.Metadata.Header.ForceMergingKey) == "merge" ? "yes" : "no",
                };
            }

            if (item.ContainsKey(Serialization.Models.Metadata.Header.RefNameKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.EmulatorVersionKey))
            {
                metadataFile.Emulator = new Emulator
                {
                    RefName = item.ReadString(Serialization.Models.Metadata.Header.RefNameKey),
                    Version = item.ReadString(Serialization.Models.Metadata.Header.EmulatorVersionKey),
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to an array of <see cref="Rom"/>
        /// </summary>
        private static Rom[] ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms == null)
                return [];

            return Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item));
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Serialization.Models.Metadata.Rom item, Serialization.Models.Metadata.Machine parent)
        {
            var row = new Rom
            {
                ParentName = parent.ReadString(Serialization.Models.Metadata.Machine.CloneOfKey),
                //ParentDescription = parent.ReadString(Serialization.Models.Metadata.Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey),
                RomName = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                RomCRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                RomSize = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                RomOf = parent.ReadString(Serialization.Models.Metadata.Machine.RomOfKey),
                MergeName = item.ReadString(Serialization.Models.Metadata.Rom.MergeKey),
            };
            return row;
        }
    }
}
