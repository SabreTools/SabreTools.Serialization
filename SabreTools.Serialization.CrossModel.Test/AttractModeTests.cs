using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
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
            Assert.Equal("favorite", row.Favorite);
            Assert.Equal("tags", row.Tags);
            Assert.Equal("playedcount", row.PlayedCount);
            Assert.Equal("playedtime", row.PlayedTime);
            Assert.Equal(true, row.FileIsAvailable);
        }
    }
}
