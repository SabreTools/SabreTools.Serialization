using SabreTools.Serialization.CrossModel;
using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class RomCenterTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new RomCenter();

            // Build the data
            Data.Models.RomCenter.MetadataFile mf = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.RomCenter.MetadataFile? newMf = serializer.Deserialize(metadata);

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
            Assert.Equal("no", dat.Split); // Converted due to filtering
            Assert.Equal("no", dat.Merge); // Converted due to filtering
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
            Assert.Null(rom.ParentDescription); // This is unmappable
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
