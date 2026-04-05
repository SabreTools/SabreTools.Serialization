using SabreTools.Hashing;
using Xunit;

namespace SabreTools.Metadata.DatItems.Formats.Test
{
    public class RomTests
    {
        #region FillMissingInformation

        [Fact]
        public void FillMissingInformation_BothEmpty()
        {
            Rom self = new Rom();
            Rom other = new Rom();

            self.FillMissingInformation(other);

            Assert.Null(self.CRC16);
            Assert.Null(self.CRC);
            Assert.Null(self.CRC64);
            Assert.Null(self.MD2);
            Assert.Null(self.MD4);
            Assert.Null(self.MD5);
            Assert.Null(self.RIPEMD128);
            Assert.Null(self.RIPEMD160);
            Assert.Null(self.SHA1);
            Assert.Null(self.SHA256);
            Assert.Null(self.SHA384);
            Assert.Null(self.SHA512);
            Assert.Null(self.SpamSum);
        }

        [Fact]
        public void FillMissingInformation_AllMissing()
        {
            Rom self = new Rom();

            Rom other = new Rom
            {
                CRC16 = "XXXXXX",
                CRC = "XXXXXX",
                CRC64 = "XXXXXX",
                MD2 = "XXXXXX",
                MD4 = "XXXXXX",
                MD5 = "XXXXXX",
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SHA384 = "XXXXXX",
                SHA512 = "XXXXXX",
                SpamSum = "XXXXXX"
            };

            self.FillMissingInformation(other);

            Assert.Equal("XXXXXX", self.CRC16);
            Assert.Equal("XXXXXX", self.CRC);
            Assert.Equal("XXXXXX", self.CRC64);
            Assert.Equal("XXXXXX", self.MD2);
            Assert.Equal("XXXXXX", self.MD4);
            Assert.Equal("XXXXXX", self.MD5);
            Assert.Equal("XXXXXX", self.RIPEMD128);
            Assert.Equal("XXXXXX", self.RIPEMD160);
            Assert.Equal("XXXXXX", self.SHA1);
            Assert.Equal("XXXXXX", self.SHA256);
            Assert.Equal("XXXXXX", self.SHA384);
            Assert.Equal("XXXXXX", self.SHA512);
            Assert.Equal("XXXXXX", self.SpamSum);
        }

        #endregion

        #region HasHashes

        [Fact]
        public void HasHashes_NoHash_False()
        {
            Rom self = new Rom();
            bool actual = self.HasHashes();
            Assert.False(actual);
        }

        [Fact]
        public void HasHashes_CRC16_True()
        {
            Rom self = new Rom
            {
                CRC16 = "XXXXXX",
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_CRC_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = "XXXXXX",
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_CRC64_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = "XXXXXX",
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_MD2_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = "XXXXXX",
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_MD4_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = "XXXXXX",
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_MD5_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = "XXXXXX",
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_RIPEMD128_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_RIPEMD160_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = "XXXXXX",
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA1_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = "XXXXXX",
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA256_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = "XXXXXX",
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA384_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = "XXXXXX",
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA512_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = "XXXXXX",
                SpamSum = string.Empty
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SpamSum_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = "XXXXXX"
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_All_True()
        {
            Rom self = new Rom
            {
                CRC16 = "XXXXXX",
                CRC = "XXXXXX",
                CRC64 = "XXXXXX",
                MD2 = "XXXXXX",
                MD4 = "XXXXXX",
                MD5 = "XXXXXX",
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SHA384 = "XXXXXX",
                SHA512 = "XXXXXX",
                SpamSum = "XXXXXX"
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_NoHash_True()
        {
            Rom self = new Rom();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_NonZeroHash_False()
        {
            Rom self = new Rom
            {
                CRC16 = "XXXXXX",
                CRC = "XXXXXX",
                CRC64 = "XXXXXX",
                MD2 = "XXXXXX",
                MD4 = "XXXXXX",
                MD5 = "XXXXXX",
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SHA384 = "XXXXXX",
                SHA512 = "XXXXXX",
                SpamSum = "XXXXXX"
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroCRC16_True()
        {
            Rom self = new Rom
            {
                CRC16 = HashType.CRC16.ZeroString,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroCRC_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = HashType.CRC32.ZeroString,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroCRC64_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = HashType.CRC64.ZeroString,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD2_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = HashType.MD2.ZeroString,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD4_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = HashType.MD4.ZeroString,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD5_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = HashType.MD5.ZeroString,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroRIPEMD128_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = HashType.RIPEMD128.ZeroString,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroRIPEMD160_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = HashType.RIPEMD160.ZeroString,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA1_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA256_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = HashType.SHA256.ZeroString,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA384_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = HashType.SHA384.ZeroString,
                SHA512 = string.Empty,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA512_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = HashType.SHA512.ZeroString,
                SpamSum = string.Empty
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSpamSum_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = string.Empty,
                SpamSum = HashType.SpamSum.ZeroString
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroAll_True()
        {
            Rom self = new Rom
            {
                CRC16 = HashType.CRC16.ZeroString,
                CRC = HashType.CRC32.ZeroString,
                CRC64 = HashType.CRC64.ZeroString,
                MD2 = HashType.MD2.ZeroString,
                MD4 = HashType.MD4.ZeroString,
                MD5 = HashType.MD5.ZeroString,
                RIPEMD128 = HashType.RIPEMD128.ZeroString,
                RIPEMD160 = HashType.RIPEMD160.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = HashType.SHA256.ZeroString,
                SHA384 = HashType.SHA384.ZeroString,
                SHA512 = HashType.SHA512.ZeroString,
                SpamSum = HashType.SpamSum.ZeroString
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        #endregion

        // TODO: Change when Machine retrieval gets fixed
        #region GetKey

        [Theory]
        [InlineData(ItemKey.NULL, false, false, "")]
        [InlineData(ItemKey.NULL, false, true, "")]
        [InlineData(ItemKey.NULL, true, false, "")]
        [InlineData(ItemKey.NULL, true, true, "")]
        [InlineData(ItemKey.Machine, false, false, "0000000000-Machine")]
        [InlineData(ItemKey.Machine, false, true, "Machine")]
        [InlineData(ItemKey.Machine, true, false, "0000000000-machine")]
        [InlineData(ItemKey.Machine, true, true, "machine")]
        [InlineData(ItemKey.CRC16, false, false, "DEADBEEF")]
        [InlineData(ItemKey.CRC16, false, true, "DEADBEEF")]
        [InlineData(ItemKey.CRC16, true, false, "deadbeef")]
        [InlineData(ItemKey.CRC16, true, true, "deadbeef")]
        [InlineData(ItemKey.CRC, false, false, "DEADBEEF")]
        [InlineData(ItemKey.CRC, false, true, "DEADBEEF")]
        [InlineData(ItemKey.CRC, true, false, "deadbeef")]
        [InlineData(ItemKey.CRC, true, true, "deadbeef")]
        [InlineData(ItemKey.CRC64, false, false, "DEADBEEF")]
        [InlineData(ItemKey.CRC64, false, true, "DEADBEEF")]
        [InlineData(ItemKey.CRC64, true, false, "deadbeef")]
        [InlineData(ItemKey.CRC64, true, true, "deadbeef")]
        [InlineData(ItemKey.MD2, false, false, "DEADBEEF")]
        [InlineData(ItemKey.MD2, false, true, "DEADBEEF")]
        [InlineData(ItemKey.MD2, true, false, "deadbeef")]
        [InlineData(ItemKey.MD2, true, true, "deadbeef")]
        [InlineData(ItemKey.MD4, false, false, "DEADBEEF")]
        [InlineData(ItemKey.MD4, false, true, "DEADBEEF")]
        [InlineData(ItemKey.MD4, true, false, "deadbeef")]
        [InlineData(ItemKey.MD4, true, true, "deadbeef")]
        [InlineData(ItemKey.MD5, false, false, "DEADBEEF")]
        [InlineData(ItemKey.MD5, false, true, "DEADBEEF")]
        [InlineData(ItemKey.MD5, true, false, "deadbeef")]
        [InlineData(ItemKey.MD5, true, true, "deadbeef")]
        [InlineData(ItemKey.RIPEMD128, false, false, "DEADBEEF")]
        [InlineData(ItemKey.RIPEMD128, false, true, "DEADBEEF")]
        [InlineData(ItemKey.RIPEMD128, true, false, "deadbeef")]
        [InlineData(ItemKey.RIPEMD128, true, true, "deadbeef")]
        [InlineData(ItemKey.RIPEMD160, false, false, "DEADBEEF")]
        [InlineData(ItemKey.RIPEMD160, false, true, "DEADBEEF")]
        [InlineData(ItemKey.RIPEMD160, true, false, "deadbeef")]
        [InlineData(ItemKey.RIPEMD160, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA1, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA1, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA256, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA256, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA384, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA384, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA384, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA384, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA512, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA512, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA512, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA512, true, true, "deadbeef")]
        [InlineData(ItemKey.SpamSum, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, true, false, "deadbeef")]
        [InlineData(ItemKey.SpamSum, true, true, "deadbeef")]
        public void GetKeyDBTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine { Name = "Machine" };

            DatItem datItem = new Rom
            {
                CRC16 = "DEADBEEF",
                CRC = "DEADBEEF",
                CRC64 = "DEADBEEF",
                MD2 = "DEADBEEF",
                MD4 = "DEADBEEF",
                MD5 = "DEADBEEF",
                RIPEMD128 = "DEADBEEF",
                RIPEMD160 = "DEADBEEF",
                SHA1 = "DEADBEEF",
                SHA256 = "DEADBEEF",
                SHA384 = "DEADBEEF",
                SHA512 = "DEADBEEF",
                SpamSum = "DEADBEEF"
            };

            string actual = datItem.GetKey(bucketedBy, machine, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
