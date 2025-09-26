using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.SeparatedValue;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
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
                items.AddRange(ConvertMachineFromInternalModel(machine, header));
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
        private static Row[] ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item, Serialization.Models.Metadata.Header? header)
        {
            var rowItems = new List<Row>();

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item, header)));
            }

            var disks = item.Read<Serialization.Models.Metadata.Disk[]>(Serialization.Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(disks, d => ConvertFromInternalModel(d, item, header)));
            }

            var media = item.Read<Serialization.Models.Metadata.Media[]>(Serialization.Models.Metadata.Machine.MediaKey);
            if (media != null && media.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(media, m => ConvertFromInternalModel(m, item, header)));
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Disk"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Disk item, Serialization.Models.Metadata.Machine parent, Serialization.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve
                InternalName = header?.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey),
                Type = "disk",
                RomName = null,
                DiskName = item.ReadString(Serialization.Models.Metadata.Disk.NameKey),
                Size = null,
                CRC = null,
                MD5 = item.ReadString(Serialization.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Disk.SHA1Key),
                SHA256 = null,
                SHA384 = null,
                SHA512 = null,
                SpamSum = null,
                Status = item.ReadString(Serialization.Models.Metadata.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Media"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Media item, Serialization.Models.Metadata.Machine parent, Serialization.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve on an item -- OriginalFilename
                InternalName = header?.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey),
                Type = "media",
                RomName = null,
                DiskName = item.ReadString(Serialization.Models.Metadata.Media.NameKey),
                Size = null,
                CRC = null,
                MD5 = item.ReadString(Serialization.Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Serialization.Models.Metadata.Media.SHA256Key),
                SHA384 = null,
                SHA512 = null,
                SpamSum = item.ReadString(Serialization.Models.Metadata.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Rom item, Serialization.Models.Metadata.Machine parent, Serialization.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.ReadString("FILENAME"), // TODO: Make this an actual key to retrieve
                InternalName = header?.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = header?.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                GameName = parent.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Serialization.Models.Metadata.Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                DiskName = null,
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Rom.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                SHA256 = item.ReadString(Serialization.Models.Metadata.Rom.SHA256Key),
                SHA384 = item.ReadString(Serialization.Models.Metadata.Rom.SHA384Key),
                SHA512 = item.ReadString(Serialization.Models.Metadata.Rom.SHA512Key),
                SpamSum = item.ReadString(Serialization.Models.Metadata.Rom.SpamSumKey),
                Status = item.ReadString(Serialization.Models.Metadata.Rom.StatusKey),
            };
            return row;
        }
    }
}
