using System.Collections.Generic;
using System.Linq;
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
            if (machines != null && machines.Any())
            {
                metadataFile.Row = machines
                    .Where(m => m != null)
                    .SelectMany(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.SeparatedValue.MetadataFile"/>
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
        /// Convert from <cref="Models.Metadata.Machine"/> to an array of <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var rowItems = new List<Row>();

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Any())
            {
                rowItems.AddRange(roms
                    .Where(r => r != null)
                    .Select(rom => ConvertFromInternalModel(rom, item)));
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null && disks.Any())
            {
                rowItems.AddRange(disks
                    .Where(d => d != null)
                    .Select(disk => ConvertFromInternalModel(disk, item)));
            }

            var media = item.Read<Models.Metadata.Media[]>(Models.Metadata.Machine.MediaKey);
            if (media != null && media.Any())
            {
                rowItems.AddRange(media
                    .Where(m => m != null)
                    .Select(medium => ConvertFromInternalModel(medium, item)));
            }

            return rowItems.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Disk item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                Description = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "disk",
                DiskName = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Media"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Media item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                Description = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "media",
                DiskName = item.ReadString(Models.Metadata.Media.NameKey),
                MD5 = item.ReadString(Models.Metadata.Media.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Media.SHA1Key),
                SHA256 = item.ReadString(Models.Metadata.Media.SHA256Key),
                SpamSum = item.ReadString(Models.Metadata.Media.SpamSumKey),
            };
            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.SeparatedValue.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item, Models.Metadata.Machine parent)
        {
            var row = new Row
            {
                GameName = parent?.ReadString(Models.Metadata.Machine.NameKey),
                Description = parent?.ReadString(Models.Metadata.Machine.DescriptionKey),
                Type = "rom",
                RomName = item.ReadString(Models.Metadata.Rom.NameKey),
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