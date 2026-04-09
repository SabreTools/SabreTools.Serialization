using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class ClrMameProTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new ClrMamePro();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new ClrMamePro();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new ClrMamePro();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new ClrMamePro();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new ClrMamePro();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new ClrMamePro();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripGameTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ClrMamePro();
            var serializer = new Writers.ClrMamePro();

            // Build the data
            Data.Models.ClrMamePro.MetadataFile mf = Build(game: true);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.ClrMamePro.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.ClrMamePro);
            Assert.NotNull(newMf.Game);
            var newGame = Assert.Single(newMf.Game);
            Validate(newGame);
            Validate(newMf.Info);
        }

        [Fact]
        public void RoundTripGameWithoutQuotesTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ClrMamePro();
            var serializer = new Writers.ClrMamePro();

            // Build the data
            Data.Models.ClrMamePro.MetadataFile mf = Build(game: true);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf, quotes: false);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.ClrMamePro.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.ClrMamePro);
            Assert.NotNull(newMf.Game);
            var newGame = Assert.Single(newMf.Game);
            Validate(newGame);
            Validate(newMf.Info);
        }

        [Fact]
        public void RoundTripMachineTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ClrMamePro();
            var serializer = new Writers.ClrMamePro();

            // Build the data
            Data.Models.ClrMamePro.MetadataFile mf = Build(game: false);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.ClrMamePro.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.ClrMamePro);
            Assert.NotNull(newMf.Game);
            var newGame = Assert.Single(newMf.Game);
            Validate(newGame);
        }

        [Fact]
        public void RoundTripMachineWithoutQuotesTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ClrMamePro();
            var serializer = new Writers.ClrMamePro();

            // Build the data
            Data.Models.ClrMamePro.MetadataFile mf = Build(game: false);

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf, quotes: false);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.ClrMamePro.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.ClrMamePro);
            Assert.NotNull(newMf.Game);
            var newGame = Assert.Single(newMf.Game);
            Validate(newGame);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.ClrMamePro.MetadataFile Build(bool game)
        {
            var cmp = new Data.Models.ClrMamePro.ClrMamePro
            {
                Name = "name",
                Description = "description",
                RootDir = "rootdir",
                Category = "category",
                Version = "version",
                Date = "date",
                Author = "author",
                Homepage = "homepage",
                Url = "url",
                Comment = "comment",
                Header = "header",
                Type = "type",
                ForceMerging = Data.Models.Metadata.MergingFlag.Merged,
                ForceZipping = true,
                ForcePacking = Data.Models.Metadata.PackingFlag.Zip,
            };

            var release = new Data.Models.ClrMamePro.Release
            {
                Name = "name",
                Region = "region",
                Language = "language",
                Date = "date",
                Default = true,
            };

            var biosset = new Data.Models.ClrMamePro.BiosSet
            {
                Name = "name",
                Description = "description",
                Default = true,
            };

            var rom = new Data.Models.ClrMamePro.Rom
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
                Merge = "merge",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Flags = "flags",
                Date = "date",
                SHA256 = "sha256",
                SHA384 = "sha384",
                SHA512 = "sha512",
                SpamSum = "spamsum",
                xxHash364 = "xxhash364",
                xxHash3128 = "xxhash3128",
                Region = "region",
                Offs = "offs",
                Serial = "serial",
                Header = "header",
                Inverted = true,
                MIA = true,
            };

            var disk = new Data.Models.ClrMamePro.Disk
            {
                Name = "name",
                MD5 = "md5",
                SHA1 = "sha1",
                Merge = "merge",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Flags = "flags",
            };

            var sample = new Data.Models.ClrMamePro.Sample
            {
                Name = "name",
            };

            var archive = new Data.Models.ClrMamePro.Archive
            {
                Name = "name",
            };

            var media = new Data.Models.ClrMamePro.Media
            {
                Name = "name",
                MD5 = "md5",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SpamSum = "spamsum",
            };

            var chip = new Data.Models.ClrMamePro.Chip
            {
                Type = Data.Models.Metadata.ChipType.CPU,
                Name = "name",
                Flags = "flags",
                Clock = 12345,
            };

            var video = new Data.Models.ClrMamePro.Video
            {
                Screen = Data.Models.Metadata.DisplayType.Vector,
                Orientation = Data.Models.Metadata.Rotation.East,
                X = 12345,
                Y = 12345,
                AspectX = 12345,
                AspectY = 12345,
                Freq = 123.45,
            };

            var sound = new Data.Models.ClrMamePro.Sound
            {
                Channels = 12345
            };

            var input = new Data.Models.ClrMamePro.Input
            {
                Players = 12345,
                Control = "control",
                Buttons = 12345,
                Coins = 12345,
                Tilt = true,
                Service = true,
            };

            var dipswitch = new Data.Models.ClrMamePro.DipSwitch
            {
                Name = "name",
                Entry = ["entry"],
                Default = true,
            };

            var driver = new Data.Models.ClrMamePro.Driver
            {
                Status = Data.Models.Metadata.SupportStatus.Good,
                Color = Data.Models.Metadata.SupportStatus.Good,
                Sound = Data.Models.Metadata.SupportStatus.Good,
                PaletteSize = "palettesize",
                Blit = Data.Models.Metadata.Blit.Plain,
            };

            // TODO: This omits Set, should that have a separate case?
            Data.Models.ClrMamePro.GameBase gameBase = game
                ? new Data.Models.ClrMamePro.Game()
                : new Data.Models.ClrMamePro.Machine();
            gameBase.Name = "name";
            gameBase.Description = "description";
            gameBase.DriverStatus = "driverstatus";
            gameBase.Year = "year";
            gameBase.Manufacturer = "manufacturer";
            gameBase.Category = "category";
            gameBase.CloneOf = "cloneof";
            gameBase.RomOf = "romof";
            gameBase.SampleOf = "sampleof";
            gameBase.Release = [release];
            gameBase.BiosSet = [biosset];
            gameBase.Rom = [rom];
            gameBase.Disk = [disk];
            gameBase.Sample = [sample];
            gameBase.Archive = [archive];
            gameBase.Media = [media];
            gameBase.Chip = [chip];
            gameBase.Video = [video];
            gameBase.Sound = sound;
            gameBase.Input = input;
            gameBase.DipSwitch = [dipswitch];
            gameBase.Driver = driver;

            var info = new Data.Models.ClrMamePro.Info
            {
                Source = ["source"],
            };

            return new Data.Models.ClrMamePro.MetadataFile
            {
                ClrMamePro = cmp,
                Game = [gameBase],
                Info = info,
            };
        }

        /// <summary>
        /// Validate a ClrMamePro
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.ClrMamePro? cmp)
        {
            Assert.NotNull(cmp);
            Assert.Equal("name", cmp.Name);
            Assert.Equal("description", cmp.Description);
            Assert.Equal("rootdir", cmp.RootDir);
            Assert.Equal("category", cmp.Category);
            Assert.Equal("version", cmp.Version);
            Assert.Equal("date", cmp.Date);
            Assert.Equal("author", cmp.Author);
            Assert.Equal("homepage", cmp.Homepage);
            Assert.Equal("url", cmp.Url);
            Assert.Equal("comment", cmp.Comment);
            Assert.Equal("header", cmp.Header);
            Assert.Equal("type", cmp.Type);
            Assert.Equal(Data.Models.Metadata.MergingFlag.Merged, cmp.ForceMerging);
            Assert.Equal(true, cmp.ForceZipping);
            Assert.Equal(Data.Models.Metadata.PackingFlag.Zip, cmp.ForcePacking);
        }

        /// <summary>
        /// Validate a GameBase
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.GameBase? gb)
        {
            Assert.NotNull(gb);
            Assert.Equal("name", gb.Name);
            Assert.Equal("description", gb.Description);
            // Assert.Equal("driverstatus", gb.DriverStatus); // TODO: Needs metadata mapping
            Assert.Equal("year", gb.Year);
            Assert.Equal("manufacturer", gb.Manufacturer);
            Assert.Equal("category", gb.Category);
            Assert.Equal("cloneof", gb.CloneOf);
            Assert.Equal("romof", gb.RomOf);
            Assert.Equal("sampleof", gb.SampleOf);

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

            Assert.NotNull(gb.Sample);
            var sample = Assert.Single(gb.Sample);
            Validate(sample);

            Assert.NotNull(gb.Archive);
            var archive = Assert.Single(gb.Archive);
            Validate(archive);

            Assert.NotNull(gb.Media);
            var media = Assert.Single(gb.Media);
            Validate(media);

            Assert.NotNull(gb.Chip);
            var chip = Assert.Single(gb.Chip);
            Validate(chip);

            Assert.NotNull(gb.Video);
            var video = Assert.Single(gb.Video);
            Validate(video);

            Validate(gb.Sound);
            Validate(gb.Input);

            Assert.NotNull(gb.DipSwitch);
            var dipswitch = Assert.Single(gb.DipSwitch);
            Validate(dipswitch);

            Validate(gb.Driver);
        }

        /// <summary>
        /// Validate a Release
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Release? release)
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
        private static void Validate(Data.Models.ClrMamePro.BiosSet? biosset)
        {
            Assert.NotNull(biosset);
            Assert.Equal("name", biosset.Name);
            Assert.Equal("description", biosset.Description);
            Assert.Equal(true, biosset.Default);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Rom? rom)
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
            Assert.Equal("merge", rom.Merge);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, rom.Status);
            Assert.Equal("flags", rom.Flags);
            Assert.Equal("date", rom.Date);
            Assert.Equal("sha256", rom.SHA256);
            Assert.Equal("sha384", rom.SHA384);
            Assert.Equal("sha512", rom.SHA512);
            Assert.Equal("spamsum", rom.SpamSum);
            Assert.Equal("xxhash364", rom.xxHash364);
            Assert.Equal("xxhash3128", rom.xxHash3128);
            Assert.Equal("region", rom.Region);
            Assert.Equal("offs", rom.Offs);
            Assert.Equal("serial", rom.Serial);
            Assert.Equal("header", rom.Header);
            Assert.Equal(true, rom.Inverted);
            Assert.Equal(true, rom.MIA);
        }

        /// <summary>
        /// Validate a Disk
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("name", disk.Name);
            Assert.Equal("md5", disk.MD5);
            Assert.Equal("sha1", disk.SHA1);
            Assert.Equal("merge", disk.Merge);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, disk.Status);
            Assert.Equal("flags", disk.Flags);
        }

        /// <summary>
        /// Validate a Sample
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("name", sample.Name);
        }

        /// <summary>
        /// Validate a Archive
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("name", archive.Name);
        }

        /// <summary>
        /// Validate a Media
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal("name", media.Name);
            Assert.Equal("md5", media.MD5);
            Assert.Equal("sha1", media.SHA1);
            Assert.Equal("sha256", media.SHA256);
            Assert.Equal("spamsum", media.SpamSum);
        }

        /// <summary>
        /// Validate a Chip
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal(Data.Models.Metadata.ChipType.CPU, chip.Type);
            Assert.Equal("name", chip.Name);
            Assert.Equal("flags", chip.Flags);
            Assert.Equal(12345, chip.Clock);
        }

        /// <summary>
        /// Validate a Video
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Video? video)
        {
            Assert.NotNull(video);
            Assert.Equal(Data.Models.Metadata.DisplayType.Vector, video.Screen);
            Assert.Equal(Data.Models.Metadata.Rotation.East, video.Orientation);
            Assert.Equal(12345, video.X);
            Assert.Equal(12345, video.Y);
            Assert.Equal(12345, video.AspectX);
            Assert.Equal(12345, video.AspectY);
            Assert.Equal(123.45, video.Freq);
        }

        /// <summary>
        /// Validate a Sound
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal(12345, sound.Channels);
        }

        /// <summary>
        /// Validate a Input
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal(12345, input.Players);
            Assert.Equal("control", input.Control);
            Assert.Equal(12345, input.Buttons);
            Assert.Equal(12345, input.Coins);
            Assert.Equal(true, input.Tilt);
            Assert.Equal(true, input.Service);
        }

        /// <summary>
        /// Validate a DipSwitch
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.DipSwitch? dipswitch)
        {
            Assert.NotNull(dipswitch);
            Assert.Equal("name", dipswitch.Name);

            Assert.NotNull(dipswitch.Entry);
            string entry = Assert.Single(dipswitch.Entry);
            Assert.Equal("entry", entry);

            Assert.Equal(true, dipswitch.Default);
        }

        /// <summary>
        /// Validate a Driver
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Status);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Color);
            Assert.Equal(Data.Models.Metadata.SupportStatus.Good, driver.Sound);
            Assert.Equal("palettesize", driver.PaletteSize);
            Assert.Equal(Data.Models.Metadata.Blit.Plain, driver.Blit);
        }

        /// <summary>
        /// Validate a ClrMamePro
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Info? info)
        {
            Assert.NotNull(info);
            Assert.NotNull(info.Source);
            string source = Assert.Single(info.Source);
            Assert.Equal("source", source);
        }
    }
}
