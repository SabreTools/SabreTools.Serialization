using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class XMIDTests
    {
        [Fact]
        public void SerializeString_Null_Null()
        {
            var serializer = new XMID();
            string? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}
