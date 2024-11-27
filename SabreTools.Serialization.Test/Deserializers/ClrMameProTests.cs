using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class ClrMameProTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Serialization.Deserializers.ClrMamePro();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.ClrMamePro();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.ClrMamePro();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Serialization.Deserializers.ClrMamePro();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Serialization.Deserializers.ClrMamePro();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Serialization.Deserializers.ClrMamePro();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("test-cmp-files1.dat", 59, true)]
        [InlineData("test-cmp-files2.dat", 312, false)]
        public void ValidFile_NonNull(string path, long count, bool expectHeader)
        {
            // Open the file for reading
            string filename = Path.Combine(Environment.CurrentDirectory, "TestData", path);

            // Deserialize the file
            var dat = Serialization.Deserializers.ClrMamePro.DeserializeFile(filename, quotes: true);

            // Validate the values
            if (expectHeader)
                Assert.NotNull(dat?.ClrMamePro);
            else
                Assert.Null(dat?.ClrMamePro);

            Assert.NotNull(dat?.Game);
            Assert.Equal(count, dat.Game.Length);
        }
    }
}