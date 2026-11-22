using System;
using System.Buffers.Binary;
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
        public void FindDirectoryRecords_RecordWithoutPaddingField_FindsNextFile()
        {
            // README.TXT;1 has an even identifier length, but its record ends after the identifier
            byte[] image = new byte[32 * 2048];
            int offset = WriteDirectory(image, 20);
            offset = WriteRecord(image, offset, Encoding.ASCII.GetBytes("README.TXT;1"), 25, NoFlags, 33 + 12);
            WriteRecord(image, offset, Encoding.ASCII.GetBytes("NEXT.TXT;1"), 26, NoFlags);

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, CreatePrimaryVolumeDescriptor(20), "NEXT.TXT");

            Assert.NotNull(actual);
            Assert.Equal(26, Assert.Single(actual).ExtentLocation.LittleEndian);
        }

        [Fact]
        public void FindDirectoryRecords_OddRecordLength_FindsNextFile()
        {
            // FIRST.TXT;1 has a one-byte system use field, which makes the record length odd
            byte[] image = new byte[32 * 2048];
            int offset = WriteDirectory(image, 20);
            offset = WriteRecord(image, offset, Encoding.ASCII.GetBytes("FIRST.TXT;1"), 25, NoFlags, 33 + 11 + 1);
            WriteRecord(image, offset, Encoding.ASCII.GetBytes("NEXT.TXT;1"), 26, NoFlags);

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, CreatePrimaryVolumeDescriptor(20), "NEXT.TXT");

            Assert.NotNull(actual);
            Assert.Equal(26, Assert.Single(actual).ExtentLocation.LittleEndian);
        }

        [Fact]
        public void ParseDirectory_TooShortRecord_KeepsRecordsBeforeIt()
        {
            // Sector 1 holds FIRST.TXT;1, DAMAGED.TXT;1 with mismatched extent locations and NEXT.TXT;1, which is too short for its identifier
            byte[] image = new byte[4 * 2048];
            int damaged = WriteDirectory(image, 1, ("FIRST.TXT;1", 2, NoFlags));
            int offset = WriteRecord(image, damaged, Encoding.ASCII.GetBytes("DAMAGED.TXT;1"), 3, NoFlags);
            image[damaged + 6] = 0xFF;
            WriteRecord(image, offset, Encoding.ASCII.GetBytes("NEXT.TXT;1"), 3, NoFlags, 40);
            var dr = new DirectoryRecord
            {
                ExtentLocation = 1,
                ExtentLength = 2048,
                FileFlags = FileFlags.DIRECTORY,
            };

            var actual = ISO9660.ParseDirectory(new MemoryStream(image), 2048, 2048, dr, false);

            Assert.NotNull(actual);
            var directory = Assert.IsType<DirectoryExtent>(actual[1]);
            Assert.Equal(3, directory.DirectoryRecords.Length);
            Assert.Equal("FIRST.TXT;1", Encoding.ASCII.GetString(directory.DirectoryRecords[2].FileIdentifier));
        }

        [Fact]
        public void ParseDirectory_TooShortFirstRecord_Null()
        {
            byte[] image = new byte[4 * 2048];
            WriteRecord(image, 1 * 2048, Constants.CurrentDirectory, 1, FileFlags.DIRECTORY, 20);
            var dr = new DirectoryRecord
            {
                ExtentLocation = 1,
                ExtentLength = 2048,
                FileFlags = FileFlags.DIRECTORY,
            };

            var actual = ISO9660.ParseDirectory(new MemoryStream(image), 2048, 2048, dr, false);

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
        public void Deserialize_DirectoryOnlyInPathTable_ParsesIt()
        {
            // The root directory at sector 20 does not hold HIDDEN at sector 30, the path table at sector 18 does
            byte[] image = new byte[32 * 2048];
            WriteVolumeDescriptorSet(image, 20, 18, 24);
            int offset = WritePathTableRecord(image, 18 * 2048, [0], 20, 1);
            WritePathTableRecord(image, offset, Encoding.ASCII.GetBytes("HIDDEN"), 30, 1);
            WriteDirectory(image, 20);
            WriteDirectory(image, 30);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(new MemoryStream(image));

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 30 }, actual.DirectoryDescriptors.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParsePathTableDirectories_DirectoryOnlyInPathTable_ParsesItsHierarchy()
        {
            // The root directory at sector 20 holds A at sector 21 but not HIDDEN at sector 30,
            // which holds SUB at sector 31 and A
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("A", 21, FileFlags.DIRECTORY));
            WriteDirectory(image, 21);
            WriteDirectory(image, 30, ("SUB", 31, FileFlags.DIRECTORY), ("A", 21, FileFlags.DIRECTORY));
            WriteDirectory(image, 31);
            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20) };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;

            ISO9660.ParsePathTableDirectories(new MemoryStream(image), 2048, vdSet, [CreatePathTableGroup(20, 21, 30, 31)], directories);

            Assert.Equal(new[] { 20, 21, 30, 31 }, directories.Keys.OrderBy(k => k));
            Assert.Equal(4, Assert.IsType<DirectoryExtent>(directories[30]).DirectoryRecords.Length);
        }

        [Fact]
        public void ParsePathTableDirectories_ReachedDirectories_ReadsNothing()
        {
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("A", 30, FileFlags.DIRECTORY));
            WriteDirectory(image, 30);
            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20) };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;
            var stream = new SeekCountingStream(image);

            ISO9660.ParsePathTableDirectories(stream, 2048, vdSet, [CreatePathTableGroup(20, 30)], directories);

            Assert.Empty(stream.SeekOrder);
            Assert.Equal(2, directories.Count);
        }

        [Fact]
        public void ParsePathTableDirectories_OtherDirectoryHierarchy_Ignored()
        {
            // The path table starts with sector 25 instead of the root directory
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20);
            WriteDirectory(image, 30);
            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20) };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;

            ISO9660.ParsePathTableDirectories(new MemoryStream(image), 2048, vdSet, [CreatePathTableGroup(25, 30)], directories);

            Assert.Equal(new[] { 20 }, directories.Keys);
        }

        [Fact]
        public void ParsePathTableDirectories_SupplementaryVolumeDescriptor_ParsesItsHierarchy()
        {
            // The path table starts with the root directory of the supplementary volume descriptor at sector 24
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20);
            WriteDirectory(image, 24);
            WriteDirectory(image, 30);
            var svd = new SupplementaryVolumeDescriptor
            {
                LogicalBlockSize = 2048,
                RootDirectoryRecord = CreatePrimaryVolumeDescriptor(24).RootDirectoryRecord,
            };

            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20), svd };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;

            ISO9660.ParsePathTableDirectories(new MemoryStream(image), 2048, vdSet, [CreatePathTableGroup(24, 30)], directories);

            Assert.Equal(new[] { 20, 24, 30 }, directories.Keys.OrderBy(k => k));
        }

        [Fact]
        public void ParsePathTableDirectories_NotStartOfDirectory_Ignored()
        {
            // Sector 30 starts with a directory record for sector 31, which holds a directory
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20);
            WriteRecord(image, 30 * 2048, Constants.CurrentDirectory, 31, FileFlags.DIRECTORY);
            WriteDirectory(image, 31);
            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20) };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;

            ISO9660.ParsePathTableDirectories(new MemoryStream(image), 2048, vdSet, [CreatePathTableGroup(20, 30)], directories);

            Assert.Equal(new[] { 20 }, directories.Keys);
        }

        [Fact]
        public void ParsePathTableDirectories_NotDirectory_Ignored()
        {
            // Sector 30 starts with a directory record for itself without the directory flag
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20);
            WriteRecord(image, 30 * 2048, Constants.CurrentDirectory, 30, NoFlags);
            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20) };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;

            ISO9660.ParsePathTableDirectories(new MemoryStream(image), 2048, vdSet, [CreatePathTableGroup(20, 30)], directories);

            Assert.Equal(new[] { 20 }, directories.Keys);
        }

        [Fact]
        public void ParsePathTableDirectories_OutsideImage_Ignored()
        {
            // The image ends 20 bytes into sector 32, and the stream is at its end
            // as after reading a directory in its last sector
            byte[] image = new byte[(32 * 2048) + 20];
            WriteDirectory(image, 20);
            var vdSet = new VolumeDescriptor[] { CreatePrimaryVolumeDescriptor(20) };
            var directories = ISO9660.ParseDirectoryDescriptors(new MemoryStream(image), 2048, vdSet)!;
            var stream = new MemoryStream(image);
            stream.Seek(0, SeekOrigin.End);

            ISO9660.ParsePathTableDirectories(stream, 2048, vdSet, [CreatePathTableGroup(20, 32, 40, -1)], directories);

            Assert.Equal(new[] { 20 }, directories.Keys);
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
        public void FindDirectoryRecords_UnusedLastDirectorySector_FindsFile()
        {
            // Root at sector 20 is two sectors long, FILE.TXT;1 is in the first sector and the second sector is unused
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20, ("FILE.TXT;1", 25, NoFlags));
            WriteBothEndian(image, (20 * 2048) + 10, 2 * 2048);
            var pvd = CreatePrimaryVolumeDescriptor(20);
            pvd.RootDirectoryRecord.ExtentLength = 2 * 2048;

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, pvd, "FILE.TXT");

            Assert.NotNull(actual);
            Assert.Equal(25, Assert.Single(actual).ExtentLocation.LittleEndian);
        }

        [Fact]
        public void FindDirectoryRecords_UnusedDirectorySectorBeforeRecords_FindsFile()
        {
            // Root at sector 20 is three sectors long, the second sector is unused and FILE.TXT;1 is in the third sector
            byte[] image = new byte[32 * 2048];
            WriteDirectory(image, 20);
            WriteBothEndian(image, (20 * 2048) + 10, 3 * 2048);
            WriteRecord(image, 22 * 2048, Encoding.ASCII.GetBytes("FILE.TXT;1"), 25, NoFlags);
            var pvd = CreatePrimaryVolumeDescriptor(20);
            pvd.RootDirectoryRecord.ExtentLength = 3 * 2048;

            var actual = ISO9660.FindDirectoryRecords(new MemoryStream(image), 2048, pvd, "FILE.TXT");

            Assert.NotNull(actual);
            Assert.Equal(25, Assert.Single(actual).ExtentLocation.LittleEndian);
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

        [Fact]
        public void HighSierra_ReadsVolumeDescriptors()
        {
            byte[] image = CreateHighSierraImage(false);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(new MemoryStream(image));

            Assert.NotNull(actual);
            Assert.Equal(2, actual.VolumeDescriptorSet.Length);
            var pvd = Assert.IsType<HighSierraPrimaryVolumeDescriptor>(actual.VolumeDescriptorSet[0]);
            Assert.Equal(16, pvd.LogicalBlockNumber.LittleEndian);
            Assert.Equal(16, pvd.LogicalBlockNumber.BigEndian);
            Assert.Equal(VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR, pvd.Type);
            Assert.Equal(Constants.HighSierraIdentifier, pvd.Identifier);
            Assert.Equal(1, pvd.Version);
            Assert.Equal("HSFTEST", Encoding.ASCII.GetString(pvd.VolumeIdentifier, 0, 7));
            Assert.Equal(2048, pvd.LogicalBlockSize.LittleEndian);
            Assert.Equal(18, pvd.PathTableLocationL);
            Assert.Equal(19, pvd.PathTableLocationM);
            Assert.Equal(FileFlags.DIRECTORY, pvd.RootDirectoryRecord.FileFlags);
            Assert.Equal("ABSTRACT.TXT", Encoding.ASCII.GetString(pvd.AbstractFileIdentifier, 0, 12));
            Assert.Equal(1, pvd.FileStructureVersion);
            Assert.Equal("APP", Encoding.ASCII.GetString(pvd.ApplicationUse, 0, 3));
            var terminator = Assert.IsType<HighSierraVolumeDescriptorSetTerminator>(actual.VolumeDescriptorSet[1]);
            Assert.Equal(17, terminator.LogicalBlockNumber.LittleEndian);
            Assert.Equal(VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR, terminator.Type);
            Assert.Equal(Constants.HighSierraIdentifier, terminator.Identifier);
        }

        [Fact]
        public void HighSierra_KeepsAllVolumeDescriptorBytes()
        {
            byte[] image = CreateHighSierraImage(false);
            int vd = 16 * 2048;
            image[vd + 15] = 0x01;
            image[vd + 80] = 0x02;
            image[vd + 96] = 0x03;
            WriteInteger(image, vd + 156, 4, 4, false);
            WriteInteger(image, vd + 160, 5, 4, false);
            WriteInteger(image, vd + 172, 6, 4, true);
            WriteInteger(image, vd + 176, 7, 4, true);
            image[vd + 855] = 0x08;
            image[vd + 1368] = 0x09;
            image[vd + 2047] = 0x0A;
            image[(17 * 2048) + 15] = 0x0B;
            image[(17 * 2048) + 2047] = 0x0C;
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(new MemoryStream(image));

            Assert.NotNull(actual);
            var pvd = Assert.IsType<HighSierraPrimaryVolumeDescriptor>(actual.VolumeDescriptorSet[0]);
            Assert.Equal(0x01, pvd.UnusedByte);
            Assert.Equal(0x02, pvd.Unused8Bytes[0]);
            Assert.Equal(0x03, pvd.Unused32Bytes[0]);
            Assert.Equal(4, pvd.SecondOptionalPathTableLocationL);
            Assert.Equal(5, pvd.ThirdOptionalPathTableLocationL);
            Assert.Equal(6, pvd.SecondOptionalPathTableLocationM);
            Assert.Equal(7, pvd.ThirdOptionalPathTableLocationM);
            Assert.Equal(0x08, pvd.ReservedByte);
            Assert.Equal(680, pvd.Reserved680Bytes.Length);
            Assert.Equal(0x09, pvd.Reserved680Bytes[0]);
            Assert.Equal(0x0A, pvd.Reserved680Bytes[679]);
            var terminator = Assert.IsType<HighSierraVolumeDescriptorSetTerminator>(actual.VolumeDescriptorSet[1]);
            Assert.Equal(2033, terminator.Reserved2033Bytes.Length);
            Assert.Equal(0x0B, terminator.Reserved2033Bytes[0]);
            Assert.Equal(0x0C, terminator.Reserved2033Bytes[2032]);
        }

        [Fact]
        public void ParseVolumeDescriptor_HighSierraBootRecord_ReadsGeneric()
        {
            byte[] image = new byte[18 * 2048];
            WriteHighSierraVolumeDescriptorHeader(image, 16, VolumeDescriptorType.BOOT_RECORD_VOLUME_DESCRIPTOR);
            image[(16 * 2048) + 15] = 0x01;
            image[(16 * 2048) + 2047] = 0x02;
            var stream = new MemoryStream(image);
            stream.Seek(16 * 2048, SeekOrigin.Begin);

            var actual = ISO9660.ParseVolumeDescriptor(stream, 2048);

            var gvd = Assert.IsType<HighSierraGenericVolumeDescriptor>(actual);
            Assert.Equal(16, gvd.LogicalBlockNumber.LittleEndian);
            Assert.Equal(VolumeDescriptorType.BOOT_RECORD_VOLUME_DESCRIPTOR, gvd.Type);
            Assert.Equal(2033, gvd.Data.Length);
            Assert.Equal(0x01, gvd.Data[0]);
            Assert.Equal(0x02, gvd.Data[2032]);
            Assert.Equal(17 * 2048, stream.Position);
        }

        [Theory]
        [InlineData(VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR)]
        [InlineData(VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)]
        [InlineData(VolumeDescriptorType.BOOT_RECORD_VOLUME_DESCRIPTOR)]
        public void ParseVolumeDescriptor_HighSierraLongSector_SkipsRemainder(VolumeDescriptorType type)
        {
            byte[] image = new byte[2 * 4096];
            WriteHighSierraVolumeDescriptorHeader(image, 0, type);
            WriteHighSierraRecord(image, 180, Constants.CurrentDirectory, 20, FileFlags.DIRECTORY);
            var stream = new MemoryStream(image);

            var actual = ISO9660.ParseVolumeDescriptor(stream, 4096);

            Assert.Equal(type, Assert.IsAssignableFrom<HighSierraVolumeDescriptor>(actual).Type);
            Assert.Equal(4096, stream.Position);
        }

        [Fact]
        public void HighSierra_ReadsPathTables()
        {
            byte[] image = CreateHighSierraImage(false);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(new MemoryStream(image));

            Assert.NotNull(actual);
            var group = Assert.Single(actual.PathTableGroups);
            Assert.NotNull(group.PathTableL);
            Assert.NotNull(group.PathTableM);
            foreach (var record in new[] { Assert.Single(group.PathTableL), Assert.Single(group.PathTableM) })
            {
                Assert.Equal(20, record.ExtentLocation);
                Assert.Equal(1, record.DirectoryIdentifierLength);
                Assert.Equal(1, record.ParentDirectoryNumber);
            }
        }

        [Fact]
        public void HighSierra_ReadsFileFlagsBeforeReservedByte()
        {
            byte[] image = CreateHighSierraImage(false);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(new MemoryStream(image));

            Assert.NotNull(actual);
            Assert.Equal(new[] { 20, 21 }, actual.DirectoryDescriptors.Keys.OrderBy(k => k));
            var root = Assert.IsType<DirectoryExtent>(actual.DirectoryDescriptors[20]);
            Assert.Equal(FileFlags.DIRECTORY, root.DirectoryRecords.Single(r => r.ExtentLocation.LittleEndian == 21).FileFlags);
            Assert.Equal(NoFlags, root.DirectoryRecords.Single(r => r.ExtentLocation.LittleEndian == 22).FileFlags);
            Assert.All(root.DirectoryRecords, r => Assert.Equal((byte)0x8B, r.ReservedByte));
            var sub = Assert.IsType<DirectoryExtent>(actual.DirectoryDescriptors[21]);
            Assert.Equal(NoFlags, sub.DirectoryRecords.Single(r => r.ExtentLocation.LittleEndian == 23).FileFlags);
        }

        [Fact]
        public void HighSierra_IsoLayoutRootRecord_ReadsRootDirectory()
        {
            byte[] image = CreateHighSierraImage(true);
            var deserializer = new ISO9660();

            var actual = deserializer.Deserialize(new MemoryStream(image));

            Assert.NotNull(actual);
            var pvd = Assert.IsType<HighSierraPrimaryVolumeDescriptor>(actual.VolumeDescriptorSet[0]);
            Assert.Equal(FileFlags.DIRECTORY, pvd.RootDirectoryRecord.FileFlags);
            Assert.Null(pvd.RootDirectoryRecord.ReservedByte);
            Assert.Equal(new[] { 20, 21 }, actual.DirectoryDescriptors.Keys.OrderBy(k => k));
        }

        [Fact]
        public void HighSierra_FindDirectoryRecords_ReadsFileFlagsBeforeReservedByte()
        {
            byte[] image = CreateHighSierraImage(false);
            var stream = new MemoryStream(image);
            stream.Seek(16 * 2048, SeekOrigin.Begin);
            var pvd = Assert.IsType<HighSierraPrimaryVolumeDescriptor>(ISO9660.ParseVolumeDescriptor(stream, 2048));

            var actual = ISO9660.FindDirectoryRecords(stream, 2048, pvd, "FILE.TXT");

            Assert.NotNull(actual);
            var record = Assert.Single(actual);
            Assert.Equal(22, record.ExtentLocation.LittleEndian);
            Assert.Equal(NoFlags, record.FileFlags);
            Assert.Equal((byte)0x8B, record.ReservedByte);
        }

        [Fact]
        public void ParseVolumeDescriptor_HighSierraIdentifierInIsoSystemIdentifier_ReadsIso()
        {
            // An ISO 9660 primary volume descriptor whose system identifier puts "CDROM" at byte 9
            byte[] sector = new byte[2048];
            sector[0] = (byte)VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR;
            Constants.StandardIdentifier.CopyTo(sector, 1);
            sector[6] = 1;
            Encoding.ASCII.GetBytes("XCDROM").CopyTo(sector, 8);
            WriteRecord(sector, 156, Constants.CurrentDirectory, 20, FileFlags.DIRECTORY);

            var actual = ISO9660.ParseVolumeDescriptor(new MemoryStream(sector), 2048);

            var pvd = Assert.IsType<PrimaryVolumeDescriptor>(actual);
            Assert.Equal(Constants.StandardIdentifier, pvd.Identifier);
            Assert.Equal("XCDROM", Encoding.ASCII.GetString(pvd.SystemIdentifier, 0, 6));
            Assert.Null(pvd.RootDirectoryRecord.ReservedByte);
        }

        [Fact]
        public void ParseVolumeDescriptorSet_ShortTail_ReturnsSet()
        {
            // A set terminator followed by fewer bytes than a volume descriptor header
            byte[] image = new byte[2048 + 8];
            image[0] = (byte)VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR;
            Constants.StandardIdentifier.CopyTo(image, 1);
            image[6] = 1;

            var actual = ISO9660.ParseVolumeDescriptorSet(new MemoryStream(image), 2048);

            Assert.Equal(VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR, Assert.Single(actual).Type);
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
        /// Write a one-sector directory extent with self and parent records and the given entries, returns the offset after the last record
        /// </summary>
        private static int WriteDirectory(byte[] image, int sector, params (string Name, int Location, FileFlags Flags)[] entries)
            => WriteDirectory(image, sector, Encoding.ASCII, entries);

        /// <summary>
        /// Write a one-sector directory extent with self and parent records and the given entries, returns the offset after the last record
        /// </summary>
        private static int WriteDirectory(byte[] image, int sector, Encoding encoding, params (string Name, int Location, FileFlags Flags)[] entries)
        {
            int offset = sector * 2048;
            offset = WriteRecord(image, offset, Constants.CurrentDirectory, sector, FileFlags.DIRECTORY);
            offset = WriteRecord(image, offset, Constants.ParentDirectory, sector, FileFlags.DIRECTORY);
            foreach (var (name, location, flags) in entries)
            {
                offset = WriteRecord(image, offset, encoding.GetBytes(name), location, flags);
            }

            return offset;
        }

        /// <summary>
        /// Write a directory record for a one-sector extent, returns the offset after the record
        /// </summary>
        private static int WriteRecord(byte[] image, int offset, byte[] identifier, int location, FileFlags flags)
            => WriteRecord(image, offset, identifier, location, flags, 33 + identifier.Length + (identifier.Length % 2 == 0 ? 1 : 0));

        /// <summary>
        /// Write a directory record with the given record length for a one-sector extent, returns the offset after the record
        /// </summary>
        private static int WriteRecord(byte[] image, int offset, byte[] identifier, int location, FileFlags flags, int length)
        {
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
        /// Create a path table group with a little-endian path table that records the given directory extents
        /// </summary>
        private static PathTableGroup CreatePathTableGroup(params int[] locations)
            => new() { PathTableL = [.. locations.Select(location => new PathTableRecord { ExtentLocation = location })] };

        /// <summary>
        /// Write a primary volume descriptor with 2048-byte blocks and a little-endian path table at sector 16,
        /// followed by a volume descriptor set terminator
        /// </summary>
        private static void WriteVolumeDescriptorSet(byte[] image, int rootSector, int pathTableSector, int pathTableSize)
        {
            int offset = 16 * 2048;
            image[offset] = (byte)VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR;
            Constants.StandardIdentifier.CopyTo(image, offset + 1);
            image[offset + 6] = 1;
            BinaryPrimitives.WriteInt16LittleEndian(image.AsSpan(offset + 128), 2048);
            BinaryPrimitives.WriteInt16BigEndian(image.AsSpan(offset + 130), 2048);
            WriteBothEndian(image, offset + 132, pathTableSize);
            BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(offset + 140), pathTableSector);
            WriteRecord(image, offset + 156, [0], rootSector, FileFlags.DIRECTORY);

            offset = 17 * 2048;
            image[offset] = (byte)VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR;
            Constants.StandardIdentifier.CopyTo(image, offset + 1);
            image[offset + 6] = 1;
        }

        /// <summary>
        /// Write a little-endian path table record, returns the offset after the record
        /// </summary>
        private static int WritePathTableRecord(byte[] image, int offset, byte[] identifier, int location, short parent)
        {
            image[offset] = (byte)identifier.Length;
            BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(offset + 2), location);
            BinaryPrimitives.WriteInt16LittleEndian(image.AsSpan(offset + 6), parent);
            identifier.CopyTo(image, offset + 8);
            return offset + 8 + identifier.Length + (identifier.Length % 2);
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
        /// Write an integer of the given size in little-endian or big-endian format
        /// </summary>
        private static void WriteInteger(byte[] image, int offset, int value, int size, bool bigEndian)
        {
            for (int i = 0; i < size; i++)
            {
                image[offset + (bigEndian ? size - 1 - i : i)] = (byte)(value >> (8 * i));
            }
        }

        /// <summary>
        /// Create a High Sierra image whose root directory at sector 20 holds SUB at sector 21 and FILE.TXT;1 at sector 22,
        /// SUB holds DEEP.TXT;1 at sector 23
        /// </summary>
        /// <param name="isoRootRecord">True to record the root directory record of the volume descriptor in the ISO 9660 layout</param>
        private static byte[] CreateHighSierraImage(bool isoRootRecord)
        {
            byte[] image = new byte[24 * 2048];

            // Standard file structure volume descriptor
            int vd = 16 * 2048;
            WriteHighSierraVolumeDescriptorHeader(image, 16, VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR);
            Encoding.ASCII.GetBytes("HSFTEST").CopyTo(image, vd + 48);
            WriteBothEndian(image, vd + 88, 24);
            WriteInteger(image, vd + 136, 2048, 2, false);
            WriteInteger(image, vd + 138, 2048, 2, true);
            WriteBothEndian(image, vd + 140, 10);
            WriteInteger(image, vd + 148, 18, 4, false);
            WriteInteger(image, vd + 164, 19, 4, true);
            if (isoRootRecord)
                WriteRecord(image, vd + 180, Constants.CurrentDirectory, 20, FileFlags.DIRECTORY);
            else
                WriteHighSierraRecord(image, vd + 180, Constants.CurrentDirectory, 20, FileFlags.DIRECTORY);

            Encoding.ASCII.GetBytes("ABSTRACT.TXT").CopyTo(image, vd + 758);
            image[vd + 854] = 1;
            Encoding.ASCII.GetBytes("APP").CopyTo(image, vd + 856);

            WriteHighSierraVolumeDescriptorHeader(image, 17, VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR);

            // Path tables with the root directory only, in both byte orders
            foreach (var (sector, bigEndian) in new[] { (18, false), (19, true) })
            {
                int offset = sector * 2048;
                WriteInteger(image, offset, 20, 4, bigEndian);
                image[offset + 5] = 1;
                WriteInteger(image, offset + 6, 1, 2, bigEndian);
            }

            WriteHighSierraDirectory(image, 20, ("SUB", 21, FileFlags.DIRECTORY), ("FILE.TXT;1", 22, NoFlags));
            WriteHighSierraDirectory(image, 21, ("DEEP.TXT;1", 23, NoFlags));
            return image;
        }

        /// <summary>
        /// Write the start of a High Sierra volume descriptor
        /// </summary>
        private static void WriteHighSierraVolumeDescriptorHeader(byte[] image, int sector, VolumeDescriptorType type)
        {
            int offset = sector * 2048;
            WriteBothEndian(image, offset, sector);
            image[offset + 8] = (byte)type;
            Constants.HighSierraIdentifier.CopyTo(image, offset + 9);
            image[offset + 14] = 1;
        }

        /// <summary>
        /// Write a one-sector High Sierra directory extent with self and parent records and the given entries
        /// </summary>
        private static void WriteHighSierraDirectory(byte[] image, int sector, params (string Name, int Location, FileFlags Flags)[] entries)
        {
            int offset = sector * 2048;
            offset = WriteHighSierraRecord(image, offset, Constants.CurrentDirectory, sector, FileFlags.DIRECTORY);
            offset = WriteHighSierraRecord(image, offset, Constants.ParentDirectory, sector, FileFlags.DIRECTORY);
            foreach (var (name, location, flags) in entries)
            {
                offset = WriteHighSierraRecord(image, offset, Encoding.ASCII.GetBytes(name), location, flags);
            }
        }

        /// <summary>
        /// Write a High Sierra directory record for a one-sector extent, returns the offset after the record
        /// </summary>
        /// <remarks>The file flags are at byte 24, the reserved byte 25 holds garbage as found on pressed discs</remarks>
        private static int WriteHighSierraRecord(byte[] image, int offset, byte[] identifier, int location, FileFlags flags)
        {
            int next = WriteRecord(image, offset, identifier, location, NoFlags);
            image[offset + 24] = (byte)flags;
            image[offset + 25] = 0x8B;
            return next;
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
