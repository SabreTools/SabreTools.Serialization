using System;
using SabreTools.Data.Models.OfflineList;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : IModelSerializer<Dat, Data.Models.Metadata.MetadataFile>
    {
        /// <inheritdoc/>
        public Dat? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj == null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var dat = header != null ? ConvertHeaderFromInternalModel(header) : new Dat();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
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
        /// Convert from <see cref="Models.Metadata.Header"/> to <see cref="Dat"/>
        /// </summary>
        private static Dat ConvertHeaderFromInternalModel(Data.Models.Metadata.Header item)
        {
            var dat = new Dat
            {
                NoNamespaceSchemaLocation = item.ReadString(Data.Models.Metadata.Header.SchemaLocationKey),
            };

            if (item.ContainsKey(Data.Models.Metadata.Header.NameKey)
                || item.ContainsKey(Data.Models.Metadata.Header.ImFolderKey)
                || item.ContainsKey(Data.Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Data.Models.Metadata.Header.SystemKey)
                || item.ContainsKey(Data.Models.Metadata.Header.ScreenshotsWidthKey)
                || item.ContainsKey(Data.Models.Metadata.Header.ScreenshotsHeightKey)
                || item.ContainsKey(Data.Models.Metadata.Header.InfosKey)
                || item.ContainsKey(Data.Models.Metadata.Header.CanOpenKey)
                || item.ContainsKey(Data.Models.Metadata.Header.NewDatKey)
                || item.ContainsKey(Data.Models.Metadata.Header.SearchKey)
                || item.ContainsKey(Data.Models.Metadata.Header.RomTitleKey))
            {
                dat.Configuration = new Configuration
                {
                    DatName = item.ReadString(Data.Models.Metadata.Header.NameKey),
                    ImFolder = item.ReadString(Data.Models.Metadata.Header.ImFolderKey),
                    DatVersion = item.ReadString(Data.Models.Metadata.Header.DatVersionKey),
                    System = item.ReadString(Data.Models.Metadata.Header.SystemKey),
                    ScreenshotsWidth = item.ReadString(Data.Models.Metadata.Header.ScreenshotsWidthKey),
                    ScreenshotsHeight = item.ReadString(Data.Models.Metadata.Header.ScreenshotsHeightKey),
                    Infos = item.Read<Infos>(Data.Models.Metadata.Header.InfosKey),
                    CanOpen = item.Read<CanOpen>(Data.Models.Metadata.Header.CanOpenKey),
                    NewDat = item.Read<NewDat>(Data.Models.Metadata.Header.NewDatKey),
                    Search = item.Read<Search>(Data.Models.Metadata.Header.SearchKey),
                    RomTitle = item.ReadString(Data.Models.Metadata.Header.RomTitleKey),
                };
            }

            if (item.ContainsKey(Data.Models.Metadata.Header.ImagesKey))
            {
                dat.GUI = new GUI
                {
                    Images = item.Read<Images>(Data.Models.Metadata.Header.ImagesKey),
                };
            }

            return dat;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Machine"/> to <see cref="Game"/>
        /// </summary>
        private static Game ConvertMachineFromInternalModel(Data.Models.Metadata.Machine item)
        {
            var game = new Game
            {
                ImageNumber = item.ReadString(Data.Models.Metadata.Machine.ImageNumberKey),
                ReleaseNumber = item.ReadString(Data.Models.Metadata.Machine.ReleaseNumberKey),
                Title = item.ReadString(Data.Models.Metadata.Machine.NameKey),
                SaveType = item.ReadString(Data.Models.Metadata.Machine.SaveTypeKey),
                Publisher = item.ReadString(Data.Models.Metadata.Machine.PublisherKey),
                Location = item.ReadString(Data.Models.Metadata.Machine.LocationKey),
                SourceRom = item.ReadString(Data.Models.Metadata.Machine.SourceRomKey),
                Language = item.ReadString(Data.Models.Metadata.Machine.LanguageKey),
                Im1CRC = item.ReadString(Data.Models.Metadata.Machine.Im1CRCKey),
                Im2CRC = item.ReadString(Data.Models.Metadata.Machine.Im2CRCKey),
                Comment = item.ReadString(Data.Models.Metadata.Machine.CommentKey),
                DuplicateID = item.ReadString(Data.Models.Metadata.Machine.DuplicateIDKey),
            };

            var roms = item.Read<Data.Models.Metadata.Rom[]>(Data.Models.Metadata.Machine.RomKey);
            if (roms != null && roms.Length > 0)
            {
                var romSizes = Array.ConvertAll(roms, r => r.ReadLong(Data.Models.Metadata.Rom.SizeKey) ?? -1);
                game.RomSize = Array.Find(romSizes, s => s > -1).ToString();

                var romCRCs = Array.ConvertAll(roms, ConvertFromInternalModel);
                game.Files = new Files { RomCRC = romCRCs };
            }

            return game;
        }

        /// <summary>
        /// Convert from <see cref="Models.Metadata.Rom"/> to <see cref="FileRomCRC"/>
        /// </summary>
        private static FileRomCRC ConvertFromInternalModel(Data.Models.Metadata.Rom item)
        {
            var fileRomCRC = new FileRomCRC
            {
                Extension = item.ReadString(Data.Models.Metadata.Rom.ExtensionKey),
                Content = item.ReadString(Data.Models.Metadata.Rom.CRCKey),
            };
            return fileRomCRC;
        }
    }
}
