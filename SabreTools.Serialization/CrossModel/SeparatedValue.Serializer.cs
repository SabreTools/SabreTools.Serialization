using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.SeparatedValue;

namespace SabreTools.Serialization.CrossModel
{
    public partial class SeparatedValue : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.HeaderKey] = item.Header,
            };

            if (item.Row != null && item.Row.Length > 0)
            {
                var first = item.Row[0];
                header["FILENAME"] = first.FileName; // TODO: Make this an actual key to retrieve on an item -- OriginalFilename
                header[Serialization.Models.Metadata.Header.NameKey] = first.FileName;
                header[Serialization.Models.Metadata.Header.DescriptionKey] = first.Description;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.GameName,
                [Serialization.Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
            };

            var datItem = ConvertToInternalModel(item);
            switch (datItem)
            {
                case Serialization.Models.Metadata.Disk disk:
                    machine[Serialization.Models.Metadata.Machine.DiskKey] = new Serialization.Models.Metadata.Disk[] { disk };
                    break;

                case Serialization.Models.Metadata.Media media:
                    machine[Serialization.Models.Metadata.Machine.MediaKey] = new Serialization.Models.Metadata.Media[] { media };
                    break;

                case Serialization.Models.Metadata.Rom rom:
                    machine[Serialization.Models.Metadata.Machine.RomKey] = new Serialization.Models.Metadata.Rom[] { rom };
                    break;
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.DatItem"/>
        /// </summary>
        private static Serialization.Models.Metadata.DatItem? ConvertToInternalModel(Row item)
        {
            return item.Type switch
            {
                "disk" => new Serialization.Models.Metadata.Disk
                {
                    [Serialization.Models.Metadata.Disk.NameKey] = item.DiskName,
                    [Serialization.Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Serialization.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                    [Serialization.Models.Metadata.Disk.StatusKey] = item.Status,
                },
                "media" => new Serialization.Models.Metadata.Media
                {
                    [Serialization.Models.Metadata.Media.NameKey] = item.DiskName,
                    [Serialization.Models.Metadata.Media.MD5Key] = item.MD5,
                    [Serialization.Models.Metadata.Media.SHA1Key] = item.SHA1,
                    [Serialization.Models.Metadata.Media.SHA256Key] = item.SHA256,
                    [Serialization.Models.Metadata.Media.SpamSumKey] = item.SpamSum,
                },
                "rom" => new Serialization.Models.Metadata.Rom
                {
                    [Serialization.Models.Metadata.Rom.NameKey] = item.RomName,
                    [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                    [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Serialization.Models.Metadata.Rom.MD5Key] = item.MD5,
                    [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                    [Serialization.Models.Metadata.Rom.SHA256Key] = item.SHA256,
                    [Serialization.Models.Metadata.Rom.SHA384Key] = item.SHA384,
                    [Serialization.Models.Metadata.Rom.SHA512Key] = item.SHA512,
                    [Serialization.Models.Metadata.Rom.SpamSumKey] = item.SpamSum,
                    [Serialization.Models.Metadata.Rom.StatusKey] = item.Status,
                },
                _ => null,
            };
        }
    }
}
