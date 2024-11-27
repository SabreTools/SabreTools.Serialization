using System;
using SabreTools.Models.OfflineList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : IModelSerializer<Dat, Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Dat? Deserialize(Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var dat = header != null ? ConvertHeaderFromInternalModel(header) : new Dat();

            var machines = obj.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
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
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.OfflineList.Dat"/>
        /// </summary>
        private static Dat ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var dat = new Dat
            {
                NoNamespaceSchemaLocation = item.ReadString(Models.Metadata.Header.SchemaLocationKey),
            };

            if (item.ContainsKey(Models.Metadata.Header.NameKey)
                || item.ContainsKey(Models.Metadata.Header.ImFolderKey)
                || item.ContainsKey(Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Models.Metadata.Header.SystemKey)
                || item.ContainsKey(Models.Metadata.Header.ScreenshotsWidthKey)
                || item.ContainsKey(Models.Metadata.Header.ScreenshotsHeightKey)
                || item.ContainsKey(Models.Metadata.Header.InfosKey)
                || item.ContainsKey(Models.Metadata.Header.CanOpenKey)
                || item.ContainsKey(Models.Metadata.Header.NewDatKey)
                || item.ContainsKey(Models.Metadata.Header.SearchKey)
                || item.ContainsKey(Models.Metadata.Header.RomTitleKey))
            {
                dat.Configuration = new Configuration
                {
                    DatName = item.ReadString(Models.Metadata.Header.NameKey),
                    ImFolder = item.ReadString(Models.Metadata.Header.ImFolderKey),
                    DatVersion = item.ReadString(Models.Metadata.Header.DatVersionKey),
                    System = item.ReadString(Models.Metadata.Header.SystemKey),
                    ScreenshotsWidth = item.ReadString(Models.Metadata.Header.ScreenshotsWidthKey),
                    ScreenshotsHeight = item.ReadString(Models.Metadata.Header.ScreenshotsHeightKey),
                    Infos = item.Read<Infos>(Models.Metadata.Header.InfosKey),
                    CanOpen = item.Read<CanOpen>(Models.Metadata.Header.CanOpenKey),
                    NewDat = item.Read<NewDat>(Models.Metadata.Header.NewDatKey),
                    Search = item.Read<Search>(Models.Metadata.Header.SearchKey),
                    RomTitle = item.ReadString(Models.Metadata.Header.RomTitleKey),
                };
            }

            if (item.ContainsKey(Models.Metadata.Header.ImagesKey))
            {
                dat.GUI = new GUI
                {
                    Images = item.Read<Images>(Models.Metadata.Header.ImagesKey),
                };
            }

            return dat;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.OfflineList.Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var game = new Game
            {
                ImageNumber = item.ReadString(Models.Metadata.Machine.ImageNumberKey),
                ReleaseNumber = item.ReadString(Models.Metadata.Machine.ReleaseNumberKey),
                Title = item.ReadString(Models.Metadata.Machine.NameKey),
                SaveType = item.ReadString(Models.Metadata.Machine.SaveTypeKey),
                Publisher = item.ReadString(Models.Metadata.Machine.PublisherKey),
                Location = item.ReadString(Models.Metadata.Machine.LocationKey),
                SourceRom = item.ReadString(Models.Metadata.Machine.SourceRomKey),
                Language = item.ReadString(Models.Metadata.Machine.LanguageKey),
                Im1CRC = item.ReadString(Models.Metadata.Machine.Im1CRCKey),
                Im2CRC = item.ReadString(Models.Metadata.Machine.Im2CRCKey),
                Comment = item.ReadString(Models.Metadata.Machine.CommentKey),
                DuplicateID = item.ReadString(Models.Metadata.Machine.DuplicateIDKey),
            };

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                var romSizes = Array.ConvertAll(roms, r => r.ReadLong(Models.Metadata.Rom.SizeKey) ?? -1);
                game.RomSize = Array.Find(romSizes, s => s > -1).ToString();

                var romCRCs = Array.ConvertAll(roms, ConvertFromInternalModel);;
                game.Files = new Files { RomCRC = romCRCs };
            }

            return game;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.OfflineList.FileRomCRC"/>
        /// </summary>
        private static FileRomCRC ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var fileRomCRC = new FileRomCRC
            {
                Extension = item.ReadString(Models.Metadata.Rom.ExtensionKey),
                Content = item.ReadString(Models.Metadata.Rom.CRCKey),
            };
            return fileRomCRC;
        }
    }
}