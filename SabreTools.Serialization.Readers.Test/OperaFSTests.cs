using System;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Models.OperaFS;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class OperaFSTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new OperaFS();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new OperaFS();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new OperaFS();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new OperaFS();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new OperaFS();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new OperaFS();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void MultipleBlocks_AllBlocksRead()
        {
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build()));

            Assert.NotNull(volume);

            // The root directory, "Sub" and their copies have 2 blocks each, "Deep" and "Other" have 1 block
            Assert.Equal(new uint[] { 1, 2, 3, 4, 10, 11, 12, 13, 14, 15, 16 }, volume.Directories.Keys.Order());
            Assert.Equal(new[] { "Sub", "Other" }, volume.Directories[16].DirectoryRecords.Select(GetFilename));
            Assert.Equal(new[] { "Deep", "LAST.BIN" }, volume.Directories[13].DirectoryRecords.Select(GetFilename));
            Assert.Equal("DEEP.BIN", GetFilename(Assert.Single(volume.Directories[10].DirectoryRecords)));
        }

        [Fact]
        public void MultipleBlocks_AvatarsShareBlocks()
        {
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build()));

            Assert.NotNull(volume);
            Assert.Same(volume.Directories[15], volume.Directories[1]);
            Assert.Same(volume.Directories[16], volume.Directories[2]);
            Assert.Same(volume.Directories[12], volume.Directories[3]);
            Assert.Same(volume.Directories[13], volume.Directories[4]);
        }

        [Fact]
        public void NextBlockLoop_Stops()
        {
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build(subNextBlock: 0)));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 2, 3, 11, 12, 14, 15, 16 }, volume.Directories.Keys.Order());
        }

        [Fact]
        public void NextBlockPastEnd_OtherBlocksRead()
        {
            // The image ends before the second block of the root directory
            byte[] image = OperaFSImageBuilder.Build()[..(16 * OperaFSImageBuilder.SectorSize)];
            var volume = new OperaFS().Deserialize(new MemoryStream(image));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 15 }, volume.Directories.Keys.Order());
            Assert.Equal("README.TXT", GetFilename(Assert.Single(volume.Directories[15].DirectoryRecords)));
        }

        [Fact]
        public void NextBlockPastBlockCount_NotRead()
        {
            // Block 2 of "Sub" would be the copy of "Other"
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build(subNextBlock: 2)));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 2, 3, 11, 12, 14, 15, 16 }, volume.Directories.Keys.Order());
        }

        [Fact]
        public void AvatarWithMoreBlocks_BlocksRead()
        {
            // "Sub" ends after its first block, its copy has 2 blocks
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build(subNextBlock: -1, subCopyNextBlock: 1)));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 2, 3, 4, 11, 12, 14, 15, 16 }, volume.Directories.Keys.Order());
            Assert.NotSame(volume.Directories[12], volume.Directories[3]);
            Assert.Equal(new[] { "Deep", "LAST.BIN" }, volume.Directories[4].DirectoryRecords.Select(GetFilename));
        }

        [Fact]
        public void AvatarOfAnotherDirectory_FirstKept()
        {
            // The copy of "Other" is recorded at the copy of "Sub"
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build(otherCopySector: 3)));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 2, 3, 4, 10, 11, 12, 13, 15, 16 }, volume.Directories.Keys.Order());
            Assert.Same(volume.Directories[12], volume.Directories[3]);
        }

        [Fact]
        public void DirectoryLoop_Skipped()
        {
            // "Deep" is recorded at the second block of "Sub"
            var volume = new OperaFS().Deserialize(new MemoryStream(OperaFSImageBuilder.Build(deepSector: 13)));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 2, 3, 4, 11, 12, 13, 14, 15, 16 }, volume.Directories.Keys.Order());
        }

        [Fact]
        public void UnreadableBlock_OtherBlocksRead()
        {
            // The second block of the root directory is all 0xFF
            byte[] image = OperaFSImageBuilder.Build();
            image.AsSpan(16 * OperaFSImageBuilder.SectorSize, OperaFSImageBuilder.SectorSize).Fill(0xFF);
            var volume = new OperaFS().Deserialize(new MemoryStream(image));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 15 }, volume.Directories.Keys.Order());
        }

        [Fact]
        public void UnreadableDirectory_Skipped()
        {
            // The first block of "Sub" is all 0xFF
            byte[] image = OperaFSImageBuilder.Build();
            image.AsSpan(12 * OperaFSImageBuilder.SectorSize, OperaFSImageBuilder.SectorSize).Fill(0xFF);
            var volume = new OperaFS().Deserialize(new MemoryStream(image));

            Assert.NotNull(volume);
            Assert.Equal(new uint[] { 1, 2, 11, 14, 15, 16 }, volume.Directories.Keys.Order());
        }

        private static string GetFilename(DirectoryRecord record)
            => Encoding.UTF8.GetString(record.Filename).TrimEnd('\0');
    }
}
