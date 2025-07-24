using System;
using SabreTools.Models.RomCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : IModelSerializer<MetadataFile, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Games?.Rom != null && obj.Games.Rom.Length > 0)
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Games.Rom, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Models.RomCenter.MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Models.Metadata.Header();

            if (item.Credits != null)
            {
                header[Models.Metadata.Header.AuthorKey] = item.Credits.Author;
                header[Models.Metadata.Header.VersionKey] = item.Credits.Version;
                header[Models.Metadata.Header.EmailKey] = item.Credits.Email;
                header[Models.Metadata.Header.HomepageKey] = item.Credits.Homepage;
                header[Models.Metadata.Header.UrlKey] = item.Credits.Url;
                header[Models.Metadata.Header.DateKey] = item.Credits.Date;
                header[Models.Metadata.Header.CommentKey] = item.Credits.Comment;
            }

            if (item.Dat != null)
            {
                header[Models.Metadata.Header.DatVersionKey] = item.Dat.Version;
                header[Models.Metadata.Header.PluginKey] = item.Dat.Plugin;

                if (item.Dat.Split == "yes" || item.Dat.Split == "1")
                    header[Models.Metadata.Header.ForceMergingKey] = "split";
                else if (item.Dat.Merge == "yes" || item.Dat.Merge == "1")
                    header[Models.Metadata.Header.ForceMergingKey] = "merge";
            }

            if (item.Emulator != null)
            {
                header[Models.Metadata.Header.RefNameKey] = item.Emulator.RefName;
                header[Models.Metadata.Header.EmulatorVersionKey] = item.Emulator.Version;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Models.RomCenter.Game"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Rom item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Models.Metadata.Machine.CloneOfKey] = item.ParentName,
                //[Models.Metadata.Machine.ParentDescriptionKey] = item.ParentDescription, // This is unmappable
                [Models.Metadata.Machine.NameKey] = item.GameName,
                [Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
                [Models.Metadata.Machine.RomKey] = new Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Models.RomCenter.Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.RomName,
                [Models.Metadata.Rom.CRCKey] = item.RomCRC,
                [Models.Metadata.Rom.SizeKey] = item.RomSize,
                [Models.Metadata.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }
    }
}
