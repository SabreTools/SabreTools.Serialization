using System;
using System.Collections.Generic;
using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
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
                items.AddRange(ConvertMachineFromInternalModel(machine, header));
            }

            metadataFile.Row = [.. items];
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.SeparatedValue.MetadataFile"/>
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
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Metadata.Machine item, Models.Metadata.Header? header)
        {
            var rowItems = new List<Row>();

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item, header)));
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(disks, d => ConvertFromInternalModel(d, item, header)));
            }

            var media = item.Read<Models.Metadata.Media[]>(Models.Metadata.Machine.MediaKey);
            if (media != null && media.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(media, m => ConvertFromInternalModel(m, item, header)));
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Disk item, Models.Metadata.Machine parent, Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve
                InternalName = header?.ReadString(Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "disk",
                RomName = null,
                DiskName = item.ReadString(Models.Metadata.Disk.NameKey),
                Size = null,
                CRC = null,
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                SHA256 = null,
                SHA384 = null,
                SHA512 = null,
                SpamSum = null,
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Media"/> to <see cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Media item, Models.Metadata.Machine parent, Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve on an item -- OriginalFilename
                InternalName = header?.ReadString(Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "media",
                RomName = null,
                DiskName = item.ReadString(Models.Metadata.Media.NameKey),
                Size = null,
                CRC = null,
                MD5 = item.ReadString(Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Media.SHA256Key),
                SHA384 = null,
                SHA512 = null,
                SpamSum = item.ReadString(Models.Metadata.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item, Models.Metadata.Machine parent, Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve
                InternalName = header?.ReadString(Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Models.Metadata.Rom.NameKey),
                DiskName = null,
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                MD5 = item.ReadString(Models.Metadata.Rom.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Models.Metadata.Rom.SpamSumKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
            };
            return row;
        }
    }
}
