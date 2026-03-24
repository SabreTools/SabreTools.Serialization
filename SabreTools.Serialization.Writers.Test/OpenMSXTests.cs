using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class OpenMSXTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new OpenMSX();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new OpenMSX();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}
