using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
{
    public class XeMIDTests
    {
        [Fact]
        public void SerializeString_Null_Null()
        {
            var serializer = new XeMID();
            string? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}