using System.IO;
using System.Linq;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class RomCenterTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new RomCenter();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new RomCenter();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new RomCenter();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new RomCenter();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new RomCenter();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new RomCenter();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new RomCenter();
            var serializer = new SabreTools.Serialization.Serializers.RomCenter();

            // Build the data
            Data.Models.RomCenter.MetadataFile mf = Build();

            // Serialize to stream
            Stream? metadata = serializer.SerializeStream(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.RomCenter.MetadataFile? newMf = deserializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Credits);
            Validate(newMf.Dat);
            Validate(newMf.Emulator);
            Validate(newMf.Games);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.RomCenter.MetadataFile Build()
        {
            var credits = new Data.Models.RomCenter.Credits
            {
                Author = "XXXXXX",
                Version = "XXXXXX",
                Email = "XXXXXX",
                Homepage = "XXXXXX",
                Url = "XXXXXX",
                Date = "XXXXXX",
                Comment = "XXXXXX",
            };

            var dat = new Data.Models.RomCenter.Dat
            {
                Version = "XXXXXX",
                Plugin = "XXXXXX",
                Split = "XXXXXX",
                Merge = "XXXXXX",
            };

            var emulator = new Data.Models.RomCenter.Emulator
            {
                RefName = "XXXXXX",
                Version = "XXXXXX",
            };

            var rom = new Data.Models.RomCenter.Rom
            {
                ParentName = "XXXXXX",
                ParentDescription = "XXXXXX",
                GameName = "XXXXXX",
                GameDescription = "XXXXXX",
                RomName = "XXXXXX",
                RomCRC = "XXXXXX",
                RomSize = "XXXXXX",
                RomOf = "XXXXXX",
                MergeName = "XXXXXX",
            };

            var games = new Data.Models.RomCenter.Games
            {
                Rom = [rom],
            };

            return new Data.Models.RomCenter.MetadataFile
            {
                Credits = credits,
                Dat = dat,
                Emulator = emulator,
                Games = games,
            };
        }

        /// <summary>
        /// Validate a Credits
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Credits? credits)
        {
            Assert.NotNull(credits);
            Assert.Equal("XXXXXX", credits.Author);
            Assert.Equal("XXXXXX", credits.Version);
            Assert.Equal("XXXXXX", credits.Email);
            Assert.Equal("XXXXXX", credits.Homepage);
            Assert.Equal("XXXXXX", credits.Url);
            Assert.Equal("XXXXXX", credits.Date);
            Assert.Equal("XXXXXX", credits.Comment);
        }

        /// <summary>
        /// Validate a Dat
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Dat? dat)
        {
            Assert.NotNull(dat);
            Assert.Equal("XXXXXX", dat.Version);
            Assert.Equal("XXXXXX", dat.Plugin);
            Assert.Equal("XXXXXX", dat.Split);
            Assert.Equal("XXXXXX", dat.Merge);
        }

        /// <summary>
        /// Validate a Emulator
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Emulator? emulator)
        {
            Assert.NotNull(emulator);
            Assert.Equal("XXXXXX", emulator.RefName);
            Assert.Equal("XXXXXX", emulator.Version);
        }

        /// <summary>
        /// Validate a Games
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Games? games)
        {
            Assert.NotNull(games);
            Assert.NotNull(games.Rom);
            var rom = Assert.Single(games.Rom);
            Validate(rom);
        }

        /// <summary>
        /// Validate a Rom
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Rom? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("XXXXXX", rom.ParentName);
            Assert.Equal("XXXXXX", rom.ParentDescription);
            Assert.Equal("XXXXXX", rom.GameName);
            Assert.Equal("XXXXXX", rom.GameDescription);
            Assert.Equal("XXXXXX", rom.RomName);
            Assert.Equal("XXXXXX", rom.RomCRC);
            Assert.Equal("XXXXXX", rom.RomSize);
            Assert.Equal("XXXXXX", rom.RomOf);
            Assert.Equal("XXXXXX", rom.MergeName);
        }
    }
}