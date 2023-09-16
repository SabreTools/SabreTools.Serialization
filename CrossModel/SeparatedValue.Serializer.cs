using System.Linq;
using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Models.Metadata.MetadataFile Serialize(MetadataFile obj)
#else
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
#endif
        {
            if (obj == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row != null && obj.Row.Any())
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = obj.Row.Select(ConvertMachineToInternalModel).ToArray();

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.MetadataFile"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.HeaderKey] = item.Header,
            };

            if (item.Row != null && item.Row.Any())
            {
                var first = item.Row[0];
                //header[Models.Metadata.Header.FileNameKey] = first.FileName; // Not possible to map
                header[Models.Metadata.Header.NameKey] = first.FileName;
                header[Models.Metadata.Header.DescriptionKey] = first.Description;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.GameName,
                [Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
            };

            var datItem = ConvertToInternalModel(item);
            switch (datItem)
            {
                case Models.Metadata.Disk disk:
                    machine[Models.Metadata.Machine.DiskKey] = new Models.Metadata.Disk[] { disk };
                    break;

                case Models.Metadata.Media media:
                    machine[Models.Metadata.Machine.MediaKey] = new Models.Metadata.Media[] { media };
                    break;

                case Models.Metadata.Rom rom:
                    machine[Models.Metadata.Machine.RomKey] = new Models.Metadata.Rom[] { rom };
                    break;
            }

            return machine;
        }

#if NET48
        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Models.Metadata.DatItem ConvertToInternalModel(Row item)
        {
            switch (item.Type)
            {
                case "disk":
                    return new Models.Metadata.Disk
                    {
                        [Models.Metadata.Disk.NameKey] = item.DiskName,
                        [Models.Metadata.Disk.MD5Key] = item.MD5,
                        [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                        [Models.Metadata.Disk.StatusKey] = item.Status,
                    };

                case "media":
                    return new Models.Metadata.Media
                    {
                        [Models.Metadata.Media.NameKey] = item.DiskName,
                        [Models.Metadata.Media.MD5Key] = item.MD5,
                        [Models.Metadata.Media.SHA1Key] = item.SHA1,
                        [Models.Metadata.Media.SHA256Key] = item.SHA256,
                        [Models.Metadata.Media.SpamSumKey] = item.SpamSum,
                    };

                case "rom":
                    return new Models.Metadata.Rom
                    {
                        [Models.Metadata.Rom.NameKey] = item.RomName,
                        [Models.Metadata.Rom.SizeKey] = item.Size,
                        [Models.Metadata.Rom.CRCKey] = item.CRC,
                        [Models.Metadata.Rom.MD5Key] = item.MD5,
                        [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                        [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                        [Models.Metadata.Rom.SHA384Key] = item.SHA384,
                        [Models.Metadata.Rom.SHA512Key] = item.SHA512,
                        [Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                        [Models.Metadata.Rom.StatusKey] = item.Status,
                    };

                default:
                    return null;
            }
        }
#else
        /// <summary>
        /// Convert from <cref="Models.SeparatedValue.Row"/> to <cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Models.Metadata.DatItem? ConvertToInternalModel(Row item)
        {
            return item.Type switch
            {
                "disk" => new Models.Metadata.Disk
                {
                    [Models.Metadata.Disk.NameKey] = item.DiskName,
                    [Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                    [Models.Metadata.Disk.StatusKey] = item.Status,
                },
                "media" => new Models.Metadata.Media
                {
                    [Models.Metadata.Media.NameKey] = item.DiskName,
                    [Models.Metadata.Media.MD5Key] = item.MD5,
                    [Models.Metadata.Media.SHA1Key] = item.SHA1,
                    [Models.Metadata.Media.SHA256Key] = item.SHA256,
                    [Models.Metadata.Media.SpamSumKey] = item.SpamSum,
                },
                "rom" => new Models.Metadata.Rom
                {
                    [Models.Metadata.Rom.NameKey] = item.RomName,
                    [Models.Metadata.Rom.SizeKey] = item.Size,
                    [Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Models.Metadata.Rom.MD5Key] = item.MD5,
                    [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                    [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                    [Models.Metadata.Rom.SHA384Key] = item.SHA384,
                    [Models.Metadata.Rom.SHA512Key] = item.SHA512,
                    [Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                    [Models.Metadata.Rom.StatusKey] = item.Status,
                },
                _ => null,
            };
        }
#endif
    }
}