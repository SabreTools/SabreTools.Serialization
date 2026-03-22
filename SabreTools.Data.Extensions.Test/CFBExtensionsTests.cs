using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class CFBExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(new byte[0], "")]
        // TODO: Create more artifical tests
        public void DecodeStreamNameTest(byte[]? bytes, string? expected)
        {
            string? actual = bytes.DecodeStreamName();
            Assert.Equal(expected, actual);
        }
    }
}
