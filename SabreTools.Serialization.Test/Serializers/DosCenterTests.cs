using System.IO;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
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
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}