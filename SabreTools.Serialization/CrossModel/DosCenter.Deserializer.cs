using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.DosCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class DosCenter : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new MetadataFile();

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            if (header != null)
                metadataFile.DosCenter = ConvertHeaderFromInternalModel(header);

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
                metadataFile.Game = Array.ConvertAll(machines, ConvertMachineFromInternalModel);

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="Models.DosCenter.DosCenter"/>
        /// </summary>
        private static SabreTools.Serialization.Models.DosCenter.DosCenter ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var dosCenter = new SabreTools.Serialization.Models.DosCenter.DosCenter
            {
                Name = item.ReadString(Serialization.Models.Metadata.Header.NameKey),
                Description = item.ReadString(Serialization.Models.Metadata.Header.DescriptionKey),
                Version = item.ReadString(Serialization.Models.Metadata.Header.VersionKey),
                Date = item.ReadString(Serialization.Models.Metadata.Header.DateKey),
                Author = item.ReadString(Serialization.Models.Metadata.Header.AuthorKey),
                Homepage = item.ReadString(Serialization.Models.Metadata.Header.HomepageKey),
                Comment = item.ReadString(Serialization.Models.Metadata.Header.CommentKey),
            };
            return dosCenter;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to <see cref="Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var game = new Game
            {
                Name = item.ReadString(Serialization.Models.Metadata.Machine.NameKey),
            };

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
                game.File = Array.ConvertAll(roms, ConvertFromInternalModel);

            return game;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="File"/>
        /// </summary>
        private static File ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var file = new File
            {
                Name = item.ReadString(Serialization.Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Serialization.Models.Metadata.Rom.SizeKey),
                CRC = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Serialization.Models.Metadata.Rom.SHA1Key),
                Date = item.ReadString(Serialization.Models.Metadata.Rom.DateKey),
            };
            return file;
        }
    }
}
