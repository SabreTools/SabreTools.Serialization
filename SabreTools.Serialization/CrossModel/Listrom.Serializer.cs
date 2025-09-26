using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : IModelSerializer<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Set != null && obj.Set.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Set, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = "MAME Listrom",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Set"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Set item)
        {
            var machine = new Data.Models.Metadata.Machine();
            if (!string.IsNullOrEmpty(item.Device))
            {
                machine[Data.Models.Metadata.Machine.NameKey] = item.Device;
                machine[Data.Models.Metadata.Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Data.Models.Metadata.Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Length > 0)
            {
                var disks = new List<Data.Models.Metadata.Disk>();
                var roms = new List<Data.Models.Metadata.Rom>();
                foreach (var file in item.Row)
                {
                    var datItem = ConvertToInternalModel(file);
                    if (datItem is Data.Models.Metadata.Disk disk)
                        disks.Add(disk);
                    else if (datItem is Data.Models.Metadata.Rom rom)
                        roms.Add(rom);
                }

                machine[Data.Models.Metadata.Machine.DiskKey] = disks.ToArray();
                machine[Data.Models.Metadata.Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Data.Models.Metadata.DatItem ConvertToInternalModel(Row item)
        {
            if (item.Size == null)
            {
                var disk = new Data.Models.Metadata.Disk
                {
                    [Data.Models.Metadata.DatItem.TypeKey] = "disk",
                    [Data.Models.Metadata.Disk.NameKey] = item.Name,
                    [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Data.Models.Metadata.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Data.Models.Metadata.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Data.Models.Metadata.Rom
                {
                    [Data.Models.Metadata.DatItem.TypeKey] = "rom",
                    [Data.Models.Metadata.Rom.NameKey] = item.Name,
                    [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                    [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Data.Models.Metadata.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Data.Models.Metadata.Rom.StatusKey] = "baddump";

                return rom;
            }
        }
    }
}
