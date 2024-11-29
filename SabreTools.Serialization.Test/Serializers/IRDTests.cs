using System.IO;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
{
    public class IRDTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new IRD();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new IRD();
            Stream? actual = serializer.Serialize(null);
            Assert.Null(actual);
        }
    }
}