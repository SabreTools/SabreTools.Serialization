using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class SoftwareListTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new Serialization.Deserializers.SoftwareList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.SoftwareList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Serialization.Deserializers.SoftwareList();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Serialization.Deserializers.SoftwareList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Serialization.Deserializers.SoftwareList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Serialization.Deserializers.SoftwareList();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }
    
        [Theory]
        [InlineData("test-softwarelist-files1.xml", 4531)]
        [InlineData("test-softwarelist-files2.xml", 2797)]
        [InlineData("test-softwarelist-files3.xml", 274)]
        public void ValidFile_NonNull(string path, long count)
        {
            // Open the file for reading
            string filename = Path.Combine(Environment.CurrentDirectory, "TestData", path);

            // Deserialize the file
            var dat = Serialization.Deserializers.SoftwareList.DeserializeFile(filename);

            // Validate the values
            Assert.NotNull(dat);
            Assert.NotNull(dat.Software);
            Assert.Equal(count, dat.Software.Length);
        }
    }
}