using System;
using SabreTools.Data.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : ICrossModel<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.HeaderKey] = item.Header,
            };

            if (item.Row != null && item.Row.Length > 0)
            {
                var first = item.Row[0];
                header["FILENAME"] = first.FileName; // TODO: Make this an actual key to retrieve on an item -- OriginalFilename
                header[Data.Models.Metadata.Header.NameKey] = first.FileName;
                header[Data.Models.Metadata.Header.DescriptionKey] = first.Description;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.NameKey] = item.GameName,
                [Data.Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
            };

            var datItem = ConvertToInternalModel(item);
            switch (datItem)
            {
                case Data.Models.Metadata.Disk disk:
                    machine[Data.Models.Metadata.Machine.DiskKey] = new Data.Models.Metadata.Disk[] { disk };
                    break;

                case Data.Models.Metadata.Media media:
                    machine[Data.Models.Metadata.Machine.MediaKey] = new Data.Models.Metadata.Media[] { media };
                    break;

                case Data.Models.Metadata.Rom rom:
                    machine[Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { rom };
                    break;
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Data.Models.Metadata.DatItem? ConvertToInternalModel(Row item)
        {
            return item.Type switch
            {
                "disk" => new Data.Models.Metadata.Disk
                {
                    [Data.Models.Metadata.Disk.NameKey] = item.DiskName,
                    [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                    [Data.Models.Metadata.Disk.StatusKey] = item.Status,
                },
                "media" => new Data.Models.Metadata.Media
                {
                    [Data.Models.Metadata.Media.NameKey] = item.DiskName,
                    [Data.Models.Metadata.Media.MD5Key] = item.MD5,
                    [Data.Models.Metadata.Media.SHA1Key] = item.SHA1,
                    [Data.Models.Metadata.Media.SHA256Key] = item.SHA256,
                    [Data.Models.Metadata.Media.SpamSumKey] = item.SpamSum,
                },
                "rom" => new Data.Models.Metadata.Rom
                {
                    [Data.Models.Metadata.Rom.NameKey] = item.RomName,
                    [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                    [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Data.Models.Metadata.Rom.MD5Key] = item.MD5,
                    [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                    [Data.Models.Metadata.Rom.SHA256Key] = item.SHA256,
                    [Data.Models.Metadata.Rom.SHA384Key] = item.SHA384,
                    [Data.Models.Metadata.Rom.SHA512Key] = item.SHA512,
                    [Data.Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                    [Data.Models.Metadata.Rom.StatusKey] = item.Status,
                },
                _ => null,
            };
        }
    }
}
