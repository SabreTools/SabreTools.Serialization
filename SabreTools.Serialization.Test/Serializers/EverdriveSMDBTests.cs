using System.IO;
using SabreTools.Serialization.Serializers;
using Xunit;

namespace SabreTools.Serialization.Test.Serializers
{
    public class EverdriveSMDBTests
    {
        [Fact]
        public void SerializeArray_Null_Null()
        {
            var serializer = new EverdriveSMDB();
            byte[]? actual = serializer.SerializeArray(null);
            Assert.Null(actual);
        }

        [Fact]
        public void SerializeStream_Null_Null()
        {
            var serializer = new EverdriveSMDB();
            Stream? actual = serializer.SerializeStream(null);
            Assert.Null(actual);
        }
    }
}