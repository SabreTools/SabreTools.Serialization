using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class ISO9660Tests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = ISO9660.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = ISO9660.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = ISO9660.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = ISO9660.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = ISO9660.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = ISO9660.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void Extract_InterleavedFiles()
        {
            // B.TXT is recorded in the interleave gaps of A.TXT, C.TXT ends one byte into its last file unit
            TestFile[] files =
            [
                new("A.TXT", 28, 1, 1, 0, (3 * BlockLength) + 500, [28, 30, 32, 34]),
                new("B.TXT", 29, 1, 1, 0, 3 * BlockLength, [29, 31, 33]),
                new("C.TXT", 36, 3, 2, 0, (7 * BlockLength) + 1, [36, 37, 38, 41, 42, 43, 46, 47]),
                new("PLAIN.TXT", 25, 0, 0, 0, (2 * BlockLength) + 100, [25, 26, 27]),
            ];
            var iso = ISO9660.Create(new MemoryStream(BuildImage(60, files)));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));

            foreach (var file in files)
            {
                Assert.Equal(ExpectedData(file), File.ReadAllBytes(Path.Combine(outputDirectory, file.Name)));
            }

            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_InterleavedFileWithExtendedAttributeRecord()
        {
            // The extended attribute record fills the first file unit (49-50), the data starts after its gap (51)
            TestFile file = new("D.TXT", 49, 2, 1, 2, (3 * BlockLength) + 10, [52, 53, 55, 56]);
            var iso = ISO9660.Create(new MemoryStream(BuildImage(60, file)));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));

            Assert.Equal(ExpectedData(file), File.ReadAllBytes(Path.Combine(outputDirectory, file.Name)));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_InterleavedFileWithoutFileUnitSize()
        {
            TestFile file = new("E.TXT", 25, 0, 1, 0, BlockLength, [25]);
            var iso = ISO9660.Create(new MemoryStream(BuildImage(30, file)));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.False(iso.Extract(outputDirectory, includeDebug: false));

            Assert.False(File.Exists(Path.Combine(outputDirectory, file.Name)));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_InterleavedFilePastEndOfImage()
        {
            // The last file unit (46-47) ends one block after the image
            TestFile file = new("C.TXT", 36, 3, 2, 0, (7 * BlockLength) + 1, [36, 37, 38, 41, 42, 43, 46]);
            var iso = ISO9660.Create(new MemoryStream(BuildImage(47, file)));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.False(iso.Extract(outputDirectory, includeDebug: false));

            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_MultiExtentFileWithInterleavedSections()
        {
            // The sections of M.TXT fill each other's gaps, N.TXT has an interleaved and a non-interleaved section
            TestFile[] sections =
            [
                new("M.TXT", 28, 1, 1, 0, 3 * BlockLength, [28, 30, 32], (byte)FileFlags.MULTI_EXTENT),
                new("M.TXT", 29, 1, 1, 0, (2 * BlockLength) + 100, [29, 31, 33]),
                new("N.TXT", 36, 2, 1, 0, 4 * BlockLength, [36, 37, 39, 40], (byte)FileFlags.MULTI_EXTENT),
                new("N.TXT", 42, 0, 0, 0, BlockLength + 7, [42, 43]),
            ];
            var iso = ISO9660.Create(new MemoryStream(BuildImage(50, sections)));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));

            foreach (string name in new[] { "M.TXT", "N.TXT" })
            {
                byte[] expected = [.. sections.Where(section => section.Name == name).SelectMany(ExpectedData)];
                Assert.Equal(expected, File.ReadAllBytes(Path.Combine(outputDirectory, name)));
            }

            Directory.Delete(outputDirectory, recursive: true);
        }

        #region Test Image

        private const int BlockLength = 2048;

        /// <summary>
        /// File in a test image
        /// </summary>
        /// <param name="DataBlocks">Logical blocks that hold the file data, in file order</param>
        private sealed record TestFile(string Name,
            int Location,
            byte FileUnitSize,
            byte InterleaveGapSize,
            byte ExtendedAttributeRecordLength,
            int Length,
            int[] DataBlocks,
            byte FileFlags = 0);

        /// <summary>
        /// Build an image with a primary volume descriptor and the files in its root directory
        /// </summary>
        /// <remarks>
        /// Every data block is filled with the file name, its location and the block index in the file,
        /// every other block after the root directory reads "GAP"
        /// </remarks>
        private static byte[] BuildImage(int blockCount, params TestFile[] files)
        {
            const int pathTableL = 18, pathTableM = 19, rootDirectory = 20;
            byte[] image = new byte[blockCount * BlockLength];
            for (int offset = (rootDirectory + 1) * BlockLength; offset < image.Length; offset += 4)
            {
                Encoding.ASCII.GetBytes("GAP ").CopyTo(image, offset);
            }

            // Primary volume descriptor
            var pvd = image.AsSpan(16 * BlockLength, BlockLength);
            pvd[0] = 1;
            Encoding.ASCII.GetBytes("CD001").CopyTo(pvd[1..]);
            pvd[6] = 1;
            WriteBothEndian(pvd[80..], blockCount);
            WriteBothEndian16(pvd[120..], 1);
            WriteBothEndian16(pvd[124..], 1);
            WriteBothEndian16(pvd[128..], BlockLength);
            WriteBothEndian(pvd[132..], 10);
            BinaryPrimitives.WriteInt32LittleEndian(pvd[140..], pathTableL);
            BinaryPrimitives.WriteInt32BigEndian(pvd[148..], pathTableM);
            DirectoryRecord([0], rootDirectory, BlockLength, 2, 0, 0, 0).CopyTo(pvd[156..]);
            pvd[881] = 1;

            // Volume descriptor set terminator
            var terminator = image.AsSpan(17 * BlockLength, BlockLength);
            terminator[0] = 255;
            Encoding.ASCII.GetBytes("CD001").CopyTo(terminator[1..]);
            terminator[6] = 1;

            // Path tables with the root directory only
            var pathTable = image.AsSpan(pathTableL * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32LittleEndian(pathTable[2..], rootDirectory);
            BinaryPrimitives.WriteInt16LittleEndian(pathTable[6..], 1);
            pathTable = image.AsSpan(pathTableM * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32BigEndian(pathTable[2..], rootDirectory);
            BinaryPrimitives.WriteInt16BigEndian(pathTable[6..], 1);

            // Root directory
            List<byte[]> records =
            [
                DirectoryRecord([0], rootDirectory, BlockLength, 2, 0, 0, 0),
                DirectoryRecord([1], rootDirectory, BlockLength, 2, 0, 0, 0),
            ];
            foreach (var file in files)
            {
                records.Add(DirectoryRecord(Encoding.ASCII.GetBytes($"{file.Name};1"),
                    file.Location,
                    file.Length,
                    file.FileFlags,
                    file.FileUnitSize,
                    file.InterleaveGapSize,
                    file.ExtendedAttributeRecordLength));
            }

            var directory = image.AsSpan(rootDirectory * BlockLength, BlockLength);
            int position = 0;
            foreach (byte[] record in records)
            {
                record.CopyTo(directory[position..]);
                position += record.Length;
            }

            foreach (var file in files)
            {
                // Extended attribute record, version 1
                if (file.ExtendedAttributeRecordLength != 0)
                {
                    var ear = image.AsSpan(file.Location * BlockLength, file.ExtendedAttributeRecordLength * BlockLength);
                    ear.Clear();
                    ear[180] = 1;
                }

                for (int i = 0; i < file.DataBlocks.Length; i++)
                {
                    BlockData(file, i).CopyTo(image, file.DataBlocks[i] * BlockLength);
                }
            }

            return image;
        }

        /// <summary>
        /// File data as an ISO 9660 reader should return it
        /// </summary>
        private static byte[] ExpectedData(TestFile file)
            => [.. Enumerable.Range(0, file.DataBlocks.Length).SelectMany(i => BlockData(file, i)).Take(file.Length)];

        /// <summary>
        /// Content of a data block, recognizable by file name, location and block index
        /// </summary>
        private static byte[] BlockData(TestFile file, int index)
        {
            byte[] text = Encoding.ASCII.GetBytes($"{file.Name}@{file.Location} block {index:D4} ");
            return [.. Enumerable.Range(0, BlockLength).Select(i => text[i % text.Length])];
        }

        private static byte[] DirectoryRecord(byte[] identifier, int location, int length, byte flags, byte fileUnitSize, byte interleaveGapSize, byte extendedAttributeRecordLength)
        {
            // Pad the record to an even length
            byte[] record = [.. new byte[33], .. identifier, .. new byte[1 - (identifier.Length % 2)]];
            record[0] = (byte)record.Length;
            record[1] = extendedAttributeRecordLength;
            WriteBothEndian(record.AsSpan(2), location);
            WriteBothEndian(record.AsSpan(10), length);
            record[25] = flags;
            record[26] = fileUnitSize;
            record[27] = interleaveGapSize;
            WriteBothEndian16(record.AsSpan(28), 1);
            record[32] = (byte)identifier.Length;
            return record;
        }

        private static void WriteBothEndian(Span<byte> destination, int value)
        {
            BinaryPrimitives.WriteInt32LittleEndian(destination, value);
            BinaryPrimitives.WriteInt32BigEndian(destination[4..], value);
        }

        private static void WriteBothEndian16(Span<byte> destination, short value)
        {
            BinaryPrimitives.WriteInt16LittleEndian(destination, value);
            BinaryPrimitives.WriteInt16BigEndian(destination[2..], value);
        }

        #endregion
    }
}
