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
    public class CDROMExtractionTests
    {
        private const int BlockLength = 2048;

        private const int SectorLength = 2352;

        /// <summary>
        /// Sector of the root directory in the test image
        /// </summary>
        private const int RootDirectory = 20;

        /// <summary>
        /// Sectors of the data track in the test image, followed by the audio sectors
        /// </summary>
        private const int DataSectors = 30;

        /// <summary>
        /// File flags of a plain file record
        /// </summary>
        private const FileFlags NoFlags = 0;

        [Fact]
        public void Extract_CDDAFile_WritesWaveFileOfRawSectors()
        {
            byte[] image = BuildImage(5,
                ("TRACK02.CDA", DataSectors, 5 * BlockLength, NoFlags, XASystemUse(0x4111)),
                ("DATA.BIN", 22, BlockLength + 50, NoFlags, []));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(Wave(RawSectors(image, DataSectors, 5)), File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Assert.Equal(ExpectedData(22, BlockLength + 50), File.ReadAllBytes(Path.Combine(outputDirectory, "DATA.BIN")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_CDDAFile_StartsWithWaveHeaderOfWindows()
        {
            // The header of CdXAAudioPhileHeader in the CDFS driver of Windows, for 5 sectors (11,760 bytes)
            byte[] header = Convert.FromHexString("52494646142E000057415645666D7420100000000100020044AC000010B102000400100064617461F02D0000");
            byte[] image = BuildImage(5, ("TRACK02.CDA", DataSectors, 5 * BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(header, File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA"))[..44]);
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_LargeCDDAFile_WritesWaveFileOfRawSectors()
        {
            byte[] image = BuildImage(2500, ("TRACK02.CDA", DataSectors, 2500 * BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(Wave(RawSectors(image, DataSectors, 2500)), File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Theory]
        [InlineData((2 * BlockLength) + 1, 3)]
        [InlineData(4 * SectorLength, 5)]
        public void Extract_CDDAFileLengthNotMultipleOfBlockLength_WritesEverySectorOfExtent(int length, int sectors)
        {
            byte[] image = BuildImage(5, ("TRACK02.CDA", DataSectors, length, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(Wave(RawSectors(image, DataSectors, sectors)), File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_CDDAFilePastEndOfImage_WritesSectorsInImage()
        {
            byte[] image = BuildImage(5, ("TRACK02.CDA", DataSectors + 1, 8 * BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: false);

            Assert.Equal(Wave(RawSectors(image, DataSectors + 1, 4)), File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_CDDAFileStartingPastEndOfImage_WritesNothing()
        {
            byte[] image = BuildImage(5, ("TRACK02.CDA", DataSectors + 5, BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: false);

            Assert.False(File.Exists(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_MultiExtentCDDAFile_WritesAllExtents()
        {
            byte[] image = BuildImage(5,
                ("TRACK02.CDA", DataSectors, 2 * BlockLength, FileFlags.MULTI_EXTENT, XASystemUse(0x4111)),
                ("TRACK02.CDA", DataSectors + 2, 3 * BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(Wave(RawSectors(image, DataSectors, 5)), File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_MultiExtentFileWithDataExtentFirst_AppendsRawSectorsWithoutHeader()
        {
            // The file does not start with a header written for it, so none of its bytes are changed
            byte[] image = BuildImage(5,
                ("TRACK02.CDA", 22, 2 * BlockLength, FileFlags.MULTI_EXTENT, []),
                ("TRACK02.CDA", DataSectors, 3 * BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal([.. ExpectedData(22, 2 * BlockLength), .. RawSectors(image, DataSectors, 3)], File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_MultiExtentCDDAFileExtractedTwice_KeepsHeaderOfFirstExtraction()
        {
            // The second extraction appends to the file of the first, and only updates a header it wrote itself
            byte[] image = BuildImage(5,
                ("TRACK02.CDA", DataSectors, 2 * BlockLength, FileFlags.MULTI_EXTENT, XASystemUse(0x4111)),
                ("TRACK02.CDA", DataSectors + 2, 3 * BlockLength, NoFlags, XASystemUse(0x4111)));
            var cdrom = CDROM.Create(new MemoryStream(image));
            Assert.NotNull(cdrom);
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(cdrom.Extract(outputDirectory, includeDebug: false));
            Assert.True(cdrom.Extract(outputDirectory, includeDebug: false));

            Assert.Equal([.. Wave(RawSectors(image, DataSectors, 5)), .. RawSectors(image, DataSectors, 5)], File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_CDDAFileListedTwice_WritesItOnce()
        {
            // As for other files, a second record of the same extent is not written again
            byte[] image = BuildImage(5,
                ("TRACK02.CDA", DataSectors, 5 * BlockLength, NoFlags, XASystemUse(0x4111)),
                ("TRACK2.CDA", DataSectors, 5 * BlockLength, NoFlags, XASystemUse(0x4111)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(Wave(RawSectors(image, DataSectors, 5)), File.ReadAllBytes(Path.Combine(outputDirectory, "TRACK02.CDA")));
            Assert.False(File.Exists(Path.Combine(outputDirectory, "TRACK2.CDA")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_XAFileWithoutCDDAAttribute_WritesUserData()
        {
            byte[] image = BuildImage(5, ("DATA.BIN", 22, (2 * BlockLength) + 50, NoFlags, XASystemUse(0x0D55)));
            string outputDirectory = Extract(image, expected: true);

            Assert.Equal(ExpectedData(22, (2 * BlockLength) + 50), File.ReadAllBytes(Path.Combine(outputDirectory, "DATA.BIN")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        [Fact]
        public void Extract_CDDAFileFromUserDataImage_WritesUserData()
        {
            // A volume without raw sectors has no audio to write, so the user data is written as before
            byte[] image = BuildImage(0, ("DATA.BIN", 22, (2 * BlockLength) + 50, NoFlags, XASystemUse(0x4111)));
            byte[] userData = [.. Enumerable.Range(0, DataSectors).SelectMany(sector => image.Skip((sector * SectorLength) + 16).Take(BlockLength))];

            var iso = ISO9660.Create(new MemoryStream(userData));
            Assert.NotNull(iso);
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.True(iso.Extract(outputDirectory, includeDebug: false));

            Assert.Equal(ExpectedData(22, (2 * BlockLength) + 50), File.ReadAllBytes(Path.Combine(outputDirectory, "DATA.BIN")));
            Directory.Delete(outputDirectory, recursive: true);
        }

        #region Test Image

        /// <summary>
        /// Extract a raw image to a new directory
        /// </summary>
        private static string Extract(byte[] image, bool expected)
        {
            var cdrom = CDROM.Create(new MemoryStream(image));
            Assert.NotNull(cdrom);

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            Assert.Equal(expected, cdrom.Extract(outputDirectory, includeDebug: false));
            return outputDirectory;
        }

        /// <summary>
        /// Build a raw image of Mode 1 data sectors with the given records in the root directory, followed by audio sectors
        /// </summary>
        /// <remarks>
        /// Every byte of a data block after the root directory holds the low byte of the block number,
        /// byte i of an audio sector holds the low byte of the sector number plus i
        /// </remarks>
        private static byte[] BuildImage(int audioSectors, params (string Name, int Location, int Length, FileFlags Flags, byte[] SystemUse)[] files)
        {
            const int pathTableL = 18, pathTableM = 19;
            byte[] userData = new byte[DataSectors * BlockLength];
            for (int block = RootDirectory + 1; block < DataSectors; block++)
            {
                userData.AsSpan(block * BlockLength, BlockLength).Fill((byte)block);
            }

            // Primary volume descriptor
            var pvd = userData.AsSpan(16 * BlockLength, BlockLength);
            pvd[0] = 1;
            Encoding.ASCII.GetBytes("CD001").CopyTo(pvd[1..]);
            pvd[6] = 1;
            WriteBothEndian(pvd[80..], DataSectors + audioSectors);
            WriteBothEndian16(pvd[120..], 1);
            WriteBothEndian16(pvd[124..], 1);
            WriteBothEndian16(pvd[128..], BlockLength);
            WriteBothEndian(pvd[132..], 10);
            BinaryPrimitives.WriteInt32LittleEndian(pvd[140..], pathTableL);
            BinaryPrimitives.WriteInt32BigEndian(pvd[148..], pathTableM);
            DirectoryRecord([0], RootDirectory, BlockLength, FileFlags.DIRECTORY, []).CopyTo(pvd[156..]);
            pvd[881] = 1;

            // Volume descriptor set terminator
            var terminator = userData.AsSpan(17 * BlockLength, BlockLength);
            terminator[0] = 255;
            Encoding.ASCII.GetBytes("CD001").CopyTo(terminator[1..]);
            terminator[6] = 1;

            // Path tables with the root directory only
            var pathTable = userData.AsSpan(pathTableL * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32LittleEndian(pathTable[2..], RootDirectory);
            BinaryPrimitives.WriteInt16LittleEndian(pathTable[6..], 1);
            pathTable = userData.AsSpan(pathTableM * BlockLength);
            pathTable[0] = 1;
            BinaryPrimitives.WriteInt32BigEndian(pathTable[2..], RootDirectory);
            BinaryPrimitives.WriteInt16BigEndian(pathTable[6..], 1);

            // Root directory
            List<byte[]> records =
            [
                DirectoryRecord([0], RootDirectory, BlockLength, FileFlags.DIRECTORY, []),
                DirectoryRecord([1], RootDirectory, BlockLength, FileFlags.DIRECTORY, []),
            ];
            foreach (var (name, location, length, flags, systemUse) in files)
            {
                records.Add(DirectoryRecord(Encoding.ASCII.GetBytes($"{name};1"), location, length, flags, systemUse));
            }

            int position = RootDirectory * BlockLength;
            foreach (byte[] record in records)
            {
                record.CopyTo(userData, position);
                position += record.Length;
            }

            // Mode 1 sectors: sync pattern, header (address as minutes, seconds, frames, then the mode), user data
            byte[] image = new byte[(DataSectors + audioSectors) * SectorLength];
            for (int sector = 0; sector < DataSectors; sector++)
            {
                var raw = image.AsSpan(sector * SectorLength, SectorLength);
                raw[1..11].Fill(0xFF);
                int address = sector + 150;
                raw[12] = ToBcd(address / 75 / 60);
                raw[13] = ToBcd(address / 75 % 60);
                raw[14] = ToBcd(address % 75);
                raw[15] = 1;
                userData.AsSpan(sector * BlockLength, BlockLength).CopyTo(raw[16..]);
            }

            for (int sector = DataSectors; sector < DataSectors + audioSectors; sector++)
            {
                for (int i = 0; i < SectorLength; i++)
                {
                    image[(sector * SectorLength) + i] = (byte)(sector + i);
                }
            }

            return image;
        }

        /// <summary>
        /// Raw sectors of the image
        /// </summary>
        private static byte[] RawSectors(byte[] image, int sector, int count)
            => image.AsSpan(sector * SectorLength, count * SectorLength).ToArray();

        /// <summary>
        /// WAVE file of raw sectors, 16-bit stereo PCM at 44100 Hz
        /// </summary>
        private static byte[] Wave(byte[] sectors)
        {
            byte[] header = new byte[44];
            Encoding.ASCII.GetBytes("RIFF").CopyTo(header, 0);
            BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(4), sectors.Length + 36);
            Encoding.ASCII.GetBytes("WAVEfmt ").CopyTo(header, 8);
            BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(16), 16);
            BinaryPrimitives.WriteInt16LittleEndian(header.AsSpan(20), 1);
            BinaryPrimitives.WriteInt16LittleEndian(header.AsSpan(22), 2);
            BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(24), 44100);
            BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(28), 44100 * 4);
            BinaryPrimitives.WriteInt16LittleEndian(header.AsSpan(32), 4);
            BinaryPrimitives.WriteInt16LittleEndian(header.AsSpan(34), 16);
            Encoding.ASCII.GetBytes("data").CopyTo(header, 36);
            BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(40), sectors.Length);
            return [.. header, .. sectors];
        }

        /// <summary>
        /// Data of a file recorded at the given block, as BuildImage fills it
        /// </summary>
        private static byte[] ExpectedData(int location, int length)
            => [.. Enumerable.Range(0, length).Select(i => (byte)(location + (i / BlockLength)))];

        /// <summary>
        /// System Use field with CD-ROM XA system use information
        /// </summary>
        private static byte[] XASystemUse(ushort attributes)
        {
            byte[] systemUse = new byte[14];
            BinaryPrimitives.WriteUInt16BigEndian(systemUse.AsSpan(4), attributes);
            Encoding.ASCII.GetBytes("XA").CopyTo(systemUse, 6);
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

        private static byte ToBcd(int value)
            => (byte)(((value / 10) << 4) | (value % 10));

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
