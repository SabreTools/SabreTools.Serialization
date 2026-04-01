using SabreTools.Data.Models.ZArchive;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class ZArchiveExtensionsTests
    {
        [Fact]
        public void EntryAtOffset_Invalid()
        {
            NameTable nt = new NameTable();
            uint offset = 0;
            NameEntry? expected = null;
            var actual = nt.EntryAtOffset(offset);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsFile_False()
        {
            var de = new DirectoryEntry();
            bool expected = false;
            bool actual = de.IsFile();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsDirectory_True()
        {
            var de = new DirectoryEntry();
            bool expected = true;
            bool actual = de.IsDirectory();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetName_Invalid()
        {
            var de = new DirectoryEntry();
            NameTable nt = new NameTable();
            string? expected = null;
            string? actual = de.GetName(nt);
            Assert.Equal(expected, actual);
        }
    }
}
