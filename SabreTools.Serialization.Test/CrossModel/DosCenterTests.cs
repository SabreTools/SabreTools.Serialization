using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class DosCenterTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Serialization.CrossModel.DosCenter();

            // Build the data
            Models.DosCenter.MetadataFile mf = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.DosCenter.MetadataFile? newMf = serializer.Deserialize(metadata);

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
                Date = "XXXXXX",
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
            Assert.Equal("XXXXXX", rom.Date);
        }
    }
}