using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class NoIntroDatabaseTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new NoIntroDatabase();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new NoIntroDatabase();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new NoIntroDatabase();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new NoIntroDatabase();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new NoIntroDatabase();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new NoIntroDatabase();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new NoIntroDatabase();
            var serializer = new Writers.NoIntroDatabase();

            // Build the data
            Data.Models.NoIntroDatabase.Datafile df = Build();

            // Serialize to stream
            Stream? metadata = serializer.SerializeStream(df);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.NoIntroDatabase.Datafile? newDf = deserializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newDf);

            Assert.NotNull(newDf.Game);
            var newGame = Assert.Single(newDf.Game);
            Validate(newGame);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.NoIntroDatabase.Datafile Build()
        {
            var archive = new Data.Models.NoIntroDatabase.Archive
            {
                Number = "XXXXXX",
                Clone = "XXXXXX",
                RegParent = "XXXXXX",
                MergeOf = "XXXXXX",
                MergeName = "XXXXXX",
                Name = "name",
                NameAlt = "XXXXXX",
                Region = "XXXXXX",
                Languages = "XXXXXX",
                ShowLang = "XXXXXX",
                LangChecked = "XXXXXX",
                Version1 = "XXXXXX",
                Version2 = "XXXXXX",
                DevStatus = "XXXXXX",
                Additional = "XXXXXX",
                Special1 = "XXXXXX",
                Special2 = "XXXXXX",
                Alt = "XXXXXX",
                GameId1 = "XXXXXX",
                GameId2 = "XXXXXX",
                Description = "description",
                Bios = "XXXXXX",
                Licensed = "XXXXXX",
                Pirate = "XXXXXX",
                Physical = "XXXXXX",
                Complete = "XXXXXX",
                Adult = "XXXXXX",
                Dat = "XXXXXX",
                Listed = "XXXXXX",
                Private = "XXXXXX",
                StickyNote = "XXXXXX",
                DatterNote = "XXXXXX",
                Categories = "XXXXXX",
            };

            var media = new Data.Models.NoIntroDatabase.Media();

            var sourceDetails = new Data.Models.NoIntroDatabase.SourceDetails
            {
                Id = "XXXXXX",
                AppendToNumber = "XXXXXX",
                Section = "XXXXXX",
                RomInfo = "XXXXXX",
                DumpDate = "XXXXXX",
                DumpDateInfo = "XXXXXX",
                ReleaseDate = "XXXXXX",
                ReleaseDateInfo = "XXXXXX",
                Dumper = "XXXXXX",
                Project = "XXXXXX",
                OriginalFormat = "XXXXXX",
                Nodump = "XXXXXX",
                Tool = "XXXXXX",
                Origin = "XXXXXX",
                Comment1 = "XXXXXX",
                Comment2 = "XXXXXX",
                Link1 = "XXXXXX",
                Link1Public = "XXXXXX",
                Link2 = "XXXXXX",
                Link2Public = "XXXXXX",
                Link3 = "XXXXXX",
                Link3Public = "XXXXXX",
                Region = "XXXXXX",
                MediaTitle = "XXXXXX",
            };

            var serials = new Data.Models.NoIntroDatabase.Serials
            {
                MediaSerial1 = "XXXXXX",
                MediaSerial2 = "XXXXXX",
                MediaSerial3 = "XXXXXX",
                PCBSerial = "XXXXXX",
                RomChipSerial1 = "XXXXXX",
                RomChipSerial2 = "XXXXXX",
                LockoutSerial = "XXXXXX",
                SaveChipSerial = "XXXXXX",
                ChipSerial = "XXXXXX",
                BoxSerial = "XXXXXX",
                MediaStamp = "XXXXXX",
                BoxBarcode = "XXXXXX",
                DigitalSerial1 = "XXXXXX",
                DigitalSerial2 = "XXXXXX",
            };

            var file = new Data.Models.NoIntroDatabase.File
            {
                Id = "XXXXXX",
                AppendToSourceId = "XXXXXX",
                ForceName = "XXXXXX",
                ForceSceneName = "XXXXXX",
                EmptyDir = "XXXXXX",
                Extension = "XXXXXX",
                Item = "XXXXXX",
                Date = "XXXXXX",
                Format = "XXXXXX",
                Note = "XXXXXX",
                Filter = "filter",
                Version = "XXXXXX",
                UpdateType = "XXXXXX",
                Size = "XXXXXX",
                CRC32 = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                Serial = "XXXXXX",
                Header = "XXXXXX",
                Bad = "XXXXXX",
                MIA = "XXXXXX",
                Unique = "XXXXXX",
                MergeName = "XXXXXX",
                UniqueAttachment = "XXXXXX",
            };

            var source = new Data.Models.NoIntroDatabase.Source
            {
                Details = sourceDetails,
                Serials = serials,
                File = [file],
            };

            var releaseDetails = new Data.Models.NoIntroDatabase.ReleaseDetails
            {
                Id = "XXXXXX",
                AppendToNumber = "XXXXXX",
                Date = "XXXXXX",
                OriginalFormat = "XXXXXX",
                Group = "XXXXXX",
                DirName = "XXXXXX",
                NfoName = "XXXXXX",
                NfoSize = "XXXXXX",
                NfoCRC = "XXXXXX",
                ArchiveName = "XXXXXX",
                RomInfo = "XXXXXX",
                Category = "XXXXXX",
                Comment = "XXXXXX",
                Tool = "XXXXXX",
                Region = "XXXXXX",
                Origin = "XXXXXX",
            };

            var release = new Data.Models.NoIntroDatabase.Release
            {
                Details = releaseDetails,
                Serials = serials,
                File = [file],
            };

            var game = new Data.Models.NoIntroDatabase.Game
            {
                Name = "name",
                Archive = archive,
                Media = [media],
                Source = [source],
                Release = [release],
            };

            return new Data.Models.NoIntroDatabase.Datafile
            {
                Game = [game],
            };
        }

        /// <summary>
        /// Validate a Game
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Game? game)
        {
            Assert.NotNull(game);
            Assert.Equal("name", game.Name);

            Validate(game.Archive);

            Assert.NotNull(game.Media);
            Data.Models.NoIntroDatabase.Media media = Assert.Single(game.Media);
            Validate(media);

            Assert.NotNull(game.Source);
            Data.Models.NoIntroDatabase.Source source = Assert.Single(game.Source);
            Validate(source);

            Assert.NotNull(game.Release);
            Data.Models.NoIntroDatabase.Release release = Assert.Single(game.Release);
            Validate(release);
        }

        /// <summary>
        /// Validate a Archive
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("XXXXXX", archive.Number);
            Assert.Equal("XXXXXX", archive.Clone);
            Assert.Equal("XXXXXX", archive.RegParent);
            Assert.Equal("XXXXXX", archive.MergeOf);
            Assert.Equal("XXXXXX", archive.MergeName);
            Assert.Equal("name", archive.Name);
            Assert.Equal("XXXXXX", archive.NameAlt);
            Assert.Equal("XXXXXX", archive.Region);
            Assert.Equal("XXXXXX", archive.Languages);
            Assert.Equal("XXXXXX", archive.ShowLang);
            Assert.Equal("XXXXXX", archive.LangChecked);
            Assert.Equal("XXXXXX", archive.Version1);
            Assert.Equal("XXXXXX", archive.Version2);
            Assert.Equal("XXXXXX", archive.DevStatus);
            Assert.Equal("XXXXXX", archive.Additional);
            Assert.Equal("XXXXXX", archive.Special1);
            Assert.Equal("XXXXXX", archive.Special2);
            Assert.Equal("XXXXXX", archive.Alt);
            Assert.Equal("XXXXXX", archive.GameId1);
            Assert.Equal("XXXXXX", archive.GameId2);
            Assert.Equal("description", archive.Description);
            Assert.Equal("XXXXXX", archive.Bios);
            Assert.Equal("XXXXXX", archive.Licensed);
            Assert.Equal("XXXXXX", archive.Pirate);
            Assert.Equal("XXXXXX", archive.Physical);
            Assert.Equal("XXXXXX", archive.Complete);
            Assert.Equal("XXXXXX", archive.Adult);
            Assert.Equal("XXXXXX", archive.Dat);
            Assert.Equal("XXXXXX", archive.Listed);
            Assert.Equal("XXXXXX", archive.Private);
            Assert.Equal("XXXXXX", archive.StickyNote);
            Assert.Equal("XXXXXX", archive.DatterNote);
            Assert.Equal("XXXXXX", archive.Categories);
        }

        /// <summary>
        /// Validate a Media
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Media? media)
        {
            Assert.NotNull(media);
        }

        /// <summary>
        /// Validate a Source
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Source? source)
        {
            Assert.NotNull(source);

            Validate(source.Details);

            Validate(source.Serials);

            Assert.NotNull(source.File);
            Data.Models.NoIntroDatabase.File file = Assert.Single(source.File);
            Validate(file);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.SourceDetails? sourceDetails)
        {
            Assert.NotNull(sourceDetails);
            Assert.Equal("XXXXXX", sourceDetails.Id);
            Assert.Equal("XXXXXX", sourceDetails.AppendToNumber);
            Assert.Equal("XXXXXX", sourceDetails.Section);
            Assert.Equal("XXXXXX", sourceDetails.RomInfo);
            Assert.Equal("XXXXXX", sourceDetails.DumpDate);
            Assert.Equal("XXXXXX", sourceDetails.DumpDateInfo);
            Assert.Equal("XXXXXX", sourceDetails.ReleaseDate);
            Assert.Equal("XXXXXX", sourceDetails.ReleaseDateInfo);
            Assert.Equal("XXXXXX", sourceDetails.Dumper);
            Assert.Equal("XXXXXX", sourceDetails.Project);
            Assert.Equal("XXXXXX", sourceDetails.OriginalFormat);
            Assert.Equal("XXXXXX", sourceDetails.Nodump);
            Assert.Equal("XXXXXX", sourceDetails.Tool);
            Assert.Equal("XXXXXX", sourceDetails.Origin);
            Assert.Equal("XXXXXX", sourceDetails.Comment1);
            Assert.Equal("XXXXXX", sourceDetails.Comment2);
            Assert.Equal("XXXXXX", sourceDetails.Link1);
            Assert.Equal("XXXXXX", sourceDetails.Link1Public);
            Assert.Equal("XXXXXX", sourceDetails.Link2);
            Assert.Equal("XXXXXX", sourceDetails.Link2Public);
            Assert.Equal("XXXXXX", sourceDetails.Link3);
            Assert.Equal("XXXXXX", sourceDetails.Link3Public);
            Assert.Equal("XXXXXX", sourceDetails.Region);
            Assert.Equal("XXXXXX", sourceDetails.MediaTitle);
        }

        /// <summary>
        /// Validate a Serials
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Serials? serials)
        {
            Assert.NotNull(serials);
            Assert.Equal("XXXXXX", serials.MediaSerial1);
            Assert.Equal("XXXXXX", serials.MediaSerial2);
            Assert.Equal("XXXXXX", serials.MediaSerial3);
            Assert.Equal("XXXXXX", serials.PCBSerial);
            Assert.Equal("XXXXXX", serials.RomChipSerial1);
            Assert.Equal("XXXXXX", serials.RomChipSerial2);
            Assert.Equal("XXXXXX", serials.LockoutSerial);
            Assert.Equal("XXXXXX", serials.SaveChipSerial);
            Assert.Equal("XXXXXX", serials.ChipSerial);
            Assert.Equal("XXXXXX", serials.BoxSerial);
            Assert.Equal("XXXXXX", serials.MediaStamp);
            Assert.Equal("XXXXXX", serials.BoxBarcode);
            Assert.Equal("XXXXXX", serials.DigitalSerial1);
            Assert.Equal("XXXXXX", serials.DigitalSerial2);
        }

        /// <summary>
        /// Validate a File
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.File? file)
        {
            Assert.NotNull(file);
            Assert.Equal("XXXXXX", file.Id);
            Assert.Equal("XXXXXX", file.AppendToSourceId);
            Assert.Equal("XXXXXX", file.ForceName);
            Assert.Equal("XXXXXX", file.ForceSceneName);
            Assert.Equal("XXXXXX", file.EmptyDir);
            Assert.Equal("XXXXXX", file.Extension);
            Assert.Equal("XXXXXX", file.Item);
            Assert.Equal("XXXXXX", file.Date);
            Assert.Equal("XXXXXX", file.Format);
            Assert.Equal("XXXXXX", file.Note);
            Assert.Equal("filter", file.Filter);
            Assert.Equal("XXXXXX", file.Version);
            Assert.Equal("XXXXXX", file.UpdateType);
            Assert.Equal("XXXXXX", file.Size);
            Assert.Equal("XXXXXX", file.CRC32);
            Assert.Equal("XXXXXX", file.MD5);
            Assert.Equal("XXXXXX", file.SHA1);
            Assert.Equal("XXXXXX", file.SHA256);
            Assert.Equal("XXXXXX", file.Serial);
            Assert.Equal("XXXXXX", file.Header);
            Assert.Equal("XXXXXX", file.Bad);
            Assert.Equal("XXXXXX", file.MIA);
            Assert.Equal("XXXXXX", file.Unique);
            Assert.Equal("XXXXXX", file.MergeName);
            Assert.Equal("XXXXXX", file.UniqueAttachment);
        }

        /// <summary>
        /// Validate a Release
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Release? release)
        {
            Assert.NotNull(release);

            Validate(release.Details);

            Validate(release.Serials);

            Assert.NotNull(release.File);
            Data.Models.NoIntroDatabase.File file = Assert.Single(release.File);
            Validate(file);
        }

        /// <summary>
        /// Validate a ReleaseDetails
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.ReleaseDetails? releaseDetails)
        {
            Assert.NotNull(releaseDetails);
            Assert.Equal("XXXXXX", releaseDetails.Id);
            Assert.Equal("XXXXXX", releaseDetails.AppendToNumber);
            Assert.Equal("XXXXXX", releaseDetails.Date);
            Assert.Equal("XXXXXX", releaseDetails.OriginalFormat);
            Assert.Equal("XXXXXX", releaseDetails.Group);
            Assert.Equal("XXXXXX", releaseDetails.DirName);
            Assert.Equal("XXXXXX", releaseDetails.NfoName);
            Assert.Equal("XXXXXX", releaseDetails.NfoSize);
            Assert.Equal("XXXXXX", releaseDetails.NfoCRC);
            Assert.Equal("XXXXXX", releaseDetails.ArchiveName);
            Assert.Equal("XXXXXX", releaseDetails.RomInfo);
            Assert.Equal("XXXXXX", releaseDetails.Category);
            Assert.Equal("XXXXXX", releaseDetails.Comment);
            Assert.Equal("XXXXXX", releaseDetails.Tool);
            Assert.Equal("XXXXXX", releaseDetails.Region);
            Assert.Equal("XXXXXX", releaseDetails.Origin);
        }
    }
}
