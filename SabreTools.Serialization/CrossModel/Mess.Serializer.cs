using System;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class Mess : IModelSerializer<Data.Models.Listxml.Mess, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(Data.Models.Listxml.Mess? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Game, Listxml.ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Listxml.Mess"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Data.Models.Listxml.Mess item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.VersionKey] = item.Version,
            };
            return header;
        }
    }
}
