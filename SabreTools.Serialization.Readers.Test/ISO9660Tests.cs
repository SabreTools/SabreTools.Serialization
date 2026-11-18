using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class ISO9660Tests
    {
        /// <summary>
        /// File flags of a plain file record
        /// </summary>
        private const FileFlags NoFlags = 0;

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
            WriteDirectory(image, 1, ("A", 2, FileFlags.DIRECTORY));
            WriteDirectory(image, 2, ("B", 3, FileFlags.DIRECTORY));
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

        [Fact]
        public void ParseDirectory_ZeroLengthDirectoryRecord_UsesCurrentDirectoryLength()
        {
            // Sector 1 holds directory A at sector 2 with a record of length 0, the "." record of A holds the length
            byte[] image = new byte[4 * 2048];
            WriteDirectory(image, 1, ("A", 2, FileFlags.DIRECTORY));
            WriteDirectory(image, 2, ("B", 3, FileFlags.DIRECTORY));
            WriteDirectory(image, 3);
            WriteBothEndian(image, (1 * 2048) + 68 + 10, 0);
            var dr = new DirectoryRecord
            {
                ExtentLocation = 1,
                ExtentLength = 2048,
                FileFlags = FileFlags.DIRECTORY,
            };

            var actual = ISO9660.ParseDirectory(new MemoryStream(image), 2048, 2048, dr, false);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 1, 2, 3 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_ReadsExtentsInVolumeOrder()
        {
            // Root at sector 20 holds A at sector 30 and B at sector 22, A holds C at sector 21
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("A", 30, FileFlags.DIRECTORY), ("B", 22, FileFlags.DIRECTORY));
            WriteDirectory(image, 30, ("C", 21, FileFlags.DIRECTORY));
            WriteDirectory(image, 22);
            WriteDirectory(image, 21);
            var stream = new SeekCountingStream(image);

            var actual = ISO9660.ParseDirectoryDescriptors(stream, 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 21, 22, 30 }, actual.Keys.OrderBy(k => k));
            Assert.Equal(new long[] { 20 * 2048, 22 * 2048, 30 * 2048, 21 * 2048 }, stream.SeekOrder);
        }

        [Fact]
        public void ParseDirectoryDescriptors_ZeroLengthDirectoryRecord_UsesCurrentDirectoryLength()
        {
            // The record of A at sector 21 has length 0, the "." record of A holds the length
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 21, FileFlags.DIRECTORY));
            WriteDirectory(image, 21, ("B", 22, FileFlags.DIRECTORY));
            WriteDirectory(image, 22);
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 0);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 21, 22 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_ZeroLengthRecordWithoutCurrentDirectory_SkipsExtent()
        {
            // The record of A at sector 21 has length 0, the extent does not start with a "." record
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 21, FileFlags.DIRECTORY));
            WriteRecord(image, 21 * 2048, Encoding.ASCII.GetBytes("X"), 22, FileFlags.DIRECTORY);
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 0);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20 }, actual.Keys);
        }

        [Fact]
        public void ParseDirectoryDescriptors_ZeroLengthRecordPastEnd_SkipsExtent()
        {
            // The record of A has length 0 and points past the end of the image, B at sector 22 is still read
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 30, FileFlags.DIRECTORY), ("B", 22, FileFlags.DIRECTORY));
            WriteDirectory(image, 22);
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 0);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 22 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_ZeroLengthRecordCurrentDirectoryPastEnd_SkipsExtent()
        {
            // The record of A at sector 23 has length 0, the "." record of A ends past the end of the image
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 23, FileFlags.DIRECTORY), ("B", 22, FileFlags.DIRECTORY));
            WriteDirectory(image, 22);
            WriteDirectory(image, 23);
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 0);
            WriteBothEndian(image, (23 * 2048) + 10, 2 * 2048);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 22 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_LengthNotWholeBlocks_UsesCurrentDirectoryLength()
        {
            // The root record has length 1, the "." record of the root holds the length
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 21, FileFlags.DIRECTORY));
            WriteDirectory(image, 21);
            var pvd = CreatePrimaryVolumeDescriptor(20);
            pvd.RootDirectoryRecord.ExtentLength = 1;

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [pvd]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 21 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_LengthNotWholeBlocksCurrentDirectoryShorter_KeepsLength()
        {
            // The record of A has length 3000 and the "." record of A 2048, B is recorded in the second sector of A
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 21, FileFlags.DIRECTORY));
            WriteDirectory(image, 21);
            WriteRecord(image, 22 * 2048, Encoding.ASCII.GetBytes("B"), 23, FileFlags.DIRECTORY);
            WriteDirectory(image, 23);
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 3000);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 21, 23 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_LengthNotWholeBlocksCurrentDirectoryPastEnd_KeepsLength()
        {
            // The record of A at sector 23 has length 102 (its three records), the "." record of A ends past the end of the image
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("A", 23, FileFlags.DIRECTORY));
            WriteDirectory(image, 22);
            WriteDirectory(image, 23, ("B", 22, FileFlags.DIRECTORY));
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 102);
            WriteBothEndian(image, (23 * 2048) + 10, 2 * 2048);

            var actual = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 22, 23 }, actual.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParseDirectoryDescriptors_FileWithExtendedAttributeRecord_ReadsExtentOnce()
        {
            // F at sector 22 has an extended attribute record and a length of 100 bytes, its extent is read once for the record
            byte[] image = new byte[24 * 2048];
            WriteDirectory(image, 20, ("F;1", 22, NoFlags));
            image[(20 * 2048) + 68 + 1] = 1;
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 100);
            var stream = new SeekCountingStream(image);

            var actual = ISO9660.ParseDirectoryDescriptors(stream, 2048, [CreatePrimaryVolumeDescriptor(20)]);

            Assert.NotNull(actual);
            Assert.Equal(1, stream.SeeksTo(22 * 2048));
        }

        [Fact]
        public void FindDirectoryRecords_NestedPath_ReadsOnlyDirectoriesOnPath()
        {
            // Root at sector 20 holds OTHER at sector 22 and SUB at sector 30, SUB holds FILE.TXT;1 at sector 25
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("OTHER", 22, FileFlags.DIRECTORY), ("SUB", 30, FileFlags.DIRECTORY));
            WriteDirectory(image, 22);
            WriteDirectory(image, 30, ("FILE.TXT;1", 25, NoFlags));
            var stream = new SeekCountingStream(image);

            var actual = ISO9660.FindDirectoryRecords(stream, 2048, CreatePrimaryVolumeDescriptor(20), "sub/file.txt");

            Assert.NotNull(actual);
            Assert.Equal(25, Assert.Single(actual).ExtentLocation.LittleEndian);
            Assert.Equal(new long[] { 20 * 2048, 30 * 2048 }, stream.SeekOrder);
        }

        [Fact]
        public void FindDirectoryRecords_ZeroLengthDirectoryRecord_UsesCurrentDirectoryLength()
        {
            // The record of SUB at sector 30 has length 0, the "." record of SUB holds the length
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("SUB", 30, FileFlags.DIRECTORY));
            WriteDirectory(image, 30, ("FILE.TXT;1", 25, NoFlags));
            WriteBothEndian(image, (20 * 2048) + 68 + 10, 0);

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, CreatePrimaryVolumeDescriptor(20), "SUB/FILE.TXT");

            Assert.NotNull(actual);
            Assert.Equal(25, Assert.Single(actual).ExtentLocation.LittleEndian);
        }

        [Fact]
        public void FindDirectoryRecords_MissingPath_Null()
        {
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("SUB", 30, FileFlags.DIRECTORY));
            WriteDirectory(image, 30);

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, CreatePrimaryVolumeDescriptor(20), "SUB/MISSING.TXT");

            Assert.Null(actual);
        }

        [Fact]
        public void FindDirectoryRecords_MultiExtentFile_ReturnsAllRecords()
        {
            // BIG.BIN;1 is recorded in two extents, at sectors 25 and 26
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("BIG.BIN;1", 25, FileFlags.MULTI_EXTENT), ("BIG.BIN;1", 26, NoFlags), ("NEXT.BIN;1", 27, NoFlags));

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, CreatePrimaryVolumeDescriptor(20), "BIG.BIN");

            Assert.NotNull(actual);
            Assert.Equal(new[] { 25, 26 }, actual.Select(r => r.ExtentLocation.LittleEndian));
        }

        [Fact]
        public void FindDirectoryRecords_SupplementaryVolumeDescriptor_UsesUnicodeNames()
        {
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, Encoding.BigEndianUnicode, ("Sub", 30, FileFlags.DIRECTORY));
            WriteDirectory(image, 30, Encoding.BigEndianUnicode, ("File.txt;1", 25, NoFlags));
            var svd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = 2048,
                RootDirectoryRecord = CreatePrimaryVolumeDescriptor(20).RootDirectoryRecord,
            };

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, svd, "Sub/File.txt");

            Assert.NotNull(actual);
            Assert.Equal(25, Assert.Single(actual).ExtentLocation.LittleEndian);
        }

        /// <summary>
        /// Create a primary volume descriptor with 2048-byte blocks and a one-sector root directory
        /// </summary>
        private static PrimaryVolumeDescriptor CreatePrimaryVolumeDescriptor(int rootSector)
        {
            return new PrimaryVolumeDescriptor
            {
                LogicalBlockSize = 2048,
                RootDirectoryRecord = new DirectoryRecord
                {
                    ExtentLocation = rootSector,
                    ExtentLength = 2048,
                    FileFlags = FileFlags.DIRECTORY,
                },
            };
        }

        /// <summary>
        /// Write a one-sector directory extent with self and parent records and the given entries
        /// </summary>
        private static void WriteDirectory(byte[] image, int sector, params (string Name, int Location, FileFlags Flags)[] entries)
            => WriteDirectory(image, sector, Encoding.ASCII, entries);

        /// <summary>
        /// Write a one-sector directory extent with self and parent records and the given entries
        /// </summary>
        private static void WriteDirectory(byte[] image, int sector, Encoding encoding, params (string Name, int Location, FileFlags Flags)[] entries)
        {
            int offset = sector * 2048;
            offset = WriteRecord(image, offset, Constants.CurrentDirectory, sector, FileFlags.DIRECTORY);
            offset = WriteRecord(image, offset, Constants.ParentDirectory, sector, FileFlags.DIRECTORY);
            foreach (var (name, location, flags) in entries)
            {
                offset = WriteRecord(image, offset, encoding.GetBytes(name), location, flags);
            }
        }

        /// <summary>
        /// Write a directory record for a one-sector extent, returns the offset after the record
        /// </summary>
        private static int WriteRecord(byte[] image, int offset, byte[] identifier, int location, FileFlags flags)
        {
            int length = 33 + identifier.Length + (identifier.Length % 2 == 0 ? 1 : 0);
            image[offset] = (byte)length;
            WriteBothEndian(image, offset + 2, location);
            WriteBothEndian(image, offset + 10, 2048);
            image[offset + 25] = (byte)flags;
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
        /// Memory stream that records absolute seeks
        /// </summary>
        private sealed class SeekCountingStream : MemoryStream
        {
            private readonly Dictionary<long, int> _seeks = [];

            /// <summary>
            /// Offsets of all absolute seeks, in order
            /// </summary>
            public List<long> SeekOrder { get; } = [];

            public SeekCountingStream(byte[] buffer) : base(buffer) { }

            public int SeeksTo(long offset)
                => _seeks.TryGetValue(offset, out int count) ? count : 0;

            public override long Seek(long offset, SeekOrigin loc)
            {
                if (loc == SeekOrigin.Begin)
                {
                    _seeks[offset] = SeeksTo(offset) + 1;
                    SeekOrder.Add(offset);
                }

                return base.Seek(offset, loc);
            }
        }
    }
}
