using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class Atari7800CartTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new Atari7800Cart();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new Atari7800Cart();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
