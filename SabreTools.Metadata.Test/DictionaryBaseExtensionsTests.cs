using SabreTools.Data.Models.Metadata;
using Xunit;

namespace SabreTools.Metadata.Test
{
    public class DictionaryBaseExtensionsTests
    {
        #region PartialEquals

        [Fact]
        public void PartialEquals_Disk_Nodumps_True()
        {
            var self = new Disk
            {
                Status = ItemStatus.Nodump,
                Name = "name",
                MD5 = string.Empty,
                SHA1 = string.Empty,
            };
            var other = new Disk
            {
                Status = ItemStatus.Nodump,
                Name = "name",
                MD5 = string.Empty,
                SHA1 = string.Empty,
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Disk_Mismatch_False()
        {
            var self = new Disk
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = string.Empty,
            };
            var other = new Disk
            {
                Name = "name",
                MD5 = string.Empty,
                SHA1 = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.False(actual);
        }

        [Fact]
        public void PartialEquals_Disk_PartialMD5_True()
        {
            var self = new Disk
            {
                Name = "XXXXXX1",
                MD5 = "XXXXXX",
                SHA1 = string.Empty,
            };
            var other = new Disk
            {
                Name = "XXXXXX2",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Disk_PartialSHA1_True()
        {
            var self = new Disk
            {
                Name = "XXXXXX1",
                MD5 = string.Empty,
                SHA1 = "XXXXXX",
            };
            var other = new Disk
            {
                Name = "XXXXXX2",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Disk_FullMatch_True()
        {
            var self = new Disk
            {
                Name = "XXXXXX1",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
            };
            var other = new Disk
            {
                Name = "XXXXXX2",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Media_Mismatch_False()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = string.Empty,
                SHA256 = "XXXXXX",
                SpamSum = string.Empty,
            };
            var other = new Media
            {
                Name = "name",
                MD5 = string.Empty,
                SHA1 = "XXXXXX",
                SHA256 = string.Empty,
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.False(actual);
        }

        [Fact]
        public void PartialEquals_Media_PartialMD5_True()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SpamSum = string.Empty,
            };
            var other = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Media_PartialSHA1_True()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = string.Empty,
                SHA1 = "XXXXXX",
                SHA256 = string.Empty,
                SpamSum = string.Empty,
            };
            var other = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Media_PartialSHA256_True()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = "XXXXXX",
                SpamSum = string.Empty,
            };
            var other = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Media_PartialSpamSum_True()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SpamSum = "XXXXXX",
            };
            var other = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Media_FullMatch_True()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };
            var other = new Media
            {
                Name = "name",
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_Nodumps_True()
        {
            var self = new Rom
            {
                Status = ItemStatus.Nodump,
                Name = "name",
                Size = 12345,
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
                [Rom.SpamSumKey] = string.Empty,
            };
            var other = new Rom
            {
                Status = ItemStatus.Nodump,
                Name = "name",
                Size = 12345,
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
                [Rom.SpamSumKey] = string.Empty,
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_Mismatch_False()
        {
            var self = new Rom
            {
                Name = "name",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "name",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.False(actual);
        }

        [Fact]
        public void PartialEquals_Rom_NoSelfSize_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_NoOtherSize_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialCRC16_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialCRC_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialCRC64_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialMD2_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialMD4_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialMD5_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialRIPEMD128_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialRIPEMD160_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialSHA1_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialSHA256_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialSHA384_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialSHA512_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_PartialSpamSum_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        [Fact]
        public void PartialEquals_Rom_FullMatch_True()
        {
            var self = new Rom
            {
                Name = "XXXXXX1",
                Size = 12345,
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
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
        }

        #endregion
    }
}
