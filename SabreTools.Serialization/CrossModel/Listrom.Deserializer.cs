using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.Listrom;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Set = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to <see cref="Set"/>
        /// </summary>
        private static Set ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var set = new Set();
            if (item.ReadString(Serialization.Models.Metadata.Machine.IsDeviceKey) == "yes")
                set.Device = item.ReadString(Serialization.Models.Metadata.Machine.NameKey);
            else
                set.Driver = item.ReadString(Serialization.Models.Metadata.Machine.NameKey);

            var rowItems = new List<Row>();

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms != null)
            {
                rowItems.AddRange(Array.ConvertAll(roms, ConvertFromInternalModel));
            }

            var disks = item.Read<Serialization.Models.Metadata.Disk[]>(Serialization.Models.Metadata.Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(Array.ConvertAll(disks, ConvertFromInternalModel));

            set.Row = [.. rowItems];
            return set;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Disk"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Disk item)
        {
            var row = new Row
            {
                Name = item.ReadString(Serialization.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Serialization.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Disk.SHA1Key),
            };

            if (item[Serialization.Models.Metadata.Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Serialization.Models.Metadata.Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var row = new Row
            {
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
            };

            if (item[Serialization.Models.Metadata.Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Serialization.Models.Metadata.Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }
    }
}
