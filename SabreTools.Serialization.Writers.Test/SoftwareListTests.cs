using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class SoftwareListTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new SoftwareList();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new SoftwareList();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}
