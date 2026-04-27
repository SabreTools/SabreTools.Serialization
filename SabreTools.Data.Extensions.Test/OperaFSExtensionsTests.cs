using SabreTools.Data.Models.OperaFS;
using SabreTools.Numerics;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class OperaFSExtensionsTests
    {
        [Fact]
        public void DirectoryDescriptor_EqualsExactly_Empty()
        {
            var dir1 = new DirectoryDescriptor();
            var dir2 = new DirectoryDescriptor();
            short actual = dir1.EqualsExactly(dir2);
            Assert.True(actual);
        }

        [Fact]
        public void DirectoryRecord_EqualsExactly_Empty()
        {
            var dr1 = new DirectoryRecord();
            var dr2 = new DirectoryRecord();
            short actual = dr1.EqualsExactly(dr2);
            Assert.True(actual);
        }
    }
}
