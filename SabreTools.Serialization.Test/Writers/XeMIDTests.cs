using SabreTools.Serialization.Writers;
using Xunit;

namespace SabreTools.Serialization.Test.Writers
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