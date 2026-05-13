using SabreTools.Data.Models.NintendoDisc;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class NintendoDiscExtensionsTests
    {
        #region GetPlatform

        [Fact]
        public void GetPlatform_Object_WiiMagic_Wii()
        {
            var header = new DiscHeader { WiiMagic = Constants.WiiMagicWord };
            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.Wii, actual);
        }

        [Fact]
        public void GetPlatform_Object_GCMagic_GC()
        {
            var header = new DiscHeader { GCMagic = Constants.GCMagicWord };
            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.GameCube, actual);
        }

        [Fact]
        public void GetPlatform_Object_ValidGameID_GC()
        {
            var header = new DiscHeader { GameId = "GABC01" };
            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.GameCube, actual);
        }

        [Fact]
        public void GetPlatform_Object_InvalidGameID_Unknown()
        {
            var header = new DiscHeader { GameId = "XABC01" };
            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.Unknown, actual);
        }

        [Fact]
        public void GetPlatform_Object_DefaultHeader_Unknown()
        {
            var header = new DiscHeader();
            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.Unknown, actual);
        }

        [Fact]
        public void GetPlatform_Bytes_WiiMagic_Wii()
        {
            byte[] header =
            [
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x00-0x07
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x08-0x0F
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x10-0x17
                0x5D, 0x1C, 0x9E, 0xA3, 0x00, 0x00, 0x00, 0x00, // 0x18-0x1F
            ];

            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.Wii, actual);
        }

        [Fact]
        public void GetPlatform_Bytes_GCMagic_GC()
        {
            byte[] header =
            [
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x00-0x07
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x08-0x0F
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 0x10-0x17
                0x00, 0x00, 0x00, 0x00, 0xC2, 0x33, 0x9F, 0x3D, // 0x18-0x1F
            ];

            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.GameCube, actual);
        }

        [Fact]
        public void GetPlatform_Bytes_ValidGameID_GC()
        {
            byte[] header = [(byte)'G', (byte)'A', (byte)'B', (byte)'C'];

            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.GameCube, actual);
        }

        [Fact]
        public void GetPlatform_Bytes_InvalidGameID_Unknown()
        {
            byte[] header = [(byte)'G', (byte)'A', (byte)'B', 0x00];

            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.Unknown, actual);
        }

        [Fact]
        public void GetPlatform_Bytes_DefaultHeader_Unknown()
        {
            byte[] header = [];

            Platform actual = header.GetPlatform();
            Assert.Equal(Platform.Unknown, actual);
        }

        #endregion

        #region IsGameCubeTitleType

        [Theory]
        [InlineData('\0', false)]
        [InlineData('a', false)]
        [InlineData('A', false)]
        [InlineData('d', false)]
        [InlineData('D', true)]
        [InlineData('g', false)]
        [InlineData('G', true)]
        [InlineData('r', false)]
        [InlineData('R', true)]
        public void IsGameCubeTitleTypeTest(char c, bool expected)
        {
            bool actual = c.IsGameCubeTitleType();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
