using System;
using SabreTools.Data.Models.RomCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : ICrossModel<MetadataFile, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Games?.Rom != null && obj.Games.Rom.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(obj.Games.Rom, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="MetadataFile"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(MetadataFile item)
        {
            var header = new Data.Models.Metadata.Header();

            if (item.Credits != null)
            {
                header[Data.Models.Metadata.Header.AuthorKey] = item.Credits.Author;
                header[Data.Models.Metadata.Header.VersionKey] = item.Credits.Version;
                header[Data.Models.Metadata.Header.EmailKey] = item.Credits.Email;
                header[Data.Models.Metadata.Header.HomepageKey] = item.Credits.Homepage;
                header[Data.Models.Metadata.Header.UrlKey] = item.Credits.Url;
                header[Data.Models.Metadata.Header.DateKey] = item.Credits.Date;
                header[Data.Models.Metadata.Header.CommentKey] = item.Credits.Comment;
            }

            if (item.Dat != null)
            {
                header[Data.Models.Metadata.Header.DatVersionKey] = item.Dat.Version;
                header[Data.Models.Metadata.Header.PluginKey] = item.Dat.Plugin;

                if (item.Dat.Split == "yes" || item.Dat.Split == "1")
                    header[Data.Models.Metadata.Header.ForceMergingKey] = "split";
                else if (item.Dat.Merge == "yes" || item.Dat.Merge == "1")
                    header[Data.Models.Metadata.Header.ForceMergingKey] = "merge";
            }

            if (item.Emulator != null)
            {
                header[Data.Models.Metadata.Header.RefNameKey] = item.Emulator.RefName;
                header[Data.Models.Metadata.Header.EmulatorVersionKey] = item.Emulator.Version;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Game"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Rom item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                [Data.Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Data.Models.Metadata.Machine.CloneOfKey] = item.ParentName,
                //[Data.Models.Metadata.Machine.ParentDescriptionKey] = item.ParentDescription, // This is unmappable
                [Data.Models.Metadata.Machine.NameKey] = item.GameName,
                [Data.Models.Metadata.Machine.DescriptionKey] = item.GameDescription,
                [Data.Models.Metadata.Machine.RomKey] = new Data.Models.Metadata.Rom[] { ConvertToInternalModel(item) },
            };

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="Rom"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.NameKey] = item.RomName,
                [Data.Models.Metadata.Rom.CRCKey] = item.RomCRC,
                [Data.Models.Metadata.Rom.SizeKey] = item.RomSize,
                [Data.Models.Metadata.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }
    }
}
