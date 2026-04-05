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
            if (obj is null)
                return null;

            var header = obj.Header;
            var metadataFile = header is not null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = obj.Machine;
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
                Header = item.HeaderRow,
            };
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item, Data.Models.Metadata.Header? header)
        {
            var rowItems = new List<Row>();

            var roms = item.Rom;
            if (roms is not null && roms.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(roms, r => ConvertFromInternalModel(r, item, header)));
            }

            var disks = item.Disk;
            if (disks is not null && disks.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(disks, d => ConvertFromInternalModel(d, item, header)));
            }

            var media = item.Media;
            if (media is not null && media.Length > 0)
            {
                rowItems.AddRange(
                    Array.ConvertAll(media, m => ConvertFromInternalModel(m, item, header)));
            }

            return [.. rowItems];
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Disk item, Data.Models.Metadata.Machine parent, Data.Models.Metadata.Header? header)
        {
            var row = new Row
            {
                FileName = header?.FileName,
                InternalName = header?.Name,
                Description = header?.Description,
                GameName = parent.Name,
                GameDescription = parent.Description,
                Type = "disk",
                RomName = null,
                DiskName = item.Name,
                Size = null,
                CRC = null,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = null,
                SHA384 = null,
                SHA512 = null,
                SpamSum = null,
                Status = item.Status,
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
                FileName = header?.FileName,
                InternalName = header?.Name,
                Description = header?.Description,
                GameName = parent.Name,
                GameDescription = parent.Description,
                Type = "media",
                RomName = null,
                DiskName = item.Name,
                Size = null,
                CRC = null,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SHA384 = null,
                SHA512 = null,
                SpamSum = item.SpamSum,
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
                FileName = header?.FileName,
                InternalName = header?.Name,
                Description = header?.Description,
                GameName = parent.Name,
                GameDescription = parent.Description,
                Type = "rom",
                RomName = item.Name,
                DiskName = null,
                Size = item.Size,
                CRC = item.CRC,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
                SHA256 = item.SHA256,
                SHA384 = item.SHA384,
                SHA512 = item.SHA512,
                SpamSum = item.SpamSum,
                Status = item.Status,
            };
            return row;
        }
    }
}
