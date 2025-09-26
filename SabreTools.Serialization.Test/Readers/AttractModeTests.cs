using System;
using System.IO;
using System.Linq;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
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
            var serializer = new SabreTools.Serialization.Writers.AttractMode();

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
            var serializer = new SabreTools.Serialization.Writers.AttractMode();

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
                Name = "XXXXXX",
                Title = "XXXXXX",
                Emulator = "XXXXXX",
                CloneOf = "XXXXXX",
                Year = "XXXXXX",
                Manufacturer = "XXXXXX",
                Category = "XXXXXX",
                Players = "XXXXXX",
                Rotation = "XXXXXX",
                Control = "XXXXXX",
                Status = "XXXXXX",
                DisplayCount = "XXXXXX",
                DisplayType = "XXXXXX",
                AltRomname = "XXXXXX",
                AltTitle = "XXXXXX",
                Extra = "XXXXXX",
                Buttons = "XXXXXX",
                Favorite = "XXXXXX",
                Tags = "XXXXXX",
                PlayedCount = "XXXXXX",
                PlayedTime = "XXXXXX",
                FileIsAvailable = "XXXXXX",
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
                Assert.True(SabreTools.Serialization.Writers.AttractMode.HeaderArrayWithRomname.SequenceEqual(header));
            else
                Assert.True(SabreTools.Serialization.Writers.AttractMode.HeaderArrayWithoutRomname.SequenceEqual(header));
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void Validate(Data.Models.AttractMode.Row? row, bool longHeader)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.Name);
            Assert.Equal("XXXXXX", row.Title);
            Assert.Equal("XXXXXX", row.Emulator);
            Assert.Equal("XXXXXX", row.CloneOf);
            Assert.Equal("XXXXXX", row.Year);
            Assert.Equal("XXXXXX", row.Manufacturer);
            Assert.Equal("XXXXXX", row.Category);
            Assert.Equal("XXXXXX", row.Players);
            Assert.Equal("XXXXXX", row.Rotation);
            Assert.Equal("XXXXXX", row.Control);
            Assert.Equal("XXXXXX", row.Status);
            Assert.Equal("XXXXXX", row.DisplayCount);
            Assert.Equal("XXXXXX", row.DisplayType);
            Assert.Equal("XXXXXX", row.AltRomname);
            Assert.Equal("XXXXXX", row.AltTitle);
            Assert.Equal("XXXXXX", row.Extra);
            Assert.Equal("XXXXXX", row.Buttons);
            if (longHeader)
            {
                Assert.Equal("XXXXXX", row.Favorite);
                Assert.Equal("XXXXXX", row.Tags);
                Assert.Equal("XXXXXX", row.PlayedCount);
                Assert.Equal("XXXXXX", row.PlayedTime);
                Assert.Equal("XXXXXX", row.FileIsAvailable);
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