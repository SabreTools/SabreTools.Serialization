using SabreTools.Hashing;
using Xunit;

namespace SabreTools.Metadata.DatItems.Formats.Test
{
    public class DiskTests
    {
        #region ConvertToRom

        [Fact]
        public void ConvertToRomTest()
        {
            DiskArea diskArea = new DiskArea { Name = "name" };

            Machine machine = new Machine { Name = "name" };

            Part part = new Part { Name = "name" };

            Source source = new Source(0, "source");

            Disk disk = new Disk
            {
                Name = "name",
                DiskArea = diskArea,
                Merge = "merge",
                Region = "region",
                Status = Data.Models.Metadata.ItemStatus.Good,
                Optional = true,
                MD5 = HashType.MD5.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
                DupeType = DupeType.All | DupeType.External,
                Machine = machine,
                Part = part,
                RemoveFlag = false,
                Source = source,
            };

            Rom actual = disk.ConvertToRom();

            Assert.Equal("name.chd", actual.Name);
            Assert.Equal("merge", actual.Merge);
            Assert.Equal("region", actual.Region);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Good, actual.Status);
            Assert.Equal(true, actual.Optional);
            Assert.Equal(HashType.MD5.ZeroString, actual.MD5);
            Assert.Equal(HashType.SHA1.ZeroString, actual.SHA1);
            Assert.Equal(DupeType.All | DupeType.External, actual.DupeType);

            DataArea? actualDataArea = actual.DataArea;
            Assert.NotNull(actualDataArea);
            Assert.Equal("name", actualDataArea.Name);

            Machine? actualMachine = actual.Machine;
            Assert.NotNull(actualMachine);
            Assert.Equal("name", actualMachine.Name);

            Assert.False(actual.RemoveFlag);

            Part? actualPart = actual.Part;
            Assert.NotNull(actualPart);
            Assert.Equal("name", actualPart.Name);

            Source? actualSource = actual.Source;
            Assert.NotNull(actualSource);
            Assert.Equal(0, actualSource.Index);
            Assert.Equal("source", actualSource.Name);
        }

        #endregion

        #region FillMissingInformation

        [Fact]
        public void FillMissingInformation_BothEmpty()
        {
            Disk self = new Disk();
            Disk other = new Disk();

            self.FillMissingInformation(other);

            Assert.Null(self.MD5);
            Assert.Null(self.SHA1);
        }

        [Fact]
        public void FillMissingInformation_AllMissing()
        {
            Disk self = new Disk();

            Disk other = new Disk
            {
                MD5 = "md5",
                SHA1 = "sha1",
            };

            self.FillMissingInformation(other);

            Assert.Equal("md5", self.MD5);
            Assert.Equal("sha1", self.SHA1);
        }

        #endregion

        #region HasHashes

        [Fact]
        public void HasHashes_NoHash_False()
        {
            Disk self = new Disk();
            bool actual = self.HasHashes();
            Assert.False(actual);
        }

        [Fact]
        public void HasHashes_MD5_True()
        {
            Disk self = new Disk
            {
                MD5 = "md5",
                SHA1 = string.Empty,
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA1_True()
        {
            Disk self = new Disk
            {
                MD5 = string.Empty,
                SHA1 = "sha1",
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_All_True()
        {
            Disk self = new Disk
            {
                MD5 = "md5",
                SHA1 = "sha1",
            };

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_NoHash_True()
        {
            Disk self = new Disk();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_NonZeroHash_False()
        {
            Disk self = new Disk
            {
                MD5 = "DEADBEEF",
                SHA1 = "DEADBEEF",
            };

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD5_True()
        {
            Disk self = new Disk
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = string.Empty,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA1_True()
        {
            Disk self = new Disk
            {
                MD5 = string.Empty,
                SHA1 = HashType.SHA1.ZeroString,
            };

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroAll_True()
        {
            Disk self = new Disk
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
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
        [InlineData(ItemKey.CRC32, false, false, "00000000")]
        [InlineData(ItemKey.CRC32, false, true, "00000000")]
        [InlineData(ItemKey.CRC32, true, false, "00000000")]
        [InlineData(ItemKey.CRC32, true, true, "00000000")]
        [InlineData(ItemKey.MD2, false, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, false, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD4, false, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, false, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD5, false, false, "DEADBEEF")]
        [InlineData(ItemKey.MD5, false, true, "DEADBEEF")]
        [InlineData(ItemKey.MD5, true, false, "deadbeef")]
        [InlineData(ItemKey.MD5, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA1, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA1, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA1, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA256, false, false, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, false, true, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, true, false, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA256, true, true, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
        [InlineData(ItemKey.SHA384, false, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, false, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA512, false, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, false, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SpamSum, false, false, "3::")]
        [InlineData(ItemKey.SpamSum, false, true, "3::")]
        [InlineData(ItemKey.SpamSum, true, false, "3::")]
        [InlineData(ItemKey.SpamSum, true, true, "3::")]
        public void GetKeyDBTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine { Name = "Machine" };

            DatItem datItem = new Disk
            {
                MD5 = "DEADBEEF",
                SHA1 = "DEADBEEF",
            };

            string actual = datItem.GetKey(bucketedBy, machine, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
