using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class QDTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new QD();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new QD();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
