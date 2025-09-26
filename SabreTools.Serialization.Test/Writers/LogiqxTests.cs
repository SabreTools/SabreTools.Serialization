using System.IO;
using SabreTools.Serialization.Writers;
using Xunit;

namespace SabreTools.Serialization.Test.Writers
{
    public class LogiqxTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new Logiqx();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new Logiqx();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}