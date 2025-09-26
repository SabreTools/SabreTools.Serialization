using System;
using System.Collections.Generic;
using SabreTools.Data.Models.SeparatedValue;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : BaseMetadataSerializer<MetadataFile>
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
                items.AddRange(ConvertMachineFromInternalModel(machine, header));
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
        private static Row[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item, Data.Models.Metadata.Header? header)
        {
            var rowItems = new List<Row>();

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item, header)));
            }

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(disks, d => ConvertFromInternalModel(d, item, header)));
            }

            var media = item.Read<Data.Models.Metadata.Media[]>(Data.Models.Metadata.Machine.MediaKey);
            if (media != null && media.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(media, m => ConvertFromInternalModel(m, item, header)));
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Disk item, Data.Models.Metadata.Machine parent, Data.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve
                InternalName = header?.ReadString(Data.Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Data.Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Data.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Data.Models.Metadata.Machine.DescriptionKey),
                Type = "disk",
                RomName = null,
                DiskName = item.ReadString(Data.Models.Metadata.Disk.NameKey),
                Size = null,
                CRC = null,
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
                SHA256 = null,
                SHA384 = null,
                SHA512 = null,
                SpamSum = null,
                Status = item.ReadString(Data.Models.Metadata.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Media"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Media item, Data.Models.Metadata.Machine parent, Data.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve on an item -- OriginalFilename
                InternalName = header?.ReadString(Data.Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Data.Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Data.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Data.Models.Metadata.Machine.DescriptionKey),
                Type = "media",
                RomName = null,
                DiskName = item.ReadString(Data.Models.Metadata.Media.NameKey),
                Size = null,
                CRC = null,
                MD5 = item.ReadString(Data.Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Data.Models.Metadata.Media.SHA256Key),
                SHA384 = null,
                SHA512 = null,
                SpamSum = item.ReadString(Data.Models.Metadata.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Rom item, Data.Models.Metadata.Machine parent, Data.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve
                InternalName = header?.ReadString(Data.Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Data.Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Data.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Data.Models.Metadata.Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                DiskName = null,
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                MD5 = item.ReadString(Data.Models.Metadata.Rom.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Data.Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Data.Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Data.Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Data.Models.Metadata.Rom.SpamSumKey),
                Status = item.ReadString(Data.Models.Metadata.Rom.StatusKey),
            };
            return row;
        }
    }
}
