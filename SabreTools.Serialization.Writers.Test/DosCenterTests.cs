using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class DosCenterTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new DosCenter();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new DosCenter();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
