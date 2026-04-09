using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class DosCenterTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new DosCenter();

            // Build the data
            Data.Models.DosCenter.MetadataFile mf = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.DosCenter.MetadataFile? newMf = serializer.Deserialize(metadata);

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
        private static Data.Models.DosCenter.MetadataFile Build()
        {
            var dc = new Data.Models.DosCenter.DosCenter
            {
                Name = "name",
                Description = "description",
                Version = "version",
                Date = "date",
                Author = "author",
                Homepage = "homepage",
                Comment = "comment",
            };

            var file = new Data.Models.DosCenter.File
            {
                Name = "name",
                Size = 12345,
                CRC = "crc32",
                SHA1 = "sha1",
                Date = "date",
            };

            var game = new Data.Models.DosCenter.Game
            {
                Name = "name",
                File = [file],
            };

            return new Data.Models.DosCenter.MetadataFile
            {
                DosCenter = dc,
                Game = [game],
            };
        }

        /// <summary>
        /// Validate a DosCenter
        /// </summary>
        private static void Validate(Data.Models.DosCenter.DosCenter? cmp)
        {
            Assert.NotNull(cmp);
            Assert.Equal("name", cmp.Name);
            Assert.Equal("description", cmp.Description);
            Assert.Equal("version", cmp.Version);
            Assert.Equal("date", cmp.Date);
            Assert.Equal("author", cmp.Author);
            Assert.Equal("homepage", cmp.Homepage);
            Assert.Equal("comment", cmp.Comment);
        }

        /// <summary>
        /// Validate a Game
        /// </summary>
        private static void Validate(Data.Models.DosCenter.Game? game)
        {
            Assert.NotNull(game);
            Assert.Equal("name", game.Name);

            Assert.NotNull(game.File);
            var file = Assert.Single(game.File);
            Validate(file);
        }

        /// <summary>
        /// Validate a File
        /// </summary>
        private static void Validate(Data.Models.DosCenter.File? rom)
        {
            Assert.NotNull(rom);
            Assert.Equal("name", rom.Name);
            Assert.Equal(12345, rom.Size);
            Assert.Equal("crc32", rom.CRC);
            Assert.Equal("sha1", rom.SHA1);
            Assert.Equal("date", rom.Date);
        }
    }
}
