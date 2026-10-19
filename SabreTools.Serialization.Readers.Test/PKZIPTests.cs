using System.IO;
using System.IO.Compression;
using System.Linq;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class PKZIPTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ValidStream_FileDataLocated()
        {
            Stream? data = CreateArchive();
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data);
            Assert.NotNull(actual);

            var localFile = Assert.Single(actual.LocalFiles!);
            var header = localFile.LocalFileHeader;
            Assert.Equal(30 + header.FileNameLength + header.ExtraFieldLength, localFile.FileDataOffset);
            Assert.Single(actual.CentralDirectoryHeaders!);
        }

        [Fact]
        public void TruncatedFileData_Null()
        {
            byte[] archive = CreateArchive().ToArray();
            Stream? data = new MemoryStream(archive, 0, 512);
            var deserializer = new PKZIP();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        /// <summary>
        /// Create an archive with one stored file of 1000 bytes
        /// </summary>
        private static MemoryStream CreateArchive()
        {
            var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var entry = archive.CreateEntry("file.bin", CompressionLevel.NoCompression);
                using var entryStream = entry.Open();
                entryStream.Write([.. Enumerable.Range(0, 1000).Select(i => (byte)i)], 0, 1000);
            }

            stream.Position = 0;
            return stream;
        }
    }
}
