using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.DosCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class DosCenter : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile();

            if (obj?.DosCenter != null)
                metadataFile[Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj.DosCenter);

            if (obj?.Game != null && obj.Game.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.DosCenter.DosCenter"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(SabreTools.Serialization.Models.DosCenter.DosCenter item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.NameKey] = item.Name,
                [Serialization.Models.Metadata.Header.DescriptionKey] = item.Description,
                [Serialization.Models.Metadata.Header.VersionKey] = item.Version,
                [Serialization.Models.Metadata.Header.DateKey] = item.Date,
                [Serialization.Models.Metadata.Header.AuthorKey] = item.Author,
                [Serialization.Models.Metadata.Header.HomepageKey] = item.Homepage,
                [Serialization.Models.Metadata.Header.CommentKey] = item.Comment,
            };
            return header;
        }

        /// <summary>
        /// Convert from <see cref="Game"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.NameKey] = item.Name,
            };

            if (item.File != null && item.File.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.File, ConvertToInternalModel);
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="File"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(File item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.Name,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.Size,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.CRC,
                [Serialization.Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Serialization.Models.Metadata.Rom.DateKey] = item.Date,
            };
            return rom;
        }
    }
}
