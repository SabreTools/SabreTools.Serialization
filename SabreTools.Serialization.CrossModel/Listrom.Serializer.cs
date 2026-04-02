using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Listrom;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Set is not null && obj.Set.Length > 0)
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
                Name =  "MAME Listrom",
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
                machine.Name = item.Device;
                machine.IsDevice = true;
            }
            else
            {
                machine.Name = item.Driver;
            }

            if (item.Row is not null && item.Row.Length > 0)
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
            if (item.Size is null)
            {
                var disk = new Data.Models.Metadata.Disk
                {
                    Name = item.Name,
                    [Data.Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Data.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk.Status = Data.Models.Metadata.ItemStatus.Nodump;
                else if (item.Bad)
                    disk.Status = Data.Models.Metadata.ItemStatus.BadDump;

                return disk;
            }
            else
            {
                var rom = new Data.Models.Metadata.Rom
                {
                    Name = item.Name,
                    [Data.Models.Metadata.Rom.SizeKey] = item.Size,
                    [Data.Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom.Status = Data.Models.Metadata.ItemStatus.Nodump;
                else if (item.Bad)
                    rom.Status = Data.Models.Metadata.ItemStatus.BadDump;

                return rom;
            }
        }
    }
}
