using SabreTools.Serialization.Wrappers;
using Xunit;

namespace SabreTools.Serialization.Test.Wrappers
{
    public class XeMIDTests
    {
        [Fact]
        public void NullString_Null()
        {
            string? data = null;
            var actual = XeMID.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyString_Null()
        {
            string? data = string.Empty;
            var actual = XeMID.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidString_Null()
        {
            string? data = "INVALID";
            var actual = XeMID.Create(data);
            Assert.Null(actual);
        }
    }
}