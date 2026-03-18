
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class XMIDTests
    {
        [Fact]
        public void NullString_Null()
        {
            string? data = null;
            var actual = XMID.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyString_Null()
        {
            string? data = string.Empty;
            var actual = XMID.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidString_Null()
        {
            string? data = "INVALID";
            var actual = XMID.Create(data);
            Assert.Null(actual);
        }
    }
}
