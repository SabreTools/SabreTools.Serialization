using System;
using System.Collections.Generic;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.EverdriveSMDB;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            var items = new List<Row>();
            foreach (var machine in machines ?? [])
            {
                items.AddRange(ConvertMachineFromInternalModel(machine));
            }

            metadataFile.Row = [.. items];

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to an array of <see cref="Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms == null || roms.Length == 0)
                return [];

            return Array.ConvertAll(roms, ConvertFromInternalModel);
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var row = new Row
            {
                SHA256 = item.ReadString(Serialization.Models.Metadata.Rom.SHA256Key),
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                MD5 = item.ReadString(Serialization.Models.Metadata.Rom.MD5Key),
                CRC32 = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
            };
            return row;
        }
    }
}
