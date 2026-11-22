using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Serialization.Readers.Test;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class UDFTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = UDF.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = UDF.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = UDF.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = UDF.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = UDF.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = UDF.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void DirectoryLoop_Extract()
        {
            var wrapper = UDF.Create(new MemoryStream(UDFImageBuilder.Build(subLoop: true)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal(UDFImageBuilder.DataBinLength, new FileInfo(Path.Combine(output, "Sub", "data.bin")).Length);
                Assert.True(Directory.Exists(Path.Combine(output, "Sub", "loop")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void ParentDirectoryName_NotExtracted()
        {
            // "Sub" is named "..", so "data.bin" would be written next to the output directory
            var wrapper = UDF.Create(new MemoryStream(UDFImageBuilder.Build(subName: "..")));
            Assert.NotNull(wrapper);

            string parent = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string output = Path.Combine(parent, "output");
            try
            {
                Directory.CreateDirectory(output);
                Assert.False(wrapper.Extract(output, false));
                Assert.False(File.Exists(Path.Combine(parent, "data.bin")));
                Assert.True(File.Exists(Path.Combine(output, "README.TXT")));
            }
            finally
            {
                if (Directory.Exists(parent))
                    Directory.Delete(parent, true);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void RecordedPastEnd_OtherFilesExtracted(bool pastPartition)
        {
            // "Media" is recorded in a partition block past the end of the image, or in a block past the end of the partition
            var wrapper = UDF.Create(new MemoryStream(UDFImageBuilder.Build(mediaPastEnd: !pastPartition, mediaPastPartition: pastPartition)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.False(wrapper.Extract(output, false));
                Assert.True(File.Exists(Path.Combine(output, "README.TXT")));
                Assert.Equal(UDFImageBuilder.DataBinLength, new FileInfo(Path.Combine(output, "Sub", "data.bin")).Length);
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void StreamAtOffset_VolumeAtOffsetExtracted()
        {
            // Another volume comes first, so reading from the start of the stream would find "Outer" instead of "Inner"
            var stream = new MemoryStream([.. UDFImageBuilder.Build(subName: "Outer"), .. UDFImageBuilder.Build(subName: "Inner")]);
            stream.Position = UDFImageBuilder.Sectors * UDFImageBuilder.SectorSize;

            var wrapper = UDF.Create(stream);
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal(UDFImageBuilder.DataBinLength, new FileInfo(Path.Combine(output, "Inner", "data.bin")).Length);
                Assert.False(Directory.Exists(Path.Combine(output, "Outer")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Theory]
        [InlineData(UDFLayout.Sparable)]
        [InlineData(UDFLayout.Virtual)]
        [InlineData(UDFLayout.Virtual150)]
        public void PartitionLayout_Extract(UDFLayout layout)
        {
            var wrapper = UDF.Create(new MemoryStream(UDFImageBuilder.Build(layout: layout)));
            Assert.NotNull(wrapper);

            byte[] data = UDFImageBuilder.Contents(UDFImageBuilder.DataBinLength, UDFImageBuilder.DataBinByte);
            byte[] big = UDFImageBuilder.Contents(UDFImageBuilder.BigBinLength, UDFImageBuilder.BigBinByte);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal("hello world", File.ReadAllText(Path.Combine(output, "README.TXT")));
                Assert.Equal(data, File.ReadAllBytes(Path.Combine(output, "Sub", "data.bin")));
                Assert.Equal(big, File.ReadAllBytes(Path.Combine(output, "Sub", "big.bin")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Theory]
        [InlineData(UDFLayout.Type1)]
        [InlineData(UDFLayout.Sparable)]
        [InlineData(UDFLayout.Virtual)]
        [InlineData(UDFLayout.Virtual150)]
        public void PartitionLayout_PrintInformation(UDFLayout layout)
        {
            var wrapper = UDF.Create(new MemoryStream(UDFImageBuilder.Build(layout: layout)));
            Assert.NotNull(wrapper);

            var builder = new StringBuilder();
            wrapper.PrintInformation(builder, false);
            string text = builder.ToString();

            Assert.Equal(layout == UDFLayout.Sparable, text.Contains("Sparing Table (Partition Reference Number 0):"));
            Assert.Equal(layout == UDFLayout.Virtual || layout == UDFLayout.Virtual150, text.Contains("Virtual Allocation Table:"));
        }

        [Fact]
        public void StreamAtOffset_WrapperFactoryUDF()
        {
            var stream = new MemoryStream([.. new byte[3 * UDFImageBuilder.SectorSize], .. UDFImageBuilder.Build()]);
            stream.Position = 3 * UDFImageBuilder.SectorSize;

            var wrapper = WrapperFactory.CreateDiscImageWrapper(stream);
            Assert.IsType<UDF>(wrapper);
        }
    }
}
