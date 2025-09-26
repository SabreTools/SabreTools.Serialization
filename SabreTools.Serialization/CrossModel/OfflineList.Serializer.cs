using System;
using SabreTools.Data.Models.OfflineList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : IModelSerializer<Dat, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Data.Models.Metadata.MetadataFile? Serialize(Dat? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Data.Models.Metadata.MetadataFile
            {
                [Data.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Games?.Game != null && item.Games.Game.Length > 0)
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

            if (item.Configuration != null)
            {
                header[Data.Models.Metadata.Header.NameKey] = item.Configuration.DatName;
                header[Data.Models.Metadata.Header.ImFolderKey] = item.Configuration.ImFolder;
                header[Data.Models.Metadata.Header.DatVersionKey] = item.Configuration.DatVersion;
                header[Data.Models.Metadata.Header.SystemKey] = item.Configuration.System;
                header[Data.Models.Metadata.Header.ScreenshotsWidthKey] = item.Configuration.ScreenshotsWidth;
                header[Data.Models.Metadata.Header.ScreenshotsHeightKey] = item.Configuration.ScreenshotsHeight;
                header[Data.Models.Metadata.Header.InfosKey] = item.Configuration.Infos;
                header[Data.Models.Metadata.Header.CanOpenKey] = item.Configuration.CanOpen;
                header[Data.Models.Metadata.Header.NewDatKey] = item.Configuration.NewDat;
                header[Data.Models.Metadata.Header.SearchKey] = item.Configuration.Search;
                header[Data.Models.Metadata.Header.RomTitleKey] = item.Configuration.RomTitle;
            }

            if (item.GUI != null)
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
                [Data.Models.Metadata.Machine.ImageNumberKey] = item.ImageNumber,
                [Data.Models.Metadata.Machine.ReleaseNumberKey] = item.ReleaseNumber,
                [Data.Models.Metadata.Machine.NameKey] = item.Title,
                [Data.Models.Metadata.Machine.SaveTypeKey] = item.SaveType,
                [Data.Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Data.Models.Metadata.Machine.LocationKey] = item.Location,
                [Data.Models.Metadata.Machine.SourceRomKey] = item.SourceRom,
                [Data.Models.Metadata.Machine.LanguageKey] = item.Language,
                [Data.Models.Metadata.Machine.Im1CRCKey] = item.Im1CRC,
                [Data.Models.Metadata.Machine.Im2CRCKey] = item.Im2CRC,
                [Data.Models.Metadata.Machine.CommentKey] = item.Comment,
                [Data.Models.Metadata.Machine.DuplicateIDKey] = item.DuplicateID,
            };

            if (item.Files?.RomCRC != null && item.Files.RomCRC.Length > 0)
            {
                machine[Data.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Files.RomCRC, romCRC =>
                        {
                            var rom = ConvertToInternalModel(romCRC);
                            rom[Data.Models.Metadata.Rom.SizeKey] = item.RomSize;
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
