using System.IO;
using System.Linq;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class PortableExecutableTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = PortableExecutable.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = PortableExecutable.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = PortableExecutable.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = PortableExecutable.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = PortableExecutable.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = PortableExecutable.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ExtractWiseOverlay_NotWise_FileNotKeptOpen()
        {
            // Any executable that is not a Wise installer, here a copy of this test assembly
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.Copy(typeof(PortableExecutableTests).Assembly.Location, path);

            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var pe = PortableExecutable.Create(stream);
                    Assert.NotNull(pe);
                    Assert.False(pe.ExtractWiseOverlay(outputDirectory, includeDebug: false));
                }

                // Opening the file exclusively fails while another handle to it is still open
                using var exclusive = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                Assert.True(exclusive.CanRead);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
