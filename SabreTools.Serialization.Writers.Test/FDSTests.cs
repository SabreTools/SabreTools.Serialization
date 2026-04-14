using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class FDSTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new FDS();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new FDS();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
