using System.Linq;
using SabreTools.Models.DosCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class DosCenter : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.DosCenter = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
            {
                metadataFile.Game = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        private static Models.DosCenter.DosCenter ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var dosCenter = new Models.DosCenter.DosCenter
            {
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                Version = item.ReadString(Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Models.Metadata.Header.DateKey),
                Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                Comment = item.ReadString(Models.Metadata.Header.CommentKey),
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.DosCenter.Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var game = new Game
            {
                Name = item.ReadString(Models.Metadata.Machine.NameKey),
            };

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                game.File = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.DosCenter.File"/>
        /// </summary>
        private static Models.DosCenter.File ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var file = new Models.DosCenter.File
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                Date = item.ReadString(Models.Metadata.Rom.DateKey),
            };
            return file;
        }
    }
}