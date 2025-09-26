using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
{
    public class CueSheetTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new CueSheet();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        #region ReadQuotedString

        [Fact]
        public void ReadQuotedString_EmptyReader_Throws()
        {
            byte[] data = Encoding.UTF8.GetBytes(string.Empty);
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
            Assert.Throws<ArgumentNullException>(() => CueSheet.ReadQuotedString(reader));
        }

        [Fact]
        public void ReadQuotedString_NoQuotes_Correct()
        {
            byte[] data = Encoding.UTF8.GetBytes("Test1 Test2");
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
            string? actual = CueSheet.ReadQuotedString(reader);
            Assert.Equal("Test1 Test2", actual);
        }

        [Fact]
        public void ReadQuotedString_SingleLineQuotes_Correct()
        {
            byte[] data = Encoding.UTF8.GetBytes("\"Test1 Test2\"");
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
            string? actual = CueSheet.ReadQuotedString(reader);
            Assert.Equal("\"Test1 Test2\"", actual);
        }

        [Fact]
        public void ReadQuotedString_MultiLineQuotes_Correct()
        {
            byte[] data = Encoding.UTF8.GetBytes("\"Test1\nTest2\"");
            var stream = new MemoryStream(data);
            var reader = new StreamReader(stream, Encoding.UTF8);
            string? actual = CueSheet.ReadQuotedString(reader);
            Assert.Equal("\"Test1\nTest2\"", actual);
        }

        #endregion
    }
}