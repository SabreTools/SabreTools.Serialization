using Xunit;

namespace SabreTools.Serialization.CrossModel.Test
{
    public class ListromTests
    {
        [Fact]
        public void RoundTripTest()
        {
            // Get the cross-model serializer
            var serializer = new Listrom();

            // Build the data
            Data.Models.Listrom.MetadataFile mf = Build();

            // Serialize to generic model
            Data.Models.Metadata.MetadataFile? metadata = serializer.Serialize(mf);
            Assert.NotNull(metadata);

            // Serialize back to original model
            Data.Models.Listrom.MetadataFile? newMf = serializer.Deserialize(metadata);

            // Validate the data
            Assert.NotNull(newMf);
            Assert.NotNull(newMf.Set);
            Assert.Equal(2, newMf.Set.Length);

            ValidateDevice(newMf.Set[0]);
            ValidateDriver(newMf.Set[1]);
        }

        /// <summary>
        /// Build model for serialization and deserialization
        /// </summary>
        private static Data.Models.Listrom.MetadataFile Build()
        {
            var rom = new Data.Models.Listrom.Row
            {
                Name = "name",
                Size = 12345,
                Bad = true,
                CRC = "crc32",
                SHA1 = "sha1",
                NoGoodDumpKnown = false,
            };

            var disk = new Data.Models.Listrom.Row
            {
                Name = "name",
                Bad = false,
                MD5 = "md5",
                SHA1 = "sha1",
                NoGoodDumpKnown = true,
            };

            var device = new Data.Models.Listrom.Set()
            {
                Device = "device",
                Row = [rom],
            };

            var driver = new Data.Models.Listrom.Set()
            {
                Driver = "driver",
                Row = [disk],
            };

            return new Data.Models.Listrom.MetadataFile
            {
                Set = [device, driver],
            };
        }

        /// <summary>
        /// Validate a Set
        /// </summary>
        private static void ValidateDevice(Data.Models.Listrom.Set? set)
        {
            Assert.NotNull(set);
            Assert.Equal("device", set.Device);

            Assert.NotNull(set.Row);
            var row = Assert.Single(set.Row);
            ValidateRom(row);
        }

        /// <summary>
        /// Validate a Set
        /// </summary>
        private static void ValidateDriver(Data.Models.Listrom.Set? set)
        {
            Assert.NotNull(set);
            Assert.Equal("driver", set.Driver);

            Assert.NotNull(set.Row);
            var row = Assert.Single(set.Row);
            ValidateDisk(row);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateRom(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("name", row.Name);
            Assert.Equal(12345, row.Size);
            Assert.True(row.Bad);
            Assert.Equal("crc32", row.CRC);
            Assert.Equal("sha1", row.SHA1);
            Assert.False(row.NoGoodDumpKnown);
        }

        /// <summary>
        /// Validate a Row
        /// </summary>
        private static void ValidateDisk(Data.Models.Listrom.Row? row)
        {
            Assert.NotNull(row);
            Assert.Equal("name", row.Name);
            Assert.False(row.Bad);
            Assert.Equal("md5", row.MD5);
            Assert.Equal("sha1", row.SHA1);
            Assert.True(row.NoGoodDumpKnown);
        }
    }
}
