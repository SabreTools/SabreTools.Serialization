using System.IO;
using Xunit;

namespace SabreTools.Serialization.Writers.Test
{
    public class XDVDFSTests
    {
        [Fact]
        public void SerializeFile_Null_False()
        {
            var serializer = new XDVDFS();
            bool actual = serializer.SerializeFile(null, null);
            Assert.False(actual);
        }
    }
}
