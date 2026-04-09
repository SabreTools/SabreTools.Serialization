using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class EverdriveSMDBTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new EverdriveSMDB();

            // Build the data
            Data.Models.EverdriveSMDB.MetadataFile mf = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.EverdriveSMDB.MetadataFile? newMf = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMf);
            Assert.NotNull(newMf.Row);
            var newRow = Assert.Single(newMf.Row);
            Validate(newRow);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.EverdriveSMDB.MetadataFile Build()
        {
            var row = new Data.Models.EverdriveSMDB.Row
            {
                SHA256 = "sha256",
                Name = "name",
                SHA1 = "sha1",
                MD5 = "md5",
                CRC32 = "crc32",
                Size = 12345,
            };

            return new Data.Models.EverdriveSMDB.MetadataFile
            {
                Row = [row],
            };
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void Validate(Data.Models.EverdriveSMDB.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("sha256", row.SHA256);
            Assert.Equal("name", row.Name);
            Assert.Equal("sha1", row.SHA1);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("crc32", row.CRC32);
            Assert.Equal(12345, row.Size);
        }
    }
}
