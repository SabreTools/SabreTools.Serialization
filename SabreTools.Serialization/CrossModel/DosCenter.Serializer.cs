using System.Linq;
using SabreTools.Models.DosCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class DosCenter : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;
            
            var metadataFile = new Models.Metadata.MetadataFile();

            if (obj?.DosCenter != null)
                metadataFile[Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.DosCenter);

            if (obj?.Game != null && obj.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = obj.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.DosCenter"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Models.DosCenter.DosCenter item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.NameKey] = item.Name,
                [Models.Metadata.Header.DescriptionKey] = item.Description,
                [Models.Metadata.Header.VersionKey] = item.Version,
                [Models.Metadata.Header.DateKey] = item.Date,
                [Models.Metadata.Header.AuthorKey] = item.Author,
                [Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Models.Metadata.Header.CommentKey] = item.Comment,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.Game"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
            };

            if (item.File != null && item.File.Any())
            {
                machine[Models.Metadata.Machine.RomKey] = item.File
                    .Where(f => f != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.DosCenter.File"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Models.DosCenter.File item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.CRCKey] = item.CRC,
                [Models.Metadata.Rom.DateKey] = item.Date,
            };
            return rom;
        }
    }
}