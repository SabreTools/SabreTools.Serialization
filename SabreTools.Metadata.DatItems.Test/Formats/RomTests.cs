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
            Assert.Null(self.CRC32);
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
                CRC16 = "crc16",
                CRC32 = "crc32",
                CRC64 = "crc64",
                MD2 = "md2",
                MD4 = "md4",
                MD5 = "md5",
                RIPEMD128 = "ripemd128",
                RIPEMD160 = "ripemd160",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SHA384 = "sha384",
                SHA512 = "sha512",
                SpamSum = "spamsum"
            };

            self.FillMissingInformation(other);

            Assert.Equal("crc16", self.CRC16);
            Assert.Equal("crc32", self.CRC32);
            Assert.Equal("crc64", self.CRC64);
            Assert.Equal("md2", self.MD2);
            Assert.Equal("md4", self.MD4);
            Assert.Equal("md5", self.MD5);
            Assert.Equal("ripemd128", self.RIPEMD128);
            Assert.Equal("ripemd160", self.RIPEMD160);
            Assert.Equal("sha1", self.SHA1);
            Assert.Equal("sha256", self.SHA256);
            Assert.Equal("sha384", self.SHA384);
            Assert.Equal("sha512", self.SHA512);
            Assert.Equal("spamsum", self.SpamSum);
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
                CRC16 = "crc16",
                CRC32 = string.Empty,
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
        public void HasHashes_CRC32_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC32 = "crc32",
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
                CRC32 = string.Empty,
                CRC64 = "crc64",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = "md2",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = "md4",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = "md5",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = "ripemd128",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = "ripemd160",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = "sha1",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = "sha256",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = "sha384",
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
                CRC32 = string.Empty,
                CRC64 = string.Empty,
                MD2 = string.Empty,
                MD4 = string.Empty,
                MD5 = string.Empty,
                RIPEMD128 = string.Empty,
                RIPEMD160 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SHA384 = string.Empty,
                SHA512 = "sha512",
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
                CRC32 = string.Empty,
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
                SpamSum = "spamsum"
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_All_True()
        {
            Rom self = new Rom
            {
                CRC16 = "crc16",
                CRC32 = "crc32",
                CRC64 = "crc64",
                MD2 = "md2",
                MD4 = "md4",
                MD5 = "md5",
                RIPEMD128 = "ripemd128",
                RIPEMD160 = "ripemd160",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SHA384 = "sha384",
                SHA512 = "sha512",
                SpamSum = "spamsum"
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
                CRC16 = "crc16",
                CRC32 = "crc32",
                CRC64 = "crc64",
                MD2 = "md2",
                MD4 = "md4",
                MD5 = "md5",
                RIPEMD128 = "ripemd128",
                RIPEMD160 = "ripemd160",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SHA384 = "sha384",
                SHA512 = "sha512",
                SpamSum = "spamsum"
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
                CRC32 = string.Empty,
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
        public void HasZeroHash_ZeroCRC32_True()
        {
            Rom self = new Rom
            {
                CRC16 = string.Empty,
                CRC32 = HashType.CRC32.ZeroString,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = string.Empty,
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
                CRC32 = HashType.CRC32.ZeroString,
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
        [InlineData(ItemKey.CRC32, false, false, "DEADBEEF")]
        [InlineData(ItemKey.CRC32, false, true, "DEADBEEF")]
        [InlineData(ItemKey.CRC32, true, false, "deadbeef")]
        [InlineData(ItemKey.CRC32, true, true, "deadbeef")]
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
                CRC32 = "DEADBEEF",
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
