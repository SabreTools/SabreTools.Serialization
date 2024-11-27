using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class HashfileTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Serialization.Deserializers.Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.Hashfile();

            var actual = deserializer.Deserialize(data, offset);

            // TODO: Unexpected result
            // Turns into an empty model
            Assert.NotNull(actual);
            Assert.Null(actual.SFV);
            Assert.Null(actual.MD2);
            Assert.Null(actual.MD4);
            Assert.Null(actual.MD5);
            Assert.Null(actual.SHA1);
            Assert.Null(actual.SHA256);
            Assert.Null(actual.SHA384);
            Assert.Null(actual.SHA512);
            Assert.Null(actual.SpamSum);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Serialization.Deserializers.Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Serialization.Deserializers.Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Serialization.Deserializers.Hashfile();

            var actual = deserializer.Deserialize(data);

            // TODO: Unexpected result
            // Turns into an empty model
            Assert.NotNull(actual);
            Assert.Null(actual.SFV);
            Assert.Null(actual.MD2);
            Assert.Null(actual.MD4);
            Assert.Null(actual.MD5);
            Assert.Null(actual.SHA1);
            Assert.Null(actual.SHA256);
            Assert.Null(actual.SHA384);
            Assert.Null(actual.SHA512);
            Assert.Null(actual.SpamSum);
        }
    }
}