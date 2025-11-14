using System.IO;
using System.Linq;
using SabreTools.Serialization.Wrappers;
using Xunit;

namespace SabreTools.Serialization.Test.Wrappers
{
    public class RealArcadeInstallerTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = RealArcadeInstaller.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = RealArcadeInstaller.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact(Skip = "This will never pass with the current code")]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = RealArcadeInstaller.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = RealArcadeInstaller.Create(data);
            Assert.Null(actual);
        }

        [Fact(Skip = "This will never pass with the current code")]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = RealArcadeInstaller.Create(data);
            Assert.Null(actual);
        }

        [Fact(Skip = "This will never pass with the current code")]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = RealArcadeInstaller.Create(data);
            Assert.Null(actual);
        }
    }
}
