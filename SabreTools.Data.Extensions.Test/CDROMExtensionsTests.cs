using System.IO;
using SabreTools.Data.Models.CDROM;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class CDROMExtensionsTests
    {
        [Theory]
        [InlineData(new byte[0], SectorMode.UNKNOWN)]
        [InlineData(new byte[] { 0x00 }, SectorMode.MODE0)]
        [InlineData(new byte[] { 0x01 }, SectorMode.MODE1)]
        [InlineData(new byte[] { 0x02 }, SectorMode.UNKNOWN)]
        [InlineData(new byte[] { 0x02, 0x00, 0x00, 0x00 }, SectorMode.MODE2_FORM1)]
        [InlineData(new byte[] { 0x02, 0x00, 0x00, 0x20 }, SectorMode.MODE2_FORM2)]
        [InlineData(new byte[] { 0x03 }, SectorMode.UNKNOWN)]
        public void GetSectorModeTest(byte[] bytes, SectorMode expected)
        {
            var stream = new MemoryStream(bytes);
            SectorMode actual = stream.GetSectorMode();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(SectorMode.UNKNOWN, 2336)]
        [InlineData(SectorMode.MODE0, 2336)]
        [InlineData(SectorMode.MODE1, 2048)]
        [InlineData(SectorMode.MODE2, 2336)]
        [InlineData(SectorMode.MODE2_FORM1, 2048)]
        [InlineData(SectorMode.MODE2_FORM2, 2324)]
        public void GetUserDataSizeTest(SectorMode mode, long expected)
        {
            long actual = mode.GetUserDataSize();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(SectorMode.UNKNOWN, 2064)]
        [InlineData(SectorMode.MODE0, 2064)]
        [InlineData(SectorMode.MODE1, 2064)]
        [InlineData(SectorMode.MODE2, 2064)]
        [InlineData(SectorMode.MODE2_FORM1, 2072)]
        [InlineData(SectorMode.MODE2_FORM2, 2072)]
        public void GetUserDataEndTest(SectorMode mode, long expected)
        {
            long actual = mode.GetUserDataEnd();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(SectorMode.UNKNOWN, 16)]
        [InlineData(SectorMode.MODE0, 16)]
        [InlineData(SectorMode.MODE1, 16)]
        [InlineData(SectorMode.MODE2, 16)]
        [InlineData(SectorMode.MODE2_FORM1, 24)]
        [InlineData(SectorMode.MODE2_FORM2, 24)]
        public void GetUserDataStart(SectorMode mode, long expected)
        {
            long actual = mode.GetUserDataStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UserDataStreamMode2ToMode1ReadTest()
        {
            byte[] data = CreateSectors(0x02, 0x01, 0x01);
            var stream = new CDROMExtensions.UserDataStream(new MemoryStream(data));

            byte[] actual = new byte[3 * 2048];
            int read = stream.Read(actual, 0, actual.Length);

            Assert.Equal(actual.Length, read);
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.Equal((byte)(i / 2048), actual[i]);
            }
        }

        [Fact]
        public void UserDataStreamMode2ToMode1PositionTest()
        {
            byte[] data = CreateSectors(0x02, 0x01, 0x01);
            var stream = new CDROMExtensions.UserDataStream(new MemoryStream(data));

            byte[] actual = new byte[2048];
            _ = stream.Read(actual, 0, actual.Length);

            Assert.Equal(2048, stream.Position);
        }

        /// <summary>
        /// Create raw sectors with the given modes, filling each user data area with the sector index
        /// </summary>
        private static byte[] CreateSectors(params byte[] modes)
        {
            byte[] data = new byte[modes.Length * Constants.CDROMSectorSize];
            for (int i = 0; i < modes.Length; i++)
            {
                long sectorStart = i * Constants.CDROMSectorSize;
                data[sectorStart + 15] = modes[i];

                long userDataStart = sectorStart + (modes[i] == 0x02 ? Constants.Mode2Form1UserDataStart : Constants.Mode1UserDataStart);
                for (long j = 0; j < 2048; j++)
                {
                    data[userDataStart + j] = (byte)i;
                }
            }

            return data;
        }
    }
}
