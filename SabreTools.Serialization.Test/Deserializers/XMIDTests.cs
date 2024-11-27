using Xunit;

namespace SabreTools.Serialization.Test.Deserializers
{
    public class XMIDTests
    {
        [Fact]
        public void NullString_Null()
        {
            string? data = null;
            var deserializer = new Serialization.Deserializers.XMID();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyString_Null()
        {
            string? data = string.Empty;
            var deserializer = new Serialization.Deserializers.XMID();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidString_Null()
        {
            string? data = "INVALID";
            var deserializer = new Serialization.Deserializers.XMID();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }
    }
}