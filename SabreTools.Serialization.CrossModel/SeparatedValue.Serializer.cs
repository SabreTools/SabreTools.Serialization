using System;
using SabreTools.Data.Models.SeparatedValue;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                Header = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row is not null && obj.Row.Length > 0)
                metadataFile.Machine = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Data.Models.Metadata.Header
            {
                HeaderRow = item.Header,
            };

            if (item.Row is not null && item.Row.Length > 0)
            {
                var first = item.Row[0];
                header.FileName = first.FileName;
                header.Name =  first.FileName;
                header.Description = first.Description;
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
                Name = item.GameName,
                Description = item.GameDescription,
            };

            var datItem = ConvertToInternalModel(item);
            switch (datItem)
            {
                case Data.Models.Metadata.Disk disk:
                    machine.Disk = new Data.Models.Metadata.Disk[] { disk };
                    break;

                case Data.Models.Metadata.Media media:
                    machine.Media = new Data.Models.Metadata.Media[] { media };
                    break;

                case Data.Models.Metadata.Rom rom:
                    machine.Rom = new Data.Models.Metadata.Rom[] { rom };
                    break;

                default:
                    // TODO: Log invalid values
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
                    Name = item.DiskName,
                    MD5 = item.MD5,
                    SHA1 = item.SHA1,
                    Status = item.Status,
                },
                "media" => new Data.Models.Metadata.Media
                {
                    Name = item.DiskName,
                    MD5 = item.MD5,
                    SHA1 = item.SHA1,
                    SHA256 = item.SHA256,
                    SpamSum = item.SpamSum,
                },
                "rom" => new Data.Models.Metadata.Rom
                {
                    Name = item.RomName,
                    Size = item.Size,
                    CRC = item.CRC,
                    MD5 = item.MD5,
                    SHA1 = item.SHA1,
                    SHA256 = item.SHA256,
                    SHA384 = item.SHA384,
                    SHA512 = item.SHA512,
                    SpamSum = item.SpamSum,
                    Status = item.Status,
                },
                _ => null,
            };
        }
    }
}
