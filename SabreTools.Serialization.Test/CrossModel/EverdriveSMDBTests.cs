using SabreTools.Serialization.CrossModel;
using Xunit;

namespace SabreTools.Serialization.Test.CrossModel
{
    public class EverdriveSMDBTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new EverdriveSMDB();

            // Build the data
            Models.EverdriveSMDB.MetadataFile mf = Build();

            // Serialize to generic model
            Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Models.EverdriveSMDB.MetadataFile? newMf = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMf);
            Assert.NotNull(newMf.Row);
            var newRow = Assert.Single(newMf.Row);
            Validate(newRow);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Models.EverdriveSMDB.MetadataFile Build()
        {
            var row = new Models.EverdriveSMDB.Row
            {
                SHA256 = "XXXXXX",
                Name = "XXXXXX",
                SHA1 = "XXXXXX",
                MD5 = "XXXXXX",
                CRC32 = "XXXXXX",
                Size = "XXXXXX",
            };

            return new Models.EverdriveSMDB.MetadataFile
            {
                Row = [row],
            };
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void Validate(Models.EverdriveSMDB.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("XXXXXX", row.SHA256);
            Assert.Equal("XXXXXX", row.Name);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.CRC32);
            Assert.Equal("XXXXXX", row.Size);
        }
    }
}