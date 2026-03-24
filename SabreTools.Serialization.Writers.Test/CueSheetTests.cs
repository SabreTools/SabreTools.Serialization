using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class CueSheetTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new CueSheet();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new CueSheet();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
