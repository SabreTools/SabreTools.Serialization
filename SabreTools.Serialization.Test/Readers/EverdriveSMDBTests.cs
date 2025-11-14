using System.IO;
using System.Linq;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
{
    public class EverdriveSMDBTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new EverdriveSMDB();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new EverdriveSMDB();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new EverdriveSMDB();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new EverdriveSMDB();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new EverdriveSMDB();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new EverdriveSMDB();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void RoundTripTest()
        {
            // Get the serializer and deserializer
            var deserializer = new EverdriveSMDB();
            var serializer = new SabreTools.Serialization.Writers.EverdriveSMDB();

            // Build the data
            Data.Models.EverdriveSMDB.MetadataFile mf = Build();

            // Serialize to stream
            Stream? actual = serializer.SerializeStream(mf);
            Assert.NotNull(actual);

            // Serialize back to original model
            Data.Models.EverdriveSMDB.MetadataFile? newMf = deserializer.Deserialize(actual);

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
                SHA256 = "XXXXXX",
                Name = "XXXXXX",
                SHA1 = "XXXXXX",
                MD5 = "XXXXXX",
                CRC32 = "XXXXXX",
                Size = "XXXXXX",
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
            Assert.Equal("XXXXXX", row.SHA256);
            Assert.Equal("XXXXXX", row.Name);
            Assert.Equal("XXXXXX", row.SHA1);
            Assert.Equal("XXXXXX", row.MD5);
            Assert.Equal("XXXXXX", row.CRC32);
            Assert.Equal("XXXXXX", row.Size);
        }
    }
}
