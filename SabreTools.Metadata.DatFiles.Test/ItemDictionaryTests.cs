using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using Xunit;

namespace SabreTools.Metadata.DatFiles.Test
{
    public class ItemDictionaryTests
    {
        #region AddItem

        [Fact]
        public void AddItem_Disk_WithHashes()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem disk = new Disk
            {
                Name = "item",
                SHA1 = "deadbeef",
                Source = source,
                Machine = machine,
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(disk, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Disk? actualDisk = actual as Disk;
            Assert.NotNull(actualDisk);
            Assert.Equal(Data.Models.Metadata.ItemStatus.None, actualDisk.Status);
        }

        [Fact]
        public void AddItem_Disk_WithoutHashes()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem disk = new Disk
            {
                Name = "item",
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(disk, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Disk? actualDisk = actual as Disk;
            Assert.NotNull(actualDisk);
            Assert.Equal(Data.Models.Metadata.ItemStatus.Nodump, actualDisk.Status);
        }

        [Fact]
        public void AddItem_File_WithHashes()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            var file = new File
            {
                SHA1 = "deadbeef",
                Source = source,
                Machine = machine
            };
            file.SetName("item");

            var dict = new ItemDictionary();
            _ = dict.AddItem(file, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Assert.True(actual is File);
            //Assert.Equal(ItemStatus.None, actual.GetStringFieldValue(File.StatusKey));
        }

        [Fact]
        public void AddItem_File_WithoutHashes()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem file = new File
            {
                Source = source,
                Machine = machine
            };
            file.SetName("item");

            var dict = new ItemDictionary();
            _ = dict.AddItem(file, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Assert.True(actual is File);
            //Assert.Equal(Data.Models.Metadata.ItemStatus.Nodump, actual.GetStringFieldValue(File.StatusKey));
        }

        [Fact]
        public void AddItem_Media_WithHashes()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem media = new Media
            {
                Name = "item",
                SHA1 = "deadbeef",
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(media, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Assert.True(actual is Media);
            //Assert.Equal(ItemStatus.None, actual.GetStringFieldValue(Data.Models.Metadata.Media.StatusKey));
        }

        [Fact]
        public void AddItem_Media_WithoutHashes()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem media = new Media
            {
                Name = "item",
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(media, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Assert.True(actual is Media);
            //Assert.Equal(Data.Models.Metadata.ItemStatus.Nodump, actual.GetStringFieldValue(Data.Models.Metadata.Media.StatusKey));
        }

        [Fact]
        public void AddItem_Rom_WithHashesWithSize()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem rom = new Rom()
            {
                Name = "item",
                Size = 12345,
            };
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "deadbeef");
            rom.Source = source;
            rom.Machine = machine;

            var dict = new ItemDictionary();
            _ = dict.AddItem(rom, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Rom? actualRom = actual as Rom;
            Assert.NotNull(actualRom);
            Assert.Equal(12345, actualRom.Size);
            Assert.Equal("deadbeef", actual.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Assert.Equal(Data.Models.Metadata.ItemStatus.None, actualRom.Status);
        }

        [Fact]
        public void AddItem_Rom_WithoutHashesWithSize()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem rom = new Rom
            {
                Name = "item",
                Size = 12345,
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(rom, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Rom? actualRom = actual as Rom;
            Assert.NotNull(actualRom);
            Assert.Equal(12345, actualRom.Size);
            Assert.Null(actual.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Assert.Equal(Data.Models.Metadata.ItemStatus.Nodump, actualRom.Status);
        }

        [Fact]
        public void AddItem_Rom_WithHashesWithoutSize()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem rom = new Rom { Name = "item" };
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "deadbeef");
            rom.Source = source;
            rom.Machine = machine;

            var dict = new ItemDictionary();
            _ = dict.AddItem(rom, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Rom? actualRom = actual as Rom;
            Assert.NotNull(actualRom);
            Assert.Null(actualRom.Size);
            Assert.Equal("deadbeef", actual.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Assert.Equal(Data.Models.Metadata.ItemStatus.None, actualRom.Status);
        }

        [Fact]
        public void AddItem_Rom_WithoutHashesWithoutSize()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem rom = new Rom
            {
                Name = "item",
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(rom, statsOnly: false);

            DatItem actual = Assert.Single(dict.GetItemsForBucket("default"));
            Rom? actualRom = actual as Rom;
            Assert.NotNull(actualRom);
            Assert.Equal(0, actualRom.Size);
            Assert.Equal(HashType.SHA1.ZeroString, actual.ReadString(Data.Models.Metadata.Rom.SHA1Key));
            Assert.Equal(Data.Models.Metadata.ItemStatus.None, actualRom.Status);
        }

        [Fact]
        public void AddItem_StatsOnly()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem item = new Rom
            {
                Name = "item",
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: true);

            Assert.Empty(dict.GetItemsForBucket("default"));
        }

        [Fact]
        public void AddItem_NormalAdd()
        {
            Source source = new Source(0, source: null);
            Machine machine = new Machine();

            DatItem item = new Rom
            {
                Name = "item",
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            Assert.Single(dict.GetItemsForBucket("default"));
        }

        #endregion

        #region ClearMarked

        [Fact]
        public void ClearMarkedTest()
        {
            // Setup the items
            Machine machine = new Machine { Name = "game-1" };

            DatItem rom1 = new Rom
            {
                Name = "rom-1",
                Size = 1024,
            };
            rom1.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            rom1.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom
            {
                Name = "rom-2",
                Size = 1024,
            };
            rom2.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            rom2.RemoveFlag = true;
            rom2.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);

            dict.ClearMarked();
            Assert.Empty(dict.GetItemsForBucket("default"));
            List<DatItem> items = dict.GetItemsForBucket("game-1");
            Assert.Single(items);
        }

        #endregion

        #region GetItemsForBucket

        [Fact]
        public void GetItemsForBucket_NullBucketName()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem item = new Rom
            {
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket(null, filter: false);

            Assert.Empty(actual);
        }

        [Fact]
        public void GetItemsForBucket_InvalidBucketName()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem item = new Rom
            {
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("INVALID", filter: false);

            Assert.Empty(actual);
        }

        [Fact]
        public void GetItemsForBucket_RemovedFilter()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem item = new Rom
            {
                RemoveFlag = true,
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("machine", filter: true);

            Assert.Empty(actual);
        }

        [Fact]
        public void GetItemsForBucket_RemovedNoFilter()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem item = new Rom
            {
                RemoveFlag = true,
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("machine", filter: false);

            Assert.Single(actual);
        }

        [Fact]
        public void GetItemsForBucket_Standard()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem item = new Rom
            {
                Source = source,
                Machine = machine
            };

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            var actual = dict.GetItemsForBucket("machine", filter: false);

            Assert.Single(actual);
        }

        #endregion

        #region RemoveBucket

        [Fact]
        public void RemoveBucketTest()
        {
            Machine machine = new Machine { Name = "game-1" };

            DatItem datItem = new Rom
            {
                Size = 1024,
                Name = "rom-1"
            };
            datItem.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            datItem.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            datItem.CopyMachineInformation(machine);

            var dict = new ItemDictionary();
            dict.AddItem(datItem, statsOnly: false);

            dict.RemoveBucket("game-1");

            Assert.Empty(dict.GetItemsForBucket("default"));
            Assert.Empty(dict.GetItemsForBucket("game-1"));
        }

        #endregion

        #region RemoveItem

        [Fact]
        public void RemoveItemTest()
        {
            Machine machine = new Machine { Name = "game-1" };

            DatItem datItem = new Rom
            {
                Size = 1024,
                Name = "rom-1"
            };
            datItem.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            datItem.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            datItem.CopyMachineInformation(machine);

            var dict = new ItemDictionary();
            dict.AddItem(datItem, statsOnly: false);

            dict.RemoveItem("game-1", (Rom)datItem.Clone(), 0);

            Assert.Empty(dict.GetItemsForBucket("default"));
            Assert.Empty(dict.GetItemsForBucket("game-1"));
        }

        #endregion

        #region BucketBy

        [Theory]
        [InlineData(ItemKey.NULL, 2)]
        [InlineData(ItemKey.Machine, 2)]
        [InlineData(ItemKey.CRC, 1)]
        [InlineData(ItemKey.SHA1, 4)]
        public void BucketByTest(ItemKey itemKey, int expected)
        {
            // Setup the items
            Machine machine1 = new Machine { Name = "game-1" };

            Machine machine2 = new Machine { Name = "game-2" };

            DatItem rom1 = new Rom
            {
                Name = "rom-1",
                Size = 1024,
            };
            rom1.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            rom1.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.CopyMachineInformation(machine1);

            DatItem rom2 = new Rom
            {
                Name = "rom-2",
                Size = 1024,
            };
            rom2.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            rom2.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom1.CopyMachineInformation(machine1);

            DatItem rom3 = new Rom
            {
                Name = "rom-3",
                Size = 1024,
            };
            rom3.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            rom3.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "00000ea4014ce66679e7e17d56ac510f67e39e26");
            rom1.CopyMachineInformation(machine2);

            DatItem rom4 = new Rom
            {
                Name = "rom-4",
                Size = 1024,
            };
            rom4.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "DEAEEF");
            rom4.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "00000151d437442e74e5134023fab8bf694a2487");
            rom1.CopyMachineInformation(machine2);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);
            dict.AddItem(rom3, statsOnly: false);
            dict.AddItem(rom4, statsOnly: false);

            dict.BucketBy(itemKey);
            Assert.Equal(expected, dict.SortedKeys.Length);
        }

        #endregion

        #region Deduplicate

        [Fact]
        public void DeduplicateTest()
        {
            // Setup the items
            Machine machine = new Machine { Name = "game-1" };

            DatItem rom1 = new Rom
            {
                Name = "rom-1",
                Size = 1024,
            };
            rom1.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom
            {
                Name = "rom-2",
                Size = 1024,
            };
            rom2.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom2.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);

            dict.Deduplicate();
            Assert.Equal(1, dict.DatStatistics.TotalCount);
        }

        #endregion

        #region GetDuplicateStatus

        [Fact]
        public void GetDuplicateStatus_NullOther_NoDupe()
        {
            var dict = new ItemDictionary();
            DatItem item = new Rom();
            DatItem? lastItem = null;
            var actual = dict.GetDuplicateStatus(item, lastItem);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatus_DifferentTypes_NoDupe()
        {
            var dict = new ItemDictionary();
            var rom = new Rom();
            DatItem? lastItem = new Disk();
            var actual = dict.GetDuplicateStatus(rom, lastItem);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatus_MismatchedHashes_NoDupe()
        {
            var dict = new ItemDictionary();

            Machine? machineA = new Machine { Name = "name-same" };

            Machine? machineB = new Machine { Name = "name-same" };

            var romA = new Rom { Name = "same-name" };
            romA.Write(Data.Models.Metadata.Rom.CRCKey, "BEEFDEAD");
            romA.Source = new Source(0);
            romA.CopyMachineInformation(machineA);

            var romB = new Rom { Name = "same-name" };
            romB.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.Source = new Source(1);
            romB.CopyMachineInformation(machineB);

            var actual = dict.GetDuplicateStatus(romA, romB);
            Assert.Equal((DupeType)0x00, actual);
        }

        [Fact]
        public void GetDuplicateStatus_DifferentSource_NameMatch_ExternalAll()
        {
            var dict = new ItemDictionary();

            Machine? machineA = new Machine { Name = "name-same" };

            Machine? machineB = new Machine { Name = "name-same" };

            var romA = new Rom { Name = "same-name" };
            romA.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.Source = new Source(0);
            romA.CopyMachineInformation(machineA);

            var romB = new Rom { Name = "same-name" };
            romB.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.Source = new Source(1);
            romB.CopyMachineInformation(machineB);

            var actual = dict.GetDuplicateStatus(romA, romB);
            Assert.Equal(DupeType.External | DupeType.All, actual);
        }

        [Fact]
        public void GetDuplicateStatus_DifferentSource_NoNameMatch_ExternalHash()
        {
            var dict = new ItemDictionary();

            Machine? machineA = new Machine { Name = "name-same" };

            Machine? machineB = new Machine { Name = "not-name-same" };

            var romA = new Rom { Name = "same-name" };
            romA.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.Source = new Source(0);
            romA.CopyMachineInformation(machineA);

            var romB = new Rom { Name = "same-name" };
            romB.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.Source = new Source(1);
            romB.CopyMachineInformation(machineB);

            var actual = dict.GetDuplicateStatus(romA, romB);
            Assert.Equal(DupeType.External | DupeType.Hash, actual);
        }

        [Fact]
        public void GetDuplicateStatus_SameSource_NameMatch_InternalAll()
        {
            var dict = new ItemDictionary();

            Machine? machineA = new Machine { Name = "name-same" };

            Machine? machineB = new Machine { Name = "name-same" };

            var romA = new Rom { Name = "same-name" };
            romA.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.Source = new Source(0);
            romA.CopyMachineInformation(machineA);

            var romB = new Rom { Name = "same-name" };
            romB.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.Source = new Source(0);
            romB.CopyMachineInformation(machineB);

            var actual = dict.GetDuplicateStatus(romA, romB);
            Assert.Equal(DupeType.Internal | DupeType.All, actual);
        }

        [Fact]
        public void GetDuplicateStatus_SameSource_NoNameMatch_InternalHash()
        {
            var dict = new ItemDictionary();

            Machine? machineA = new Machine { Name = "name-same" };

            Machine? machineB = new Machine { Name = "not-name-same" };

            var romA = new Rom { Name = "same-name" };
            romA.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romA.Source = new Source(0);
            romA.CopyMachineInformation(machineA);

            var romB = new Rom { Name = "same-name" };
            romB.Write(Data.Models.Metadata.Rom.CRCKey, "DEADBEEF");
            romB.Source = new Source(0);
            romB.CopyMachineInformation(machineB);

            var actual = dict.GetDuplicateStatus(romA, romB);
            Assert.Equal(DupeType.Internal | DupeType.Hash, actual);
        }

        #endregion

        #region GetDuplicates

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void GetDuplicatesTest(bool hasDuplicate, int expected)
        {
            // Setup the items
            Machine machine = new Machine { Name = "game-1" };

            DatItem rom1 = new Rom
            {
                Name = "rom-1",
                Size = 1024,
            };
            rom1.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom
            {
                Name = "rom-2",
                Size = 1024,
            };
            rom2.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom2.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem(rom1, statsOnly: false);
            dict.AddItem(rom2, statsOnly: false);

            // Setup the test item
            DatItem rom = new Rom
            {
                Name = "rom-1",
                Size = hasDuplicate ? 1024 : 2048,
            };
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom.CopyMachineInformation(machine);

            var actual = dict.GetDuplicates(rom);
            Assert.Equal(expected, actual.Count);
        }

        #endregion

        #region HasDuplicates

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasDuplicatesTest(bool expected)
        {
            // Setup the items
            Machine machine = new Machine { Name = "game-1" };

            DatItem rom1 = new Rom
            {
                Name = "rom-1",
                Size = 1024,
            };
            rom1.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.CopyMachineInformation(machine);

            DatItem rom2 = new Rom
            {
                Name = "rom-2",
                Size = 1024,
            };
            rom2.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "000000e948edcb4f7704b8af85a77a3339ecce44");
            rom1.CopyMachineInformation(machine);

            // Setup the dictionary
            var dict = new ItemDictionary();
            dict.AddItem("game-1", rom1);
            dict.AddItem("game-1", rom2);

            // Setup the test item
            DatItem rom = new Rom
            {
                Name = "rom-1",
                Size = expected ? 1024 : 2048,
            };
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, "0000000fbbb37f8488100b1b4697012de631a5e6");
            rom1.CopyMachineInformation(machine);

            bool actual = dict.HasDuplicates(rom);
            Assert.Equal(expected, actual);
        }

        #endregion

        #region RecalculateStats

        [Fact]
        public void RecalculateStatsTest()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem item = new Rom
            {
                Name = "rom",
                Size = 12345,
            };
            item.Write<string?>(Data.Models.Metadata.Rom.CRCKey, "deadbeef");
            item.Source = source;
            item.Machine = machine;

            var dict = new ItemDictionary();
            _ = dict.AddItem(item, statsOnly: false);

            Assert.Equal(1, dict.DatStatistics.TotalCount);
            Assert.Equal(1, dict.DatStatistics.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(12345, dict.DatStatistics.TotalSize);
            Assert.Equal(1, dict.DatStatistics.GetHashCount(HashType.CRC32));
            Assert.Equal(0, dict.DatStatistics.GetHashCount(HashType.MD5));

            item.Write<string?>(Data.Models.Metadata.Rom.MD5Key, "deadbeef");

            dict.RecalculateStats();

            Assert.Equal(1, dict.DatStatistics.TotalCount);
            Assert.Equal(1, dict.DatStatistics.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(12345, dict.DatStatistics.TotalSize);
            Assert.Equal(1, dict.DatStatistics.GetHashCount(HashType.CRC32));
            Assert.Equal(1, dict.DatStatistics.GetHashCount(HashType.MD5));
        }

        #endregion
    }
}
