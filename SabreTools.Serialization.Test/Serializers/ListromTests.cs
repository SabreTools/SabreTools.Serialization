using System.IO;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
{
    public class ListromTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new Listrom();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new Listrom();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}