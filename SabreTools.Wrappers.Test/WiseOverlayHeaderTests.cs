using System.IO;
using System.Linq;
using SabreTools.Data.Models.WiseInstaller;
using SabreTools.Data.Models.WiseInstaller.Actions;
using SabreTools.IO.Compression.Deflate;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class WiseOverlayHeaderTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = WiseOverlayHeader.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = WiseOverlayHeader.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = WiseOverlayHeader.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = WiseOverlayHeader.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = WiseOverlayHeader.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = WiseOverlayHeader.Create(data);
            Assert.Null(actual);
        }

        #region ExtractFile

        [Fact]
        public void ExtractFile_CustomDialogSet_InvalidFilenameCharacters()
        {
            // Deflated "Install under NT?" followed by its CRC-32
            byte[] data =
            [
                0xF3, 0xCC, 0x2B, 0x2E, 0x49, 0xCC, 0xC9, 0x51, 0x28, 0xCD, 0x4B, 0x49,
                0x2D, 0x52, 0xF0, 0x0B, 0xB1, 0x07, 0x00, 0x74, 0x09, 0x90, 0x8A,
            ];
            var header = new WiseOverlayHeader(new OverlayHeader(), new MemoryStream(data));
            var obj = new CustomDialogSet
            {
                DeflateStart = 0,
                DeflateEnd = (uint)data.Length,
                InflatedSize = 17,
                DisplayVariable = "DISPLAY",
                Name = "Install under NT?",
            };

            string outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(outputDirectory);
            try
            {
                var actual = header.ExtractFile(obj, outputDirectory, includeDebug: false);
                Assert.Equal(ExtractionStatus.GOOD, actual);
                Assert.Single(Directory.GetFiles(outputDirectory));
            }
            finally
            {
                Directory.Delete(outputDirectory, recursive: true);
            }
        }

        #endregion
    }
}
