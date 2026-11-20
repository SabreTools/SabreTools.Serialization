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
    public class ISO9660AssociatedFileTests
    {
        private const int BlockLength = 2048;

        /// <summary>
        /// Block of the root directory in the test image
        /// </summary>
        private const int RootDirectory = 20;

        /// <summary>
        /// File flags of a plain file record
        /// </summary>
        private const FileFlags NoFlags = 0;

        /// <summary>
        /// Offset of the resource fork in the AppleDouble file
        /// </summary>
        private const int ResourceForkOffset = 0xEE2;

        [Fact]
        public void Extract_AssociatedFileInOwnExtent_WritesAppleDoubleFile()
        {
            // The associated file of DATA.BIN is recorded first, in its own extent
            string outputDirectory = Extract(BuildImage(30,
                ("DATA.BIN", 25, (3 * BlockLength) + 100, FileFlags.ASSOCIATED_FILE, AppleSystemUse("rsrcRSED", 0)),
                ("DATA.BIN", 22, BlockLength + 50, NoFlags, [])), expected: true);

            Assert.Equal(ExpectedData(22, BlockLength + 50), File.ReadAllBytes(Path.Combine(outputDirectory, "DATA.BIN")));
            Assert.Equal(ExpectedAppleDouble(FinderInfo("rsrcRSED", 0), ExpectedData(25, (3 * BlockLength) + 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._DATA.BIN")));
            Assert.False(File.Exists(Path.Combine(outputDirectory, "DATA.BIN.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AssociatedFileWithoutAppleExtensions_WritesItAsRecorded()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("DATA.BIN", 25, (3 * BlockLength) + 100, FileFlags.ASSOCIATED_FILE, []),
                ("DATA.BIN", 22, BlockLength + 50, NoFlags, [])), expected: true);

            Assert.Equal(ExpectedData(22, BlockLength + 50), File.ReadAllBytes(Path.Combine(outputDirectory, "DATA.BIN")));
            Assert.Equal(ExpectedData(25, (3 * BlockLength) + 100),
                File.ReadAllBytes(Path.Combine(outputDirectory, "DATA.BIN.associated")));
            Assert.False(File.Exists(Path.Combine(outputDirectory, "._DATA.BIN")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AssociatedFileSharingExtentWithEmptyFile_WritesBoth()
        {
            // HELP.TXT is empty and recorded at the start of the extent of its associated file
            string outputDirectory = Extract(BuildImage(30,
                ("HELP.TXT", 25, 500, FileFlags.ASSOCIATED_FILE, []),
                ("HELP.TXT", 25, 0, NoFlags, [])), expected: true);

            Assert.Empty(File.ReadAllBytes(Path.Combine(outputDirectory, "HELP.TXT")));
            Assert.Equal(ExpectedData(25, 500), File.ReadAllBytes(Path.Combine(outputDirectory, "HELP.TXT.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AppleExtensionOnBothRecords_TakesFinderInfoOfFile()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, AppleSystemUse("rsrcRSED", 0)),
                ("README", 22, 10, NoFlags, AppleSystemUse("TEXTttxt", 0x0100))), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("TEXTttxt", 0x0100), ExpectedData(25, 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._README")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AppleExtensionOnAssociatedFileOnly_TakesFinderInfoOfAssociatedFile()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("PLAYER", 25, 100, FileFlags.ASSOCIATED_FILE, AppleSystemUse("APPLDPI6", 0x2000)),
                ("PLAYER", 22, 0, NoFlags, [])), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("APPLDPI6", 0x2000), ExpectedData(25, 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._PLAYER")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AppleExtensionOnFileOnly_TakesFinderInfoOfFile()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, AppleSystemUse("TEXTttxt", 0x0100))), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("TEXTttxt", 0x0100), ExpectedData(25, 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._README")));
            Assert.False(File.Exists(Path.Combine(outputDirectory, "README.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AssociatedFileWithoutFile_TakesFinderInfoOfAssociatedFile()
        {
            // Neither A nor C has a file, the record after A belongs to another file and C is the last record
            string outputDirectory = Extract(BuildImage(30,
                ("A", 25, 100, FileFlags.ASSOCIATED_FILE, AppleSystemUse("AAAAaaaa", 1)),
                ("B", 22, 10, NoFlags, AppleSystemUse("BBBBbbbb", 2)),
                ("C", 26, 100, FileFlags.ASSOCIATED_FILE, AppleSystemUse("CCCCcccc", 3))), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("AAAAaaaa", 1), ExpectedData(25, 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._A")));
            Assert.Equal(ExpectedData(22, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "B")));
            Assert.Equal(ExpectedAppleDouble(FinderInfo("CCCCcccc", 3), ExpectedData(26, 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._C")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_EmptyAssociatedFilesSharingExtent_WritesEach()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("LICENSE.TXT", 25, 0, FileFlags.ASSOCIATED_FILE, AppleSystemUse("ttrottxt", 0)),
                ("LICENSE.TXT", 22, 10, NoFlags, []),
                ("ORD_INFO", 25, 0, FileFlags.ASSOCIATED_FILE, AppleSystemUse("TEXTttxt", 0)),
                ("ORD_INFO", 23, 10, NoFlags, [])), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("ttrottxt", 0), []),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._LICENSE.TXT")));
            Assert.Equal(ExpectedAppleDouble(FinderInfo("TEXTttxt", 0), []),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._ORD_INFO")));
            Assert.Equal(ResourceForkOffset, new FileInfo(Path.Combine(outputDirectory, "._ORD_INFO")).Length);
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_MultiExtentAssociatedFile_WritesAllExtents()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("APP", 22, BlockLength, FileFlags.ASSOCIATED_FILE | FileFlags.MULTI_EXTENT, []),
                ("APP", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("APP", 27, 10, NoFlags, AppleSystemUse("APPLaapl", 0))), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("APPLaapl", 0), [.. ExpectedData(22, BlockLength), .. ExpectedData(25, 100)]),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._APP")));
            Assert.Equal(ExpectedData(27, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "APP")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_MultiExtentAssociatedFileWithoutLastExtent_WritesWhatIsRecorded()
        {
            // The first extent of an associated file is followed by its file, by another associated file, or by nothing
            string outputDirectory = Extract(BuildImage(30,
                ("APP", 22, BlockLength, FileFlags.ASSOCIATED_FILE | FileFlags.MULTI_EXTENT, []),
                ("APP", 27, 10, NoFlags, []),
                ("BIN", 23, BlockLength, FileFlags.ASSOCIATED_FILE | FileFlags.MULTI_EXTENT, []),
                ("CFG", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("ZZZ", 24, BlockLength, FileFlags.ASSOCIATED_FILE | FileFlags.MULTI_EXTENT, [])), expected: true);

            Assert.Equal(ExpectedData(22, BlockLength), File.ReadAllBytes(Path.Combine(outputDirectory, "APP.associated")));
            Assert.Equal(ExpectedData(27, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "APP")));
            Assert.Equal(ExpectedData(23, BlockLength), File.ReadAllBytes(Path.Combine(outputDirectory, "BIN.associated")));
            Assert.Equal(ExpectedData(25, 100), File.ReadAllBytes(Path.Combine(outputDirectory, "CFG.associated")));
            Assert.Equal(ExpectedData(24, BlockLength), File.ReadAllBytes(Path.Combine(outputDirectory, "ZZZ.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_TwoAssociatedFilesOfSameName_IsReported()
        {
            // Neither record has the multi-extent flag, so they are not two extents of one associated file
            string outputDirectory = Extract(BuildImage(30,
                ("APP", 22, BlockLength, FileFlags.ASSOCIATED_FILE, []),
                ("APP", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("APP", 27, 10, NoFlags, [])), expected: false);

            Assert.Equal(ExpectedData(22, BlockLength), File.ReadAllBytes(Path.Combine(outputDirectory, "APP.associated")));
            Assert.Equal(ExpectedData(27, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "APP")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AssociatedFileListedTwice_WritesItOnce()
        {
            byte[] image = BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, []));
            ListRootDirectoryAgain(image, 21,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, []));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(ExpectedData(25, 100), File.ReadAllBytes(Path.Combine(outputDirectory, "README.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_OtherAssociatedFileOfSameName_IsReported()
        {
            byte[] image = BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, []));
            ListRootDirectoryAgain(image, 21,
                ("README", 26, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, []));
            string outputDirectory = Extract(image, expected: false);

            Assert.Equal(ExpectedData(25, 100), File.ReadAllBytes(Path.Combine(outputDirectory, "README.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_FileNamedLikeAppleDoubleFile_IsKept()
        {
            string outputDirectory = Extract(BuildImage(30,
                ("._HELP.TXT", 22, 100, NoFlags, []),
                ("HELP.TXT", 25, 500, FileFlags.ASSOCIATED_FILE, AppleSystemUse("TEXTttxt", 0)),
                ("HELP.TXT", 23, 10, NoFlags, [])), expected: false);

            Assert.Equal(ExpectedData(22, 100), File.ReadAllBytes(Path.Combine(outputDirectory, "._HELP.TXT")));
            Assert.Equal(ExpectedData(23, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "HELP.TXT")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Theory]
        [InlineData(26)]
        [InlineData(27)]
        public void Extract_InterleavedAssociatedFile_IsReported(int field)
        {
            byte[] image = BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, []));

            // File unit size (26) or interleave gap size (27) of the record after the current and parent directory records
            image[(RootDirectory * BlockLength) + 34 + 34 + field] = 1;
            string outputDirectory = Extract(image, expected: false);

            Assert.False(File.Exists(Path.Combine(outputDirectory, "README.associated")));
            Assert.Equal(ExpectedData(22, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "README")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AssociatedFileBeyondEndOfImage_WritesNothingOfIt()
        {
            // The first extent is in the image, the second one is not
            string outputDirectory = Extract(BuildImage(30,
                ("APP", 22, BlockLength, FileFlags.ASSOCIATED_FILE | FileFlags.MULTI_EXTENT, []),
                ("APP", 40, 100, FileFlags.ASSOCIATED_FILE, []),
                ("APP", 27, 10, NoFlags, [])), expected: false);

            Assert.False(File.Exists(Path.Combine(outputDirectory, "APP.associated")));
            Assert.Equal(ExpectedData(27, 10), File.ReadAllBytes(Path.Combine(outputDirectory, "APP")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_AgainAfterOutputRemoved_WritesAssociatedFileAgain()
        {
            var iso = ISO9660.Create(new MemoryStream(BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, []),
                ("README", 22, 10, NoFlags, []))));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));
            Directory.Delete(outputDirectory, recursive: true);
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));

            Assert.Equal(ExpectedData(25, 100), File.ReadAllBytes(Path.Combine(outputDirectory, "README.associated")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Theory]
        [InlineData("41410E0254455854747478740100")]
        [InlineData("4E4D05004141410E0254455854747478740100")]
        public void Extract_SystemUseField_ReadsAppleExtension(string systemUse)
        {
            string outputDirectory = Extract(BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, Convert.FromHexString(systemUse)),
                ("README", 22, 10, NoFlags, [])), expected: true);

            Assert.Equal(ExpectedAppleDouble(FinderInfo("TEXTttxt", 0x0100), ExpectedData(25, 100)),
                File.ReadAllBytes(Path.Combine(outputDirectory, "._README")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Theory]
        [InlineData("41410E0154455854747478740100")]
        [InlineData("41410C025445585474747874")]
        [InlineData("5A5A0341410E0254455854747478740100")]
        [InlineData("000000000000584100000000000041410E0254455854")]
        public void Extract_SystemUseFieldWithoutAppleExtension_WritesItAsRecorded(string systemUse)
        {
            string outputDirectory = Extract(BuildImage(30,
                ("README", 25, 100, FileFlags.ASSOCIATED_FILE, Convert.FromHexString(systemUse)),
                ("README", 22, 10, NoFlags, [])), expected: true);

            Assert.Equal(ExpectedData(25, 100), File.ReadAllBytes(Path.Combine(outputDirectory, "README.associated")));
            Assert.False(File.Exists(Path.Combine(outputDirectory, "._README")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        #region Test Image

        /// <summary>
        /// Extract an image to a new directory
        /// </summary>
        private static string Extract(byte[] image, bool expected)
        {
            var iso = ISO9660.Create(new MemoryStream(image));
            Assert.NotNull(iso);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.Equal(expected, iso.Extract(outputDirectory, includeDebug: false));
            return outputDirectory;
        }

        /// <summary>
        /// Build an image with a primary volume descriptor and the given records in its root directory
        /// </summary>
        /// <remarks>Every byte of a block after the root directory holds the low byte of the block number</remarks>
        private static byte[] BuildImage(int blockCount, params (string Name, int Location, int Length, FileFlags Flags, byte[] SystemUse)[] files)
        {
            const int pathTableL = 18, pathTableM = 19;
            byte[] image = new byte[blockCount * BlockLength];
            for (int block = RootDirectory + 1; block < blockCount; block++)
            {
                image.AsSpan(block * BlockLength, BlockLength).Fill((byte)block);
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
            DirectoryRecord([0], RootDirectory, BlockLength, FileFlags.DIRECTORY, []).CopyTo(pvd[156..]);
            pvd[881] = 1;

            // Volume descriptor set terminator
            var terminator = image.AsSpan(17 * BlockLength, BlockLength);
            terminator[0] = 255;
            Encoding.ASCII.GetBytes("CD001").CopyTo(terminator[1..]);
            terminator[6] = 1;

            // Path tables with the root directory only
            var pathTable = image.AsSpan(pathTableL * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32LittleEndian(pathTable[2..], RootDirectory);
            BinaryPrimitives.WriteInt16LittleEndian(pathTable[6..], 1);
            pathTable = image.AsSpan(pathTableM * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32BigEndian(pathTable[2..], RootDirectory);
            BinaryPrimitives.WriteInt16BigEndian(pathTable[6..], 1);

            WriteDirectory(image, RootDirectory, files);
            return image;
        }

        /// <summary>
        /// List a second root directory at the big-endian extent location of the root directory record
        /// </summary>
        private static void ListRootDirectoryAgain(byte[] image, int block, params (string Name, int Location, int Length, FileFlags Flags, byte[] SystemUse)[] files)
        {
            BinaryPrimitives.WriteInt32BigEndian(image.AsSpan((16 * BlockLength) + 156 + 6), block);
            image.AsSpan(block * BlockLength, BlockLength).Clear();
            WriteDirectory(image, block, files);
        }

        /// <summary>
        /// Write a directory with the given records after its current and parent directory records
        /// </summary>
        private static void WriteDirectory(byte[] image, int block, (string Name, int Location, int Length, FileFlags Flags, byte[] SystemUse)[] files)
        {
            List<byte[]> records =
            [
                DirectoryRecord([0], block, BlockLength, FileFlags.DIRECTORY, []),
                DirectoryRecord([1], block, BlockLength, FileFlags.DIRECTORY, []),
            ];
            foreach (var (name, location, length, flags, systemUse) in files)
            {
                records.Add(DirectoryRecord(Encoding.ASCII.GetBytes($"{name};1"), location, length, flags, systemUse));
            }

            int position = block * BlockLength;
            foreach (byte[] record in records)
            {
                record.CopyTo(image, position);
                position += record.Length;
            }
        }

        /// <summary>
        /// Data of a file recorded at the given block, as BuildImage fills it
        /// </summary>
        private static byte[] ExpectedData(int location, int length)
            => [.. Enumerable.Range(0, length).Select(i => (byte)(location + (i / BlockLength)))];

        /// <summary>
        /// AppleDouble file with the given Finder type, creator and flags and resource fork
        /// </summary>
        private static byte[] ExpectedAppleDouble(byte[] finderInfo, byte[] resourceFork)
        {
            // Header as macOS writes it to a FAT volume, up to the end of its empty extended attributes header
            byte[] appleDouble = new byte[ResourceForkOffset + resourceFork.Length];
            Convert.FromHexString("00051607000200004D6163204F53205820202020202020200002000000090000003200000EB00000000200000EE2").CopyTo(appleDouble, 0);
            BinaryPrimitives.WriteInt32BigEndian(appleDouble.AsSpan(0x2E), resourceFork.Length);
            finderInfo.CopyTo(appleDouble, 0x32);
            Convert.FromHexString("415454520000000000000EE200000078").CopyTo(appleDouble, 0x54);
            resourceFork.CopyTo(appleDouble, ResourceForkOffset);
            return appleDouble;
        }

        /// <summary>
        /// Finder type and creator, followed by the Finder flags
        /// </summary>
        private static byte[] FinderInfo(string typeAndCreator, ushort flags)
        {
            byte[] finderInfo = new byte[10];
            Encoding.ASCII.GetBytes(typeAndCreator).CopyTo(finderInfo, 0);
            BinaryPrimitives.WriteUInt16BigEndian(finderInfo.AsSpan(8), flags);
            return finderInfo;
        }

        /// <summary>
        /// System Use field with CD-ROM XA system use information and the Apple extension for HFS
        /// </summary>
        private static byte[] AppleSystemUse(string typeAndCreator, ushort flags)
        {
            byte[] systemUse = new byte[28];
            Encoding.ASCII.GetBytes("XA").CopyTo(systemUse, 6);
            Encoding.ASCII.GetBytes("AA").CopyTo(systemUse, 14);
            systemUse[16] = 14;
            systemUse[17] = 2;
            FinderInfo(typeAndCreator, flags).CopyTo(systemUse, 18);
            return systemUse;
        }

        private static byte[] DirectoryRecord(byte[] identifier, int location, int length, FileFlags flags, byte[] systemUse)
        {
            // Pad the identifier and the System Use field to an even record length
            byte[] record = [.. new byte[33], .. identifier, .. new byte[1 - (identifier.Length % 2)], .. systemUse, .. new byte[systemUse.Length % 2]];
            record[0] = (byte)record.Length;
            WriteBothEndian(record.AsSpan(2), location);
            WriteBothEndian(record.AsSpan(10), length);
            record[25] = (byte)flags;
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
