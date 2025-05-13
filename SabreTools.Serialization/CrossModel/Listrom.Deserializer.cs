using System;
using System.Collections.Generic;
using SabreTools.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Set = Array.ConvertAll(machines, ConvertMachineFromInternalModel);
            
            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Models.Listrom.Set"/>
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
                rowItems.AddRange(Array.ConvertAll(roms, ConvertFromInternalModel));
            }

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(Array.ConvertAll(disks, ConvertFromInternalModel));

            set.Row = [.. rowItems];
            return set;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Models.Listrom.Row"/>
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
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Models.Listrom.Row"/>
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