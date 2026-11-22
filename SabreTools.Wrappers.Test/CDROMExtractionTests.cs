using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.ISO9660;
using Xunit;
using static SabreTools.Wrappers.Test.ISO9660ImageBuilder;

namespace SabreTools.Wrappers.Test
{
    public class CDROMExtractionTests
    {
        private const int SectorLength = 2352;

        /// <summary>
        /// Sectors of the data track in the test image, followed by the audio sectors
        /// </summary>
        private const int DataSectors = 30;

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
            byte[] userData = ISO9660ImageBuilder.BuildImage(DataSectors, files);

            // The volume space also holds the audio sectors
            WriteBothEndian(userData.AsSpan((16 * BlockLength) + 80), DataSectors + audioSectors);

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
        /// System Use field with CD-ROM XA system use information
        /// </summary>
        private static byte[] XASystemUse(ushort attributes)
        {
            byte[] systemUse = new byte[14];
            BinaryPrimitives.WriteUInt16BigEndian(systemUse.AsSpan(4), attributes);
            Encoding.ASCII.GetBytes("XA").CopyTo(systemUse, 6);
            return systemUse;
        }

        private static byte ToBcd(int value)
            => (byte)(((value / 10) << 4) | (value % 10));

        #endregion
    }
}
