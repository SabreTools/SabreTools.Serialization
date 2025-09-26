using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.RomCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : IModelSerializer<MetadataFile, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Games?.Rom != null && obj.Games.Rom.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Games.Rom, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Serialization.Models.Metadata.Header();

            if (item.Credits != null)
            {
                header[Serialization.Models.Metadata.Header.AuthorKey] = item.Credits.Author;
                header[Serialization.Models.Metadata.Header.VersionKey] = item.Credits.Version;
                header[Serialization.Models.Metadata.Header.EmailKey] = item.Credits.Email;
                header[Serialization.Models.Metadata.Header.HomepageKey] = item.Credits.Homepage;
                header[Serialization.Models.Metadata.Header.UrlKey] = item.Credits.Url;
                header[Serialization.Models.Metadata.Header.DateKey] = item.Credits.Date;
                header[Serialization.Models.Metadata.Header.CommentKey] = item.Credits.Comment;
            }

            if (item.Dat != null)
            {
                header[Serialization.Models.Metadata.Header.DatVersionKey] = item.Dat.Version;
                header[Serialization.Models.Metadata.Header.PluginKey] = item.Dat.Plugin;

                if (item.Dat.Split == "yes" || item.Dat.Split == "1")
                    header[Serialization.Models.Metadata.Header.ForceMergingKey] = "split";
                else if (item.Dat.Merge == "yes" || item.Dat.Merge == "1")
                    header[Serialization.Models.Metadata.Header.ForceMergingKey] = "merge";
            }

            if (item.Emulator != null)
            {
                header[Serialization.Models.Metadata.Header.RefNameKey] = item.Emulator.RefName;
                header[Serialization.Models.Metadata.Header.EmulatorVersionKey] = item.Emulator.Version;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Game"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Rom item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Serialization.Models.Metadata.Machine.CloneOfKey] = item.ParentName,
                //[Serialization.Models.Metadata.Machine.ParentDescriptionKey] = item.ParentDescription, // This is unmappable
                [Serialization.Models.Metadata.Machine.NameKey] = item.GameName,
                [Serialization.Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
                [Serialization.Models.Metadata.Machine.RomKey] = new Serialization.Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.NameKey] = item.RomName,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.RomCRC,
                [Serialization.Models.Metadata.Rom.SizeKey] = item.RomSize,
                [Serialization.Models.Metadata.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }
    }
}
