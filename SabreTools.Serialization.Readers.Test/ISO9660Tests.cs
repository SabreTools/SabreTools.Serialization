using System.Collections.Generic;
using System.IO;
using System.Linq;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class ISO9660Tests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ParseDirectory_AmbiguousExtentLocation_ParsesBothExtents()
        {
            // Little-endian location points to sector 1, big-endian location to sector 2
            byte[] image = new byte[4 * 2048];
            WriteDirectory(image, 1);
            WriteDirectory(image, 2);
            var dr = new DirectoryRecord
            {
                ExtentLocation = new BothInt32(1, 2),
                ExtentLength = 2048,
                FileFlags = FileFlags.DIRECTORY,
            };

            var actual = ISO9660.ParseDirectory(new MemoryStream(image), 2048, 2048, dr, false);

            Assert.NotNull(actual);
            Assert.True(actual.ContainsKey(1));
            Assert.True(actual.ContainsKey(2));
        }

        [Fact]
        public void ParseDirectory_ConsistentExtentLocation_ReadsEachDirectoryOnce()
        {
            // Sector 1 holds directory A at sector 2, which holds directory B at sector 3
            byte[] image = new byte[5 * 2048];
            WriteDirectory(image, 1, "A", 2);
            WriteDirectory(image, 2, "B", 3);
            WriteDirectory(image, 3);
            var dr = new DirectoryRecord
            {
                ExtentLocation = 1,
                ExtentLength = 2048,
                FileFlags = FileFlags.DIRECTORY,
            };
            var stream = new SeekCountingStream(image);

            var actual = ISO9660.ParseDirectory(stream, 2048, 2048, dr, false);

            Assert.NotNull(actual);
            Assert.Equal(3, actual.Count);
            Assert.Equal(1, stream.SeeksTo(1 * 2048));
            Assert.Equal(1, stream.SeeksTo(2 * 2048));
            Assert.Equal(1, stream.SeeksTo(3 * 2048));
        }

        /// <summary>
        /// Write a one-sector directory extent with self and parent records and an optional child directory
        /// </summary>
        private static void WriteDirectory(byte[] image, int sector, string? childName = null, int childSector = 0)
        {
            int offset = sector * 2048;
            offset = WriteRecord(image, offset, Constants.CurrentDirectory, sector);
            offset = WriteRecord(image, offset, Constants.ParentDirectory, sector);
            if (childName is not null)
                WriteRecord(image, offset, System.Text.Encoding.ASCII.GetBytes(childName), childSector);
        }

        /// <summary>
        /// Write a directory record for a one-sector directory extent, returns the offset after the record
        /// </summary>
        private static int WriteRecord(byte[] image, int offset, byte[] identifier, int location)
        {
            int length = 33 + identifier.Length + (identifier.Length % 2 == 0 ? 1 : 0);
            image[offset] = (byte)length;
            WriteBothEndian(image, offset + 2, location);
            WriteBothEndian(image, offset + 10, 2048);
            image[offset + 25] = (byte)FileFlags.DIRECTORY;
            image[offset + 28] = 1;
            image[offset + 31] = 1;
            image[offset + 32] = (byte)identifier.Length;
            identifier.CopyTo(image, offset + 33);
            return offset + length;
        }

        /// <summary>
        /// Write a 32-bit value in both-endian format
        /// </summary>
        private static void WriteBothEndian(byte[] image, int offset, int value)
        {
            for (int i = 0; i < 4; i++)
            {
                image[offset + i] = (byte)(value >> (8 * i));
                image[offset + 7 - i] = (byte)(value >> (8 * i));
            }
        }

        /// <summary>
        /// Memory stream that counts absolute seeks per offset
        /// </summary>
        private sealed class SeekCountingStream : MemoryStream
        {
            private readonly Dictionary<long, int> _seeks = [];

            public SeekCountingStream(byte[] buffer) : base(buffer) { }

            public int SeeksTo(long offset)
                => _seeks.TryGetValue(offset, out int count) ? count : 0;

            public override long Seek(long offset, SeekOrigin loc)
            {
                if (loc == SeekOrigin.Begin)
                    _seeks[offset] = SeeksTo(offset) + 1;

                return base.Seek(offset, loc);
            }
        }
    }
}
