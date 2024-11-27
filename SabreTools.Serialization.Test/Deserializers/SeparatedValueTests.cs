using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class SeparatedValueTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Serialization.Deserializers.SeparatedValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.SeparatedValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.SeparatedValue();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Serialization.Deserializers.SeparatedValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Serialization.Deserializers.SeparatedValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Serialization.Deserializers.SeparatedValue();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("test-csv-files1.csv", ',', 2)]
        [InlineData("test-csv-files2.csv", ',', 2)]
        [InlineData("test-ssv-files1.ssv", ';', 2)]
        [InlineData("test-ssv-files2.ssv", ';', 2)]
        [InlineData("test-tsv-files1.tsv", '\t', 2)]
        [InlineData("test-tsv-files2.tsv", '\t', 2)]
        public void ValidFile_NonNull(string path, char delim, long count)
        {
            // Open the file for reading
            string filename = Path.Combine(Environment.CurrentDirectory, "TestData", path);

            // Deserialize the file
            var dat = Serialization.Deserializers.SeparatedValue.DeserializeFile(filename, delim);

            // Validate the values
            Assert.NotNull(dat?.Row);
            Assert.Equal(count, dat.Row.Length);
        }
    }
}