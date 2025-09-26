using SabreTools.Serialization.CrossModel;
using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class AttractModeTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new AttractMode();

            // Build the data
            Data.Models.AttractMode.MetadataFile mf = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.AttractMode.MetadataFile? newMf = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMf);
            Validate(newMf.Header);
            Assert.NotNull(newMf.Row);
            var newRow = Assert.Single(newMf.Row);
            Validate(newRow);
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
        private static void Validate(string[]? header)
        {
            Assert.NotNull(header);
            string column = Assert.Single(header);
            Assert.Equal("header", column);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void Validate(Data.Models.AttractMode.Row? row)
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
            Assert.Equal("XXXXXX", row.Favorite);
            Assert.Equal("XXXXXX", row.Tags);
            Assert.Equal("XXXXXX", row.PlayedCount);
            Assert.Equal("XXXXXX", row.PlayedTime);
            Assert.Equal("XXXXXX", row.FileIsAvailable);
        }
    }
}