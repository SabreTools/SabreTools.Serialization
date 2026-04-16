using SabreTools.Metadata.DatFiles.Formats;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using Xunit;

namespace SabreTools.Metadata.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region AddItemsFromChildren

        [Fact]
        public void AddItemsFromChildren_Items_Dedup()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromChildren(subfolder: true, skipDedup: false);

            Assert.Equal(2, datFile.GetItemsForBucket("parent").Count);
        }

        [Fact]
        public void AddItemsFromChildren_Items_SkipDedup()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromChildren(subfolder: true, skipDedup: true);

            Assert.Equal(3, datFile.GetItemsForBucket("parent").Count);
        }

        [Fact]
        public void AddItemsFromChildren_ItemsDB_Dedup()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            parentItem.SourceIndex = sourceIndex;
            matchChildItem.SourceIndex = sourceIndex;
            noMatchChildItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            parentItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            matchChildItem.MachineIndex = deviceMachineIndex;
            noMatchChildItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(parentItem, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromChildren(subfolder: true, skipDedup: false);

            Assert.Equal(2, datFile.GetItemsForBucketDB("parent").Count);
        }

        [Fact]
        public void AddItemsFromChildren_ItemsDB_SkipDedup()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            parentItem.SourceIndex = sourceIndex;
            matchChildItem.SourceIndex = sourceIndex;
            noMatchChildItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            parentItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            matchChildItem.MachineIndex = deviceMachineIndex;
            noMatchChildItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(parentItem, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromChildren(subfolder: true, skipDedup: true);

            Assert.Equal(3, datFile.GetItemsForBucketDB("parent").Count);
        }

        #endregion

        #region AddItemsFromCloneOfParent

        [Fact]
        public void AddItemsFromCloneOfParent_Items()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromCloneOfParent();

            Assert.Equal(2, datFile.GetItemsForBucket("child").Count);
        }

        [Fact]
        public void AddItemsFromCloneOfParent_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            parentItem.SourceIndex = sourceIndex;
            matchChildItem.SourceIndex = sourceIndex;
            noMatchChildItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            parentItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            matchChildItem.MachineIndex = deviceMachineIndex;
            noMatchChildItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(parentItem, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromCloneOfParent();

            Assert.Equal(2, datFile.GetItemsForBucketDB("child").Count);
        }

        #endregion

        #region AddItemsFromDevices

        [Theory]
        [InlineData(false, false, 4)]
        [InlineData(false, true, 4)]
        [InlineData(true, false, 3)]
        [InlineData(true, true, 3)]
        public void AddItemsFromDevices_Items(bool deviceOnly, bool useSlotOptions, int expected)
        {
            Source source = new Source(0, source: null);

            Machine deviceMachine = new Machine
            {
                Name = "device",
                IsDevice = true
            };

            Machine slotOptionMachine = new Machine { Name = "slotoption", };

            Machine itemMachine = new Machine { Name = "machine", };

            DatItem deviceItem = new Sample
            {
                Name = "device_item",
                Machine = deviceMachine,
                Source = source,
            };

            DatItem slotOptionItem = new Sample
            {
                Name = "slot_option_item",
                Machine = slotOptionMachine,
                Source = source,
            };

            DatItem datItem = new Rom
            {
                Name = "rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = itemMachine,
                Source = source
            };

            DatItem deviceRef = new DeviceRef
            {
                Name = "device",
                Machine = itemMachine,
                Source = source,
            };

            DatItem slotOption = new SlotOption
            {
                Name = "slotoption",
                Machine = itemMachine,
                Source = source,
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(deviceItem, statsOnly: false);
            datFile.AddItem(slotOptionItem, statsOnly: false);
            datFile.AddItem(datItem, statsOnly: false);
            datFile.AddItem(deviceRef, statsOnly: false);
            datFile.AddItem(slotOption, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromDevices(deviceOnly, useSlotOptions);

            Assert.Equal(expected, datFile.GetItemsForBucket("machine").Count);
        }

        [Theory]
        [InlineData(false, false, 4)]
        [InlineData(false, true, 4)]
        [InlineData(true, false, 3)]
        [InlineData(true, true, 3)]
        public void AddItemsFromDevices_ItemsDB(bool deviceOnly, bool useSlotOptions, int expected)
        {
            Source source = new Source(0, source: null);

            Machine deviceMachine = new Machine
            {
                Name = "device",
                IsDevice = true,
            };

            Machine slotOptionMachine = new Machine { Name = "slotoption", };

            Machine itemMachine = new Machine { Name = "machine", };

            DatItem deviceItem = new Sample { Name = "device_item", };

            DatItem slotOptionItem = new Sample { Name = "slot_option_item", };

            DatItem datItem = new Rom
            {
                Name = "rom",
                Size = 12345,
                CRC32 = "deadbeef"
            };

            DatItem deviceRef = new DeviceRef
            {
                Name = "device",
            };

            DatItem slotOption = new SlotOption
            {
                Name = "slotoption"
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            deviceItem.SourceIndex = sourceIndex;
            slotOptionItem.SourceIndex = sourceIndex;
            datItem.SourceIndex = sourceIndex;
            deviceRef.SourceIndex = sourceIndex;
            slotOption.SourceIndex = sourceIndex;

            long deviceMachineIndex = datFile.AddMachineDB(deviceMachine);
            deviceItem.MachineIndex = deviceMachineIndex;

            long slotOptionMachineIndex = datFile.AddMachineDB(slotOptionMachine);
            slotOptionItem.MachineIndex = slotOptionMachineIndex;

            long itemMachineIndex = datFile.AddMachineDB(itemMachine);
            datItem.MachineIndex = itemMachineIndex;
            deviceRef.MachineIndex = itemMachineIndex;
            slotOption.MachineIndex = itemMachineIndex;

            _ = datFile.AddItemDB(deviceItem, statsOnly: false);
            _ = datFile.AddItemDB(slotOptionItem, statsOnly: false);
            _ = datFile.AddItemDB(datItem, statsOnly: false);
            _ = datFile.AddItemDB(deviceRef, statsOnly: false);
            _ = datFile.AddItemDB(slotOption, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromDevices(deviceOnly, useSlotOptions);

            Assert.Equal(expected, datFile.GetItemsForBucketDB("machine").Count);
        }

        #endregion

        #region AddItemsFromRomOfParent

        [Fact]
        public void AddItemsFromRomOfParent_Items()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                RomOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromRomOfParent();

            Assert.Equal(2, datFile.GetItemsForBucket("child").Count);
        }

        [Fact]
        public void AddItemsFromRomOfParent_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                RomOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            parentItem.SourceIndex = sourceIndex;
            matchChildItem.SourceIndex = sourceIndex;
            noMatchChildItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            parentItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            matchChildItem.MachineIndex = deviceMachineIndex;
            noMatchChildItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(parentItem, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.AddItemsFromRomOfParent();

            Assert.Equal(2, datFile.GetItemsForBucketDB("child").Count);
        }

        #endregion

        #region RemoveBiosAndDeviceSets

        [Fact]
        public void RemoveBiosAndDeviceSets_Items()
        {
            Source source = new Source(0, source: null);

            Machine biosMachine = new Machine
            {
                Name = "bios",
                IsBios = true
            };

            Machine deviceMachine = new Machine
            {
                Name = "device",
                IsDevice = true
            };

            DatItem biosItem = new Rom
            {
                Machine = biosMachine,
                Source = source
            };

            DatItem deviceItem = new Rom
            {
                Machine = deviceMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(biosItem, statsOnly: false);
            datFile.AddItem(deviceItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveBiosAndDeviceSets();

            Assert.Empty(datFile.GetItemsForBucket("bios"));
            Assert.Empty(datFile.GetItemsForBucket("device"));
        }

        [Fact]
        public void RemoveBiosAndDeviceSets_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine biosMachine = new Machine
            {
                Name = "bios",
                IsBios = true
            };

            Machine deviceMachine = new Machine
            {
                Name = "device",
                IsDevice = true
            };

            DatItem biosItem = new Rom();
            DatItem deviceItem = new Rom();

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            biosItem.SourceIndex = sourceIndex;
            deviceItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(biosMachine);
            biosItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(deviceMachine);
            deviceItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(biosItem, statsOnly: false);
            _ = datFile.AddItemDB(deviceItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveBiosAndDeviceSets();

            Assert.Empty(datFile.GetMachinesDB());
        }

        #endregion

        #region RemoveItemsFromCloneOfChild

        [Fact]
        public void RemoveItemsFromCloneOfChild_Items()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine
            {
                Name = "parent",
                RomOf = "romof"
            };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent"
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveItemsFromCloneOfChild();

            Assert.Single(datFile.GetItemsForBucket("parent"));
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("child"));
            Machine? actualMachine = actual.Machine;
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.Name);
            Assert.Equal("romof", actualMachine.RomOf);
        }

        [Fact]
        public void RemoveItemsFromCloneOfChild_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine
            {
                Name = "parent",
                RomOf = "romof"
            };

            Machine childMachine = new Machine
            {
                Name = "child",
                CloneOf = "parent"
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            parentItem.SourceIndex = sourceIndex;
            matchChildItem.SourceIndex = sourceIndex;
            noMatchChildItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            parentItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            matchChildItem.MachineIndex = deviceMachineIndex;
            noMatchChildItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(parentItem, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveItemsFromCloneOfChild();

            Assert.Single(datFile.GetItemsForBucketDB("parent"));
            long actual = Assert.Single(datFile.GetItemsForBucketDB("child")).Key;
            Machine? actualMachine = datFile.GetMachineForItemDB(actual).Value;
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.Name);
            Assert.Equal("romof", actualMachine.RomOf);
        }

        #endregion

        #region RemoveItemsFromRomOfChild

        [Fact]
        public void RemoveItemsFromRomOfChild_Items()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                RomOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(parentItem, statsOnly: false);
            datFile.AddItem(matchChildItem, statsOnly: false);
            datFile.AddItem(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveItemsFromRomOfChild();

            Assert.Single(datFile.GetItemsForBucket("parent"));
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("child"));
            Machine? actualMachine = actual.Machine;
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.Name);
        }

        [Fact]
        public void RemoveItemsFromRomOfChild_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine parentMachine = new Machine { Name = "parent" };

            Machine childMachine = new Machine
            {
                Name = "child",
                RomOf = "parent",
                IsBios = true
            };

            DatItem parentItem = new Rom
            {
                Name = "parent_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = parentMachine,
                Source = source
            };

            DatItem matchChildItem = new Rom
            {
                Name = "match_child_rom",
                Size = 12345,
                CRC32 = "deadbeef",
                Machine = childMachine,
                Source = source
            };

            DatItem noMatchChildItem = new Rom
            {
                Name = "no_match_child_rom",
                Size = 12345,
                CRC32 = "beefdead",
                Machine = childMachine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            parentItem.SourceIndex = sourceIndex;
            matchChildItem.SourceIndex = sourceIndex;
            noMatchChildItem.SourceIndex = sourceIndex;

            long biosMachineIndex = datFile.AddMachineDB(parentMachine);
            parentItem.MachineIndex = biosMachineIndex;

            long deviceMachineIndex = datFile.AddMachineDB(childMachine);
            matchChildItem.MachineIndex = deviceMachineIndex;
            noMatchChildItem.MachineIndex = deviceMachineIndex;

            _ = datFile.AddItemDB(parentItem, statsOnly: false);
            _ = datFile.AddItemDB(matchChildItem, statsOnly: false);
            _ = datFile.AddItemDB(noMatchChildItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveItemsFromRomOfChild();

            Assert.Single(datFile.GetItemsForBucketDB("parent"));
            long actual = Assert.Single(datFile.GetItemsForBucketDB("child")).Key;
            Machine? actualMachine = datFile.GetMachineForItemDB(actual).Value;
            Assert.NotNull(actualMachine);
            Assert.Equal("child", actualMachine.Name);
        }

        #endregion

        #region RemoveMachineRelationshipTags

        [Fact]
        public void RemoveMachineRelationshipTags_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine
            {
                Name = "machine",
                CloneOf = "cloneof",
                RomOf = "romof",
                SampleOf = "sampleof"
            };

            DatItem item = new Rom
            {
                Machine = machine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(item, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveMachineRelationshipTags();

            DatItem actualItem = Assert.Single(datFile.GetItemsForBucket("machine"));
            Machine? actual = actualItem.Machine;
            Assert.NotNull(actual);
            Assert.Null(actual.CloneOf);
            Assert.Null(actual.RomOf);
            Assert.Null(actual.SampleOf);
        }

        [Fact]
        public void RemoveMachineRelationshipTags_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine
            {
                Name = "machine",
                CloneOf = "cloneof",
                RomOf = "romof",
                SampleOf = "sampleof"
            };

            DatItem item = new Rom();

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            item.SourceIndex = sourceIndex;
            long machineIndex = datFile.AddMachineDB(machine);
            item.MachineIndex = machineIndex;

            _ = datFile.AddItemDB(item, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.RemoveMachineRelationshipTags();

            Machine actual = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Null(actual.CloneOf);
            Assert.Null(actual.RomOf);
            Assert.Null(actual.SampleOf);
        }

        #endregion
    }
}
