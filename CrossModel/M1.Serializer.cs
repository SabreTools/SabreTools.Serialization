using System.Linq;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class M1 : IModelSerializer<Models.Listxml.M1, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Models.Metadata.MetadataFile Serialize(Models.Listxml.M1 item)
#else
        public Models.Metadata.MetadataFile? Serialize(Models.Listxml.M1? item)
#endif
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Game
                    .Where(g => g != null)
                    .Select(Listxml.ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.M1"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.Listxml.M1 item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.VersionKey] = item.Version,
            };
            return header;
        }
    }
}