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
    }
}
