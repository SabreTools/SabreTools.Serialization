using SabreTools.Data.Models.Metadata;
using SabreTools.Hashing;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class MetadataExtensionsTests
    {
        #region ConvertToRom

        [Fact]
        public void ConvertToRom_EmptyDisk_EmptyRom()
        {
            var self = new Disk();
            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(ItemType.Rom, actual.ItemType);
            Assert.Null(actual.Name);
            Assert.Null(actual.Merge);
            Assert.Null(actual.Region);
            Assert.Null(actual.Status);
            Assert.Null(actual.Optional);
            Assert.Null(actual.MD5);
            Assert.Null(actual.SHA1);
        }

        [Fact]
        public void ConvertToRom_FilledDisk_FilledRom()
        {
            var self = new Disk
            {
                Name = "name",
                Merge = "merge",
                Region = "region",
                Status = ItemStatus.Good,
                Optional = true,
                MD5 = "md5",
                SHA1 = "sha1",
            };

            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(ItemType.Rom, actual.ItemType);
            Assert.Equal("name.chd", actual.Name);
            Assert.Equal("merge", actual.Merge);
            Assert.Equal("region", actual.Region);
            Assert.Equal(ItemStatus.Good, actual.Status);
            Assert.Equal(true, actual.Optional);
            Assert.Equal("md5", actual.MD5);
            Assert.Equal("sha1", actual.SHA1);
        }

        [Fact]
        public void ConvertToRom_EmptyMedia_EmptyRom()
        {
            var self = new Media();
            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(ItemType.Rom, actual.ItemType);
            Assert.Null(actual.Name);
            Assert.Null(actual.MD5);
            Assert.Null(actual.SHA1);
            Assert.Null(actual.SHA256);
            Assert.Null(actual.SpamSum);
        }

        [Fact]
        public void ConvertToRom_FilledMedia_FilledRom()
        {
            var self = new Media
            {
                Name = "name",
                MD5 = "md5",
                SHA1 = "sha1",
                SHA256 = "sha256",
                SpamSum = "spamsum",
            };

            Rom? actual = self.ConvertToRom();

            Assert.NotNull(actual);
            Assert.Equal(ItemType.Rom, actual.ItemType);
            Assert.Equal("name.aaruf", actual.Name);
            Assert.Equal("md5", actual.MD5);
            Assert.Equal("sha1", actual.SHA1);
            Assert.Equal("sha256", actual.SHA256);
            Assert.Equal("spamsum", actual.SpamSum);
        }

        #endregion

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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Status = ItemStatus.Nodump,
                Name = "name",
                Size = 12345,
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
                SpamSum = string.Empty,
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
                CRC16 = "XXXXXX",
                CRC = string.Empty,
                CRC64 = "XXXXXX",
                MD2 = string.Empty,
                MD4 = "XXXXXX",
                MD5 = string.Empty,
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = string.Empty,
                SHA1 = "XXXXXX",
                SHA256 = string.Empty,
                SHA384 = "XXXXXX",
                SHA512 = string.Empty,
                SpamSum = "XXXXXX",
            };
            var other = new Rom
            {
                Name = "name",
                Size = 12345,
                CRC16 = string.Empty,
                CRC = "XXXXXX",
                CRC64 = string.Empty,
                MD2 = "XXXXXX",
                MD4 = string.Empty,
                MD5 = "XXXXXX",
                RIPEMD128 = string.Empty,
                RIPEMD160 = "XXXXXX",
                SHA1 = string.Empty,
                SHA256 = "XXXXXX",
                SHA384 = string.Empty,
                SHA512 = "XXXXXX",
                SpamSum = string.Empty,
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
                SpamSum = "XXXXXX",
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = "XXXXXX",
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = string.Empty,
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = "XXXXXX",
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
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
                SpamSum = "XXXXXX",
            };
            var other = new Rom
            {
                Name = "XXXXXX2",
                Size = 12345,
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.PartialEquals(other);
            Assert.True(actual);
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
                MD5 = "md5",
                SHA1 = string.Empty,
            };
            Disk other = new Disk
            {
                MD5 = string.Empty,
                SHA1 = "sha1",
            };

            bool actual = self.HashMatch(other);
            Assert.False(actual);
        }

        [Fact]
        public void HashMatch_Disk_PartialMD5_True()
        {
            Disk self = new Disk
            {
                MD5 = "md5",
                SHA1 = string.Empty,
            };
            Disk other = new Disk
            {
                MD5 = "md5",
                SHA1 = "sha1",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Disk_PartialSHA1_True()
        {
            Disk self = new Disk
            {
                MD5 = string.Empty,
                SHA1 = "sha1",
            };
            Disk other = new Disk
            {
                MD5 = "md5",
                SHA1 = "sha1",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Disk_FullMatch_True()
        {
            Disk self = new Disk
            {
                MD5 = "md5",
                SHA1 = "sha1",
            };
            Disk other = new Disk
            {
                MD5 = "md5",
                SHA1 = "sha1",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_Mismatch_False()
        {
            Media self = new Media
            {
                MD5 = "md5",
                SHA1 = string.Empty,
                SHA256 = "sha256",
                SpamSum = string.Empty,
            };
            Media other = new Media
            {
                MD5 = string.Empty,
                SHA1 = "sha1",
                SHA256 = string.Empty,
                SpamSum = "spamsum",
            };

            bool actual = self.HashMatch(other);
            Assert.False(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialMD5_True()
        {
            Media self = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SpamSum = string.Empty,
            };
            Media other = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialSHA1_True()
        {
            Media self = new Media
            {
                MD5 = string.Empty,
                SHA1 = "XXXXXX",
                SHA256 = string.Empty,
                SpamSum = string.Empty,
            };
            Media other = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialSHA256_True()
        {
            Media self = new Media
            {
                MD5 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = "XXXXXX",
                SpamSum = string.Empty,
            };
            Media other = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_PartialSpamSum_True()
        {
            Media self = new Media
            {
                MD5 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SpamSum = "XXXXXX",
            };
            Media other = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Media_FullMatch_True()
        {
            Media self = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };
            Media other = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_Mismatch_False()
        {
            Rom self = new Rom
            {
                CRC16 = "XXXXXX",
                CRC = string.Empty,
                CRC64 = "XXXXXX",
                MD2 = string.Empty,
                MD4 = "XXXXXX",
                MD5 = string.Empty,
                RIPEMD128 = "XXXXXX",
                RIPEMD160 = string.Empty,
                SHA1 = "XXXXXX",
                SHA256 = string.Empty,
                SHA384 = "XXXXXX",
                SHA512 = string.Empty,
                SpamSum = "XXXXXX",
            };
            Rom other = new Rom
            {
                CRC16 = string.Empty,
                CRC = "XXXXXX",
                CRC64 = string.Empty,
                MD2 = "XXXXXX",
                MD4 = string.Empty,
                MD5 = "XXXXXX",
                RIPEMD128 = string.Empty,
                RIPEMD160 = "XXXXXX",
                SHA1 = string.Empty,
                SHA256 = "XXXXXX",
                SHA384 = string.Empty,
                SHA512 = "XXXXXX",
                SpamSum = string.Empty,
            };

            bool actual = self.HashMatch(other);
            Assert.False(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialCRC16_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialCRC_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialCRC64_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialMD2_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialMD4_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialMD5_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialRIPEMD128_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialRIPEMD160_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA1_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA256_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA384_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSHA512_True()
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
                SpamSum = string.Empty,
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_PartialSpamSum_True()
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
                SpamSum = "XXXXXX",
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        [Fact]
        public void HashMatch_Rom_FullMatch_True()
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
                SpamSum = "XXXXXX",
            };
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HashMatch(other);
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_Disk_NoHash_True()
        {
            var self = new Disk();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_NonZeroHash_False()
        {
            var self = new Disk
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_ZeroMD5_True()
        {
            var self = new Disk
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_ZeroSHA1_True()
        {
            var self = new Disk
            {
                MD5 = string.Empty,
                SHA1 = HashType.SHA1.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Disk_ZeroAll_True()
        {
            var self = new Disk
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_NoHash_True()
        {
            var self = new Media();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_NonZeroHash_False()
        {
            var self = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroMD5_True()
        {
            var self = new Media
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroSHA1_True()
        {
            var self = new Media
            {
                MD5 = string.Empty,
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = string.Empty,
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroSHA256_True()
        {
            var self = new Media
            {
                MD5 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = HashType.SHA256.ZeroString,
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroSpamSum_True()
        {
            var self = new Media
            {
                MD5 = string.Empty,
                SHA1 = string.Empty,
                SHA256 = string.Empty,
                SpamSum = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Media_ZeroAll_True()
        {
            var self = new Media
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = HashType.SHA256.ZeroString,
                SpamSum = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_NoHash_True()
        {
            var self = new Rom();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_NonZeroHash_False()
        {
            var self = new Rom
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
                SpamSum = "XXXXXX",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroCRC16_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroCRC_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroCRC64_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroMD2_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroMD4_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroMD5_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroRIPEMD128_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroRIPEMD160_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA1_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA256_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA384_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSHA512_True()
        {
            var self = new Rom
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
                SpamSum = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroSpamSum_True()
        {
            var self = new Rom
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
                SpamSum = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_Rom_ZeroAll_True()
        {
            var self = new Rom
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
                SpamSum = HashType.SpamSum.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        #endregion

        #region FillMissingHashes

        [Fact]
        public void FillMissingHashes_Disk_BothEmpty()
        {
            var self = new Disk();
            var other = new Disk();

            self.FillMissingHashes(other);
            Assert.Empty(self);
        }

        [Fact]
        public void FillMissingHashes_Disk_AllMissing()
        {
            var self = new Disk();
            var other = new Disk
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
            };

            self.FillMissingHashes(other);
        }

        [Fact]
        public void FillMissingHashes_Media_BothEmpty()
        {
            var self = new Media();
            var other = new Media();
            self.FillMissingHashes(other);
            Assert.Empty(self);
        }

        [Fact]
        public void FillMissingHashes_Media_AllMissing()
        {
            var self = new Media();
            var other = new Media
            {
                MD5 = "XXXXXX",
                SHA1 = "XXXXXX",
                SHA256 = "XXXXXX",
                SpamSum = "XXXXXX",
            };

            self.FillMissingHashes(other);
        }

        [Fact]
        public void FillMissingHashes_Rom_BothEmpty()
        {
            var self = new Rom();
            var other = new Rom();
            self.FillMissingHashes(other);
            Assert.Empty(self);
        }

        [Fact]
        public void FillMissingHashes_Rom_AllMissing()
        {
            var self = new Rom();
            var other = new Rom
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
                SpamSum = "XXXXXX",
            };

            self.FillMissingHashes(other);
        }

        #endregion

        #region String to Enum

        [Theory]
        [InlineData(null, null)]
        [InlineData("plain", Blit.Plain)]
        [InlineData("dirty", Blit.Dirty)]
        public void AsBlitTest(string? field, Blit? expected)
        {
            Blit? actual = field.AsBlit();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("cpu", ChipType.CPU)]
        [InlineData("audio", ChipType.Audio)]
        public void AsChipTypeTest(string? field, ChipType? expected)
        {
            ChipType? actual = field.AsChipType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("joy", ControlType.Joy)]
        [InlineData("stick", ControlType.Stick)]
        [InlineData("paddle", ControlType.Paddle)]
        [InlineData("pedal", ControlType.Pedal)]
        [InlineData("lightgun", ControlType.Lightgun)]
        [InlineData("positional", ControlType.Positional)]
        [InlineData("dial", ControlType.Dial)]
        [InlineData("trackball", ControlType.Trackball)]
        [InlineData("mouse", ControlType.Mouse)]
        [InlineData("only_buttons", ControlType.OnlyButtons)]
        [InlineData("keypad", ControlType.Keypad)]
        [InlineData("keyboard", ControlType.Keyboard)]
        [InlineData("mahjong", ControlType.Mahjong)]
        [InlineData("hanafuda", ControlType.Hanafuda)]
        [InlineData("gambling", ControlType.Gambling)]
        public void AsControlTypeTest(string? field, ControlType? expected)
        {
            ControlType? actual = field.AsControlType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("unknown", DeviceType.Unknown)]
        [InlineData("cartridge", DeviceType.Cartridge)]
        [InlineData("floppydisk", DeviceType.FloppyDisk)]
        [InlineData("harddisk", DeviceType.HardDisk)]
        [InlineData("cylinder", DeviceType.Cylinder)]
        [InlineData("cassette", DeviceType.Cassette)]
        [InlineData("punchcard", DeviceType.PunchCard)]
        [InlineData("punchtape", DeviceType.PunchTape)]
        [InlineData("printout", DeviceType.Printout)]
        [InlineData("serial", DeviceType.Serial)]
        [InlineData("parallel", DeviceType.Parallel)]
        [InlineData("snapshot", DeviceType.Snapshot)]
        [InlineData("quickload", DeviceType.QuickLoad)]
        [InlineData("memcard", DeviceType.MemCard)]
        [InlineData("cdrom", DeviceType.CDROM)]
        [InlineData("magtape", DeviceType.MagTape)]
        [InlineData("romimage", DeviceType.ROMImage)]
        [InlineData("midiin", DeviceType.MIDIIn)]
        [InlineData("midiout", DeviceType.MIDIOut)]
        [InlineData("picture", DeviceType.Picture)]
        [InlineData("vidfile", DeviceType.VidFile)]
        public void AsDeviceTypeTest(string? field, DeviceType? expected)
        {
            DeviceType? actual = field.AsDeviceType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("raster", DisplayType.Raster)]
        [InlineData("vector", DisplayType.Vector)]
        [InlineData("lcd", DisplayType.LCD)]
        [InlineData("svg", DisplayType.SVG)]
        [InlineData("unknown", DisplayType.Unknown)]
        public void AsDisplayTypeTest(string? field, DisplayType? expected)
        {
            DisplayType? actual = field.AsDisplayType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("big", Endianness.Big)]
        [InlineData("little", Endianness.Little)]
        public void AsEndiannessTest(string? field, Endianness? expected)
        {
            Endianness? actual = field.AsEndianness();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("unemulated", FeatureStatus.Unemulated)]
        [InlineData("imperfect", FeatureStatus.Imperfect)]
        public void AsFeatureStatusTest(string? field, FeatureStatus? expected)
        {
            FeatureStatus? actual = field.AsFeatureStatus();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("protection", FeatureType.Protection)]
        [InlineData("palette", FeatureType.Palette)]
        [InlineData("graphics", FeatureType.Graphics)]
        [InlineData("sound", FeatureType.Sound)]
        [InlineData("controls", FeatureType.Controls)]
        [InlineData("keyboard", FeatureType.Keyboard)]
        [InlineData("mouse", FeatureType.Mouse)]
        [InlineData("microphone", FeatureType.Microphone)]
        [InlineData("camera", FeatureType.Camera)]
        [InlineData("disk", FeatureType.Disk)]
        [InlineData("printer", FeatureType.Printer)]
        [InlineData("lan", FeatureType.Lan)]
        [InlineData("wan", FeatureType.Wan)]
        [InlineData("timing", FeatureType.Timing)]
        public void AsFeatureTypeTest(string? field, FeatureType? expected)
        {
            FeatureType? actual = field.AsFeatureType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("none", ItemStatus.None)]
        [InlineData("no", ItemStatus.None)]
        [InlineData("good", ItemStatus.Good)]
        [InlineData("baddump", ItemStatus.BadDump)]
        [InlineData("nodump", ItemStatus.Nodump)]
        [InlineData("yes", ItemStatus.Nodump)]
        [InlineData("verified", ItemStatus.Verified)]
        public void AsItemStatusTest(string? field, ItemStatus? expected)
        {
            ItemStatus? actual = field.AsItemStatus();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("load16_byte", LoadFlag.Load16Byte)]
        [InlineData("load16_word", LoadFlag.Load16Word)]
        [InlineData("load16_word_swap", LoadFlag.Load16WordSwap)]
        [InlineData("load32_byte", LoadFlag.Load32Byte)]
        [InlineData("load32_word", LoadFlag.Load32Word)]
        [InlineData("load32_word_swap", LoadFlag.Load32WordSwap)]
        [InlineData("load32_dword", LoadFlag.Load32DWord)]
        [InlineData("load64_word", LoadFlag.Load64Word)]
        [InlineData("load64_word_swap", LoadFlag.Load64WordSwap)]
        [InlineData("reload", LoadFlag.Reload)]
        [InlineData("fill", LoadFlag.Fill)]
        [InlineData("continue", LoadFlag.Continue)]
        [InlineData("reload_plain", LoadFlag.ReloadPlain)]
        [InlineData("ignore", LoadFlag.Ignore)]
        public void AsLoadFlagTest(string? field, LoadFlag? expected)
        {
            LoadFlag? actual = field.AsLoadFlag();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, MergingFlag.None)]
        [InlineData("none", MergingFlag.None)]
        [InlineData("split", MergingFlag.Split)]
        [InlineData("merged", MergingFlag.Merged)]
        [InlineData("nonmerged", MergingFlag.NonMerged)]
        [InlineData("unmerged", MergingFlag.NonMerged)]
        [InlineData("fullmerged", MergingFlag.FullMerged)]
        [InlineData("device", MergingFlag.DeviceNonMerged)]
        [InlineData("devicenonmerged", MergingFlag.DeviceNonMerged)]
        [InlineData("deviceunmerged", MergingFlag.DeviceNonMerged)]
        [InlineData("full", MergingFlag.FullNonMerged)]
        [InlineData("fullnonmerged", MergingFlag.FullNonMerged)]
        [InlineData("fullunmerged", MergingFlag.FullNonMerged)]
        public void AsMergingFlagTest(string? field, MergingFlag expected)
        {
            MergingFlag actual = field.AsMergingFlag();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, NodumpFlag.None)]
        [InlineData("none", NodumpFlag.None)]
        [InlineData("obsolete", NodumpFlag.Obsolete)]
        [InlineData("required", NodumpFlag.Required)]
        [InlineData("ignore", NodumpFlag.Ignore)]
        public void AsNodumpFlagTest(string? field, NodumpFlag expected)
        {
            NodumpFlag actual = field.AsNodumpFlag();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("rom", OpenMSXSubType.Rom)]
        [InlineData("megarom", OpenMSXSubType.MegaRom)]
        [InlineData("sccpluscart", OpenMSXSubType.SCCPlusCart)]
        public void AsOpenMSXSubTypeTest(string? field, OpenMSXSubType? expected)
        {
            OpenMSXSubType? actual = field.AsOpenMSXSubType();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, PackingFlag.None)]
        [InlineData("none", PackingFlag.None)]
        [InlineData("yes", PackingFlag.Zip)]
        [InlineData("zip", PackingFlag.Zip)]
        [InlineData("no", PackingFlag.Unzip)]
        [InlineData("unzip", PackingFlag.Unzip)]
        [InlineData("partial", PackingFlag.Partial)]
        [InlineData("flat", PackingFlag.Flat)]
        [InlineData("fileonly", PackingFlag.FileOnly)]
        public void AsPackingFlagTest(string? field, PackingFlag expected)
        {
            PackingFlag actual = field.AsPackingFlag();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("eq", Relation.Equal)]
        [InlineData("ne", Relation.NotEqual)]
        [InlineData("gt", Relation.GreaterThan)]
        [InlineData("le", Relation.LessThanOrEqual)]
        [InlineData("lt", Relation.LessThan)]
        [InlineData("ge", Relation.GreaterThanOrEqual)]
        public void AsRelationTest(string? field, Relation? expected)
        {
            Relation? actual = field.AsRelation();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("0", Rotation.North)]
        [InlineData("north", Rotation.North)]
        [InlineData("vertical", Rotation.North)]
        [InlineData("90", Rotation.East)]
        [InlineData("east", Rotation.East)]
        [InlineData("horizontal", Rotation.East)]
        [InlineData("180", Rotation.South)]
        [InlineData("south", Rotation.South)]
        [InlineData("270", Rotation.West)]
        [InlineData("west", Rotation.West)]
        public void AsRotationTest(string? field, Rotation? expected)
        {
            Rotation? actual = field.AsRotation();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("no", Runnable.No)]
        [InlineData("partial", Runnable.Partial)]
        [InlineData("yes", Runnable.Yes)]
        public void AsRunnableTest(string? field, Runnable? expected)
        {
            Runnable? actual = field.AsRunnable();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("none", SoftwareListStatus.None)]
        [InlineData("original", SoftwareListStatus.Original)]
        [InlineData("compatible", SoftwareListStatus.Compatible)]
        public void AsSoftwareListStatusTest(string? field, SoftwareListStatus? expected)
        {
            SoftwareListStatus? actual = field.AsSoftwareListStatus();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("no", Supported.No)]
        [InlineData("unsupported", Supported.No)]
        [InlineData("partial", Supported.Partial)]
        [InlineData("yes", Supported.Yes)]
        [InlineData("supported", Supported.Yes)]
        public void AsSupportedTest(string? field, Supported? expected)
        {
            Supported? actual = field.AsSupported();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("good", SupportStatus.Good)]
        [InlineData("imperfect", SupportStatus.Imperfect)]
        [InlineData("preliminary", SupportStatus.Preliminary)]
        public void AsSupportStatusTest(string? field, SupportStatus? expected)
        {
            SupportStatus? actual = field.AsSupportStatus();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("8", Width.Byte)]
        [InlineData("byte", Width.Byte)]
        [InlineData("16", Width.Short)]
        [InlineData("short", Width.Short)]
        [InlineData("32", Width.Int)]
        [InlineData("int", Width.Int)]
        [InlineData("64", Width.Long)]
        [InlineData("long", Width.Long)]
        public void AsWidth(string? field, Width? expected)
        {
            Width? actual = field.AsWidth();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("INVALID", null)]
        [InlineData("1", true)]
        [InlineData("yes", true)]
        [InlineData("True", true)]
        [InlineData("0", false)]
        [InlineData("no", false)]
        [InlineData("False", false)]
        public void AsYesNoTest(string? field, bool? expected)
        {
            bool? actual = field.AsYesNo();
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Enum to String

        [Theory]
        [InlineData((Blit)int.MaxValue, null)]
        [InlineData(Blit.Plain, "plain")]
        [InlineData(Blit.Dirty, "dirty")]
        public void FromBlitTest(Blit field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ChipType)int.MaxValue, null)]
        [InlineData(ChipType.CPU, "cpu")]
        [InlineData(ChipType.Audio, "audio")]
        public void FromChipTypeTest(ChipType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ControlType)int.MaxValue, null)]
        [InlineData(ControlType.Joy, "joy")]
        [InlineData(ControlType.Stick, "stick")]
        [InlineData(ControlType.Paddle, "paddle")]
        [InlineData(ControlType.Pedal, "pedal")]
        [InlineData(ControlType.Lightgun, "lightgun")]
        [InlineData(ControlType.Positional, "positional")]
        [InlineData(ControlType.Dial, "dial")]
        [InlineData(ControlType.Trackball, "trackball")]
        [InlineData(ControlType.Mouse, "mouse")]
        [InlineData(ControlType.OnlyButtons, "only_buttons")]
        [InlineData(ControlType.Keypad, "keypad")]
        [InlineData(ControlType.Keyboard, "keyboard")]
        [InlineData(ControlType.Mahjong, "mahjong")]
        [InlineData(ControlType.Hanafuda, "hanafuda")]
        [InlineData(ControlType.Gambling, "gambling")]
        public void FromControlTypeTest(ControlType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((DeviceType)int.MaxValue, null)]
        [InlineData(DeviceType.Unknown, "unknown")]
        [InlineData(DeviceType.Cartridge, "cartridge")]
        [InlineData(DeviceType.FloppyDisk, "floppydisk")]
        [InlineData(DeviceType.HardDisk, "harddisk")]
        [InlineData(DeviceType.Cylinder, "cylinder")]
        [InlineData(DeviceType.Cassette, "cassette")]
        [InlineData(DeviceType.PunchCard, "punchcard")]
        [InlineData(DeviceType.PunchTape, "punchtape")]
        [InlineData(DeviceType.Printout, "printout")]
        [InlineData(DeviceType.Serial, "serial")]
        [InlineData(DeviceType.Parallel, "parallel")]
        [InlineData(DeviceType.Snapshot, "snapshot")]
        [InlineData(DeviceType.QuickLoad, "quickload")]
        [InlineData(DeviceType.MemCard, "memcard")]
        [InlineData(DeviceType.CDROM, "cdrom")]
        [InlineData(DeviceType.MagTape, "magtape")]
        [InlineData(DeviceType.ROMImage, "romimage")]
        [InlineData(DeviceType.MIDIIn, "midiin")]
        [InlineData(DeviceType.MIDIOut, "midiout")]
        [InlineData(DeviceType.Picture, "picture")]
        [InlineData(DeviceType.VidFile, "vidfile")]
        public void FromDeviceTypeTest(DeviceType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((DisplayType)int.MaxValue, null)]
        [InlineData(DisplayType.Raster, "raster")]
        [InlineData(DisplayType.Vector, "vector")]
        [InlineData(DisplayType.LCD, "lcd")]
        [InlineData(DisplayType.SVG, "svg")]
        [InlineData(DisplayType.Unknown, "unknown")]
        public void FromDisplayTypeTest(DisplayType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Endianness)int.MaxValue, null)]
        [InlineData(Endianness.Big, "big")]
        [InlineData(Endianness.Little, "little")]
        public void FromEndiannessTest(Endianness field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((FeatureStatus)int.MaxValue, null)]
        [InlineData(FeatureStatus.Unemulated, "unemulated")]
        [InlineData(FeatureStatus.Imperfect, "imperfect")]
        public void FromFeatureStatusTest(FeatureStatus field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((FeatureType)int.MaxValue, null)]
        [InlineData(FeatureType.Protection, "protection")]
        [InlineData(FeatureType.Palette, "palette")]
        [InlineData(FeatureType.Graphics, "graphics")]
        [InlineData(FeatureType.Sound, "sound")]
        [InlineData(FeatureType.Controls, "controls")]
        [InlineData(FeatureType.Keyboard, "keyboard")]
        [InlineData(FeatureType.Mouse, "mouse")]
        [InlineData(FeatureType.Microphone, "microphone")]
        [InlineData(FeatureType.Camera, "camera")]
        [InlineData(FeatureType.Disk, "disk")]
        [InlineData(FeatureType.Printer, "printer")]
        [InlineData(FeatureType.Lan, "lan")]
        [InlineData(FeatureType.Wan, "wan")]
        [InlineData(FeatureType.Timing, "timing")]
        public void FromFeatureTypeTest(FeatureType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ItemStatus)int.MaxValue, true, null)]
        [InlineData((ItemStatus)int.MaxValue, false, null)]
        [InlineData(ItemStatus.None, true, "no")]
        [InlineData(ItemStatus.None, false, "none")]
        [InlineData(ItemStatus.Good, true, "good")]
        [InlineData(ItemStatus.Good, false, "good")]
        [InlineData(ItemStatus.BadDump, true, "baddump")]
        [InlineData(ItemStatus.BadDump, false, "baddump")]
        [InlineData(ItemStatus.Nodump, true, "yes")]
        [InlineData(ItemStatus.Nodump, false, "nodump")]
        [InlineData(ItemStatus.Verified, true, "verified")]
        [InlineData(ItemStatus.Verified, false, "verified")]
        public void FromItemStatusTest(ItemStatus field, bool useSecond, string? expected)
        {
            string? actual = field.AsStringValue(useSecond);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((LoadFlag)int.MaxValue, null)]
        [InlineData(LoadFlag.Load16Byte, "load16_byte")]
        [InlineData(LoadFlag.Load16Word, "load16_word")]
        [InlineData(LoadFlag.Load16WordSwap, "load16_word_swap")]
        [InlineData(LoadFlag.Load32Byte, "load32_byte")]
        [InlineData(LoadFlag.Load32Word, "load32_word")]
        [InlineData(LoadFlag.Load32WordSwap, "load32_word_swap")]
        [InlineData(LoadFlag.Load32DWord, "load32_dword")]
        [InlineData(LoadFlag.Load64Word, "load64_word")]
        [InlineData(LoadFlag.Load64WordSwap, "load64_word_swap")]
        [InlineData(LoadFlag.Reload, "reload")]
        [InlineData(LoadFlag.Fill, "fill")]
        [InlineData(LoadFlag.Continue, "continue")]
        [InlineData(LoadFlag.ReloadPlain, "reload_plain")]
        [InlineData(LoadFlag.Ignore, "ignore")]
        public void FromLoadFlagTest(LoadFlag field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(MergingFlag.None, true, "none")]
        [InlineData(MergingFlag.None, false, "none")]
        [InlineData(MergingFlag.Split, true, "split")]
        [InlineData(MergingFlag.Split, false, "split")]
        [InlineData(MergingFlag.Merged, true, "merged")]
        [InlineData(MergingFlag.Merged, false, "merged")]
        [InlineData(MergingFlag.NonMerged, true, "unmerged")]
        [InlineData(MergingFlag.NonMerged, false, "nonmerged")]
        [InlineData(MergingFlag.FullMerged, true, "fullmerged")]
        [InlineData(MergingFlag.FullMerged, false, "fullmerged")]
        [InlineData(MergingFlag.DeviceNonMerged, true, "devicenonmerged")]
        [InlineData(MergingFlag.DeviceNonMerged, false, "device")]
        [InlineData(MergingFlag.FullNonMerged, true, "fullnonmerged")]
        [InlineData(MergingFlag.FullNonMerged, false, "full")]
        public void FromMergingFlagTest(MergingFlag field, bool useSecond, string? expected)
        {
            string? actual = field.AsStringValue(useSecond);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(NodumpFlag.None, "none")]
        [InlineData(NodumpFlag.Obsolete, "obsolete")]
        [InlineData(NodumpFlag.Required, "required")]
        [InlineData(NodumpFlag.Ignore, "ignore")]
        public void FromNodumpFlagTest(NodumpFlag field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((OpenMSXSubType)int.MaxValue, null)]
        [InlineData(OpenMSXSubType.Rom, "rom")]
        [InlineData(OpenMSXSubType.MegaRom, "megarom")]
        [InlineData(OpenMSXSubType.SCCPlusCart, "sccpluscart")]
        public void FromOpenMSXSubTypeTest(OpenMSXSubType field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(PackingFlag.None, true, "none")]
        [InlineData(PackingFlag.None, false, "none")]
        [InlineData(PackingFlag.Zip, true, "yes")]
        [InlineData(PackingFlag.Zip, false, "zip")]
        [InlineData(PackingFlag.Unzip, true, "no")]
        [InlineData(PackingFlag.Unzip, false, "unzip")]
        [InlineData(PackingFlag.Partial, true, "partial")]
        [InlineData(PackingFlag.Partial, false, "partial")]
        [InlineData(PackingFlag.Flat, true, "flat")]
        [InlineData(PackingFlag.Flat, false, "flat")]
        [InlineData(PackingFlag.FileOnly, true, "fileonly")]
        [InlineData(PackingFlag.FileOnly, false, "fileonly")]
        public void FromPackingFlagTest(PackingFlag field, bool useSecond, string? expected)
        {
            string? actual = field.AsStringValue(useSecond);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Relation)int.MaxValue, null)]
        [InlineData(Relation.Equal, "eq")]
        [InlineData(Relation.NotEqual, "ne")]
        [InlineData(Relation.GreaterThan, "gt")]
        [InlineData(Relation.LessThanOrEqual, "le")]
        [InlineData(Relation.LessThan, "lt")]
        [InlineData(Relation.GreaterThanOrEqual, "ge")]
        public void FromRelationTest(Relation field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(Rotation.North, true, "vertical")]
        [InlineData(Rotation.North, false, "0")]
        [InlineData(Rotation.East, true, "horizontal")]
        [InlineData(Rotation.East, false, "90")]
        [InlineData(Rotation.South, true, "vertical")]
        [InlineData(Rotation.South, false, "180")]
        [InlineData(Rotation.West, true, "horizontal")]
        [InlineData(Rotation.West, false, "270")]
        public void FromRotationTest(Rotation field, bool useSecond, string? expected)
        {
            string? actual = field.AsStringValue(useSecond);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Runnable)int.MaxValue, null)]
        [InlineData(Runnable.No, "no")]
        [InlineData(Runnable.Partial, "partial")]
        [InlineData(Runnable.Yes, "yes")]
        public void FromRunnableTest(Runnable field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((SoftwareListStatus)int.MaxValue, null)]
        [InlineData(SoftwareListStatus.None, "none")]
        [InlineData(SoftwareListStatus.Original, "original")]
        [InlineData(SoftwareListStatus.Compatible, "compatible")]
        public void FromSoftwareListStatusTest(SoftwareListStatus field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Supported)int.MaxValue, true, null)]
        [InlineData((Supported)int.MaxValue, false, null)]
        [InlineData(Supported.No, true, "unsupported")]
        [InlineData(Supported.No, false, "no")]
        [InlineData(Supported.Partial, true, "partial")]
        [InlineData(Supported.Partial, false, "partial")]
        [InlineData(Supported.Yes, true, "supported")]
        [InlineData(Supported.Yes, false, "yes")]
        public void FromSupportedTest(Supported field, bool useSecond, string? expected)
        {
            string? actual = field.AsStringValue(useSecond);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((SupportStatus)int.MaxValue, null)]
        [InlineData(SupportStatus.Good, "good")]
        [InlineData(SupportStatus.Imperfect, "imperfect")]
        [InlineData(SupportStatus.Preliminary, "preliminary")]
        public void FromSupportStatusTest(SupportStatus field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Width)int.MaxValue, null)]
        [InlineData(Width.Byte, "8")]
        [InlineData(Width.Short, "16")]
        [InlineData(Width.Int, "32")]
        [InlineData(Width.Long, "64")]
        public void FromWidthTest(Width field, string? expected)
        {
            string? actual = field.AsStringValue();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(true, "yes")]
        [InlineData(false, "no")]
        public void FromYesNo(bool? field, string? expected)
        {
            string? actual = field.FromYesNo();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
