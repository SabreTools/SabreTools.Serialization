using System;
using SabreTools.Data.Models.DosCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class DosCenter : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            if (header is not null)
                metadataFile.DosCenter = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines is not null && machines.Length > 0)
                metadataFile.Game = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        private static Data.Models.DosCenter.DosCenter ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var dosCenter = new Data.Models.DosCenter.DosCenter
            {
                Name = item.Name,
                Description = item.Description,
                Version = item.Version,
                Date = item.Date,
                Author = item.Author,
                Homepage = item.Homepage,
                Comment = item.Comment,
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var game = new Game
            {
                Name = item.Name,
            };

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms is not null && roms.Length > 0)
                game.File = Array.ConvertAll(roms, ConvertFromInternalModel);

            return game;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="File"/>
        /// </summary>
        private static File ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var file = new File
            {
                Name = item.Name,
                Size = item.Size,
                CRC = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Data.Models.Metadata.Rom.SHA1Key),
                Date = item.ReadString(Data.Models.Metadata.Rom.DateKey),
            };
            return file;
        }
    }
}
