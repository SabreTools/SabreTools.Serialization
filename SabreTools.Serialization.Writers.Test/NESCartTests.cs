using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class NESCartTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new NESCart();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new NESCart();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
