using System;
using System.IO;
using System.Linq;
using SabreTools.Hashing;
using SabreTools.Serialization.Deserializers;
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
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new Hashfile();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Theory]
        [InlineData("test-sfv-files.sfv", HashType.CRC32, 100)]
        [InlineData("test-md5-files.md5", HashType.MD5, 100)]
        [InlineData("test-sha1-files.sha1", HashType.SHA1, 100)]
        [InlineData("test-sha256.sha256", HashType.SHA256, 1)]
        [InlineData("test-sha384.sha384", HashType.SHA384, 1)]
        [InlineData("test-sha512.sha512", HashType.SHA512, 1)]
        [InlineData("test-spamsum.spamsum", HashType.SpamSum, 1)]
        public void ValidFile_NonNull(string path, HashType hash, long count)
        {
            // Open the file for reading
            string filename = Path.Combine(Environment.CurrentDirectory, "TestData", path);

            // Deserialize the file
            var dat = Hashfile.DeserializeFile(filename, hash);

            // Validate the values
            Assert.NotNull(dat);

            switch (hash)
            {
                case HashType.CRC32:
                    Assert.NotNull(dat.SFV);
                    Assert.Equal(count, dat.SFV.Length);
                    break;
                case HashType.MD5:
                    Assert.NotNull(dat.MD5);
                    Assert.Equal(count, dat.MD5.Length);
                    break;
                case HashType.SHA1:
                    Assert.NotNull(dat.SHA1);
                    Assert.Equal(count, dat.SHA1.Length);
                    break;
                case HashType.SHA256:
                    Assert.NotNull(dat.SHA256);
                    Assert.Equal(count, dat.SHA256.Length);
                    break;
                case HashType.SHA384:
                    Assert.NotNull(dat.SHA384);
                    Assert.Equal(count, dat.SHA384.Length);
                    break;
                case HashType.SHA512:
                    Assert.NotNull(dat.SHA512);
                    Assert.Equal(count, dat.SHA512.Length);
                    break;
                case HashType.SpamSum:
                    Assert.NotNull(dat.SpamSum);
                    Assert.Equal(count, dat.SpamSum.Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hash));
            }
        }
    }
}