using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(Models.Metadata.MetadataFile obj)
#else
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj)
#endif
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Set = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }
            
            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.Listrom.Set"/>
        /// </summary>
        private static Set ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var set = new Set();
            if (item.ReadString(Models.Metadata.Machine.IsDeviceKey) == "yes")
                set.Device = item.ReadString(Models.Metadata.Machine.NameKey);
            else
                set.Driver = item.ReadString(Models.Metadata.Machine.NameKey);

            var rowItems = new List<Row>();

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null)
            {
                rowItems.AddRange(roms.Where(r => r != null).Select(ConvertFromInternalModel));
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(disks.Where(d => d != null).Select(ConvertFromInternalModel));

            set.Row = rowItems.ToArray();
            return set;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var row = new Row
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
            };

            if (item[Models.Metadata.Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Metadata.Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.Listrom.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var row = new Row
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
            };

            if (item[Models.Metadata.Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Models.Metadata.Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }
    }
}