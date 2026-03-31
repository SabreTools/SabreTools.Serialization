using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class OfflineListTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new OfflineList();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new OfflineList();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
