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
            if (obj is null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Machine;
            if (machines is not null && machines.Length > 0)
                metadataFile.Set = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Set"/>
        /// </summary>
        private static Set ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var set = new Set();
            if (item.IsDevice == true)
                set.Device = item.Name;
            else
                set.Driver = item.Name;

            var rowItems = new List<Row>();

            var roms = item.Rom;
            if (roms is not null)
            {
                rowItems.AddRange(Array.ConvertAll(roms, ConvertFromInternalModel));
            }

            var disks = item.Disk;
            if (disks is not null)
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
                Name = item.Name,
                MD5 = item.MD5,
                SHA1 = item.SHA1,
            };

            if (item.Status == Data.Models.Metadata.ItemStatus.Nodump)
                row.NoGoodDumpKnown = true;
            else if (item.Status == Data.Models.Metadata.ItemStatus.BadDump)
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
                Name = item.Name,
                Size = item.Size,
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
            };

            if (item.Status == Data.Models.Metadata.ItemStatus.Nodump)
                row.NoGoodDumpKnown = true;
            else if (item.Status == Data.Models.Metadata.ItemStatus.BadDump)
                row.Bad = true;

            return row;
        }
    }
}
