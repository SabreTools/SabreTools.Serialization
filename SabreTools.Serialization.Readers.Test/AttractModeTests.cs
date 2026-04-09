using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class AttractModeTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new AttractMode();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new AttractMode();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new AttractMode();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new AttractMode();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new AttractMode();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            var data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new AttractMode();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripShortTest()
        {
            // Get the serializer and deserializer
            var deserializer = new AttractMode();
            var serializer = new Writers.AttractMode();

            // Build the data
            Data.Models.AttractMode.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf, longHeader: false);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.AttractMode.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Header, longHeader: false);
            Assert.NotNull(newMf.Row);
            var newRow = Assert.Single(newMf.Row);
            Validate(newRow, longHeader: false);
        }

        [Fact]
        public void RoundTripLongTest()
        {
            // Get the serializer and deserializer
            var deserializer = new AttractMode();
            var serializer = new Writers.AttractMode();

            // Build the data
            Data.Models.AttractMode.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf, longHeader: true);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.AttractMode.MetadataFile? newMf = deserializer.Deserialize(actual);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Header, longHeader: true);
            Assert.NotNull(newMf.Row);
            var newRow = Assert.Single(newMf.Row);
            Validate(newRow, longHeader: true);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.AttractMode.MetadataFile Build()
        {
            string[] header = ["header"];

            var row = new Data.Models.AttractMode.Row
            {
                Name = "name",
                Title = "title",
                Emulator = "emulator",
                CloneOf = "cloneof",
                Year = "year",
                Manufacturer = "manufacturer",
                Category = "category",
                Players = "players",
                Rotation = "rotation",
                Control = "control",
                Status = "status",
                DisplayCount = "displaycount",
                DisplayType = "displaytype",
                AltRomname = "altromname",
                AltTitle = "alttitle",
                Extra = "extra",
                Buttons = "buttons",
                Favorite = "favorite",
                Tags = "tags",
                PlayedCount = "playedcount",
                PlayedTime = "playedtime",
                FileIsAvailable = true,
            };

            return new Data.Models.AttractMode.MetadataFile
            {
                Header = header,
                Row = [row],
            };
        }

        /// <summary>
        /// Validate a header
        /// </summary>
        private static void Validate(string[]? header, bool longHeader)
        {
            Assert.NotNull(header);
            if (longHeader)
                Assert.True(Writers.AttractMode.HeaderArrayWithRomname.SequenceEqual(header));
            else
                Assert.True(Writers.AttractMode.HeaderArrayWithoutRomname.SequenceEqual(header));
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void Validate(Data.Models.AttractMode.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("name", row.Name);
            Assert.Equal("title", row.Title);
            Assert.Equal("emulator", row.Emulator);
            Assert.Equal("cloneof", row.CloneOf);
            Assert.Equal("year", row.Year);
            Assert.Equal("manufacturer", row.Manufacturer);
            Assert.Equal("category", row.Category);
            Assert.Equal("players", row.Players);
            Assert.Equal("rotation", row.Rotation);
            Assert.Equal("control", row.Control);
            Assert.Equal("status", row.Status);
            Assert.Equal("displaycount", row.DisplayCount);
            Assert.Equal("displaytype", row.DisplayType);
            Assert.Equal("altromname", row.AltRomname);
            Assert.Equal("alttitle", row.AltTitle);
            Assert.Equal("extra", row.Extra);
            Assert.Equal("buttons", row.Buttons);
            if (longHeader)
            {
                Assert.Equal("favorite", row.Favorite);
                Assert.Equal("tags", row.Tags);
                Assert.Equal("playedcount", row.PlayedCount);
                Assert.Equal("playedtime", row.PlayedTime);
                Assert.Equal(true, row.FileIsAvailable);
            }
            else
            {
                Assert.Null(row.Favorite);
                Assert.Null(row.Tags);
                Assert.Null(row.PlayedCount);
                Assert.Null(row.PlayedTime);
                Assert.Null(row.FileIsAvailable);
            }
        }
    }
}
