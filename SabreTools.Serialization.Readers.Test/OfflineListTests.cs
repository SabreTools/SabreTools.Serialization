using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class OfflineListTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new OfflineList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new OfflineList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new OfflineList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new OfflineList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new OfflineList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new OfflineList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new OfflineList();
            var serializer = new Writers.OfflineList();

            // Build the data
            Data.Models.OfflineList.Dat dat = Build();

            // Serialize to stream
            Stream? metadata = serializer.SerializeStream(dat);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.OfflineList.Dat? newDat = deserializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newDat);
            // Assert.Equal("nonamespaceschemalocation", newDat.NoNamespaceSchemaLocation);  // TODO: Fix this based on schema
            Validate(newDat.Configuration);
            Validate(newDat.Games);
            Validate(newDat.GUI);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.OfflineList.Dat Build()
        {
            var infos = new Data.Models.OfflineList.Infos
            {
                Title = new Data.Models.OfflineList.Title
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                Location = new Data.Models.OfflineList.Location
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                Publisher = new Data.Models.OfflineList.Publisher
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                SourceRom = new Data.Models.OfflineList.SourceRom
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                SaveType = new Data.Models.OfflineList.SaveType
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                RomSize = new Data.Models.OfflineList.RomSize
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                ReleaseNumber = new Data.Models.OfflineList.ReleaseNumber
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                ImageNumber = new Data.Models.OfflineList.ImageNumber
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                LanguageNumber = new Data.Models.OfflineList.LanguageNumber
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                Comment = new Data.Models.OfflineList.Comment
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                RomCRC = new Data.Models.OfflineList.RomCRC
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                Im1CRC = new Data.Models.OfflineList.Im1CRC
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                Im2CRC = new Data.Models.OfflineList.Im2CRC
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
                Languages = new Data.Models.OfflineList.Languages
                {
                    Visible = "visible",
                    InNamingOption = "innamingoption",
                    Default = true,
                },
            };

            var canopen = new Data.Models.OfflineList.CanOpen
            {
                Extension = ["ext"],
            };

            var daturl = new Data.Models.OfflineList.DatUrl
            {
                FileName = "filename",
                Content = "content",
            };

            var newdat = new Data.Models.OfflineList.NewDat
            {
                DatVersionUrl = "datversionurl",
                DatUrl = daturl,
                ImUrl = "imurl",
            };

            var find = new Data.Models.OfflineList.Find
            {
                Operation = "operation",
                Value = "value",
                Content = "content",
            };

            var to = new Data.Models.OfflineList.To
            {
                Value = "value",
                Default = true,
                Auto = "auto",
                Find = [find],
            };

            var search = new Data.Models.OfflineList.Search
            {
                To = [to],
            };

            var configuration = new Data.Models.OfflineList.Configuration
            {
                DatName = "datname",
                ImFolder = "imfolder",
                DatVersion = "datversion",
                System = "system",
                ScreenshotsWidth = "screenshotswidth",
                ScreenshotsHeight = "screenshotsheight",
                Infos = infos,
                CanOpen = canopen,
                NewDat = newdat,
                Search = search,
                RomTitle = "romtitle",
            };

            var fileromcrc = new Data.Models.OfflineList.FileRomCRC
            {
                Extension = "extension",
                Content = "content",
            };

            var files = new Data.Models.OfflineList.Files
            {
                RomCRC = [fileromcrc],
            };

            var game = new Data.Models.OfflineList.Game
            {
                ImageNumber = "imagenumber",
                ReleaseNumber = "releasenumber",
                Title = "title",
                SaveType = "savetype",
                RomSize = 12345,
                Publisher = "publisher",
                Location = "location",
                SourceRom = "sourcerom",
                Language = "language",
                Files = files,
                Im1CRC = "im1crc",
                Im2CRC = "im2crc",
                Comment = "comment",
                DuplicateID = "duplicateid",
            };

            var games = new Data.Models.OfflineList.Games
            {
                Game = [game],
            };

            var image = new Data.Models.OfflineList.Image
            {
                X = "x",
                Y = "y",
                Width = "width",
                Height = "height",
            };

            var images = new Data.Models.OfflineList.Images
            {
                Width = "width",
                Height = "height",
                Image = [image],
            };

            var gui = new Data.Models.OfflineList.GUI
            {
                Images = images,
            };

            return new Data.Models.OfflineList.Dat
            {
                NoNamespaceSchemaLocation = "nonamespaceschemalocation",
                Configuration = configuration,
                Games = games,
                GUI = gui,
            };
        }

        /// <summary>
        /// Validate a Configuration
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Configuration? configuration)
        {
            Assert.NotNull(configuration);
            Assert.Equal("datname", configuration.DatName);
            Assert.Equal("imfolder", configuration.ImFolder);
            Assert.Equal("datversion", configuration.DatVersion);
            Assert.Equal("system", configuration.System);
            Assert.Equal("screenshotswidth", configuration.ScreenshotsWidth);
            Assert.Equal("screenshotsheight", configuration.ScreenshotsHeight);
            Validate(configuration.Infos);
            Validate(configuration.CanOpen);
            Validate(configuration.NewDat);
            Validate(configuration.Search);
            Assert.Equal("romtitle", configuration.RomTitle);
        }

        /// <summary>
        /// Validate a Infos
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Infos? infos)
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
        private static void Validate(Data.Models.OfflineList.InfoBase? info)
        {
            Assert.NotNull(info);
            Assert.Equal("visible", info.Visible);
            Assert.Equal("innamingoption", info.InNamingOption);
            Assert.Equal(true, info.Default);
        }

        /// <summary>
        /// Validate a CanOpen
        /// </summary>
        private static void Validate(Data.Models.OfflineList.CanOpen? canopen)
        {
            Assert.NotNull(canopen);
            Assert.NotNull(canopen.Extension);
            string extension = Assert.Single(canopen.Extension);
            Assert.Equal("ext", extension);
        }

        /// <summary>
        /// Validate a NewDat
        /// </summary>
        private static void Validate(Data.Models.OfflineList.NewDat? newdat)
        {
            Assert.NotNull(newdat);
            Assert.Equal("datversionurl", newdat.DatVersionUrl);
            Validate(newdat.DatUrl);
            Assert.Equal("imurl", newdat.ImUrl);
        }

        /// <summary>
        /// Validate a DatUrl
        /// </summary>
        private static void Validate(Data.Models.OfflineList.DatUrl? daturl)
        {
            Assert.NotNull(daturl);
            Assert.Equal("filename", daturl.FileName);
            Assert.Equal("content", daturl.Content);
        }

        /// <summary>
        /// Validate a Search
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Search? search)
        {
            Assert.NotNull(search);
            Assert.NotNull(search.To);
            var to = Assert.Single(search.To);
            Validate(to);
        }

        /// <summary>
        /// Validate a To
        /// </summary>
        private static void Validate(Data.Models.OfflineList.To? to)
        {
            Assert.NotNull(to);
            Assert.Equal("value", to.Value);
            Assert.Equal(true, to.Default);
            Assert.Equal("auto", to.Auto);

            Assert.NotNull(to.Find);
            var find = Assert.Single(to.Find);
            Validate(find);
        }

        /// <summary>
        /// Validate a Find
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Find? find)
        {
            Assert.NotNull(find);
            Assert.Equal("operation", find.Operation);
            Assert.Equal("value", find.Value);
            Assert.Equal("content", find.Content);
        }

        /// <summary>
        /// Validate a Games
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Games? games)
        {
            Assert.NotNull(games);
            Assert.NotNull(games.Game);
            var game = Assert.Single(games.Game);
            Validate(game);
        }

        /// <summary>
        /// Validate a Game
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Game? game)
        {
            Assert.NotNull(game);
            Assert.Equal("imagenumber", game.ImageNumber);
            Assert.Equal("releasenumber", game.ReleaseNumber);
            Assert.Equal("title", game.Title);
            Assert.Equal("savetype", game.SaveType);
            Assert.Equal(12345, game.RomSize);
            Assert.Equal("publisher", game.Publisher);
            Assert.Equal("location", game.Location);
            Assert.Equal("sourcerom", game.SourceRom);
            Assert.Equal("language", game.Language);
            Validate(game.Files);
            Assert.Equal("im1crc", game.Im1CRC);
            Assert.Equal("im2crc", game.Im2CRC);
            Assert.Equal("comment", game.Comment);
            Assert.Equal("duplicateid", game.DuplicateID);
        }

        /// <summary>
        /// Validate a Files
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Files? files)
        {
            Assert.NotNull(files);
            Assert.NotNull(files.RomCRC);
            var fileromcrc = Assert.Single(files.RomCRC);
            Validate(fileromcrc);
        }

        /// <summary>
        /// Validate a FileRomCRC
        /// </summary>
        private static void Validate(Data.Models.OfflineList.FileRomCRC? fileromcrc)
        {
            Assert.NotNull(fileromcrc);
            Assert.Equal("extension", fileromcrc.Extension);
            Assert.Equal("content", fileromcrc.Content);
        }

        /// <summary>
        /// Validate a GUI
        /// </summary>
        private static void Validate(Data.Models.OfflineList.GUI? gui)
        {
            Assert.NotNull(gui);
            Validate(gui.Images);
        }

        /// <summary>
        /// Validate a Images
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Images? images)
        {
            Assert.NotNull(images);
            Assert.Equal("width", images.Width);
            Assert.Equal("height", images.Height);

            Assert.NotNull(images.Image);
            var image = Assert.Single(images.Image);
            Validate(image);
        }

        /// <summary>
        /// Validate a Image
        /// </summary>
        private static void Validate(Data.Models.OfflineList.Image? image)
        {
            Assert.NotNull(image);
            Assert.Equal("x", image.X);
            Assert.Equal("y", image.Y);
            Assert.Equal("width", image.Width);
            Assert.Equal("height", image.Height);
        }
    }
}
