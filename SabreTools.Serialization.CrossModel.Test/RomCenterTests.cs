using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
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
                Author = "author",
                Version = "version",
                Email = "email",
                Homepage = "homepage",
                Url = "url",
                Date = "date",
                Comment = "comment",
            };

            var dat = new Data.Models.RomCenter.Dat
            {
                Version = "version",
                Plugin = "plugin",
                Split = "split",
                Merge = "merge",
            };

            var emulator = new Data.Models.RomCenter.Emulator
            {
                RefName = "refname",
                Version = "version",
            };

            var rom = new Data.Models.RomCenter.Rom
            {
                ParentName = "parentname",
                ParentDescription = "parentdescription",
                GameName = "gamename",
                GameDescription = "description",
                RomName = "romname",
                RomCRC = "romcrc",
                RomSize = 12345,
                RomOf = "romof",
                MergeName = "mergename",
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
            Assert.Equal("author", credits.Author);
            Assert.Equal("version", credits.Version);
            Assert.Equal("email", credits.Email);
            Assert.Equal("homepage", credits.Homepage);
            Assert.Equal("url", credits.Url);
            Assert.Equal("date", credits.Date);
            Assert.Equal("comment", credits.Comment);
        }

        /// <summary>
        /// Validate a Dat
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Dat? dat)
        {
            Assert.NotNull(dat);
            Assert.Equal("version", dat.Version);
            Assert.Equal("plugin", dat.Plugin);
            Assert.Equal("no", dat.Split); // Converted due to filtering
            Assert.Equal("no", dat.Merge); // Converted due to filtering
        }

        /// <summary>
        /// Validate a Emulator
        /// </summary>
        private static void Validate(Data.Models.RomCenter.Emulator? emulator)
        {
            Assert.NotNull(emulator);
            Assert.Equal("refname", emulator.RefName);
            Assert.Equal("version", emulator.Version);
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
            Assert.Equal("parentname", rom.ParentName);
            Assert.Null(rom.ParentDescription); // This is unmappable
            Assert.Equal("gamename", rom.GameName);
            Assert.Equal("description", rom.GameDescription);
            Assert.Equal("romname", rom.RomName);
            Assert.Equal("romcrc", rom.RomCRC);
            Assert.Equal(12345, rom.RomSize);
            Assert.Equal("romof", rom.RomOf);
            Assert.Equal("mergename", rom.MergeName);
        }
    }
}
