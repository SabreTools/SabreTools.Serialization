using SabreTools.Data.Models.ZArchive;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class ZArchiveExtensionsTests
    {
        [Fact]
        public void GetName_Null()
        {
            var de = new DirectoryEntry();
            NameTable nt = new NameTable();
            string? expected = null;
            string? actual = de.GetName(nt);
            Assert.Equal(expected, actual);
        }
    }
}
