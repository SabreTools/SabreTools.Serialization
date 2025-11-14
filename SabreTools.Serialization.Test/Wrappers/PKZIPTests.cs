using System;
using System.IO;
using System.Linq;
using SabreTools.Serialization.Wrappers;
using Xunit;

namespace SabreTools.Serialization.Test.Wrappers
{
    public class PKZIPTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = PKZIP.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = PKZIP.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = PKZIP.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = PKZIP.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = PKZIP.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = PKZIP.Create(data);
            Assert.Null(actual);
        }

        #region FindParts

        [Theory]
        [InlineData("single.zip", 1)]
        [InlineData("single.zipx", 1)]
        [InlineData("multi.zip", 4)]
        [InlineData("multix.zipx", 4)]
        [InlineData("multi-split.zip.001", 3)]
        public void FindPartsTest(string filename, int expectedParts)
        {
            string firstPart = Path.Combine(Environment.CurrentDirectory, "TestData", "PKZIP", filename);
            var actual = PKZIP.FindParts(firstPart);
            Assert.Equal(expectedParts, actual.Count);
        }

        #endregion
    }
}
