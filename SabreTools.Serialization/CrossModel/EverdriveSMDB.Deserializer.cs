using System;
using System.Collections.Generic;
using SabreTools.Data.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : IModelSerializer<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
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
            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms == null || roms.Length == 0)
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
                SHA256 = item.ReadString(Data.Models.Metadata.Rom.SHA256Key),
                Name = item.ReadString(Data.Models.Metadata.Rom.NameKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                MD5 = item.ReadString(Data.Models.Metadata.Rom.MD5Key),
                CRC32 = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                Size = item.ReadString(Data.Models.Metadata.Rom.SizeKey),
            };
            return row;
        }
    }
}
