using System;
using System.IO;
using System.Linq;
using SabreTools.Serialization.Deserializers;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class DosCenterTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new DosCenter();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new DosCenter();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new DosCenter();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new DosCenter();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new DosCenter();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new DosCenter();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new Serialization.Deserializers.DosCenter();
            var serializer = new Serialization.Serializers.DosCenter();

            // Build the data
            Models.DosCenter.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.Serialize(mf);
            Assert.NotNull(actual);

            // Serialize back to original model
            Models.DosCenter.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.DosCenter);
            Assert.NotNull(newMf.Game);
            var newGame = Assert.Single(newMf.Game);
            Validate(newGame);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.DosCenter.MetadataFile Build()
        {
            var dc = new Models.DosCenter.DosCenter
            {
                Name = "XXXXXX",
                Description = "XXXXXX",
                Version = "XXXXXX",
                Date = "XXXXXX",
                Author = "XXXXXX",
                Homepage = "XXXXXX",
                Comment = "XXXXXX",
            };

            var file = new Models.DosCenter.File
            {
                Name = "XXXXXX",
                Size = "XXXXXX",
                CRC = "XXXXXX",
                SHA1 = "XXXXXX",
                Date = "XXXXXX XXXXXX",
            };

            var game = new Models.DosCenter.Game
            {
                Name = "XXXXXX",
                File = [file],
            };

            return new Models.DosCenter.MetadataFile
            {
                DosCenter = dc,
                Game = [game],
            };
        }

        /// <summary>
        /// Validate a DosCenter
        /// </summary>
        private static void Validate(Models.DosCenter.DosCenter? cmp)
        {
            Assert.NotNull(cmp);
            Assert.Equal("XXXXXX", cmp.Name);
            Assert.Equal("XXXXXX", cmp.Description);
            Assert.Equal("XXXXXX", cmp.Version);
            Assert.Equal("XXXXXX", cmp.Date);
            Assert.Equal("XXXXXX", cmp.Author);
            Assert.Equal("XXXXXX", cmp.Homepage);
            Assert.Equal("XXXXXX", cmp.Comment);
        }

        /// <summary>
        /// Validate a Game
        /// </summary>
        private static void Validate(Models.DosCenter.Game? game)
        {
            Assert.NotNull(game);
            Assert.Equal("XXXXXX", game.Name);

            Assert.NotNull(game.File);
            var file = Assert.Single(game.File);
            Validate(file);
        }

        /// <summary>
        /// Validate a File
        /// </summary>
        private static void Validate(Models.DosCenter.File? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("XXXXXX", rom.Name);
            Assert.Equal("XXXXXX", rom.Size);
            Assert.Equal("XXXXXX", rom.CRC);
            Assert.Equal("XXXXXX", rom.SHA1);
            Assert.Equal("XXXXXX XXXXXX", rom.Date);
        }
    }
}