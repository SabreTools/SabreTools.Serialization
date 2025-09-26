using System.IO;
using System.Linq;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
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
            var serializer = new SabreTools.Serialization.Serializers.ClrMamePro();

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
        }

        [Fact]
        public void RoundTripGameWithoutQuotesTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ClrMamePro();
            var serializer = new SabreTools.Serialization.Serializers.ClrMamePro();

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
        }

        [Fact]
        public void RoundTripMachineTest()
        {
            // Get the serializer and deserializer
            var deserializer = new ClrMamePro();
            var serializer = new SabreTools.Serialization.Serializers.ClrMamePro();

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
            var serializer = new SabreTools.Serialization.Serializers.ClrMamePro();

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
                Name = "XXXXXX",
                Description = "XXXXXX",
                RootDir = "XXXXXX",
                Category = "XXXXXX",
                Version = "XXXXXX",
                Date = "XXXXXX",
                Author = "XXXXXX",
                Homepage = "XXXXXX",
                Url = "XXXXXX",
                Comment = "XXXXXX",
                Header = "XXXXXX",
                Type = "XXXXXX",
                ForceMerging = "XXXXXX",
                ForceZipping = "XXXXXX",
                ForcePacking = "XXXXXX",
            };

            var release = new Data.Models.ClrMamePro.Release
            {
                Name = "XXXXXX",
                Region = "XXXXXX",
                Language = "XXXXXX",
                Date = "XXXXXX",
                Default = "XXXXXX",
            };

            var biosset = new Data.Models.ClrMamePro.BiosSet
            {
                Name = "XXXXXX",
                Description = "XXXXXX",
                Default = "XXXXXX",
            };

            var rom = new Data.Models.ClrMamePro.Rom
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
                Merge = "XXXXXX",
                Status = "XXXXXX",
                Flags = "XXXXXX",
                Date = "XXXXXX",
                SHA256 = "XXXXXX",
                SHA384 = "XXXXXX",
                SHA512 = "XXXXXX",
                SpamSum = "XXXXXX",
                xxHash364 = "XXXXXX",
                xxHash3128 = "XXXXXX",
                Region = "XXXXXX",
                Offs = "XXXXXX",
                Serial = "XXXXXX",
                Header = "XXXXXX",
                Inverted = "XXXXXX",
                MIA = "XXXXXX",
            };

            var disk = new Data.Models.ClrMamePro.Disk
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                Merge = "XXXXXX",
                Status = "XXXXXX",
                Flags = "XXXXXX",
            };

            var sample = new Data.Models.ClrMamePro.Sample
            {
                Name = "XXXXXX",
            };

            var archive = new Data.Models.ClrMamePro.Archive
            {
                Name = "XXXXXX",
            };

            var media = new Data.Models.ClrMamePro.Media
            {
                Name = "XXXXXX",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            var chip = new Data.Models.ClrMamePro.Chip
            {
                Type = "XXXXXX",
                Name = "XXXXXX",
                Flags = "XXXXXX",
                Clock = "XXXXXX",
            };

            var video = new Data.Models.ClrMamePro.Video
            {
                Screen = "XXXXXX",
                Orientation = "XXXXXX",
                X = "XXXXXX",
                Y = "XXXXXX",
                AspectX = "XXXXXX",
                AspectY = "XXXXXX",
                Freq = "XXXXXX",
            };

            var sound = new Data.Models.ClrMamePro.Sound
            {
                Channels = "XXXXXX",
            };

            var input = new Data.Models.ClrMamePro.Input
            {
                Players = "XXXXXX",
                Control = "XXXXXX",
                Buttons = "XXXXXX",
                Coins = "XXXXXX",
                Tilt = "XXXXXX",
                Service = "XXXXXX",
            };

            var dipswitch = new Data.Models.ClrMamePro.DipSwitch
            {
                Name = "XXXXXX",
                Entry = ["XXXXXX"],
                Default = "XXXXXX",
            };

            var driver = new Data.Models.ClrMamePro.Driver
            {
                Status = "XXXXXX",
                Color = "XXXXXX",
                Sound = "XXXXXX",
                PaletteSize = "XXXXXX",
                Blit = "XXXXXX",
            };

            // TODO: This omits Set, should that have a separate case?
            Data.Models.ClrMamePro.GameBase gameBase = game
                ? new Data.Models.ClrMamePro.Game()
                : new Data.Models.ClrMamePro.Machine();
            gameBase.Name = "XXXXXX";
            gameBase.Description = "XXXXXX";
            gameBase.Year = "XXXXXX";
            gameBase.Manufacturer = "XXXXXX";
            gameBase.Category = "XXXXXX";
            gameBase.CloneOf = "XXXXXX";
            gameBase.RomOf = "XXXXXX";
            gameBase.SampleOf = "XXXXXX";
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

            return new Data.Models.ClrMamePro.MetadataFile
            {
                ClrMamePro = cmp,
                Game = [gameBase],
            };
        }

        /// <summary>
        /// Validate a ClrMamePro
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.ClrMamePro? cmp)
        {
            Assert.NotNull(cmp);
            Assert.Equal("XXXXXX", cmp.Name);
            Assert.Equal("XXXXXX", cmp.Description);
            Assert.Equal("XXXXXX", cmp.RootDir);
            Assert.Equal("XXXXXX", cmp.Category);
            Assert.Equal("XXXXXX", cmp.Version);
            Assert.Equal("XXXXXX", cmp.Date);
            Assert.Equal("XXXXXX", cmp.Author);
            Assert.Equal("XXXXXX", cmp.Homepage);
            Assert.Equal("XXXXXX", cmp.Url);
            Assert.Equal("XXXXXX", cmp.Comment);
            Assert.Equal("XXXXXX", cmp.Header);
            Assert.Equal("XXXXXX", cmp.Type);
            Assert.Equal("XXXXXX", cmp.ForceMerging);
            Assert.Equal("XXXXXX", cmp.ForceZipping);
            Assert.Equal("XXXXXX", cmp.ForcePacking);
        }

        /// <summary>
        /// Validate a GameBase
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.GameBase? gb)
        {
            Assert.NotNull(gb);
            Assert.Equal("XXXXXX", gb.Name);
            Assert.Equal("XXXXXX", gb.Description);
            Assert.Equal("XXXXXX", gb.Year);
            Assert.Equal("XXXXXX", gb.Manufacturer);
            Assert.Equal("XXXXXX", gb.Category);
            Assert.Equal("XXXXXX", gb.CloneOf);
            Assert.Equal("XXXXXX", gb.RomOf);
            Assert.Equal("XXXXXX", gb.SampleOf);

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
            Assert.Equal("XXXXXX", release.Name);
            Assert.Equal("XXXXXX", release.Region);
            Assert.Equal("XXXXXX", release.Language);
            Assert.Equal("XXXXXX", release.Date);
            Assert.Equal("XXXXXX", release.Default);
        }

        /// <summary>
        /// Validate a BiosSet
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.BiosSet? biosset)
        {
            Assert.NotNull(biosset);
            Assert.Equal("XXXXXX", biosset.Name);
            Assert.Equal("XXXXXX", biosset.Description);
            Assert.Equal("XXXXXX", biosset.Default);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Rom? rom)
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
            Assert.Equal("XXXXXX", rom.Merge);
            Assert.Equal("XXXXXX", rom.Status);
            Assert.Equal("XXXXXX", rom.Flags);
            Assert.Equal("XXXXXX", rom.Date);
            Assert.Equal("XXXXXX", rom.SHA256);
            Assert.Equal("XXXXXX", rom.SHA384);
            Assert.Equal("XXXXXX", rom.SHA512);
            Assert.Equal("XXXXXX", rom.SpamSum);
            Assert.Equal("XXXXXX", rom.xxHash364);
            Assert.Equal("XXXXXX", rom.xxHash3128);
            Assert.Equal("XXXXXX", rom.Region);
            Assert.Equal("XXXXXX", rom.Offs);
            Assert.Equal("XXXXXX", rom.Serial);
            Assert.Equal("XXXXXX", rom.Header);
            Assert.Equal("XXXXXX", rom.Inverted);
            Assert.Equal("XXXXXX", rom.MIA);
        }

        /// <summary>
        /// Validate a Disk
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Disk? disk)
        {
            Assert.NotNull(disk);
            Assert.Equal("XXXXXX", disk.Name);
            Assert.Equal("XXXXXX", disk.MD5);
            Assert.Equal("XXXXXX", disk.SHA1);
            Assert.Equal("XXXXXX", disk.Merge);
            Assert.Equal("XXXXXX", disk.Status);
            Assert.Equal("XXXXXX", disk.Flags);
        }

        /// <summary>
        /// Validate a Sample
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Sample? sample)
        {
            Assert.NotNull(sample);
            Assert.Equal("XXXXXX", sample.Name);
        }

        /// <summary>
        /// Validate a Archive
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Archive? archive)
        {
            Assert.NotNull(archive);
            Assert.Equal("XXXXXX", archive.Name);
        }

        /// <summary>
        /// Validate a Media
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Media? media)
        {
            Assert.NotNull(media);
            Assert.Equal("XXXXXX", media.Name);
            Assert.Equal("XXXXXX", media.MD5);
            Assert.Equal("XXXXXX", media.SHA1);
            Assert.Equal("XXXXXX", media.SHA256);
            Assert.Equal("XXXXXX", media.SpamSum);
        }

        /// <summary>
        /// Validate a Chip
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Chip? chip)
        {
            Assert.NotNull(chip);
            Assert.Equal("XXXXXX", chip.Type);
            Assert.Equal("XXXXXX", chip.Name);
            Assert.Equal("XXXXXX", chip.Flags);
            Assert.Equal("XXXXXX", chip.Clock);
        }

        /// <summary>
        /// Validate a Video
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Video? video)
        {
            Assert.NotNull(video);
            Assert.Equal("XXXXXX", video.Screen);
            Assert.Equal("XXXXXX", video.Orientation);
            Assert.Equal("XXXXXX", video.X);
            Assert.Equal("XXXXXX", video.Y);
            Assert.Equal("XXXXXX", video.AspectX);
            Assert.Equal("XXXXXX", video.AspectY);
            Assert.Equal("XXXXXX", video.Freq);
        }

        /// <summary>
        /// Validate a Sound
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Sound? sound)
        {
            Assert.NotNull(sound);
            Assert.Equal("XXXXXX", sound.Channels);
        }

        /// <summary>
        /// Validate a Input
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Input? input)
        {
            Assert.NotNull(input);
            Assert.Equal("XXXXXX", input.Players);
            Assert.Equal("XXXXXX", input.Control);
            Assert.Equal("XXXXXX", input.Buttons);
            Assert.Equal("XXXXXX", input.Coins);
            Assert.Equal("XXXXXX", input.Tilt);
            Assert.Equal("XXXXXX", input.Service);
        }

        /// <summary>
        /// Validate a DipSwitch
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.DipSwitch? dipswitch)
        {
            Assert.NotNull(dipswitch);
            Assert.Equal("XXXXXX", dipswitch.Name);

            Assert.NotNull(dipswitch.Entry);
            string entry = Assert.Single(dipswitch.Entry);
            Assert.Equal("XXXXXX", entry);

            Assert.Equal("XXXXXX", dipswitch.Default);
        }

        /// <summary>
        /// Validate a Driver
        /// </summary>
        private static void Validate(Data.Models.ClrMamePro.Driver? driver)
        {
            Assert.NotNull(driver);
            Assert.Equal("XXXXXX", driver.Status);
            Assert.Equal("XXXXXX", driver.Color);
            Assert.Equal("XXXXXX", driver.Sound);
            Assert.Equal("XXXXXX", driver.PaletteSize);
            Assert.Equal("XXXXXX", driver.Blit);
        }
    }
}