using SabreTools.Hashing;
using Xunit;

namespace SabreTools.Metadata.DatItems.Formats.Test
{
    public class MediaTests
    {
        #region ConvertToRom

        [Fact]
        public void ConvertToRomTest()
        {
            Machine machine = new Machine();
            machine.Name = "name";

            Source source = new Source(0, "XXXXXX");

            Media media = new Media();
            media.Name = "name";
            media.MD5 = HashType.MD5.ZeroString;
            media.SHA1 = HashType.SHA1.ZeroString;
            media.SHA256 = HashType.SHA256.ZeroString;
            media.SpamSum = HashType.SpamSum.ZeroString;
            media.DupeType = DupeType.All | DupeType.External;
            media.Machine = machine;
            media.RemoveFlag = false;
            media.Source = source;

            Rom actual = media.ConvertToRom();

            Assert.Equal("name.aaruf", actual.Name);
            Assert.Equal(HashType.MD5.ZeroString, actual.ReadString(Data.Models.Metadata.Rom.MD5Key));
            Assert.Equal(HashType.SHA1.ZeroString, actual.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Assert.Equal(HashType.SHA256.ZeroString, actual.ReadString(Data.Models.Metadata.Rom.SHA256Key));
            Assert.Equal(HashType.SpamSum.ZeroString, actual.ReadString(Data.Models.Metadata.Rom.SpamSumKey));
            Assert.Equal(DupeType.All | DupeType.External, actual.DupeType);

            Machine? actualMachine = actual.Machine;
            Assert.NotNull(actualMachine);
            Assert.Equal("name", actualMachine.Name);

            Assert.False(actual.RemoveFlag);

            Source? actualSource = actual.Source;
            Assert.NotNull(actualSource);
            Assert.Equal(0, actualSource.Index);
            Assert.Equal("XXXXXX", actualSource.Name);
        }

        #endregion

        #region FillMissingInformation

        [Fact]
        public void FillMissingInformation_BothEmpty()
        {
            Media self = new Media();
            Media other = new Media();

            self.FillMissingInformation(other);

            Assert.Null(self.MD5);
            Assert.Null(self.SHA1);
            Assert.Null(self.SHA256);
            Assert.Null(self.SpamSum);
        }

        [Fact]
        public void FillMissingInformation_AllMissing()
        {
            Media self = new Media();

            Media other = new Media();
            other.MD5 = "XXXXXX";
            other.SHA1 = "XXXXXX";
            other.SHA256 = "XXXXXX";
            other.SpamSum = "XXXXXX";

            self.FillMissingInformation(other);

            Assert.Equal("XXXXXX", self.MD5);
            Assert.Equal("XXXXXX", self.SHA1);
            Assert.Equal("XXXXXX", self.SHA256);
            Assert.Equal("XXXXXX", self.SpamSum);
        }

        #endregion

        #region HasHashes

        [Fact]
        public void HasHashes_NoHash_False()
        {
            Media self = new Media();
            bool actual = self.HasHashes();
            Assert.False(actual);
        }

        [Fact]
        public void HasHashes_MD5_True()
        {
            Media self = new Media();
            self.MD5 = "XXXXXX";
            self.SHA1 = string.Empty;
            self.SHA256 = string.Empty;
            self.SpamSum = string.Empty;

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA1_True()
        {
            Media self = new Media();
            self.MD5 = string.Empty;
            self.SHA1 = "XXXXXX";
            self.SHA256 = string.Empty;
            self.SpamSum = string.Empty;

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SHA256_True()
        {
            Media self = new Media();
            self.MD5 = string.Empty;
            self.SHA1 = string.Empty;
            self.SHA256 = "XXXXXX";
            self.SpamSum = string.Empty;

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_SpamSum_True()
        {
            Media self = new Media();
            self.MD5 = string.Empty;
            self.SHA1 = string.Empty;
            self.SHA256 = string.Empty;
            self.SpamSum = "XXXXXX";

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        [Fact]
        public void HasHashes_All_True()
        {
            Media self = new Media();
            self.MD5 = "XXXXXX";
            self.SHA1 = "XXXXXX";
            self.SHA256 = "XXXXXX";
            self.SpamSum = "XXXXXX";

            bool actual = self.HasHashes();
            Assert.True(actual);
        }

        #endregion

        #region HasZeroHash

        [Fact]
        public void HasZeroHash_NoHash_True()
        {
            Media self = new Media();
            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_NonZeroHash_False()
        {
            Media self = new Media();
            self.MD5 = "XXXXXX";
            self.SHA1 = "XXXXXX";
            self.SHA256 = "XXXXXX";
            self.SpamSum = "XXXXXX";

            bool actual = self.HasZeroHash();
            Assert.False(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroMD5_True()
        {
            Media self = new Media();
            self.MD5 = HashType.MD5.ZeroString;
            self.SHA1 = string.Empty;
            self.SHA256 = string.Empty;
            self.SpamSum = string.Empty;

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA1_True()
        {
            Media self = new Media();
            self.MD5 = string.Empty;
            self.SHA1 = HashType.SHA1.ZeroString;
            self.SHA256 = string.Empty;
            self.SpamSum = string.Empty;

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSHA256_True()
        {
            Media self = new Media();
            self.MD5 = string.Empty;
            self.SHA1 = string.Empty;
            self.SHA256 = HashType.SHA256.ZeroString;
            self.SpamSum = string.Empty;

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroSpamSum_True()
        {
            Media self = new Media();
            self.MD5 = string.Empty;
            self.SHA1 = string.Empty;
            self.SHA256 = string.Empty;
            self.SpamSum = HashType.SpamSum.ZeroString;

            bool actual = self.HasZeroHash();
            Assert.True(actual);
        }

        [Fact]
        public void HasZeroHash_ZeroAll_True()
        {
            Media self = new Media();
            self.MD5 = HashType.MD5.ZeroString;
            self.SHA1 = HashType.SHA1.ZeroString;
            self.SHA256 = HashType.SHA256.ZeroString;
            self.SpamSum = HashType.SpamSum.ZeroString;

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
        [InlineData(ItemKey.CRC, false, false, "00000000")]
        [InlineData(ItemKey.CRC, false, true, "00000000")]
        [InlineData(ItemKey.CRC, true, false, "00000000")]
        [InlineData(ItemKey.CRC, true, true, "00000000")]
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
        [InlineData(ItemKey.SHA256, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SHA256, true, false, "deadbeef")]
        [InlineData(ItemKey.SHA256, true, true, "deadbeef")]
        [InlineData(ItemKey.SHA384, false, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, false, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, false, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA384, true, true, "38b060a751ac96384cd9327eb1b1e36a21fdb71114be07434c0cc7bf63f6e1da274edebfe76f65fbd51ad2f14898b95b")]
        [InlineData(ItemKey.SHA512, false, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, false, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, false, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SHA512, true, true, "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e")]
        [InlineData(ItemKey.SpamSum, false, false, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, false, true, "DEADBEEF")]
        [InlineData(ItemKey.SpamSum, true, false, "deadbeef")]
        [InlineData(ItemKey.SpamSum, true, true, "deadbeef")]
        public void GetKeyDBTest(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine();
            machine.Name = "Machine";

            DatItem datItem = new Media
            {
                MD5 = "DEADBEEF",
                SHA1 = "DEADBEEF",
                SHA256 = "DEADBEEF",
                SpamSum = "DEADBEEF"
            };
            string actual = datItem.GetKey(bucketedBy, machine, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
