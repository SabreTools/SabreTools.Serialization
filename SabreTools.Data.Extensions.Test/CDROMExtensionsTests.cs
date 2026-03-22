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

        // TODO: Figure out how to add ISO9660Stream tests
    }
}
