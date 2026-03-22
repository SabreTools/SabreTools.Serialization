using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class XboxExecutableExtensionsTests
    {
        [Theory]
        [InlineData(0x00000000, "0000-000")]
        [InlineData(0x4142000F, "AB-015")]
        [InlineData(0x3132F000, "12-61440")]
        public void ToFormattedXBETitleIDTest(uint value, string expected)
        {
            string actual = value.ToFormattedXBETitleID();
            Assert.Equal(expected, actual);
        }
    }
}
