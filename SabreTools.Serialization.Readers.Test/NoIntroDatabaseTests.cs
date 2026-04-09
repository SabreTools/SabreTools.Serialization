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
                Number = "number",
                Clone = "clone",
                RegParent = "regparent",
                MergeOf = "mergeof",
                MergeName = "mergename",
                Name = "name",
                NameAlt = "namealt",
                Region = "region",
                Languages = "languages",
                ShowLang = "showlang",
                LangChecked = "langchecked",
                Version1 = "version1",
                Version2 = "version2",
                DevStatus = "devstatus",
                Additional = "additional",
                Special1 = "special1",
                Special2 = "special2",
                Alt = "alt",
                GameId1 = "gameid1",
                GameId2 = "gameid2",
                Description = "description",
                Bios = "bios",
                Licensed = "licensed",
                Pirate = "pirate",
                Physical = "physical",
                Complete = "complete",
                Adult = "adult",
                Dat = "dat",
                Listed = "listed",
                Private = "private",
                StickyNote = "stickynote",
                DatterNote = "datternote",
                Categories = "categories",
            };

            var media = new Data.Models.NoIntroDatabase.Media();

            var sourceDetails = new Data.Models.NoIntroDatabase.SourceDetails
            {
                Id = "id",
                AppendToNumber = "appendtonumber",
                Section = "section",
                RomInfo = "rominfo",
                DumpDate = "dumpdate",
                DumpDateInfo = true,
                ReleaseDate = "releasedate",
                ReleaseDateInfo = true,
                Dumper = "dumper",
                Project = "project",
                OriginalFormat = "originalformat",
                Nodump = true,
                Tool = "tool",
                Origin = "origin",
                Comment1 = "comment1",
                Comment2 = "comment2",
                Link1 = "link1",
                Link1Public = true,
                Link2 = "link2",
                Link2Public = true,
                Link3 = "link3",
                Link3Public = true,
                Region = "region",
                MediaTitle = "mediatitle",
            };

            var serials = new Data.Models.NoIntroDatabase.Serials
            {
                BoxBarcode = "boxbarcode",
                BoxSerial = "boxserial",
                ChipSerial = "chipserial",
                DigitalSerial1 = "digitalserial1",
                DigitalSerial2 = "digitalserial2",
                LockoutSerial = "lockoutserial",
                MediaSerial1 = "mediaserial1",
                MediaSerial2 = "mediaserial2",
                MediaSerial3 = "mediaserial3",
                MediaStamp = "mediastamp",
                PCBSerial = "pcbserial",
                RomChipSerial1 = "romchipserial1",
                RomChipSerial2 = "romchipserial2",
                SaveChipSerial = "savechipserial",
            };

            var file = new Data.Models.NoIntroDatabase.File
            {
                Id = "id",
                AppendToSourceId = "appendtosourceid",
                ForceName = "forcename",
                ForceSceneName = "forcescenename",
                EmptyDir = "emptydir",
                Extension = "extension",
                Item = "item",
                Date = "date",
                Format = "format",
                Note = "note",
                Filter = "filter",
                Version = "version",
                UpdateType = "updatetype",
                Size = "size",
                CRC32 = "crc32",
                MD5 = "md5",
                SHA1 = "sha1",
                SHA256 = "sha256",
                Serial = "serial",
                Header = "header",
                Bad = "bad",
                MIA = "mia",
                Unique = "unique",
                MergeName = "mergename",
                UniqueAttachment = "uniqueattachment",
            };

            var source = new Data.Models.NoIntroDatabase.Source
            {
                Details = sourceDetails,
                Serials = serials,
                File = [file],
            };

            var releaseDetails = new Data.Models.NoIntroDatabase.ReleaseDetails
            {
                Id = "id",
                AppendToNumber = "appendtonumber",
                Date = "date",
                OriginalFormat = "originalformat",
                Group = "group",
                DirName = "dirname",
                NfoName = "nfoname",
                NfoSize = "nfosize",
                NfoCRC = "nfocrc",
                ArchiveName = "archivename",
                RomInfo = "rominfo",
                Category = "category",
                Comment = "comment",
                Tool = "tool",
                Region = "region",
                Origin = "origin",
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
            Assert.Equal("number", archive.Number);
            Assert.Equal("clone", archive.Clone);
            Assert.Equal("regparent", archive.RegParent);
            Assert.Equal("mergeof", archive.MergeOf);
            Assert.Equal("mergename", archive.MergeName);
            Assert.Equal("name", archive.Name);
            Assert.Equal("namealt", archive.NameAlt);
            Assert.Equal("region", archive.Region);
            Assert.Equal("languages", archive.Languages);
            Assert.Equal("showlang", archive.ShowLang);
            Assert.Equal("langchecked", archive.LangChecked);
            Assert.Equal("version1", archive.Version1);
            Assert.Equal("version2", archive.Version2);
            Assert.Equal("devstatus", archive.DevStatus);
            Assert.Equal("additional", archive.Additional);
            Assert.Equal("special1", archive.Special1);
            Assert.Equal("special2", archive.Special2);
            Assert.Equal("alt", archive.Alt);
            Assert.Equal("gameid1", archive.GameId1);
            Assert.Equal("gameid2", archive.GameId2);
            Assert.Equal("description", archive.Description);
            Assert.Equal("bios", archive.Bios);
            Assert.Equal("licensed", archive.Licensed);
            Assert.Equal("pirate", archive.Pirate);
            Assert.Equal("physical", archive.Physical);
            Assert.Equal("complete", archive.Complete);
            Assert.Equal("adult", archive.Adult);
            Assert.Equal("dat", archive.Dat);
            Assert.Equal("listed", archive.Listed);
            Assert.Equal("private", archive.Private);
            Assert.Equal("stickynote", archive.StickyNote);
            Assert.Equal("datternote", archive.DatterNote);
            Assert.Equal("categories", archive.Categories);
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
            Assert.Equal("id", sourceDetails.Id);
            Assert.Equal("appendtonumber", sourceDetails.AppendToNumber);
            Assert.Equal("section", sourceDetails.Section);
            Assert.Equal("rominfo", sourceDetails.RomInfo);
            Assert.Equal("dumpdate", sourceDetails.DumpDate);
            Assert.Equal(true, sourceDetails.DumpDateInfo);
            Assert.Equal("releasedate", sourceDetails.ReleaseDate);
            Assert.Equal(true, sourceDetails.ReleaseDateInfo);
            Assert.Equal("dumper", sourceDetails.Dumper);
            Assert.Equal("project", sourceDetails.Project);
            Assert.Equal("originalformat", sourceDetails.OriginalFormat);
            Assert.Equal(true, sourceDetails.Nodump);
            Assert.Equal("tool", sourceDetails.Tool);
            Assert.Equal("origin", sourceDetails.Origin);
            Assert.Equal("comment1", sourceDetails.Comment1);
            Assert.Equal("comment2", sourceDetails.Comment2);
            Assert.Equal("link1", sourceDetails.Link1);
            Assert.Equal(true, sourceDetails.Link1Public);
            Assert.Equal("link2", sourceDetails.Link2);
            Assert.Equal(true, sourceDetails.Link2Public);
            Assert.Equal("link3", sourceDetails.Link3);
            Assert.Equal(true, sourceDetails.Link3Public);
            Assert.Equal("region", sourceDetails.Region);
            Assert.Equal("mediatitle", sourceDetails.MediaTitle);
        }

        /// <summary>
        /// Validate a Serials
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.Serials? serials)
        {
            Assert.NotNull(serials);
            Assert.Equal("boxbarcode", serials.BoxBarcode);
            Assert.Equal("boxserial", serials.BoxSerial);
            Assert.Equal("chipserial", serials.ChipSerial);
            Assert.Equal("digitalserial1", serials.DigitalSerial1);
            Assert.Equal("digitalserial2", serials.DigitalSerial2);
            Assert.Equal("lockoutserial", serials.LockoutSerial);
            Assert.Equal("mediaserial1", serials.MediaSerial1);
            Assert.Equal("mediaserial2", serials.MediaSerial2);
            Assert.Equal("mediaserial3", serials.MediaSerial3);
            Assert.Equal("mediastamp", serials.MediaStamp);
            Assert.Equal("pcbserial", serials.PCBSerial);
            Assert.Equal("romchipserial1", serials.RomChipSerial1);
            Assert.Equal("romchipserial2", serials.RomChipSerial2);
            Assert.Equal("savechipserial", serials.SaveChipSerial);
        }

        /// <summary>
        /// Validate a File
        /// </summary>
        private static void Validate(Data.Models.NoIntroDatabase.File? file)
        {
            Assert.NotNull(file);
            Assert.Equal("id", file.Id);
            Assert.Equal("appendtosourceid", file.AppendToSourceId);
            Assert.Equal("forcename", file.ForceName);
            Assert.Equal("forcescenename", file.ForceSceneName);
            Assert.Equal("emptydir", file.EmptyDir);
            Assert.Equal("extension", file.Extension);
            Assert.Equal("item", file.Item);
            Assert.Equal("date", file.Date);
            Assert.Equal("format", file.Format);
            Assert.Equal("note", file.Note);
            Assert.Equal("filter", file.Filter);
            Assert.Equal("version", file.Version);
            Assert.Equal("updatetype", file.UpdateType);
            Assert.Equal("size", file.Size);
            Assert.Equal("crc32", file.CRC32);
            Assert.Equal("md5", file.MD5);
            Assert.Equal("sha1", file.SHA1);
            Assert.Equal("sha256", file.SHA256);
            Assert.Equal("serial", file.Serial);
            Assert.Equal("header", file.Header);
            Assert.Equal("bad", file.Bad);
            Assert.Equal("mia", file.MIA);
            Assert.Equal("unique", file.Unique);
            Assert.Equal("mergename", file.MergeName);
            Assert.Equal("uniqueattachment", file.UniqueAttachment);
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
            Assert.Equal("id", releaseDetails.Id);
            Assert.Equal("appendtonumber", releaseDetails.AppendToNumber);
            Assert.Equal("date", releaseDetails.Date);
            Assert.Equal("originalformat", releaseDetails.OriginalFormat);
            Assert.Equal("group", releaseDetails.Group);
            Assert.Equal("dirname", releaseDetails.DirName);
            Assert.Equal("nfoname", releaseDetails.NfoName);
            Assert.Equal("nfosize", releaseDetails.NfoSize);
            Assert.Equal("nfocrc", releaseDetails.NfoCRC);
            Assert.Equal("archivename", releaseDetails.ArchiveName);
            Assert.Equal("rominfo", releaseDetails.RomInfo);
            Assert.Equal("category", releaseDetails.Category);
            Assert.Equal("comment", releaseDetails.Comment);
            Assert.Equal("tool", releaseDetails.Tool);
            Assert.Equal("region", releaseDetails.Region);
            Assert.Equal("origin", releaseDetails.Origin);
        }
    }
}
