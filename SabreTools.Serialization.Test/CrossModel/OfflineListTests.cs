using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class OfflineListTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Serialization.CrossModel.OfflineList();

            // Build the data
            Models.OfflineList.Dat dat = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(dat);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.OfflineList.Dat? newDat = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newDat);
            Assert.Equal("XXXXXX", newDat.NoNamespaceSchemaLocation);
            Validate(newDat.Configuration);
            Validate(newDat.Games);
            Validate(newDat.GUI);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.OfflineList.Dat Build()
        {
            var infos = new Models.OfflineList.Infos
            {
                Title = new Models.OfflineList.Title
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                Location = new Models.OfflineList.Location
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                Publisher = new Models.OfflineList.Publisher
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                SourceRom = new Models.OfflineList.SourceRom
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                SaveType = new Models.OfflineList.SaveType
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                RomSize = new Models.OfflineList.RomSize
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                ReleaseNumber = new Models.OfflineList.ReleaseNumber
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                ImageNumber = new Models.OfflineList.ImageNumber
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                LanguageNumber = new Models.OfflineList.LanguageNumber
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                Comment = new Models.OfflineList.Comment
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                RomCRC = new Models.OfflineList.RomCRC
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                Im1CRC = new Models.OfflineList.Im1CRC
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                Im2CRC = new Models.OfflineList.Im2CRC
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
                Languages = new Models.OfflineList.Languages
                {
                    Visible = "XXXXXX",
                    InNamingOption = "XXXXXX",
                    Default = "XXXXXX",
                },
            };

            var canopen = new Models.OfflineList.CanOpen
            {
                Extension = ["XXXXXX"],
            };

            var daturl = new Models.OfflineList.DatUrl
            {
                FileName = "XXXXXX",
                Content = "XXXXXX",
            };

            var newdat = new Models.OfflineList.NewDat
            {
                DatVersionUrl = "XXXXXX",
                DatUrl = daturl,
                ImUrl = "XXXXXX",
            };

            var find = new Models.OfflineList.Find
            {
                Operation = "XXXXXX",
                Value = "XXXXXX",
                Content = "XXXXXX",
            };

            var to = new Models.OfflineList.To
            {
                Value = "XXXXXX",
                Default = "XXXXXX",
                Auto = "XXXXXX",
                Find = [find],
            };

            var search = new Models.OfflineList.Search
            {
                To = [to],
            };

            var configuration = new Models.OfflineList.Configuration
            {
                DatName = "XXXXXX",
                ImFolder = "XXXXXX",
                DatVersion = "XXXXXX",
                System = "XXXXXX",
                ScreenshotsWidth = "XXXXXX",
                ScreenshotsHeight = "XXXXXX",
                Infos = infos,
                CanOpen = canopen,
                NewDat = newdat,
                Search = search,
                RomTitle = "XXXXXX",
            };

            var fileromcrc = new Models.OfflineList.FileRomCRC
            {
                Extension = "XXXXXX",
                Content = "XXXXXX",
            };

            var files = new Models.OfflineList.Files
            {
                RomCRC = [fileromcrc],
            };

            var game = new Models.OfflineList.Game
            {
                ImageNumber = "XXXXXX",
                ReleaseNumber = "XXXXXX",
                Title = "XXXXXX",
                SaveType = "XXXXXX",
                RomSize = "XXXXXX",
                Publisher = "XXXXXX",
                Location = "XXXXXX",
                SourceRom = "XXXXXX",
                Language = "XXXXXX",
                Files = files,
                Im1CRC = "XXXXXX",
                Im2CRC = "XXXXXX",
                Comment = "XXXXXX",
                DuplicateID = "XXXXXX",
            };

            var games = new Models.OfflineList.Games
            {
                Game = [game],
            };

            var image = new Models.OfflineList.Image
            {
                X = "XXXXXX",
                Y = "XXXXXX",
                Width = "XXXXXX",
                Height = "XXXXXX",
            };

            var images = new Models.OfflineList.Images
            {
                Width = "XXXXXX",
                Height = "XXXXXX",
                Image = [image],
            };

            var gui = new Models.OfflineList.GUI
            {
                Images = images,
            };

            return new Models.OfflineList.Dat
            {
                NoNamespaceSchemaLocation = "XXXXXX",
                Configuration = configuration,
                Games = games,
                GUI = gui,
            };
        }

        /// <summary>
        /// Validate a Configuration
        /// </summary>
        private static void Validate(Models.OfflineList.Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("XXXXXX", configuration.DatName);
            Assert.Equal("XXXXXX", configuration.ImFolder);
            Assert.Equal("XXXXXX", configuration.DatVersion);
            Assert.Equal("XXXXXX", configuration.System);
            Assert.Equal("XXXXXX", configuration.ScreenshotsWidth);
            Assert.Equal("XXXXXX", configuration.ScreenshotsHeight);
            Validate(configuration.Infos);
            Validate(configuration.CanOpen);
            Validate(configuration.NewDat);
            Validate(configuration.Search);
            Assert.Equal("XXXXXX", configuration.RomTitle);
        }

        /// <summary>
        /// Validate a Infos
        /// </summary>
        private static void Validate(Models.OfflineList.Infos? infos)
        {
            Assert.NotNull(infos);
            Validate(infos.Title);
            Validate(infos.Location);
            Validate(infos.Publisher);
            Validate(infos.SourceRom);
            Validate(infos.SaveType);
            Validate(infos.RomSize);
            Validate(infos.ReleaseNumber);
            Validate(infos.ImageNumber);
            Validate(infos.LanguageNumber);
            Validate(infos.Comment);
            Validate(infos.RomCRC);
            Validate(infos.Im1CRC);
            Validate(infos.Im2CRC);
            Validate(infos.Languages);
        }

        /// <summary>
        /// Validate a InfoBase
        /// </summary>
        private static void Validate(Models.OfflineList.InfoBase? info)
        {
            Assert.NotNull(info);
            Assert.Equal("XXXXXX", info.Visible);
            Assert.Equal("XXXXXX", info.InNamingOption);
            Assert.Equal("XXXXXX", info.Default);
        }

        /// <summary>
        /// Validate a CanOpen
        /// </summary>
        private static void Validate(Models.OfflineList.CanOpen? canopen)
        {
            Assert.NotNull(canopen);
            Assert.NotNull(canopen.Extension);
            string extension = Assert.Single(canopen.Extension);
            Assert.Equal("XXXXXX", extension);
        }

        /// <summary>
        /// Validate a NewDat
        /// </summary>
        private static void Validate(Models.OfflineList.NewDat? newdat)
        {
            Assert.NotNull(newdat);
            Assert.Equal("XXXXXX", newdat.DatVersionUrl);
            Validate(newdat.DatUrl);
            Assert.Equal("XXXXXX", newdat.ImUrl);
        }

        /// <summary>
        /// Validate a DatUrl
        /// </summary>
        private static void Validate(Models.OfflineList.DatUrl? daturl)
        {
            Assert.NotNull(daturl);
            Assert.Equal("XXXXXX", daturl.FileName);
            Assert.Equal("XXXXXX", daturl.Content);
        }

        /// <summary>
        /// Validate a Search
        /// </summary>
        private static void Validate(Models.OfflineList.Search? search)
        {
            Assert.NotNull(search);
            Assert.NotNull(search.To);
            var to = Assert.Single(search.To);
            Validate(to);
        }

        /// <summary>
        /// Validate a To
        /// </summary>
        private static void Validate(Models.OfflineList.To? to)
        {
            Assert.NotNull(to);
            Assert.Equal("XXXXXX", to.Value);
            Assert.Equal("XXXXXX", to.Default);
            Assert.Equal("XXXXXX", to.Auto);

            Assert.NotNull(to.Find);
            var find = Assert.Single(to.Find);
            Validate(find);
        }

        /// <summary>
        /// Validate a Find
        /// </summary>
        private static void Validate(Models.OfflineList.Find? find)
        {
            Assert.NotNull(find);
            Assert.Equal("XXXXXX", find.Operation);
            Assert.Equal("XXXXXX", find.Value);
            Assert.Equal("XXXXXX", find.Content);
        }

        /// <summary>
        /// Validate a Games
        /// </summary>
        private static void Validate(Models.OfflineList.Games? games)
        {
            Assert.NotNull(games);
            Assert.NotNull(games.Game);
            var game = Assert.Single(games.Game);
            Validate(game);
        }

        /// <summary>
        /// Validate a Game
        /// </summary>
        private static void Validate(Models.OfflineList.Game? game)
        {
            Assert.NotNull(game);
            Assert.Equal("XXXXXX", game.ImageNumber);
            Assert.Equal("XXXXXX", game.ReleaseNumber);
            Assert.Equal("XXXXXX", game.Title);
            Assert.Equal("XXXXXX", game.SaveType);
            Assert.Equal("0", game.RomSize); // Converted due to filtering
            Assert.Equal("XXXXXX", game.Publisher);
            Assert.Equal("XXXXXX", game.Location);
            Assert.Equal("XXXXXX", game.SourceRom);
            Assert.Equal("XXXXXX", game.Language);
            Validate(game.Files);
            Assert.Equal("XXXXXX", game.Im1CRC);
            Assert.Equal("XXXXXX", game.Im2CRC);
            Assert.Equal("XXXXXX", game.Comment);
            Assert.Equal("XXXXXX", game.DuplicateID);
        }

        /// <summary>
        /// Validate a Files
        /// </summary>
        private static void Validate(Models.OfflineList.Files? files)
        {
            Assert.NotNull(files);
            Assert.NotNull(files.RomCRC);
            var fileromcrc = Assert.Single(files.RomCRC);
            Validate(fileromcrc);
        }

        /// <summary>
        /// Validate a FileRomCRC
        /// </summary>
        private static void Validate(Models.OfflineList.FileRomCRC? fileromcrc)
        {
            Assert.NotNull(fileromcrc);
            Assert.Equal("XXXXXX", fileromcrc.Extension);
            Assert.Equal("XXXXXX", fileromcrc.Content);
        }

        /// <summary>
        /// Validate a GUI
        /// </summary>
        private static void Validate(Models.OfflineList.GUI? gui)
        {
            Assert.NotNull(gui);
            Validate(gui.Images);
        }

        /// <summary>
        /// Validate a Images
        /// </summary>
        private static void Validate(Models.OfflineList.Images? images)
        {
            Assert.NotNull(images);
            Assert.Equal("XXXXXX", images.Width);
            Assert.Equal("XXXXXX", images.Height);

            Assert.NotNull(images.Image);
            var image = Assert.Single(images.Image);
            Validate(image);
        }

        /// <summary>
        /// Validate a Image
        /// </summary>
        private static void Validate(Models.OfflineList.Image? image)
        {
            Assert.NotNull(image);
            Assert.Equal("XXXXXX", image.X);
            Assert.Equal("XXXXXX", image.Y);
            Assert.Equal("XXXXXX", image.Width);
            Assert.Equal("XXXXXX", image.Height);
        }
    }
}