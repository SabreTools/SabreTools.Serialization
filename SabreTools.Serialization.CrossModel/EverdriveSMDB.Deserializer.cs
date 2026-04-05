using System;
using System.Collections.Generic;
using SabreTools.Data.Models.EverdriveSMDB;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Machine;
            var items = new List<Row>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Row = [.. items];

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to an array of <see cref="Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var roms = item.Rom;
            if (roms is null || roms.Length == 0)
                return [];

            return Array.ConvertAll(roms, ConvertFromInternalModel);
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var row = new Row
            {
                SHA256 = item.SHA256,
                Name = item.Name,
                SHA1 = item.SHA1,
                MD5 = item.MD5,
                CRC32 = item.CRC,
                Size = item.Size,
            };
            return row;
        }
    }
}
