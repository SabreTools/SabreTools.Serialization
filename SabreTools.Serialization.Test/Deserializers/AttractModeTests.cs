using System;
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
            Assert.Null(actual);
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
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("test-attractmode-files.txt", 11)]
        public void ValidFile_NonNull(string path, long count)
        {
            // Open the file for reading
            string filename = Path.Combine(Environment.CurrentDirectory, "TestData", path);

            // Deserialize the file
            var dat = Serialization.Deserializers.AttractMode.DeserializeFile(filename);

            // Validate texpected: he values
            Assert.NotNull(dat?.Row);
            Assert.Equal(count, dat.Row.Length);

            // Validate we're not missing any attributes or elements
            foreach (var file in dat.Row)
            {
                Assert.NotNull(file);
            }
        }
    }
}