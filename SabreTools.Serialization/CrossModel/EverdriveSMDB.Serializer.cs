using System;
using SabreTools.Data.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : IModelSerializer<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.NameKey] = "Everdrive SMDB",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };
            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Data.Models.Metadata.Rom.NameKey] = item.Name,
                [Data.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Data.Models.Metadata.Rom.MD5Key] = item.MD5,
                [Data.Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Data.Models.Metadata.Rom.SizeKey] = item.Size,
            };
            return rom;
        }
    }
}
