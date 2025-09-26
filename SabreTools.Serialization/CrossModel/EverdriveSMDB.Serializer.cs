using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.EverdriveSMDB;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = "Everdrive SMDB",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.RomKey] = new Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };
            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.MD5Key] = item.MD5,
                [Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Models.Metadata.Rom.SizeKey] = item.Size,
            };
            return rom;
        }
    }
}
