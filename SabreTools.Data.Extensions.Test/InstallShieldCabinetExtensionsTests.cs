using SabreTools.Data.Models.InstallShieldCabinet;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class InstallShieldCabinetExtensionsTests
    {
        #region IsCompressed

        [Fact]
        public void IsCompressed_Null_True()
        {
            FileDescriptor? fd = null;
            bool actual = fd.IsCompressed();
            Assert.True(actual);
        }

        [Fact]
        public void IsCompressed_Compressed_True()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = FileFlags.FILE_COMPRESSED };
            bool actual = fd.IsCompressed();
            Assert.True(actual);
        }

        [Fact]
        public void IsCompressed_NotCompressed_False()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = 0 };
            bool actual = fd.IsCompressed();
            Assert.False(actual);
        }

        #endregion

        #region IsInvalid

        [Fact]
        public void IsInvalid_Null_True()
        {
            FileDescriptor? fd = null;
            bool actual = fd.IsInvalid();
            Assert.True(actual);
        }

        [Fact]
        public void IsInvalid_Invalid_True()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = FileFlags.FILE_INVALID };
            bool actual = fd.IsInvalid();
            Assert.True(actual);
        }

        [Fact]
        public void IsInvalid_Valid_False()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = 0 };
            bool actual = fd.IsInvalid();
            Assert.False(actual);
        }

        #endregion

        #region IsObfuscated

        [Fact]
        public void IsObfuscated_Null_False()
        {
            FileDescriptor? fd = null;
            bool actual = fd.IsObfuscated();
            Assert.False(actual);
        }

        [Fact]
        public void IsObfuscated_Obfuscated_True()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = FileFlags.FILE_OBFUSCATED };
            bool actual = fd.IsObfuscated();
            Assert.True(actual);
        }

        [Fact]
        public void IsObfuscated_NotObfuscated_False()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = 0 };
            bool actual = fd.IsObfuscated();
            Assert.False(actual);
        }

        #endregion

        #region IsSplit

        [Fact]
        public void IsSplit_Null_False()
        {
            FileDescriptor? fd = null;
            bool actual = fd.IsSplit();
            Assert.False(actual);
        }

        [Fact]
        public void IsSplit_Split_True()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = FileFlags.FILE_SPLIT };
            bool actual = fd.IsSplit();
            Assert.True(actual);
        }

        [Fact]
        public void IsSplit_NotSplitFalse()
        {
            FileDescriptor? fd = new FileDescriptor { Flags = 0 };
            bool actual = fd.IsSplit();
            Assert.False(actual);
        }

        #endregion

        #region GetMajorVersion

        [Fact]
        public void GetMajorVersion_NullCabinet_NegativeOne()
        {
            Cabinet? cabinet = null;
            int actual = cabinet.GetMajorVersion();
            Assert.Equal(-1, actual);
        }

        [Fact]
        public void GetMajorVersion_NullHeader_NegativeOne()
        {
            CommonHeader? cabinet = null;
            int actual = cabinet.GetMajorVersion();
            Assert.Equal(-1, actual);
        }

        [Theory]
        [InlineData(0x00000000, 0)]
        [InlineData(0x00000004, 4)]
        [InlineData(0x01000000, 0)]
        [InlineData(0x01004000, 4)]
        [InlineData(0x02000000, 0)]
        [InlineData(0x02000190, 4)]
        [InlineData(0x04000000, 0)]
        [InlineData(0x04000190, 4)]
        public void GetMajorVersionTest(uint version, int expected)
        {
            CommonHeader? cabinet = new CommonHeader { Version = version };
            int actual = cabinet.GetMajorVersion();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
