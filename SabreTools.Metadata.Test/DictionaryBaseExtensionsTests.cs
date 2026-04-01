using SabreTools.Data.Models.Metadata;
using Xunit;

namespace SabreTools.Metadata.Test
{
    public class DictionaryBaseExtensionsTests
    {
        #region EqualTo

        [Fact]
        public void EqualTo_MismatchedTypes_False()
        {
            DictionaryBase self = new Disk();
            DictionaryBase other = new Rom();

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Disk_Nodumps_True()
        {
            DictionaryBase self = new Disk
            {
                [Disk.StatusKey] = "nodump",
                Name = "XXXXXX",
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = string.Empty,
            };
            DictionaryBase other = new Disk
            {
                [Disk.StatusKey] = "NODUMP",
                Name = "XXXXXX",
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = string.Empty,
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Disk_Mismatch_False()
        {
            DictionaryBase self = new Disk
            {
                Name = "XXXXXX",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = string.Empty,
            };
            DictionaryBase other = new Disk
            {
                Name = "XXXXXX",
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Disk_PartialMD5_True()
        {
            DictionaryBase self = new Disk
            {
                Name = "XXXXXX1",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = string.Empty,
            };
            DictionaryBase other = new Disk
            {
                Name = "XXXXXX2",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Disk_PartialSHA1_True()
        {
            DictionaryBase self = new Disk
            {
                Name = "XXXXXX1",
                [Disk.MD5Key] = string.Empty,
                [Disk.SHA1Key] = "XXXXXX",
            };
            DictionaryBase other = new Disk
            {
                Name = "XXXXXX2",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Disk_FullMatch_True()
        {
            DictionaryBase self = new Disk
            {
                Name = "XXXXXX1",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };
            DictionaryBase other = new Disk
            {
                Name = "XXXXXX2",
                [Disk.MD5Key] = "XXXXXX",
                [Disk.SHA1Key] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Media_Mismatch_False()
        {
            DictionaryBase self = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = string.Empty,
            };
            DictionaryBase other = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Media_PartialMD5_True()
        {
            DictionaryBase self = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = string.Empty,
            };
            DictionaryBase other = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Media_PartialSHA1_True()
        {
            DictionaryBase self = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = string.Empty,
            };
            DictionaryBase other = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Media_PartialSHA256_True()
        {
            DictionaryBase self = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = string.Empty,
            };
            DictionaryBase other = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Media_PartialSpamSum_True()
        {
            DictionaryBase self = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = string.Empty,
                [Media.SHA1Key] = string.Empty,
                [Media.SHA256Key] = string.Empty,
                [Media.SpamSumKey] = "XXXXXX",
            };
            DictionaryBase other = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Media_FullMatch_True()
        {
            DictionaryBase self = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };
            DictionaryBase other = new Media
            {
                Name = "XXXXXX",
                [Media.MD5Key] = "XXXXXX",
                [Media.SHA1Key] = "XXXXXX",
                [Media.SHA256Key] = "XXXXXX",
                [Media.SpamSumKey] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_Nodumps_True()
        {
            DictionaryBase self = new Rom
            {
                [Rom.StatusKey] = "nodump",
                Name = "XXXXXX",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                [Rom.StatusKey] = "NODUMP",
                Name = "XXXXXX",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_Mismatch_False()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Rom_NoSelfSize_True()
        {
            DictionaryBase self = new Rom
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_NoOtherSize_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialCRC16_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialCRC_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialCRC64_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialMD2_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialMD4_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialMD5_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialRIPEMD128_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialRIPEMD160_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialSHA1_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialSHA256_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialSHA384_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialSHA512_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_PartialSpamSum_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Rom_FullMatch_True()
        {
            DictionaryBase self = new Rom
            {
                Name = "XXXXXX1",
                [Rom.SizeKey] = 12345,
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
            DictionaryBase other = new Rom
            {
                Name = "XXXXXX2",
                [Rom.SizeKey] = 12345,
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

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Other_BothEmpty_True()
        {
            DictionaryBase self = new Sample();
            DictionaryBase other = new Sample();

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        [Fact]
        public void EqualTo_Other_MismatchedCount_False()
        {
            DictionaryBase self = new Sample
            {
                ["KEY1"] = "XXXXXX",
            };
            DictionaryBase other = new Sample
            {
                ["KEY1"] = "XXXXXX",
                ["KEY2"] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Other_MismatchedKeys_False()
        {
            DictionaryBase self = new Sample
            {
                ["KEY1"] = "XXXXXX",
            };
            DictionaryBase other = new Sample
            {
                ["KEY2"] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Other_MismatchedValues_False()
        {
            DictionaryBase self = new Sample
            {
                ["KEY1"] = "XXXXXX",
            };
            DictionaryBase other = new Sample
            {
                ["KEY1"] = "ZZZZZZ",
            };

            bool actual = self.EqualTo(other);
            Assert.False(actual);
        }

        [Fact]
        public void EqualTo_Other_Matching_True()
        {
            DictionaryBase self = new Sample
            {
                ["KEY1"] = "XXXXXX",
            };
            DictionaryBase other = new Sample
            {
                ["KEY1"] = "XXXXXX",
            };

            bool actual = self.EqualTo(other);
            Assert.True(actual);
        }

        #endregion
    }
}
