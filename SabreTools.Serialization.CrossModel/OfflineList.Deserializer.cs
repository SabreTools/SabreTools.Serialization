using System;
using SabreTools.Data.Models.OfflineList;

namespace SabreTools.Serialization.CrossModel
{
    public partial class OfflineList : BaseMetadataSerializer<Dat>
    {
        /// <inheritdoc/>
        public override Dat? Deserialize(Data.Models.Metadata.MetadataFile? obj)
        {
            if (obj is null)
                return null;

            var header = obj.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            var dat = header is not null ? ConvertHeaderFromInternalModel(header) : new Dat();

            var machines = obj.Read<Data.Models.Metadata.Machine[]>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines is not null && machines.Length > 0)
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

            if (item.Name is not null
                || item.ContainsKey(Data.Models.Metadata.Header.ImFolderKey)
                || item.DatVersion is not null
                || item.System is not null
                || item.ScreenshotsHeight is not null
                || item.ScreenshotsWidth is not null
                || item.ContainsKey(Data.Models.Metadata.Header.InfosKey)
                || item.ContainsKey(Data.Models.Metadata.Header.CanOpenKey)
                || item.ContainsKey(Data.Models.Metadata.Header.NewDatKey)
                || item.ContainsKey(Data.Models.Metadata.Header.SearchKey)
                || item.RomTitle is not null)
            {
                dat.Configuration = new Configuration
                {
                    DatName = item.Name,
                    ImFolder = item.ReadString(Data.Models.Metadata.Header.ImFolderKey),
                    DatVersion = item.DatVersion,
                    System = item.System,
                    ScreenshotsWidth = item.ScreenshotsHeight,
                    ScreenshotsHeight = item.ScreenshotsWidth,
                    Infos = item.Read<Infos>(Data.Models.Metadata.Header.InfosKey),
                    CanOpen = item.Read<CanOpen>(Data.Models.Metadata.Header.CanOpenKey),
                    NewDat = item.Read<NewDat>(Data.Models.Metadata.Header.NewDatKey),
                    Search = item.Read<Search>(Data.Models.Metadata.Header.SearchKey),
                    RomTitle = item.RomTitle,
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
                ImageNumber = item.ImageNumber,
                ReleaseNumber = item.ReleaseNumber,
                Title = item.Name,
                SaveType = item.SaveType,
                Publisher = item.Publisher,
                Location = item.Location,
                SourceRom = item.SourceRom,
                Language = item.Language,
                Im1CRC = item.Im1CRC,
                Im2CRC = item.Im2CRC,
                Comment = item.Comment is null ? null : string.Join(", ", item.Comment),
                DuplicateID = item.DuplicateID,
            };

            var roms = item.Rom;
            if (roms is not null && roms.Length > 0)
            {
                var romSizes = Array.ConvertAll(roms, r => r.Size ?? -1);
                game.RomSize = Array.Find(romSizes, s => s > -1);

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
