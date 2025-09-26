using SabreTools.Serialization.Readers;
using Xunit;

namespace SabreTools.Serialization.Test.Readers
{
    public class XeMIDTests
    {
        [Fact]
        public void NullString_Null()
        {
            string? data = null;
            var deserializer = new XeMID();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyString_Null()
        {
            string? data = string.Empty;
            var deserializer = new XeMID();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidString_Null()
        {
            string? data = "INVALID";
            var deserializer = new XeMID();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }
    }
}