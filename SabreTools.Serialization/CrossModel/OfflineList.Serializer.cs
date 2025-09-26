using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.OfflineList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : IModelSerializer<Dat, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Serialization.Models.Metadata.MetadataFile? Serialize(Dat? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Serialization.Models.Metadata.MetadataFile
            {
                [Serialization.Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Games?.Game != null && item.Games.Game.Length > 0)
            {
                metadataFile[Serialization.Models.Metadata.MetadataFile.MachineKey]
                    = Array.ConvertAll(item.Games.Game, ConvertMachineToInternalModel);
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <see cref="Dat"/> to <see cref="Serialization.Models.Metadata.Header"/>
        /// </summary>
        private static Serialization.Models.Metadata.Header ConvertHeaderToInternalModel(Dat item)
        {
            var header = new Serialization.Models.Metadata.Header
            {
                [Serialization.Models.Metadata.Header.SchemaLocationKey] = item.NoNamespaceSchemaLocation,
            };

            if (item.Configuration != null)
            {
                header[Serialization.Models.Metadata.Header.NameKey] = item.Configuration.DatName;
                header[Serialization.Models.Metadata.Header.ImFolderKey] = item.Configuration.ImFolder;
                header[Serialization.Models.Metadata.Header.DatVersionKey] = item.Configuration.DatVersion;
                header[Serialization.Models.Metadata.Header.SystemKey] = item.Configuration.System;
                header[Serialization.Models.Metadata.Header.ScreenshotsWidthKey] = item.Configuration.ScreenshotsWidth;
                header[Serialization.Models.Metadata.Header.ScreenshotsHeightKey] = item.Configuration.ScreenshotsHeight;
                header[Serialization.Models.Metadata.Header.InfosKey] = item.Configuration.Infos;
                header[Serialization.Models.Metadata.Header.CanOpenKey] = item.Configuration.CanOpen;
                header[Serialization.Models.Metadata.Header.NewDatKey] = item.Configuration.NewDat;
                header[Serialization.Models.Metadata.Header.SearchKey] = item.Configuration.Search;
                header[Serialization.Models.Metadata.Header.RomTitleKey] = item.Configuration.RomTitle;
            }

            if (item.GUI != null)
            {
                header[Serialization.Models.Metadata.Header.ImagesKey] = item.GUI.Images;
            }

            return header;
        }

        /// <summary>
        /// Convert from <see cref="Game"/> to <see cref="Serialization.Models.Metadata.Machine"/>
        /// </summary>
        private static Serialization.Models.Metadata.Machine ConvertMachineToInternalModel(Game item)
        {
            var machine = new Serialization.Models.Metadata.Machine
            {
                [Serialization.Models.Metadata.Machine.ImageNumberKey] = item.ImageNumber,
                [Serialization.Models.Metadata.Machine.ReleaseNumberKey] = item.ReleaseNumber,
                [Serialization.Models.Metadata.Machine.NameKey] = item.Title,
                [Serialization.Models.Metadata.Machine.SaveTypeKey] = item.SaveType,
                [Serialization.Models.Metadata.Machine.PublisherKey] = item.Publisher,
                [Serialization.Models.Metadata.Machine.LocationKey] = item.Location,
                [Serialization.Models.Metadata.Machine.SourceRomKey] = item.SourceRom,
                [Serialization.Models.Metadata.Machine.LanguageKey] = item.Language,
                [Serialization.Models.Metadata.Machine.Im1CRCKey] = item.Im1CRC,
                [Serialization.Models.Metadata.Machine.Im2CRCKey] = item.Im2CRC,
                [Serialization.Models.Metadata.Machine.CommentKey] = item.Comment,
                [Serialization.Models.Metadata.Machine.DuplicateIDKey] = item.DuplicateID,
            };

            if (item.Files?.RomCRC != null && item.Files.RomCRC.Length > 0)
            {
                machine[Serialization.Models.Metadata.Machine.RomKey]
                    = Array.ConvertAll(item.Files.RomCRC, romCRC =>
                        {
                            var rom = ConvertToInternalModel(romCRC);
                            rom[Serialization.Models.Metadata.Rom.SizeKey] = item.RomSize;
                            return rom;
                        });
            }

            return machine;
        }

        /// <summary>
        /// Convert from <see cref="FileRomCRC"/> to <see cref="Serialization.Models.Metadata.Rom"/>
        /// </summary>
        private static Serialization.Models.Metadata.Rom ConvertToInternalModel(FileRomCRC item)
        {
            var rom = new Serialization.Models.Metadata.Rom
            {
                [Serialization.Models.Metadata.Rom.ExtensionKey] = item.Extension,
                [Serialization.Models.Metadata.Rom.CRCKey] = item.Content,
            };
            return rom;
        }
    }
}
