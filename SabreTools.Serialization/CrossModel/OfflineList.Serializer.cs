using System.Linq;
using SabreTools.Models.OfflineList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : IModelSerializer<Dat, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Models.Metadata.MetadataFile? Serialize(Dat? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Games?.Game != null && item.Games.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Games.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Dat"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Dat item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.SchemaLocationKey] = item.NoNamespaceSchemaLocation,
            };

            if (item.Configuration != null)
            {
                header[Models.Metadata.Header.NameKey] = item.Configuration.DatName;
                header[Models.Metadata.Header.ImFolderKey] = item.Configuration.ImFolder;
                header[Models.Metadata.Header.DatVersionKey] = item.Configuration.DatVersion;
                header[Models.Metadata.Header.SystemKey] = item.Configuration.System;
                header[Models.Metadata.Header.ScreenshotsWidthKey] = item.Configuration.ScreenshotsWidth;
                header[Models.Metadata.Header.ScreenshotsHeightKey] = item.Configuration.ScreenshotsHeight;
                header[Models.Metadata.Header.InfosKey] = item.Configuration.Infos;
                header[Models.Metadata.Header.CanOpenKey] = item.Configuration.CanOpen;
                header[Models.Metadata.Header.NewDatKey] = item.Configuration.NewDat;
                header[Models.Metadata.Header.SearchKey] = item.Configuration.Search;
                header[Models.Metadata.Header.RomTitleKey] = item.Configuration.RomTitle;
            }

            if (item.GUI != null)
            {
                header[Models.Metadata.Header.ImagesKey] = item.GUI.Images;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Game"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.ImageNumberKey] = item.ImageNumber,
                [Models.Metadata.Machine.ReleaseNumberKey] = item.ReleaseNumber,
                [Models.Metadata.Machine.NameKey] = item.Title,
                [Models.Metadata.Machine.SaveTypeKey] = item.SaveType,
                [Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Models.Metadata.Machine.LocationKey] = item.Location,
                [Models.Metadata.Machine.SourceRomKey] = item.SourceRom,
                [Models.Metadata.Machine.LanguageKey] = item.Language,
                [Models.Metadata.Machine.Im1CRCKey] = item.Im1CRC,
                [Models.Metadata.Machine.Im2CRCKey] = item.Im2CRC,
                [Models.Metadata.Machine.CommentKey] = item.Comment,
                [Models.Metadata.Machine.DuplicateIDKey] = item.DuplicateID,
            };

            if (item.Files?.RomCRC != null && item.Files.RomCRC.Any())
            {
                machine[Models.Metadata.Machine.RomKey] = item.Files.RomCRC
                    .Where(r => r != null)
                    .Select(romCRC =>
                    {
                        var rom = ConvertToInternalModel(romCRC);
                        rom[Models.Metadata.Rom.SizeKey] = item.RomSize;
                        return rom;
                    })
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.FileRomCRC"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(FileRomCRC item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.ExtensionKey] = item.Extension,
                [Models.Metadata.Rom.CRCKey] = item.Content,
            };
            return rom;
        }
    }
}