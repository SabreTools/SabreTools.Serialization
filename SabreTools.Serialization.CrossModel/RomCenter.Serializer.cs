using System;
using SabreTools.Data.Models.RomCenter;

namespace SabreTools.Serialization.CrossModel
{
    public partial class RomCenter : BaseMetadataSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(obj),
            };

            if (obj?.Games?.Rom is not null && obj.Games.Rom.Length > 0)
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

            if (item.Credits is not null)
            {
                header.Author = item.Credits.Author;
                header.Version = item.Credits.Version;
                header.Email = item.Credits.Email;
                header.Homepage = item.Credits.Homepage;
                header.Url = item.Credits.Url;
                header.Date = item.Credits.Date;
                header.Comment = item.Credits.Comment;
            }

            if (item.Dat is not null)
            {
                header.DatVersion = item.Dat.Version;
                header.Plugin = item.Dat.Plugin;

                if (item.Dat.Split == "yes" || item.Dat.Split == "1")
                    header.ForceMerging = Data.Models.Metadata.MergingFlag.Split;
                else if (item.Dat.Merge == "yes" || item.Dat.Merge == "1")
                    header.ForceMerging = Data.Models.Metadata.MergingFlag.Merged;
            }

            if (item.Emulator is not null)
            {
                header.RefName = item.Emulator.RefName;
                header.EmulatorVersion = item.Emulator.Version;
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
                RomOf = item.RomOf,
                CloneOf = item.ParentName,
                //[Data.Models.Metadata.Machine.ParentDescriptionKey] = item.ParentDescription, // This is unmappable
                Name = item.GameName,
                Description = item.GameDescription,
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
                Name = item.RomName,
                [Data.Models.Metadata.Rom.CRCKey] = item.RomCRC,
                Size = item.RomSize,
                [Data.Models.Metadata.Rom.MergeKey] = item.MergeName,
            };
            return rom;
        }
    }
}
