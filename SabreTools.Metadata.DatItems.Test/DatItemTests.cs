using System;
using SabreTools.Metadata.DatItems.Formats;
using Xunit;

namespace SabreTools.Metadata.DatItems.Test
{
    public class DatItemTests
    {
        #region Private Testing Classes

        /// <summary>
        /// Testing implementation of Data.Models.Metadata.DatItem
        /// </summary>
        private class TestDatItemModel : Data.Models.Metadata.DatItem, ICloneable
        {
            public string? Name { get; set; }

            /// <inheritdoc/>
            public object Clone() => new TestDatItemModel { Name = Name };
        }

        /// <summary>
        /// Testing implementation of DatItem
        /// </summary>
        private class TestDatItem : DatItem<TestDatItemModel>
        {
            public override Data.Models.Metadata.ItemType ItemType => Data.Models.Metadata.ItemType.Blank;

            public string? Name
            {
                get => (_internal as TestDatItemModel)?.Name;
                set => (_internal as TestDatItemModel)?.Name = value;
            }

            public TestDatItem() { }

            /// <inheritdoc/>
            public override object Clone() => new TestDatItem()
            {
                _internal = GetInternalClone(),
            };

            /// <inheritdoc/>
            public override TestDatItemModel GetInternalClone()
            {
                return (_internal as TestDatItemModel)?.Clone() as TestDatItemModel ?? [];
            }

            /// <inheritdoc/>
            public override string? GetName() => Name;

            /// <inheritdoc/>
            public override void SetName(string? name) => Name = name;


            /// <inheritdoc/>
            public override bool Equals(DatItem<TestDatItemModel>? other)
            {
                // If the other value is invalid
                if (other is null)
                    return false;

                // If the type matches
                if (other is TestDatItem otherTestDatItem)
                    return _internal.Equals(otherTestDatItem._internal);

                // Everything else fails
                return false;
            }
        }

        #endregion

        #region CopyMachineInformation

        [Fact]
        public void CopyMachineInformation_NewItem_Overwrite()
        {
            var romA = new Rom();

            var romB = new Rom { Machine = null };

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.Machine;
            Assert.NotNull(actualMachineA);
            Assert.Null(actualMachineA.Name);
        }

        [Fact]
        public void CopyMachineInformation_EmptyItem_NoChange()
        {
            Machine? machineA = new Machine { Name = "machineA" };

            var romA = new Rom { Machine = machineA };

            var romB = new Rom { Machine = null };

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.Machine;
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineA", actualMachineA.Name);
        }

        [Fact]
        public void CopyMachineInformation_NullMachine_NoChange()
        {
            Machine? machineA = new Machine { Name = "machineA" };

            Machine? machineB = null;

            var romA = new Rom { Machine = machineA };

            var romB = new Rom { Machine = machineB };

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.Machine;
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineA", actualMachineA.Name);
        }

        [Fact]
        public void CopyMachineInformation_EmptyMachine_Overwrite()
        {
            Machine? machineA = new Machine { Name = "machineA" };

            Machine? machineB = new Machine();

            var romA = new Rom { Machine = machineA };

            var romB = new Rom { Machine = machineB };

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.Machine;
            Assert.NotNull(actualMachineA);
            Assert.Null(actualMachineA.Name);
        }

        [Fact]
        public void CopyMachineInformation_FilledMachine_Overwrite()
        {
            Machine? machineA = new Machine { Name = "machineA" };

            Machine? machineB = new Machine { Name = "machineB" };

            var romA = new Rom { Machine = machineA };

            var romB = new Rom { Machine = machineB };

            romA.CopyMachineInformation(romB);
            var actualMachineA = romA.Machine;
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineB", actualMachineA.Name);
        }

        [Fact]
        public void CopyMachineInformation_MismatchedType_Overwrite()
        {
            Machine? machineA = new Machine { Name = "machineA" };

            Machine? machineB = new Machine { Name = "machineB" };

            var romA = new Rom { Machine = machineA };

            var diskB = new Disk { Machine = machineB };

            romA.CopyMachineInformation(diskB);
            var actualMachineA = romA.Machine;
            Assert.NotNull(actualMachineA);
            Assert.Equal("machineB", actualMachineA.Name);
        }

        #endregion

        #region CompareTo

        [Fact]
        public void CompareTo_NullOther_Returns1()
        {
            DatItem self = new Rom();
            DatItem? other = null;

            int actual = self.CompareTo(other);
            Assert.Equal(1, actual);
        }

        [Fact]
        public void CompareTo_DifferentOther_Returns1()
        {
            DatItem self = new Rom { Name = "name" };

            DatItem? other = new Disk { Name = "name" };

            int actual = self.CompareTo(other);
            Assert.Equal(1, actual);
        }

        [Fact]
        public void CompareTo_Empty_Returns1()
        {
            DatItem self = new Rom();
            DatItem? other = new Rom();

            int actual = self.CompareTo(other);
            Assert.Equal(1, actual);
        }

        [Theory]
        [InlineData(null, null, 0)]
        [InlineData("name", null, 1)]
        [InlineData("name", "other", -1)]
        [InlineData(null, "name", -1)]
        [InlineData("other", "name", 1)]
        [InlineData("name", "name", 0)]
        public void CompareTo_NamesOnly(string? selfName, string? otherName, int expected)
        {
            DatItem self = new Rom { Name = selfName };
            self.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");

            DatItem? other = new Rom { Name = otherName };
            other.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");

            int actual = self.CompareTo(other);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Equals

        [Fact]
        public void Equals_Null_False()
        {
            DatItem self = new TestDatItem();
            DatItem? other = null;

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_MismatchedType_False()
        {
            DatItem self = new TestDatItem();
            DatItem? other = new Rom();

            bool actual = self.Equals(other);
            Assert.False(actual);
        }

        [Fact]
        public void Equals_DefaultInternal_True()
        {
            DatItem self = new TestDatItem();
            DatItem? other = new TestDatItem();

            bool actual = self.Equals(other);
            Assert.True(actual);
        }

        [Fact]
        public void Equals_EqualInternal_True()
        {
            DatItem self = new TestDatItem();
            self.SetName("name");

            DatItem? other = new TestDatItem();
            other.SetName("name");

            bool actual = self.Equals(other);
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
        [InlineData(ItemKey.CRC16, false, false, "0000")]
        [InlineData(ItemKey.CRC16, false, true, "0000")]
        [InlineData(ItemKey.CRC16, true, false, "0000")]
        [InlineData(ItemKey.CRC16, true, true, "0000")]
        [InlineData(ItemKey.CRC, false, false, "00000000")]
        [InlineData(ItemKey.CRC, false, true, "00000000")]
        [InlineData(ItemKey.CRC, true, false, "00000000")]
        [InlineData(ItemKey.CRC, true, true, "00000000")]
        [InlineData(ItemKey.CRC64, false, false, "0000000000000000")]
        [InlineData(ItemKey.CRC64, false, true, "0000000000000000")]
        [InlineData(ItemKey.CRC64, true, false, "0000000000000000")]
        [InlineData(ItemKey.CRC64, true, true, "0000000000000000")]
        [InlineData(ItemKey.MD2, false, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, false, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, false, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD2, true, true, "8350e5a3e24c153df2275c9f80692773")]
        [InlineData(ItemKey.MD4, false, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, false, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, false, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD4, true, true, "31d6cfe0d16ae931b73c59d7e0c089c0")]
        [InlineData(ItemKey.MD5, false, false, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, false, true, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, true, false, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.MD5, true, true, "d41d8cd98f00b204e9800998ecf8427e")]
        [InlineData(ItemKey.RIPEMD128, false, false, "cdf26213a150dc3ecb610f18f6b38b46")]
        [InlineData(ItemKey.RIPEMD128, false, true, "cdf26213a150dc3ecb610f18f6b38b46")]
        [InlineData(ItemKey.RIPEMD128, true, false, "cdf26213a150dc3ecb610f18f6b38b46")]
        [InlineData(ItemKey.RIPEMD128, true, true, "cdf26213a150dc3ecb610f18f6b38b46")]
        [InlineData(ItemKey.RIPEMD160, false, false, "9c1185a5c5e9fc54612808977ee8f548b2258d31")]
        [InlineData(ItemKey.RIPEMD160, false, true, "9c1185a5c5e9fc54612808977ee8f548b2258d31")]
        [InlineData(ItemKey.RIPEMD160, true, false, "9c1185a5c5e9fc54612808977ee8f548b2258d31")]
        [InlineData(ItemKey.RIPEMD160, true, true, "9c1185a5c5e9fc54612808977ee8f548b2258d31")]
        [InlineData(ItemKey.SHA1, false, false, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, false, true, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, true, false, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
        [InlineData(ItemKey.SHA1, true, true, "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
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
        public void GetKeyDB_DefaultImplementation(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine { Name = "Machine" };

            DatItem datItem = new Blank();

            string actual = datItem.GetKey(bucketedBy, machine, source, lower, norename);
            Assert.Equal(expected, actual);
        }

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
        [InlineData(ItemKey.SpamSum, false, false, "BASE64")]
        [InlineData(ItemKey.SpamSum, false, true, "BASE64")]
        [InlineData(ItemKey.SpamSum, true, false, "base64")]
        [InlineData(ItemKey.SpamSum, true, true, "base64")]
        public void GetKeyDB_CustomImplementation(ItemKey bucketedBy, bool lower, bool norename, string expected)
        {
            Source source = new Source(0);

            Machine machine = new Machine { Name = "Machine" };

            DatItem datItem = new Rom();
            datItem.Write(Data.Models.Metadata.Rom.CRC16Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.CRC64Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.MD2Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.MD4Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.MD5Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.RIPEMD128Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.RIPEMD160Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.SHA1Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.SHA256Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.SHA384Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.SHA512Key, "DEADBEEF");
            datItem.Write(Data.Models.Metadata.Rom.SpamSumKey, "BASE64");

            string actual = datItem.GetKey(bucketedBy, machine, source, lower, norename);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region Clone

        [Fact]
        public void CloneTest()
        {
            DatItem item = new Sample { Name = "name" };

            object clone = item.Clone();
            Sample? actual = clone as Sample;
            Assert.NotNull(actual);
            Assert.Equal("name", actual.Name);
        }

        #endregion

        #region GetInternalClone

        [Fact]
        public void GetInternalCloneTest()
        {
            DatItem<TestDatItemModel> item = new TestDatItem { Name = "name" };
            TestDatItemModel actual = item.GetInternalClone();
            Assert.Equal("name", actual.Name);
        }

        #endregion
    }
}
