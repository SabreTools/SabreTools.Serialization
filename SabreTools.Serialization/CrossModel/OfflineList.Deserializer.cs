using System;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.OfflineList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : IModelSerializer<Dat, Serialization.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Dat? Deserialize(Serialization.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Serialization.Models.Metadata.Header>(Serialization.Models.Metadata.MetadataFile.HeaderKey);
            var dat = header != null ? ConvertHeaderFromInternalModel(header) : new Dat();

            var machines = obj.Read<Serialization.Models.Metadata.Machine[]>(Serialization.Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Length > 0)
            {
                dat.Games = new Games
                {
                    Game = Array.ConvertAll(machines, ConvertMachineFromInternalModel),
                };
            }

            return dat;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Header"/> to <see cref="Dat"/>
        /// </summary>
        private static Dat ConvertHeaderFromInternalModel(Serialization.Models.Metadata.Header item)
        {
            var dat = new Dat
            {
                NoNamespaceSchemaLocation = item.ReadString(Serialization.Models.Metadata.Header.SchemaLocationKey),
            };

            if (item.ContainsKey(Serialization.Models.Metadata.Header.NameKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.ImFolderKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.SystemKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.ScreenshotsWidthKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.ScreenshotsHeightKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.InfosKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.CanOpenKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.NewDatKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.SearchKey)
                || item.ContainsKey(Serialization.Models.Metadata.Header.RomTitleKey))
            {
                dat.Configuration = new Configuration
                {
                    DatName = item.ReadString(Serialization.Models.Metadata.Header.NameKey),
                    ImFolder = item.ReadString(Serialization.Models.Metadata.Header.ImFolderKey),
                    DatVersion = item.ReadString(Serialization.Models.Metadata.Header.DatVersionKey),
                    System = item.ReadString(Serialization.Models.Metadata.Header.SystemKey),
                    ScreenshotsWidth = item.ReadString(Serialization.Models.Metadata.Header.ScreenshotsWidthKey),
                    ScreenshotsHeight = item.ReadString(Serialization.Models.Metadata.Header.ScreenshotsHeightKey),
                    Infos = item.Read<Infos>(Serialization.Models.Metadata.Header.InfosKey),
                    CanOpen = item.Read<CanOpen>(Serialization.Models.Metadata.Header.CanOpenKey),
                    NewDat = item.Read<NewDat>(Serialization.Models.Metadata.Header.NewDatKey),
                    Search = item.Read<Search>(Serialization.Models.Metadata.Header.SearchKey),
                    RomTitle = item.ReadString(Serialization.Models.Metadata.Header.RomTitleKey),
                };
            }

            if (item.ContainsKey(Serialization.Models.Metadata.Header.ImagesKey))
            {
                dat.GUI = new GUI
                {
                    Images = item.Read<Images>(Serialization.Models.Metadata.Header.ImagesKey),
                };
            }

            return dat;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Machine"/> to <see cref="Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Serialization.Models.Metadata.Machine item)
        {
            var game = new Game
            {
                ImageNumber = item.ReadString(Serialization.Models.Metadata.Machine.ImageNumberKey),
                ReleaseNumber = item.ReadString(Serialization.Models.Metadata.Machine.ReleaseNumberKey),
                Title = item.ReadString(Serialization.Models.Metadata.Machine.NameKey),
                SaveType = item.ReadString(Serialization.Models.Metadata.Machine.SaveTypeKey),
                Publisher = item.ReadString(Serialization.Models.Metadata.Machine.PublisherKey),
                Location = item.ReadString(Serialization.Models.Metadata.Machine.LocationKey),
                SourceRom = item.ReadString(Serialization.Models.Metadata.Machine.SourceRomKey),
                Language = item.ReadString(Serialization.Models.Metadata.Machine.LanguageKey),
                Im1CRC = item.ReadString(Serialization.Models.Metadata.Machine.Im1CRCKey),
                Im2CRC = item.ReadString(Serialization.Models.Metadata.Machine.Im2CRCKey),
                Comment = item.ReadString(Serialization.Models.Metadata.Machine.CommentKey),
                DuplicateID = item.ReadString(Serialization.Models.Metadata.Machine.DuplicateIDKey),
            };

            var roms = item.Read<Serialization.Models.Metadata.Rom[]>(Serialization.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                var romSizes = Array.ConvertAll(roms, r => r.ReadLong(Serialization.Models.Metadata.Rom.SizeKey) ?? -1);
                game.RomSize = Array.Find(romSizes, s => s > -1).ToString();

                var romCRCs = Array.ConvertAll(roms, ConvertFromInternalModel);
                game.Files = new Files { RomCRC = romCRCs };
            }

            return game;
        }

        /// <summary>
        /// Convert from <see cref="Serialization.Models.Metadata.Rom"/> to <see cref="FileRomCRC"/>
        /// </summary>
        private static FileRomCRC ConvertFromInternalModel(Serialization.Models.Metadata.Rom item)
        {
            var fileRomCRC = new FileRomCRC
            {
                Extension = item.ReadString(Serialization.Models.Metadata.Rom.ExtensionKey),
                Content = item.ReadString(Serialization.Models.Metadata.Rom.CRCKey),
            };
            return fileRomCRC;
        }
    }
}
