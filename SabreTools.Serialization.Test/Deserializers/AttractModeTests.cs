using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class AttractModeTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Serialization.Deserializers.AttractMode();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.AttractMode();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.AttractMode();

            var actual = deserializer.Deserialize(data, offset);

            // TODO: Unexpected result
            // Turns into a single, malformatted header line
            Assert.NotNull(actual);
            Assert.NotNull(actual.Header);
            var col = Assert.Single(actual.Header);
            Assert.Equal(1024, col.Length);
            Assert.NotNull(actual.Row);
            Assert.Empty(actual.Row);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Serialization.Deserializers.AttractMode();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Serialization.Deserializers.AttractMode();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            var data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Serialization.Deserializers.AttractMode();

            var actual = deserializer.Deserialize(data);

            // TODO: Unexpected result
            // Turns into a single, malformatted header line
            Assert.NotNull(actual);
            Assert.NotNull(actual.Header);
            var col = Assert.Single(actual.Header);
            Assert.Equal(1024, col.Length);
            Assert.NotNull(actual.Row);
            Assert.Empty(actual.Row);
        }
    }
}