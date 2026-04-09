using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class LogiqxTests
    {
        [Fact]
        public void RoundTripGameTest()
        {
            // Get the cross-model serializer
            var serializer = new Logiqx();

            // Build the data
            Data.Models.Logiqx.Datafile df = Build(game: true);

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(df);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.Logiqx.Datafile? newDf = serializer.Deserialize(metadata, game: true);

            // Validate the data
            Assert.NotNull(newDf);
            Assert.Equal("build", newDf.Build);
            Assert.Equal(true, newDf.Debug);
            Assert.Equal("schemalocation", newDf.SchemaLocation);
            Validate(newDf.Header);

            Assert.NotNull(newDf.Game);
            Assert.Equal(2, newDf.Game.Length);
            Validate(newDf.Game[0], nested: false);
            Validate(newDf.Game[1], nested: true);

            // TODO: Unsupported for round-trip
            Assert.Null(newDf.Dir);
        }

        [Fact]
        public void RoundTripMachineTest()
        {
            // Get the cross-model serializer
            var serializer = new Logiqx();

            // Build the data
            Data.Models.Logiqx.Datafile df = Build(game: false);

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(df);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.Logiqx.Datafile? newDf = serializer.Deserialize(metadata, game: false);

            // Validate the data
            Assert.NotNull(newDf);
            Assert.Equal("build", newDf.Build);
            Assert.Equal(true, newDf.Debug);
            Assert.Equal("schemalocation", newDf.SchemaLocation);
            Validate(newDf.Header);

            Assert.NotNull(newDf.Game);
            Assert.Equal(2, newDf.Game.Length);
            Validate(newDf.Game[0], nested: false);
            Validate(newDf.Game[1], nested: true);

            // TODO: Unsupported for round-trip
            Assert.Null(newDf.Dir);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.Logiqx.Datafile Build(bool game)
        {
            var romvault = new Data.Models.Logiqx.RomVault
            {
                Header = "header",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceNodump = Data.Models.Metadata.NodumpFlag.Required,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
            };

            var clrmamepro = new Data.Models.Logiqx.ClrMamePro
            {
                Header = "header",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceNodump = Data.Models.Metadata.NodumpFlag.Required,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
            };

            var romcenter = new Data.Models.Logiqx.RomCenter
            {
                Plugin = "plugin",
                RomMode = Data.Models.Metadata.MergingFlag.Merged,
                BiosMode = Data.Models.Metadata.MergingFlag.Merged,
                SampleMode = Data.Models.Metadata.MergingFlag.Merged,
                LockRomMode = true,
                LockBiosMode = true,
                LockSampleMode = true,
            };

            var header = new Data.Models.Logiqx.Header
            {
                Id = "id",
                Name = "name",
                Description = "description",
                RootDir = "rootdir",
                Category = "category",
                Version = "version",
                Date = "date",
                Author = "author",
                Email = "email",
                Homepage = "homepage",
                Url = "url",
                Comment = "comment",
                Type = "type",
                RomVault = romvault,
                ClrMamePro = clrmamepro,
                RomCenter = romcenter,
            };

            var trurip = new Data.Models.Logiqx.Trurip
            {
                TitleID = "titleid",
                Publisher = "publisher",
                Developer = "developer",
                Year = "year",
                Genre = "genre",
                Subgenre = "subgenre",
                Ratings = "ratings",
                Score = "score",
                Players = "players",
                Enabled = "enabled",
                CRC = "crc32",
                Source = "source",
                CloneOf = "cloneof",
                RelatedTo = "relatedto",
            };

            var release = new Data.Models.Logiqx.Release
            {
                Name = "name",
                Region = "region",
                Language = "language",
                Date = "date",
                Default = true,
            };

            var biosset = new Data.Models.Logiqx.BiosSet
            {
                Name = "name",
                Description = "description",
                Default = true,
            };

            var rom = new Data.Models.Logiqx.Rom
            {
                Name = "name",
                Size = 12345,
                CRC16 = "crc16",
                CRC = "crc32",
                CRC64 = "crc64",
                MD2 = "md2",
                MD4 = "md4",
                MD5 = "md5",
                RIPEMD128 = "ripemd128",
                RIPEMD160 = "ripemd160",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SHA384 = "sha384",
                SHA512 = "sha512",
                SpamSum = "spamsum",
                xxHash364 = "xxhash364",
                xxHash3128 = "xxhash3128",
                Merge = "merge",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Serial = "serial",
                Header = "header",
                Date = "date",
                Inverted = true,
                MIA = true,
            };

            var disk = new Data.Models.Logiqx.Disk
            {
                Name = "name",
                MD5 = "md5",
                SHA1 = "sha1",
                Merge = "merge",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Region = "region",
            };

            var media = new Data.Models.Logiqx.Media
            {
                Name = "name",
                MD5 = "md5",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SpamSum = "spamsum",
            };

            var deviceRef = new Data.Models.Logiqx.DeviceRef
            {
                Name = "name",
            };

            var sample = new Data.Models.Logiqx.Sample
            {
                Name = "name",
            };

            var archive = new Data.Models.Logiqx.Archive
            {
                Name = "name",
            };

            var driver = new Data.Models.Logiqx.Driver
            {
                Status = Data.Models.Metadata.SupportStatus.Good,
                Emulation = Data.Models.Metadata.SupportStatus.Good,
                Cocktail = Data.Models.Metadata.SupportStatus.Good,
                SaveState = Data.Models.Metadata.Supported.Yes,
                RequiresArtwork = true,
                Unofficial = true,
                NoSoundHardware = true,
                Incomplete = true,
            };

            var softwarelist = new Data.Models.Logiqx.SoftwareList
            {
                Tag = "tag",
                Name = "name",
                Status = Data.Models.Metadata.SoftwareListStatus.Original,
                Filter = "filter",
            };

            Data.Models.Logiqx.GameBase gameBase = game
                ? new Data.Models.Logiqx.Game()
                : new Data.Models.Logiqx.Machine();
            gameBase.Name = "name";
            gameBase.SourceFile = "sourcefile";
            gameBase.IsBios = true;
            gameBase.IsDevice = true;
            gameBase.IsMechanical = true;
            gameBase.CloneOf = "cloneof";
            gameBase.RomOf = "romof";
            gameBase.SampleOf = "sampleof";
            gameBase.Board = "board";
            gameBase.RebuildTo = "rebuildto";
            gameBase.Id = "id";
            gameBase.CloneOfId = "cloneofid";
            gameBase.Runnable = Data.Models.Metadata.Runnable.Yes;
            gameBase.Comment = ["comment"];
            gameBase.Description = "description";
            gameBase.Year = "year";
            gameBase.Manufacturer = "manufacturer";
            gameBase.Publisher = "publisher";
            gameBase.Category = ["category"];
            gameBase.Trurip = trurip;
            gameBase.Release = [release];
            gameBase.BiosSet = [biosset];
            gameBase.Rom = [rom];
            gameBase.Disk = [disk];
            gameBase.Media = [media];
            gameBase.DeviceRef = [deviceRef];
            gameBase.Sample = [sample];
            gameBase.Archive = [archive];
            gameBase.Driver = driver;
            gameBase.SoftwareList = [softwarelist];

            var subdir = new Data.Models.Logiqx.Dir
            {
                Name = "name",
                Game = [gameBase],
            };

            var dir = new Data.Models.Logiqx.Dir
            {
                Name = "name",
                Subdir = [subdir],
            };

            return new Data.Models.Logiqx.Datafile
            {
                Build = "build",
                Debug = true,
                SchemaLocation = "schemalocation",
                Header = header,
                Game = [gameBase],
                Dir = [dir],
            };
        }

        /// <summary>
        /// Validate a Header
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Header? header)
        {
            Assert.NotNull(header);
            Assert.Equal("id", header.Id);
            Assert.Equal("name", header.Name);
            Assert.Equal("description", header.Description);
            Assert.Equal("rootdir", header.RootDir);
            Assert.Equal("category", header.Category);
            Assert.Equal("version", header.Version);
            Assert.Equal("date", header.Date);
            Assert.Equal("author", header.Author);
            Assert.Equal("email", header.Email);
            Assert.Equal("homepage", header.Homepage);
            Assert.Equal("url", header.Url);
            Assert.Equal("comment", header.Comment);
            Assert.Equal("type", header.Type);
            Validate(header.RomVault);
            Validate(header.ClrMamePro);
            Validate(header.RomCenter);
        }

        /// <summary>
        /// Validate a RomVault
        /// </summary>
        private static void Validate(Data.Models.Logiqx.RomVault? rv)
        {
            Assert.NotNull(rv);
            Assert.Equal("header", rv.Header);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, rv.ForceMerging);
            Assert.Equal(Data.Models.Metadata.NodumpFlag.Required, rv.ForceNodump);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, rv.ForcePacking);
        }

        /// <summary>
        /// Validate a ClrMamePro
        /// </summary>
        private static void Validate(Data.Models.Logiqx.ClrMamePro? cmp)
        {
            Assert.NotNull(cmp);
            Assert.Equal("header", cmp.Header);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, cmp.ForceMerging);
            Assert.Equal(Data.Models.Metadata.NodumpFlag.Required, cmp.ForceNodump);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, cmp.ForcePacking);
        }

        /// <summary>
        /// Validate a RomCenter
        /// </summary>
        private static void Validate(Data.Models.Logiqx.RomCenter? rc)
        {
            Assert.NotNull(rc);
            Assert.Equal("plugin", rc.Plugin);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, rc.RomMode);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, rc.BiosMode);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, rc.SampleMode);
            Assert.Equal(true, rc.LockRomMode);
            Assert.Equal(true, rc.LockBiosMode);
            Assert.Equal(true, rc.LockSampleMode);
        }

        /// <summary>
        /// Validate a GameBase
        /// </summary>
        private static void Validate(Data.Models.Logiqx.GameBase? gb, bool nested)
        {
            Assert.NotNull(gb);
            if (nested)
                Assert.Equal("name\\name\\name", gb.Name);
            else
                Assert.Equal("name", gb.Name);
            Assert.Equal("sourcefile", gb.SourceFile);
            Assert.Equal(true, gb.IsBios);
            Assert.Equal(true, gb.IsDevice);
            Assert.Equal(true, gb.IsMechanical);
            Assert.Equal("cloneof", gb.CloneOf);
            Assert.Equal("romof", gb.RomOf);
            Assert.Equal("sampleof", gb.SampleOf);
            Assert.Equal("board", gb.Board);
            Assert.Equal("rebuildto", gb.RebuildTo);
            Assert.Equal("id", gb.Id);
            Assert.Equal("cloneofid", gb.CloneOfId);
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, gb.Runnable);

            Assert.NotNull(gb.Comment);
            string comment = Assert.Single(gb.Comment);
            Assert.Equal("comment", comment);

            Assert.Equal("description", gb.Description);
            Assert.Equal("year", gb.Year);
            Assert.Equal("manufacturer", gb.Manufacturer);
            Assert.Equal("publisher", gb.Publisher);

            Assert.NotNull(gb.Category);
            string category = Assert.Single(gb.Category);
            Assert.Equal("category", category);

            Validate(gb.Trurip);

            Assert.NotNull(gb.Release);
            var release = Assert.Single(gb.Release);
            Validate(release);

            Assert.NotNull(gb.BiosSet);
            var biosset = Assert.Single(gb.BiosSet);
            Validate(biosset);

            Assert.NotNull(gb.Rom);
            var rom = Assert.Single(gb.Rom);
            Validate(rom);

            Assert.NotNull(gb.Disk);
            var disk = Assert.Single(gb.Disk);
            Validate(disk);

            Assert.NotNull(gb.Media);
            var media = Assert.Single(gb.Media);
            Validate(media);

            Assert.NotNull(gb.DeviceRef);
            var deviceref = Assert.Single(gb.DeviceRef);
            Validate(deviceref);

            Assert.NotNull(gb.Sample);
            var sample = Assert.Single(gb.Sample);
            Validate(sample);

            Assert.NotNull(gb.Archive);
            var archive = Assert.Single(gb.Archive);
            Validate(archive);

            Validate(gb.Driver);

            Assert.NotNull(gb.SoftwareList);
            var softwarelist = Assert.Single(gb.SoftwareList);
            Validate(softwarelist);
        }

        /// <summary>
        /// Validate a Trurip
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Trurip? trurip)
        {
            Assert.NotNull(trurip);
            Assert.Equal("titleid", trurip.TitleID);
            Assert.Equal("publisher", trurip.Publisher);
            Assert.Equal("developer", trurip.Developer);
            Assert.Equal("year", trurip.Year);
            Assert.Equal("genre", trurip.Genre);
            Assert.Equal("subgenre", trurip.Subgenre);
            Assert.Equal("ratings", trurip.Ratings);
            Assert.Equal("score", trurip.Score);
            Assert.Equal("players", trurip.Players);
            Assert.Equal("enabled", trurip.Enabled);
            Assert.Equal("crc32", trurip.CRC);
            Assert.Equal("source", trurip.Source);
            Assert.Equal("cloneof", trurip.CloneOf);
            Assert.Equal("relatedto", trurip.RelatedTo);
        }

        /// <summary>
        /// Validate a Release
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Release? release)
        {
            Assert.NotNull(release);
            Assert.Equal("name", release.Name);
            Assert.Equal("region", release.Region);
            Assert.Equal("language", release.Language);
            Assert.Equal("date", release.Date);
            Assert.Equal(true, release.Default);
        }

        /// <summary>
        /// Validate a BiosSet
        /// </summary>
        private static void Validate(Data.Models.Logiqx.BiosSet? biosset)
        {
            Assert.NotNull(biosset);
            Assert.Equal("name", biosset.Name);
            Assert.Equal("description", biosset.Description);
            Assert.Equal(true, biosset.Default);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("name", rom.Name);
            Assert.Equal(12345, rom.Size);
            Assert.Equal("crc16", rom.CRC16);
            Assert.Equal("crc32", rom.CRC);
            Assert.Equal("crc64", rom.CRC64);
            Assert.Equal("md2", rom.MD2);
            Assert.Equal("md4", rom.MD4);
            Assert.Equal("md5", rom.MD5);
            Assert.Equal("ripemd128", rom.RIPEMD128);
            Assert.Equal("ripemd160", rom.RIPEMD160);
            Assert.Equal("sha1", rom.SHA1);
            Assert.Equal("sha256", rom.SHA256);
            Assert.Equal("sha384", rom.SHA384);
            Assert.Equal("sha512", rom.SHA512);
            Assert.Equal("spamsum", rom.SpamSum);
            Assert.Equal("xxhash364", rom.xxHash364);
            Assert.Equal("xxhash3128", rom.xxHash3128);
            Assert.Equal("merge", rom.Merge);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal("serial", rom.Serial);
            Assert.Equal("header", rom.Header);
            Assert.Equal("date", rom.Date);
            Assert.Equal(true, rom.Inverted);
            Assert.Equal(true, rom.MIA);
        }

        /// <summary>
        /// Validate a Disk
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("name", disk.Name);
            Assert.Equal("md5", disk.MD5);
            Assert.Equal("sha1", disk.SHA1);
            Assert.Equal("merge", disk.Merge);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, disk.Status);
            Assert.Equal("region", disk.Region);
        }

        /// <summary>
        /// Validate a Media
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal("name", media.Name);
            Assert.Equal("md5", media.MD5);
            Assert.Equal("sha1", media.SHA1);
            Assert.Equal("sha256", media.SHA256);
            Assert.Equal("spamsum", media.SpamSum);
        }

        /// <summary>
        /// Validate a DeviceRef
        /// </summary>
        private static void Validate(Data.Models.Logiqx.DeviceRef? deviceref)
        {
            Assert.NotNull(deviceref);
            Assert.Equal("name", deviceref.Name);
        }

        /// <summary>
        /// Validate a Sample
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("name", sample.Name);
        }

        /// <summary>
        /// Validate a Archive
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("name", archive.Name);
        }

        /// <summary>
        /// Validate a Driver
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Status);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Emulation);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Cocktail);
            Assert.Equal(Data.Models.Metadata.Supported.Yes, driver.SaveState);
            Assert.Equal(true, driver.RequiresArtwork);
            Assert.Equal(true, driver.Unofficial);
            Assert.Equal(true, driver.NoSoundHardware);
            Assert.Equal(true, driver.Incomplete);
        }

        /// <summary>
        /// Validate a SoftwareList
        /// </summary>
        private static void Validate(Data.Models.Logiqx.SoftwareList? softwarelist)
        {
            Assert.NotNull(softwarelist);
            Assert.Equal("tag", softwarelist.Tag);
            Assert.Equal("name", softwarelist.Name);
            Assert.Equal(Data.Models.Metadata.SoftwareListStatus.Original, softwarelist.Status);
            Assert.Equal("filter", softwarelist.Filter);
        }
    }
}
