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

            var header = obj.Header;
            var dat = header is not null ? ConvertHeaderFromInternalModel(header) : new Dat();

            var machines = obj.Machine;
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
                NoNamespaceSchemaLocation = item.SchemaLocation,
            };

            if (item.Name is not null
                || item.ImFolder is not null
                || item.DatVersion is not null
                || item.System is not null
                || item.ScreenshotsHeight is not null
                || item.ScreenshotsWidth is not null
                || item.Infos is not null
                || item.CanOpen is not null
                || item.NewDat is not null
                || item.Search is not null
                || item.RomTitle is not null)
            {
                dat.Configuration = new Configuration
                {
                    DatName = item.Name,
                    ImFolder = item.ImFolder,
                    DatVersion = item.DatVersion,
                    System = item.System,
                    ScreenshotsWidth = item.ScreenshotsHeight,
                    ScreenshotsHeight = item.ScreenshotsWidth,
                    Infos = item.Infos,
                    CanOpen = item.CanOpen,
                    NewDat = item.NewDat,
                    Search = item.Search,
                    RomTitle = item.RomTitle,
                };
            }

            if (item.Images is not null)
                dat.GUI = new GUI { Images = item.Images };

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
                Extension = item.Extension,
                Content = item.CRC32,
            };
            return fileRomCRC;
        }
    }
}
