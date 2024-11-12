using System;
using System.Linq;
using SabreTools.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
            {
                metadataFile.Row = machines
                    .Where(m => m != null)
                    .SelectMany(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to an array of <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        private static Row[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null || roms.Length == 0)
                return [];

            return Array.ConvertAll(roms, ConvertFromInternalModel);
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.EverdriveSMDB.Row"/>
        /// </summary>
        private static Row ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var row = new Row
            {
                SHA256 = item.ReadString(Models.Metadata.Rom.SHA256Key),
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                MD5 = item.ReadString(Models.Metadata.Rom.MD5Key),
                CRC32 = item.ReadString(Models.Metadata.Rom.CRCKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
            };
            return row;
        }
    }
}