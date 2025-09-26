using SabreTools.Serialization.CrossModel;
using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
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
            Assert.Equal("XXXXXX", newDf.Debug);
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
            Assert.Equal("XXXXXX", newDf.Debug);
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
                ForceMerging = "XXXXXX",
                ForceNodump = "XXXXXX",
                ForcePacking = "XXXXXX",
            };

            var romcenter = new Data.Models.Logiqx.RomCenter
            {
                Plugin = "XXXXXX",
                RomMode = "XXXXXX",
                BiosMode = "XXXXXX",
                SampleMode = "XXXXXX",
                LockRomMode = "XXXXXX",
                LockBiosMode = "XXXXXX",
                LockSampleMode = "XXXXXX",
            };

            var header = new Data.Models.Logiqx.Header
            {
                Id = "XXXXXX",
                Name = "XXXXXX",
                Description = "XXXXXX",
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
                Name = "XXXXXX",
                Region = "XXXXXX",
                Language = "XXXXXX",
                Date = "XXXXXX",
                Default = "XXXXXX",
            };

            var biosset = new Data.Models.Logiqx.BiosSet
            {
                Name = "XXXXXX",
                Description = "XXXXXX",
                Default = "XXXXXX",
            };

            var rom = new Data.Models.Logiqx.Rom
            {
                Name = "XXXXXX",
                Size = "XXXXXX",
                CRC = "XXXXXX",
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
                Status = "XXXXXX",
                Serial = "XXXXXX",
                Header = "XXXXXX",
                Date = "XXXXXX",
                Inverted = "XXXXXX",
                MIA = "XXXXXX",
            };

            var disk = new Data.Models.Logiqx.Disk
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Merge = "XXXXXX",
                Status = "XXXXXX",
                Region = "XXXXXX",
            };

            var media = new Data.Models.Logiqx.Media
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            var deviceRef = new Data.Models.Logiqx.DeviceRef
            {
                Name = "XXXXXX",
            };

            var sample = new Data.Models.Logiqx.Sample
            {
                Name = "XXXXXX",
            };

            var archive = new Data.Models.Logiqx.Archive
            {
                Name = "XXXXXX",
            };

            var driver = new Data.Models.Logiqx.Driver
            {
                Status = "XXXXXX",
                Emulation = "XXXXXX",
                Cocktail = "XXXXXX",
                SaveState = "XXXXXX",
                RequiresArtwork = "XXXXXX",
                Unofficial = "XXXXXX",
                NoSoundHardware = "XXXXXX",
                Incomplete = "XXXXXX",
            };

            var softwarelist = new Data.Models.Logiqx.SoftwareList
            {
                Tag = "XXXXXX",
                Name = "XXXXXX",
                Status = "XXXXXX",
                Filter = "XXXXXX",
            };

            Data.Models.Logiqx.GameBase gameBase = game
                ? new Data.Models.Logiqx.Game()
                : new Data.Models.Logiqx.Machine();
            gameBase.Name = "XXXXXX";
            gameBase.SourceFile = "XXXXXX";
            gameBase.IsBios = "XXXXXX";
            gameBase.IsDevice = "XXXXXX";
            gameBase.IsMechanical = "XXXXXX";
            gameBase.CloneOf = "XXXXXX";
            gameBase.RomOf = "XXXXXX";
            gameBase.SampleOf = "XXXXXX";
            gameBase.Board = "XXXXXX";
            gameBase.RebuildTo = "XXXXXX";
            gameBase.Id = "XXXXXX";
            gameBase.CloneOfId = "XXXXXX";
            gameBase.Runnable = "XXXXXX";
            gameBase.Comment = ["XXXXXX"];
            gameBase.Description = "XXXXXX";
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
                Name = "XXXXXX",
                Game = [gameBase],
            };

            var dir = new Data.Models.Logiqx.Dir
            {
                Name = "XXXXXX",
                Subdir = [subdir],
            };

            return new Data.Models.Logiqx.Datafile
            {
                Build = "XXXXXX",
                Debug = "XXXXXX",
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
            Assert.Equal("XXXXXX", header.Name);
            Assert.Equal("XXXXXX", header.Description);
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
            Assert.Equal("XXXXXX", cmp.ForceMerging);
            Assert.Equal("XXXXXX", cmp.ForceNodump);
            Assert.Equal("XXXXXX", cmp.ForcePacking);
        }

        /// <summary>
        /// Validate a RomCenter
        /// </summary>
        private static void Validate(Data.Models.Logiqx.RomCenter? rc)
        {
            Assert.NotNull(rc);
            Assert.Equal("XXXXXX", rc.Plugin);
            Assert.Equal("XXXXXX", rc.RomMode);
            Assert.Equal("XXXXXX", rc.BiosMode);
            Assert.Equal("XXXXXX", rc.SampleMode);
            Assert.Equal("XXXXXX", rc.LockRomMode);
            Assert.Equal("XXXXXX", rc.LockBiosMode);
            Assert.Equal("XXXXXX", rc.LockSampleMode);
        }

        /// <summary>
        /// Validate a GameBase
        /// </summary>
        private static void Validate(Data.Models.Logiqx.GameBase? gb, bool nested)
        {
            Assert.NotNull(gb);
            if (nested)
                Assert.Equal("XXXXXX\\XXXXXX\\XXXXXX", gb.Name);
            else
                Assert.Equal("XXXXXX", gb.Name);
            Assert.Equal("XXXXXX", gb.SourceFile);
            Assert.Equal("XXXXXX", gb.IsBios);
            Assert.Equal("XXXXXX", gb.IsDevice);
            Assert.Equal("XXXXXX", gb.IsMechanical);
            Assert.Equal("XXXXXX", gb.CloneOf);
            Assert.Equal("XXXXXX", gb.RomOf);
            Assert.Equal("XXXXXX", gb.SampleOf);
            Assert.Equal("XXXXXX", gb.Board);
            Assert.Equal("XXXXXX", gb.RebuildTo);
            Assert.Equal("XXXXXX", gb.Id);
            Assert.Equal("XXXXXX", gb.CloneOfId);
            Assert.Equal("XXXXXX", gb.Runnable);

            Assert.NotNull(gb.Comment);
            string comment = Assert.Single(gb.Comment);
            Assert.Equal("XXXXXX", comment);

            Assert.Equal("XXXXXX", gb.Description);
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
            Assert.Equal("XXXXXX", release.Name);
            Assert.Equal("XXXXXX", release.Region);
            Assert.Equal("XXXXXX", release.Language);
            Assert.Equal("XXXXXX", release.Date);
            Assert.Equal("XXXXXX", release.Default);
        }

        /// <summary>
        /// Validate a BiosSet
        /// </summary>
        private static void Validate(Data.Models.Logiqx.BiosSet? biosset)
        {
            Assert.NotNull(biosset);
            Assert.Equal("XXXXXX", biosset.Name);
            Assert.Equal("XXXXXX", biosset.Description);
            Assert.Equal("XXXXXX", biosset.Default);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("XXXXXX", rom.Name);
            Assert.Equal("XXXXXX", rom.Size);
            Assert.Equal("XXXXXX", rom.CRC);
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
            Assert.Equal("XXXXXX", rom.Status);
            Assert.Equal("XXXXXX", rom.Serial);
            Assert.Equal("XXXXXX", rom.Header);
            Assert.Equal("XXXXXX", rom.Date);
            Assert.Equal("XXXXXX", rom.Inverted);
            Assert.Equal("XXXXXX", rom.MIA);
        }

        /// <summary>
        /// Validate a Disk
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("XXXXXX", disk.Name);
            Assert.Equal("XXXXXX", disk.MD5);
            Assert.Equal("XXXXXX", disk.SHA1);
            Assert.Equal("XXXXXX", disk.Merge);
            Assert.Equal("XXXXXX", disk.Status);
            Assert.Equal("XXXXXX", disk.Region);
        }

        /// <summary>
        /// Validate a Media
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal("XXXXXX", media.Name);
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
            Assert.Equal("XXXXXX", deviceref.Name);
        }

        /// <summary>
        /// Validate a Sample
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("XXXXXX", sample.Name);
        }

        /// <summary>
        /// Validate a Archive
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("XXXXXX", archive.Name);
        }

        /// <summary>
        /// Validate a Driver
        /// </summary>
        private static void Validate(Data.Models.Logiqx.Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal("XXXXXX", driver.Status);
            Assert.Equal("XXXXXX", driver.Emulation);
            Assert.Equal("XXXXXX", driver.Cocktail);
            Assert.Equal("XXXXXX", driver.SaveState);
            Assert.Equal("XXXXXX", driver.RequiresArtwork);
            Assert.Equal("XXXXXX", driver.Unofficial);
            Assert.Equal("XXXXXX", driver.NoSoundHardware);
            Assert.Equal("XXXXXX", driver.Incomplete);
        }

        /// <summary>
        /// Validate a SoftwareList
        /// </summary>
        private static void Validate(Data.Models.Logiqx.SoftwareList? softwarelist)
        {
            Assert.NotNull(softwarelist);
            Assert.Equal("XXXXXX", softwarelist.Tag);
            Assert.Equal("XXXXXX", softwarelist.Name);
            Assert.Equal("XXXXXX", softwarelist.Status);
            Assert.Equal("XXXXXX", softwarelist.Filter);
        }
    }
}