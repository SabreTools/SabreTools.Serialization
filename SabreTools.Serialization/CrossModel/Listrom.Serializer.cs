using System;
using System.Collections.Generic;
using SabreTools.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Set != null && obj.Set.Length > 0)
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Set, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Listrom.MetadataFile"/> to <see cref="Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "MAME Listrom",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.Listrom.Set"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Set item)
        {
            var machine = new Models.Metadata.Machine();
            if (!string.IsNullOrEmpty(item.Device))
            {
                machine[Models.Metadata.Machine.NameKey] = item.Device;
                machine[Models.Metadata.Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Models.Metadata.Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Length > 0)
            {
                var disks = new List<Models.Metadata.Disk>();
                var roms = new List<Models.Metadata.Rom>();
                foreach (var file in item.Row)
                {
                    var datItem = ConvertToInternalModel(file);
                    if (datItem is Models.Metadata.Disk disk)
                        disks.Add(disk);
                    else if (datItem is Models.Metadata.Rom rom)
                        roms.Add(rom);
                }

                machine[Models.Metadata.Machine.DiskKey] = disks.ToArray();
                machine[Models.Metadata.Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Models.Listrom.Row"/> to <see cref="Models.Metadata.DatItem"/>
        /// </summary>
        private static Models.Metadata.DatItem ConvertToInternalModel(Row item)
        {
            if (item.Size == null)
            {
                var disk = new Models.Metadata.Disk
                {
                    [Models.Metadata.DatItem.TypeKey] = "disk",
                    [Models.Metadata.Disk.NameKey] = item.Name,
                    [Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Models.Metadata.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Models.Metadata.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Models.Metadata.Rom
                {
                    [Models.Metadata.DatItem.TypeKey] = "rom",
                    [Models.Metadata.Rom.NameKey] = item.Name,
                    [Models.Metadata.Rom.SizeKey] = item.Size,
                    [Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Models.Metadata.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Models.Metadata.Rom.StatusKey] = "baddump";

                return rom;
            }
        }
    }
}
