using SabreTools.Data.Models.Metadata;
using SabreTools.Hashing;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class MetadataExtensionsTests
    {
        #region ConvertToRom

        [Fact]
        public void ConvertToRom_Null_Null()
        {
            DictionaryBase? self = null;
            Rom? actual = self.ConvertToRom();
            Assert.Null(actual);
        }

        [Fact]
        public void ConvertToRom_EmptyDisk_EmptyRom()
        {
            DictionaryBase? self = new Disk();
            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(8, actual.Count);
            Assert.Equal(ItemType.Rom, actual["_type"]);
            Assert.Null(actual[Rom.NameKey]);
            Assert.Null(actual[Rom.MergeKey]);
            Assert.Null(actual[Rom.RegionKey]);
            Assert.Null(actual[Rom.StatusKey]);
            Assert.Null(actual[Rom.OptionalKey]);
            Assert.Null(actual[Rom.MD5Key]);
            Assert.Null(actual[Rom.SHA1Key]);
        }

        [Fact]
        public void ConvertToRom_FilledDisk_FilledRom()
        {
            DictionaryBase? self = new Disk
            {
                [Disk.NameKey] = "XXXXXX",
                [Disk.MergeKey] = "XXXXXX",
                [Disk.RegionKey] = "XXXXXX",
                [Disk.StatusKey] = "XXXXXX",
                [Disk.OptionalKey] = "XXXXXX",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(8, actual.Count);
            Assert.Equal(ItemType.Rom, actual["_type"]);
            Assert.Equal("XXXXXX.chd", actual[Rom.NameKey]);
            Assert.Equal("XXXXXX", actual[Rom.MergeKey]);
            Assert.Equal("XXXXXX", actual[Rom.RegionKey]);
            Assert.Equal("XXXXXX", actual[Rom.StatusKey]);
            Assert.Equal("XXXXXX", actual[Rom.OptionalKey]);
            Assert.Equal("XXXXXX", actual[Rom.MD5Key]);
            Assert.Equal("XXXXXX", actual[Rom.SHA1Key]);
        }

        [Fact]
        public void ConvertToRom_EmptyMedia_EmptyRom()
        {
            DictionaryBase? self = new Media();
            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(6, actual.Count);
            Assert.Equal(ItemType.Rom, actual["_type"]);
            Assert.Null(actual[Rom.NameKey]);
            Assert.Null(actual[Rom.MD5Key]);
            Assert.Null(actual[Rom.SHA1Key]);
            Assert.Null(actual[Rom.SHA256Key]);
            Assert.Null(actual[Rom.SpamSumKey]);
        }

        [Fact]
        public void ConvertToRom_FilledMedia_FilledRom()
        {
            DictionaryBase? self = new Media
            {
                [Media.NameKey] = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(6, actual.Count);
            Assert.Equal(ItemType.Rom, actual["_type"]);
            Assert.Equal("XXXXXX.aaruf", actual[Rom.NameKey]);
            Assert.Equal("XXXXXX", actual[Rom.MD5Key]);
            Assert.Equal("XXXXXX", actual[Rom.SHA1Key]);
            Assert.Equal("XXXXXX", actual[Rom.SHA256Key]);
            Assert.Equal("XXXXXX", actual[Rom.SpamSumKey]);
        }

        [Fact]
        public void ConvertToRom_Other_Null()
        {
            DictionaryBase? self = new Sample();
            Rom? actual = self.ConvertToRom();
            Assert.Null(actual);
        }

        #endregion

        #region ConditionalHashEquals

        [Theory]
        [InlineData(null, null, true)]
        [InlineData(new byte[0], new byte[0], true)]
        [InlineData(new byte[] { 0x01 }, new byte[0], true)]
        [InlineData(new byte[0], new byte[] { 0x01 }, true)]
        [InlineData(new byte[] { 0x01 }, new byte[] { 0x01 }, true)]
        [InlineData(new byte[] { 0x01, 0x02 }, new byte[] { 0x01 }, false)]
        [InlineData(new byte[] { 0x01 }, new byte[] { 0x01, 0x02 }, false)]
        [InlineData(new byte[] { 0x01, 0x02 }, new byte[] { 0x02, 0x01 }, false)]
        public void ConditionalHashEquals_Array(byte[]? firstHash, byte[]? secondHash, bool expected)
        {
            bool actual = MetadataExtensions.ConditionalHashEquals(firstHash, secondHash);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData("", "", true)]
        [InlineData("01", "", true)]
        [InlineData("", "01", true)]
        [InlineData("01", "01", true)]
        [InlineData("0102", "01", false)]
        [InlineData("01", "0102", false)]
        [InlineData("0102", "0201", false)]
        public void ConditionalHashEquals_String(string? firstHash, string? secondHash, bool expected)
        {
            bool actual = MetadataExtensions.ConditionalHashEquals(firstHash, secondHash);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region HashMatch

        [Fact]
        public void HashMatch_Disk_Mismatch_False()
        {
            Disk self = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = string.Empty,
            };
            Disk other = new Disk
            {
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.False(actual);
        }

        [Fact]
        public void HashMatch_Disk_PartialMD5_True()
        {
            Disk self = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = string.Empty,
            };
            Disk other = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Disk_PartialSHA1_True()
        {
            Disk self = new Disk
            {
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = "XXXXXX",
            };
            Disk other = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Disk_FullMatch_True()
        {
            Disk self = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };
            Disk other = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_Mismatch_False()
        {
            Media self = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = string.Empty,
            };
            Media other = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.False(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialMD5_True()
        {
            Media self = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = string.Empty,
            };
            Media other = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialSHA1_True()
        {
            Media self = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = string.Empty,
            };
            Media other = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialSHA256_True()
        {
            Media self = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = string.Empty,
            };
            Media other = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialSpamSum_True()
        {
            Media self = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = "XXXXXX",
            };
            Media other = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_FullMatch_True()
        {
            Media self = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };
            Media other = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_Mismatch_False()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = "XXXXXX",
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HashMatch(other);
            Assert.False(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialCRC16_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialCRC_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialCRC64_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialMD2_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialMD4_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialMD5_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialRIPEMD128_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialRIPEMD160_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA1_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA256_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA384_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA512_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = string.Empty,
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSpamSum_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = "XXXXXX",
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_FullMatch_True()
        {
            Rom self = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };
            Rom other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_Disk_NoHash_True()
        {
            DictionaryBase self = new Disk();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_NonZeroHash_False()
        {
            DictionaryBase self = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_ZeroMD5_True()
        {
            DictionaryBase self = new Disk
            {
                [Disk.MD5Key] = HashType.MD5.ZeroString,
                [Disk.SHA1Key] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_ZeroSHA1_True()
        {
            DictionaryBase self = new Disk
            {
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = HashType.SHA1.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_ZeroAll_True()
        {
            DictionaryBase self = new Disk
            {
                [Disk.MD5Key] = HashType.MD5.ZeroString,
                [Disk.SHA1Key] = HashType.SHA1.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_NoHash_True()
        {
            DictionaryBase self = new Media();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_NonZeroHash_False()
        {
            DictionaryBase self = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroMD5_True()
        {
            DictionaryBase self = new Media
            {
                [Media.MD5Key] = HashType.MD5.ZeroString,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroSHA1_True()
        {
            DictionaryBase self = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = HashType.SHA1.ZeroString,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroSHA256_True()
        {
            DictionaryBase self = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = HashType.SHA256.ZeroString,
                [Media.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroSpamSum_True()
        {
            DictionaryBase self = new Media
            {
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroAll_True()
        {
            DictionaryBase self = new Media
            {
                [Media.MD5Key] = HashType.MD5.ZeroString,
                [Media.SHA1Key] = HashType.SHA1.ZeroString,
                [Media.SHA256Key] = HashType.SHA256.ZeroString,
                [Media.SpamSumKey] = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_NoHash_True()
        {
            DictionaryBase self = new Rom();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_NonZeroHash_False()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroCRC16_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = HashType.CRC16.ZeroString,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroCRC_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = HashType.CRC32.ZeroString,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroCRC64_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = HashType.CRC64.ZeroString,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroMD2_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = HashType.MD2.ZeroString,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroMD4_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = HashType.MD4.ZeroString,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroMD5_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = HashType.MD5.ZeroString,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroRIPEMD128_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = HashType.RIPEMD128.ZeroString,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroRIPEMD160_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = HashType.RIPEMD160.ZeroString,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA1_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = HashType.SHA1.ZeroString,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA256_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = HashType.SHA256.ZeroString,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA384_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = HashType.SHA384.ZeroString,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA512_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = HashType.SHA512.ZeroString,
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSpamSum_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = string.Empty,
                [Rom.CRCKey] = string.Empty,
                [Rom.CRC64Key] = string.Empty,
                [Rom.MD2Key] = string.Empty,
                [Rom.MD4Key] = string.Empty,
                [Rom.MD5Key] = string.Empty,
                [Rom.RIPEMD128Key] = string.Empty,
                [Rom.RIPEMD160Key] = string.Empty,
                [Rom.SHA1Key] = string.Empty,
                [Rom.SHA256Key] = string.Empty,
                [Rom.SHA384Key] = string.Empty,
                [Rom.SHA512Key] = string.Empty,
                [Rom.SpamSumKey] = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroAll_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.CRC16Key] = HashType.CRC16.ZeroString,
                [Rom.CRCKey] = HashType.CRC32.ZeroString,
                [Rom.CRC64Key] = HashType.CRC64.ZeroString,
                [Rom.MD2Key] = HashType.MD2.ZeroString,
                [Rom.MD4Key] = HashType.MD4.ZeroString,
                [Rom.MD5Key] = HashType.MD5.ZeroString,
                [Rom.RIPEMD128Key] = HashType.RIPEMD128.ZeroString,
                [Rom.RIPEMD160Key] = HashType.RIPEMD160.ZeroString,
                [Rom.SHA1Key] = HashType.SHA1.ZeroString,
                [Rom.SHA256Key] = HashType.SHA256.ZeroString,
                [Rom.SHA384Key] = HashType.SHA384.ZeroString,
                [Rom.SHA512Key] = HashType.SHA512.ZeroString,
                [Rom.SpamSumKey] = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Other_False()
        {
            DictionaryBase self = new Sample();
            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        #endregion

        #region FillMissingHashes

        [Fact]
        public void FillMissingHashes_Disk_BothEmpty()
        {
            DictionaryBase self = new Disk();
            DictionaryBase other = new Disk();

            self.FillMissingHashes(other);
            Assert.Single(self);
        }

        [Fact]
        public void FillMissingHashes_Disk_AllMissing()
        {
            DictionaryBase self = new Disk();
            DictionaryBase other = new Disk
            {
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            self.FillMissingHashes(other);
        }

        [Fact]
        public void FillMissingHashes_Media_BothEmpty()
        {
            DictionaryBase self = new Media();
            DictionaryBase other = new Media();
            self.FillMissingHashes(other);
            Assert.Single(self);
        }

        [Fact]
        public void FillMissingHashes_Media_AllMissing()
        {
            DictionaryBase self = new Media();
            DictionaryBase other = new Media
            {
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            self.FillMissingHashes(other);
        }

        [Fact]
        public void FillMissingHashes_Rom_BothEmpty()
        {
            DictionaryBase self = new Rom();
            DictionaryBase other = new Rom();
            self.FillMissingHashes(other);
            Assert.Single(self);
        }

        [Fact]
        public void FillMissingHashes_Rom_AllMissing()
        {
            DictionaryBase self = new Rom();
            DictionaryBase other = new Rom
            {
                [Rom.CRC16Key] = "XXXXXX",
                [Rom.CRCKey] = "XXXXXX",
                [Rom.CRC64Key] = "XXXXXX",
                [Rom.MD2Key] = "XXXXXX",
                [Rom.MD4Key] = "XXXXXX",
                [Rom.MD5Key] = "XXXXXX",
                [Rom.RIPEMD128Key] = "XXXXXX",
                [Rom.RIPEMD160Key] = "XXXXXX",
                [Rom.SHA1Key] = "XXXXXX",
                [Rom.SHA256Key] = "XXXXXX",
                [Rom.SHA384Key] = "XXXXXX",
                [Rom.SHA512Key] = "XXXXXX",
                [Rom.SpamSumKey] = "XXXXXX",
            };

            self.FillMissingHashes(other);
        }

        #endregion
    }
}
