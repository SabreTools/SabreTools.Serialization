using System.IO;
using SabreTools.Serialization.Writers;
using Xunit;

namespace SabreTools.Serialization.Test.Writers
{
    public class ListxmlTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new Listxml();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new Listxml();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}