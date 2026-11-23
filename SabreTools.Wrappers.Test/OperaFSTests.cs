using System;
using System.IO;
using System.Linq;
using SabreTools.Serialization.Readers.Test;
using Xunit;

namespace SabreTools.Wrappers.Test
{
    public class OperaFSTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var actual = OperaFS.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var actual = OperaFS.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var actual = OperaFS.Create(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var actual = OperaFS.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var actual = OperaFS.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var actual = OperaFS.Create(data);
            Assert.Null(actual);
        }

        [Fact]
        public void MultipleBlocks_Extract()
        {
            var wrapper = OperaFS.Create(new MemoryStream(OperaFSImageBuilder.Build()));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal(5, Directory.GetFiles(output, "*", SearchOption.AllDirectories).Length);
                Assert.Equal("hello world", File.ReadAllText(Path.Combine(output, "README.TXT")));
                Assert.Equal("data", File.ReadAllText(Path.Combine(output, "Sub", "DATA.BIN")));
                Assert.Equal("last", File.ReadAllText(Path.Combine(output, "Sub", "LAST.BIN")));
                Assert.Equal("deep", File.ReadAllText(Path.Combine(output, "Sub", "Deep", "DEEP.BIN")));
                Assert.Equal("other", File.ReadAllText(Path.Combine(output, "Other", "OTHER.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void NextBlockLoop_Extract()
        {
            var wrapper = OperaFS.Create(new MemoryStream(OperaFSImageBuilder.Build(subNextBlock: 0)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal("data", File.ReadAllText(Path.Combine(output, "Sub", "DATA.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void NextBlockPastBlockCount_NotExtracted()
        {
            // Block 2 of "Sub" would be the copy of "Other"
            var wrapper = OperaFS.Create(new MemoryStream(OperaFSImageBuilder.Build(subNextBlock: 2)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.True(File.Exists(Path.Combine(output, "Sub", "DATA.BIN")));
                Assert.True(File.Exists(Path.Combine(output, "Other", "OTHER.BIN")));
                Assert.False(File.Exists(Path.Combine(output, "Sub", "OTHER.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void LastBlockBeforeBlockCount_NotFollowed()
        {
            // "Sub" has 3 blocks in its entry and ends after its second block, and the sector before it is "Other"
            var wrapper = OperaFS.Create(new MemoryStream(OperaFSImageBuilder.Build(subBlockCount: 3)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.True(File.Exists(Path.Combine(output, "Sub", "LAST.BIN")));
                Assert.True(File.Exists(Path.Combine(output, "Other", "OTHER.BIN")));
                Assert.False(File.Exists(Path.Combine(output, "Sub", "OTHER.BIN")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void NextBlockPastEnd_OtherFilesExtracted()
        {
            // The image ends before the second block of the root directory
            byte[] image = OperaFSImageBuilder.Build()[..(16 * OperaFSImageBuilder.SectorSize)];
            var wrapper = OperaFS.Create(new MemoryStream(image));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal("hello world", File.ReadAllText(Path.Combine(output, "README.TXT")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void DirectoryLoop_ExtractedOnce()
        {
            // "Deep" is recorded at the second block of "Sub", so it holds "Deep" and "LAST.BIN"
            var wrapper = OperaFS.Create(new MemoryStream(OperaFSImageBuilder.Build(deepSector: 13)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal("last", File.ReadAllText(Path.Combine(output, "Sub", "Deep", "LAST.BIN")));
                Assert.False(Directory.Exists(Path.Combine(output, "Sub", "Deep", "Deep")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void EmptyDirectory_Extract()
        {
            // "Deep" has no entries, and the rest of its block is filler
            var wrapper = OperaFS.Create(new MemoryStream(OperaFSImageBuilder.Build(emptyDeep: true)));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Empty(Directory.GetFileSystemEntries(Path.Combine(output, "Sub", "Deep")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }

        [Fact]
        public void UnreadableDirectory_OtherFilesExtracted()
        {
            // The first block of "Sub" is all 0xFF
            byte[] image = OperaFSImageBuilder.Build();
            image.AsSpan(12 * OperaFSImageBuilder.SectorSize, OperaFSImageBuilder.SectorSize).Fill(0xFF);
            var wrapper = OperaFS.Create(new MemoryStream(image));
            Assert.NotNull(wrapper);

            string output = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Directory.CreateDirectory(output);
                Assert.True(wrapper.Extract(output, false));
                Assert.Equal("hello world", File.ReadAllText(Path.Combine(output, "README.TXT")));
                Assert.Equal("other", File.ReadAllText(Path.Combine(output, "Other", "OTHER.BIN")));
                Assert.False(Directory.Exists(Path.Combine(output, "Sub")));
            }
            finally
            {
                if (Directory.Exists(output))
                    Directory.Delete(output, true);
            }
        }
    }
}
