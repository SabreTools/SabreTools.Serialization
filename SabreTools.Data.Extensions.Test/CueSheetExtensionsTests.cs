using SabreTools.Data.Models.CueSheets;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class CueSheetExtensionsTests
    {
        [Theory]
        [InlineData("SINGLE-DENSITY AREA", CueDensityArea.SingleDensity)]
        [InlineData("HIGH-DENSITY AREA", CueDensityArea.HighDensity)]
        [InlineData("HIGH-DENSITY", CueDensityArea.None)]
        [InlineData("", CueDensityArea.None)]
        [InlineData("SESSION 01", CueDensityArea.None)]
        public void GetDensityAreaTest(string remark, CueDensityArea expected)
        {
            CueDensityArea actual = CueSheetExtensions.GetDensityArea(remark);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("SESSION 01", 1)]
        [InlineData("SESSION 2", 2)]
        [InlineData("SESSION 02        (*)", 2)]
        [InlineData("SESSION 02\"Second\"", 2)]
        [InlineData("SESSION", null)]
        [InlineData("SESSION X", null)]
        [InlineData("", null)]
        [InlineData("LBA: 0", null)]
        [InlineData("(*) SESSION commands are not supported by all applications", null)]
        public void GetSessionTest(string remark, int? expected)
        {
            int? actual = CueSheetExtensions.GetSession(remark);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("SESSION 02", 2, CueDensityArea.SingleDensity)]
        [InlineData("HIGH-DENSITY AREA", 1, CueDensityArea.HighDensity)]
        [InlineData("SESSION X", 1, CueDensityArea.SingleDensity)]
        [InlineData("LBA: 0", 1, CueDensityArea.SingleDensity)]
        [InlineData("", 1, CueDensityArea.SingleDensity)]
        public void ReadRemarkTest(string remark, int expectedSession, CueDensityArea expectedDensityArea)
        {
            int session = 1;
            var densityArea = CueDensityArea.SingleDensity;
            CueSheetExtensions.ReadRemark(remark, ref session, ref densityArea);
            Assert.Equal(expectedSession, session);
            Assert.Equal(expectedDensityArea, densityArea);
        }

        [Theory]
        [InlineData("", new string[0])]
        [InlineData("  TRACK 01 MODE1/2352", new string[] { "TRACK", "01", "MODE1/2352" })]
        [InlineData("FILE \"Track 01.bin\" BINARY", new string[] { "FILE", "\"Track 01.bin\"", "BINARY" })]
        [InlineData("SESSION 02\"Second\"", new string[] { "SESSION", "02", "\"Second\"" })]
        public void SplitLineTest(string line, string[] expected)
        {
            string[] actual = CueSheetExtensions.SplitLine(line);
            Assert.Equal(expected, actual);
        }
    }
}
