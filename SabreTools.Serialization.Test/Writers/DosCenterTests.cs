using System.IO;
using SabreTools.Serialization.Writers;
using Xunit;

namespace SabreTools.Serialization.Test.Writers
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
