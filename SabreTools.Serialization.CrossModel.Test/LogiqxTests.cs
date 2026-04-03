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
            Assert.Equal("XXXXXX", newDf.Build);
            Assert.Equal(true, newDf.Debug);
            Assert.Equal("XXXXXX", newDf.SchemaLocation);
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
            Assert.Equal("XXXXXX", newDf.Build);
            Assert.Equal(true, newDf.Debug);
            Assert.Equal("XXXXXX", newDf.SchemaLocation);
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
            var clrmamepro = new Data.Models.Logiqx.ClrMamePro
            {
                Header = "XXXXXX",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceNodump = Data.Models.Metadata.NodumpFlag.Required,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
            };

            var romcenter = new Data.Models.Logiqx.RomCenter
            {
                Plugin = "XXXXXX",
                RomMode = Data.Models.Metadata.MergingFlag.Merged,
                BiosMode = Data.Models.Metadata.MergingFlag.Merged,
                SampleMode = Data.Models.Metadata.MergingFlag.Merged,
                LockRomMode = true,
                LockBiosMode = true,
                LockSampleMode = true,
            };

            var header = new Data.Models.Logiqx.Header
            {
                Id = "XXXXXX",
                Name = "name",
                Description = "description",
                RootDir = "XXXXXX",
                Category = "XXXXXX",
                Version = "XXXXXX",
                Date = "XXXXXX",
                Author = "XXXXXX",
                Email = "XXXXXX",
                Homepage = "XXXXXX",
                Url = "XXXXXX",
                Comment = "XXXXXX",
                Type = "XXXXXX",
                ClrMamePro = clrmamepro,
                RomCenter = romcenter,
            };

            var trurip = new Data.Models.Logiqx.Trurip
            {
                TitleID = "XXXXXX",
                Publisher = "XXXXXX",
                Developer = "XXXXXX",
                Year = "XXXXXX",
                Genre = "XXXXXX",
                Subgenre = "XXXXXX",
                Ratings = "XXXXXX",
                Score = "XXXXXX",
                Players = "XXXXXX",
                Enabled = "XXXXXX",
                CRC = "XXXXXX",
                Source = "XXXXXX",
                CloneOf = "XXXXXX",
                RelatedTo = "XXXXXX",
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
                CRC16 = "XXXXXX",
                CRC = "XXXXXX",
                CRC64 = "XXXXXX",
                MD2 = "XXXXXX",
                MD4 = "XXXXXX",
                MD5 = "XXXXXX",
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SHA384 = "XXXXXX",
                SHA512 = "XXXXXX",
                SpamSum = "XXXXXX",
                xxHash364 = "XXXXXX",
                xxHash3128 = "XXXXXX",
                Merge = "XXXXXX",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Serial = "XXXXXX",
                Header = "XXXXXX",
                Date = "XXXXXX",
                Inverted = true,
                MIA = true,
            };

            var disk = new Data.Models.Logiqx.Disk
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Merge = "XXXXXX",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Region = "XXXXXX",
            };

            var media = new Data.Models.Logiqx.Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
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
            gameBase.SourceFile = "XXXXXX";
            gameBase.IsBios = true;
            gameBase.IsDevice = true;
            gameBase.IsMechanical = true;
            gameBase.CloneOf = "XXXXXX";
            gameBase.RomOf = "XXXXXX";
            gameBase.SampleOf = "XXXXXX";
            gameBase.Board = "XXXXXX";
            gameBase.RebuildTo = "XXXXXX";
            gameBase.Id = "XXXXXX";
            gameBase.CloneOfId = "XXXXXX";
            gameBase.Runnable = Data.Models.Metadata.Runnable.Yes;
            gameBase.Comment = ["XXXXXX"];
            gameBase.Description = "description";
            gameBase.Year = "XXXXXX";
            gameBase.Manufacturer = "XXXXXX";
            gameBase.Publisher = "XXXXXX";
            gameBase.Category = ["XXXXXX"];
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
                Build = "XXXXXX",
                Debug = true,
                SchemaLocation = "XXXXXX",
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
            Assert.Equal("XXXXXX", header.Id);
            Assert.Equal("name", header.Name);
            Assert.Equal("description", header.Description);
            Assert.Equal("XXXXXX", header.RootDir);
            Assert.Equal("XXXXXX", header.Category);
            Assert.Equal("XXXXXX", header.Version);
            Assert.Equal("XXXXXX", header.Date);
            Assert.Equal("XXXXXX", header.Author);
            Assert.Equal("XXXXXX", header.Email);
            Assert.Equal("XXXXXX", header.Homepage);
            Assert.Equal("XXXXXX", header.Url);
            Assert.Equal("XXXXXX", header.Comment);
            Assert.Equal("XXXXXX", header.Type);
            Validate(header.ClrMamePro);
            Validate(header.RomCenter);
        }

        /// <summary>
        /// Validate a ClrMamePro
        /// </summary>
        private static void Validate(Data.Models.Logiqx.ClrMamePro? cmp)
        {
            Assert.NotNull(cmp);
            Assert.Equal("XXXXXX", cmp.Header);
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
            Assert.Equal("XXXXXX", rc.Plugin);
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
            Assert.Equal("XXXXXX", gb.SourceFile);
            Assert.Equal(true, gb.IsBios);
            Assert.Equal(true, gb.IsDevice);
            Assert.Equal(true, gb.IsMechanical);
            Assert.Equal("XXXXXX", gb.CloneOf);
            Assert.Equal("XXXXXX", gb.RomOf);
            Assert.Equal("XXXXXX", gb.SampleOf);
            Assert.Equal("XXXXXX", gb.Board);
            Assert.Equal("XXXXXX", gb.RebuildTo);
            Assert.Equal("XXXXXX", gb.Id);
            Assert.Equal("XXXXXX", gb.CloneOfId);
            Assert.Equal(Data.Models.Metadata.Runnable.Yes, gb.Runnable);

            Assert.NotNull(gb.Comment);
            string comment = Assert.Single(gb.Comment);
            Assert.Equal("XXXXXX", comment);

            Assert.Equal("description", gb.Description);
            Assert.Equal("XXXXXX", gb.Year);
            Assert.Equal("XXXXXX", gb.Manufacturer);
            Assert.Equal("XXXXXX", gb.Publisher);

            Assert.NotNull(gb.Category);
            string category = Assert.Single(gb.Category);
            Assert.Equal("XXXXXX", category);

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
            Assert.Equal("XXXXXX", trurip.TitleID);
            Assert.Equal("XXXXXX", trurip.Publisher);
            Assert.Equal("XXXXXX", trurip.Developer);
            Assert.Equal("XXXXXX", trurip.Year);
            Assert.Equal("XXXXXX", trurip.Genre);
            Assert.Equal("XXXXXX", trurip.Subgenre);
            Assert.Equal("XXXXXX", trurip.Ratings);
            Assert.Equal("XXXXXX", trurip.Score);
            Assert.Equal("XXXXXX", trurip.Players);
            Assert.Equal("XXXXXX", trurip.Enabled);
            Assert.Equal("XXXXXX", trurip.CRC);
            Assert.Equal("XXXXXX", trurip.Source);
            Assert.Equal("XXXXXX", trurip.CloneOf);
            Assert.Equal("XXXXXX", trurip.RelatedTo);
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
            Assert.Equal("XXXXXX", rom.CRC16);
            Assert.Equal("XXXXXX", rom.CRC);
            Assert.Equal("XXXXXX", rom.CRC64);
            Assert.Equal("XXXXXX", rom.MD2);
            Assert.Equal("XXXXXX", rom.MD4);
            Assert.Equal("XXXXXX", rom.MD5);
            Assert.Equal("XXXXXX", rom.RIPEMD128);
            Assert.Equal("XXXXXX", rom.RIPEMD160);
            Assert.Equal("XXXXXX", rom.SHA1);
            Assert.Equal("XXXXXX", rom.SHA256);
            Assert.Equal("XXXXXX", rom.SHA384);
            Assert.Equal("XXXXXX", rom.SHA512);
            Assert.Equal("XXXXXX", rom.SpamSum);
            Assert.Equal("XXXXXX", rom.xxHash364);
            Assert.Equal("XXXXXX", rom.xxHash3128);
            Assert.Equal("XXXXXX", rom.Merge);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal("XXXXXX", rom.Serial);
            Assert.Equal("XXXXXX", rom.Header);
            Assert.Equal("XXXXXX", rom.Date);
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
            Assert.Equal("XXXXXX", disk.MD5);
            Assert.Equal("XXXXXX", disk.SHA1);
            Assert.Equal("XXXXXX", disk.Merge);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, disk.Status);
            Assert.Equal("XXXXXX", disk.Region);
        }

        /// <summary>
        /// Validate a Media
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal("name", media.Name);
            Assert.Equal("XXXXXX", media.MD5);
            Assert.Equal("XXXXXX", media.SHA1);
            Assert.Equal("XXXXXX", media.SHA256);
            Assert.Equal("XXXXXX", media.SpamSum);
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
