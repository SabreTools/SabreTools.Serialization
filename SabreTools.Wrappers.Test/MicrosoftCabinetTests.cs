using System;
using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class MicrosoftCabinetTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = MicrosoftCabinet.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = MicrosoftCabinet.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = MicrosoftCabinet.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = MicrosoftCabinet.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = MicrosoftCabinet.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = MicrosoftCabinet.Create(data);
            Assert.Null(actual);
        }

        #region Extract

        [Fact]
        public void Extract_LastBlockEndsTwoFiles()
        {
            // One MS-ZIP block that holds both files
            string path = Path.Combine(Environment.CurrentDirectory, "TestData", "MicrosoftCabinet", "mszip.cab");
            var cabinet = MicrosoftCabinet.Create(File.ReadAllBytes(path), 0);
            Assert.NotNull(cabinet);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            cabinet.Extract(outputDirectory, includeDebug: true);

            Assert.Equal("Hello, ", File.ReadAllText(Path.Combine(outputDirectory, "a.txt")));
            Assert.Equal("World!", File.ReadAllText(Path.Combine(outputDirectory, "b.txt")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        #endregion
    }
}
