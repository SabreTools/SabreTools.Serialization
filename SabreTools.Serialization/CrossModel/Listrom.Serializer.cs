using System.Collections.Generic;
using System.Linq;
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
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = obj.Set
                    .Where(s => s != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.MetadataFile"/> to <cref="Header"/>
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
        /// Convert from <cref="Models.Listrom.Set"/> to <cref="Models.Metadata.Machine"/>
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
                var datItems = new List<Models.Metadata.DatItem>();
                foreach (var file in item.Row)
                {
                    datItems.Add(ConvertToInternalModel(file));
                }

                machine[Models.Metadata.Machine.DiskKey] = datItems.Where(i => i.ReadString(Models.Metadata.DatItem.TypeKey) == "disk").Select(d => d as Models.Metadata.Disk).ToArray();
                machine[Models.Metadata.Machine.RomKey] = datItems.Where(i => i.ReadString(Models.Metadata.DatItem.TypeKey) == "rom").Select(d => d as Models.Metadata.Rom).ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listrom.Row"/> to <cref="Models.Metadata.DatItem"/>
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