using System;
using SabreTools.Data.Models.OfflineList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : BaseMetadataSerializer<Dat>
    {
        /// <inheritdoc/>
        public override Data.Models.Metadata.MetadataFile? Serialize(Dat? item)
        {
            if (item is null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Games?.Game is not null && item.Games.Game.Length > 0)
            {
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Games.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Dat"/> to <see cref="Models.Metadata.Header"/>
        /// </summary>
        private static Data.Models.Metadata.Header ConvertHeaderToInternalModel(Dat item)
        {
            var header = new Data.Models.Metadata.Header
            {
                [Data.Models.Metadata.Header.SchemaLocationKey] = item.NoNamespaceSchemaLocation,
            };

            if (item.Configuration is not null)
            {
                header.Name =  item.Configuration.DatName;
                header[Data.Models.Metadata.Header.ImFolderKey] = item.Configuration.ImFolder;
                header.DatVersion = item.Configuration.DatVersion;
                header.System = item.Configuration.System;
                header.ScreenshotsHeight = item.Configuration.ScreenshotsWidth;
                header.ScreenshotsWidth = item.Configuration.ScreenshotsHeight;
                header[Data.Models.Metadata.Header.InfosKey] = item.Configuration.Infos;
                header[Data.Models.Metadata.Header.CanOpenKey] = item.Configuration.CanOpen;
                header[Data.Models.Metadata.Header.NewDatKey] = item.Configuration.NewDat;
                header[Data.Models.Metadata.Header.SearchKey] = item.Configuration.Search;
                header.RomTitle = item.Configuration.RomTitle;
            }

            if (item.GUI is not null)
            {
                header[Data.Models.Metadata.Header.ImagesKey] = item.GUI.Images;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Game"/> to <see cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Data.Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Data.Models.Metadata.Machine
            {
                ImageNumber = item.ImageNumber,
                ReleaseNumber = item.ReleaseNumber,
                Name = item.Title,
                SaveType = item.SaveType,
                Publisher = item.Publisher,
                Location = item.Location,
                SourceRom = item.SourceRom,
                Language = item.Language,
                Im1CRC = item.Im1CRC,
                Im2CRC = item.Im2CRC,
                Comment = item.Comment is null ? null : [item.Comment],
                DuplicateID = item.DuplicateID,
            };

            if (item.Files?.RomCRC is not null && item.Files.RomCRC.Length > 0)
            {
                machine.Rom
                    = Array.ConvertAll(item.Files.RomCRC, romCRC =>
                        {
                            var rom = ConvertToInternalModel(romCRC);
                            rom.Size = item.RomSize;
                            return rom;
                        });
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="FileRomCRC"/> to <see cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Data.Models.Metadata.Rom ConvertToInternalModel(FileRomCRC item)
        {
            var rom = new Data.Models.Metadata.Rom
            {
                [Data.Models.Metadata.Rom.ExtensionKey] = item.Extension,
                [Data.Models.Metadata.Rom.CRCKey] = item.Content,
            };
            return rom;
        }
    }
}
