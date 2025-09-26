using System;
using System.Collections.Generic;
using SabreTools.Data.Models.Listrom;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Listrom : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Set = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Set"/>
        /// </summary>
        private static Set ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var set = new Set();
            if (item.ReadString(Data.Models.Metadata.Machine.IsDeviceKey) == "yes")
                set.Device = item.ReadString(Data.Models.Metadata.Machine.NameKey);
            else
                set.Driver = item.ReadString(Data.Models.Metadata.Machine.NameKey);

            var rowItems = new List<Row>();

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms != null)
            {
                rowItems.AddRange(Array.ConvertAll(roms, ConvertFromInternalModel));
            }

            var disks = item.Read<Data.Models.Metadata.Disk[]>(Data.Models.Metadata.Machine.DiskKey);
            if (disks != null)
                rowItems.AddRange(Array.ConvertAll(disks, ConvertFromInternalModel));

            set.Row = [.. rowItems];
            return set;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Disk"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Disk item)
        {
            var row = new Row
            {
                Name = item.ReadString(Data.Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Data.Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Data.Models.Metadata.Disk.SHA1Key),
            };

            if (item[Data.Models.Metadata.Disk.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Data.Models.Metadata.Disk.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var row = new Row
            {
                Name = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
            };

            if (item[Data.Models.Metadata.Rom.StatusKey] as string == "nodump")
                row.NoGoodDumpKnown = true;
            else if (item[Data.Models.Metadata.Rom.StatusKey] as string == "baddump")
                row.Bad = true;

            return row;
        }
    }
}
