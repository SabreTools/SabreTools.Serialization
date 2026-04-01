using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class NoIntroDatabaseTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new NoIntroDatabase();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new NoIntroDatabase();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}
