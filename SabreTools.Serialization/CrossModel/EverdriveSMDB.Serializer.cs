using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.EverdriveSMDB;

namespace SabreTools.Serialization.CrossModel
{
    public partial class EverdriveSMDB : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(),
            };

            if (obj?.Row != null && obj.Row.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Row, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref=Serialization.Models.Metadata."Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel()
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.NameKey] = "Everdrive SMDB",
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Row item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.RomKey] = new Serialization.Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };
            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Row"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(Row item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.SHA256Key] = item.SHA256,
                [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Rom.MD5Key] = item.MD5,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC32,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
            };
            return rom;
        }
    }
}
