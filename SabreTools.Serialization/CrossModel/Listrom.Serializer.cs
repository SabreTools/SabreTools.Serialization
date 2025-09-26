using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.Listrom;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Set != null && obj.Set.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Set, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.NameKey] = "MAME Listrom",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Set"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Set item)
        {
            var machine = new Serialization.Models.Metadata.Machine();
            if (!string.IsNullOrEmpty(item.Device))
            {
                machine[Serialization.Models.Metadata.Machine.NameKey] = item.Device;
                machine[Serialization.Models.Metadata.Machine.IsDeviceKey] = "yes";
            }
            else
            {
                machine[Serialization.Models.Metadata.Machine.NameKey] = item.Driver;
            }

            if (item.Row != null && item.Row.Length > 0)
            {
                var disks = new List<Serialization.Models.Metadata.Disk>();
                var roms = new List<Serialization.Models.Metadata.Rom>();
                foreach (var file in item.Row)
                {
                    var datItem = ConvertToInternalModel(file);
                    if (datItem is Serialization.Models.Metadata.Disk disk)
                        disks.Add(disk);
                    else if (datItem is Serialization.Models.Metadata.Rom rom)
                        roms.Add(rom);
                }

                machine[Serialization.Models.Metadata.Machine.DiskKey] = disks.ToArray();
                machine[Serialization.Models.Metadata.Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.DatItem"/>
        /// </summary>
        private static Serialization.Models.Metadata.DatItem ConvertToInternalModel(Row item)
        {
            if (item.Size == null)
            {
                var disk = new Serialization.Models.Metadata.Disk
                {
                    [Serialization.Models.Metadata.DatItem.TypeKey] = "disk",
                    [Serialization.Models.Metadata.Disk.NameKey] = item.Name,
                    [Serialization.Models.Metadata.Disk.MD5Key] = item.MD5,
                    [Serialization.Models.Metadata.Disk.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    disk[Serialization.Models.Metadata.Disk.StatusKey] = "nodump";
                else if (item.Bad)
                    disk[Serialization.Models.Metadata.Disk.StatusKey] = "baddump";

                return disk;
            }
            else
            {
                var rom = new Serialization.Models.Metadata.Rom
                {
                    [Serialization.Models.Metadata.DatItem.TypeKey] = "rom",
                    [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                    [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                    [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC,
                    [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                };

                if (item.NoGoodDumpKnown)
                    rom[Serialization.Models.Metadata.Rom.StatusKey] = "nodump";
                else if (item.Bad)
                    rom[Serialization.Models.Metadata.Rom.StatusKey] = "baddump";

                return rom;
            }
        }
    }
}
