using System.Collections.Generic;
using SabreTools.Metadata.DatFiles.Formats;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Metadata.Filter;
using Xunit;

namespace SabreTools.Metadata.DatFiles.Test
{
    public partial class DatFileTests
    {
        #region ExecuteFilters

        [Fact]
        public void ExecuteFilters_Items()
        {
            FilterObject filterObject = new FilterObject("rom.crc", "deadbeef", Operation.NotEquals);
            FilterRunner filterRunner = new FilterRunner([filterObject]);

            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem datItem = new Rom
            {
                Name = "rom.bin",
                CRC32 = "deadbeef",
                Machine = machine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.ExecuteFilters(filterRunner);

            var actualDatItems = datFile.GetItemsForBucket("machine");
            DatItem actualRom = Assert.Single(actualDatItems);
            Assert.True(actualRom.RemoveFlag);
        }

        [Fact]
        public void ExecuteFilters_ItemsDB()
        {
            FilterObject filterObject = new FilterObject("rom.crc", "deadbeef", Operation.NotEquals);
            FilterRunner filterRunner = new FilterRunner([filterObject]);

            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem datItem = new Rom
            {
                Name = "rom.bin",
                CRC32 = "deadbeef",
                Machine = machine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            datItem.SourceIndex = sourceIndex;
            long machineIndex = datFile.AddMachineDB(machine);
            datItem.MachineIndex = machineIndex;

            _ = datFile.AddItemDB(datItem, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.ExecuteFilters(filterRunner);

            var actualDatItems = datFile.GetItemsForBucketDB("machine");
            DatItem actualRom = Assert.Single(actualDatItems).Value;
            Assert.True(actualRom.RemoveFlag);
        }

        #endregion

        #region MachineDescriptionToName

        [Fact]
        public void MachineDescriptionToName_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine
            {
                Name = "machine",
                Description = "description"
            };

            DatItem datItem = new Rom
            {
                Machine = machine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.MachineDescriptionToName();

            // The name of the bucket is not expected to change
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("machine"));
            Machine? actualMachine = actual.Machine;
            Assert.NotNull(actualMachine);
            Assert.Equal("description", actualMachine.Name);
            Assert.Equal("description", actualMachine.Description);
        }

        [Fact]
        public void MachineDescriptionToName_ItemsDB()
        {
            Machine machine = new Machine
            {
                Name = "machine",
                Description = "description"
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            _ = datFile.AddMachineDB(machine);

            datFile.MachineDescriptionToName();

            Machine actualMachine = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Equal("description", actualMachine.Name);
            Assert.Equal("description", actualMachine.Description);
        }

        #endregion

        #region SetOneRomPerGame

        [Fact]
        public void SetOneRomPerGame_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem rom = new Rom
            {
                Name = "rom.bin",
                Machine = machine,
                Source = source
            };

            DatItem disk = new Disk
            {
                Name = "disk",
                Machine = machine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(rom, statsOnly: false);
            datFile.AddItem(disk, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.SetOneRomPerGame();

            var actualDatItems = datFile.GetItemsForBucket("machine");
            Assert.Equal(2, actualDatItems.Count);

            DatItem actualRom = Assert.Single(actualDatItems.FindAll(i => i is Rom));
            Machine? actualRomMachine = actualRom.Machine;
            Assert.NotNull(actualRomMachine);
            Assert.Equal("machine/rom", actualRomMachine.Name);

            DatItem actualDisk = Assert.Single(actualDatItems.FindAll(i => i is Disk));
            Machine? actualDiskMachine = actualDisk.Machine;
            Assert.NotNull(actualDiskMachine);
            Assert.Equal("machine/disk", actualDiskMachine.Name);
        }

        [Fact]
        public void SetOneRomPerGame_ItemsDB()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "machine" };

            DatItem rom = new Rom { Name = "rom.bin" };

            DatItem disk = new Disk { Name = "disk" };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);

            long sourceIndex = datFile.AddSourceDB(source);
            rom.SourceIndex = sourceIndex;
            disk.SourceIndex = sourceIndex;

            long machineIndex = datFile.AddMachineDB(machine);
            rom.MachineIndex = machineIndex;
            disk.MachineIndex = machineIndex;

            _ = datFile.AddItemDB(rom, statsOnly: false);
            _ = datFile.AddItemDB(disk, statsOnly: false);

            datFile.BucketBy(ItemKey.Machine);
            datFile.SetOneRomPerGame();

            var actualDatItems = datFile.GetItemsForBucketDB("machine");
            Assert.Equal(2, actualDatItems.Count);

            var actualRom = Assert.Single(actualDatItems, i => i.Value is Rom);
            var actualRomMachine = datFile.GetMachineDB(actualRom.Value.MachineIndex);
            Assert.NotNull(actualRomMachine.Value);
            Assert.Equal("machine/rom", actualRomMachine.Value.Name);

            var actualDisk = Assert.Single(actualDatItems, i => i.Value is Disk);
            var actualDiskMachine = datFile.GetMachineDB(actualDisk.Value.MachineIndex);
            Assert.NotNull(actualDiskMachine.Value);
            Assert.Equal("machine/disk", actualDiskMachine.Value.Name);
        }

        #endregion

        #region SetOneGamePerRegion

        [Fact]
        public void SetOneGamePerRegion_Items()
        {
            Machine nowhereMachine = new Machine { Name = "machine (Nowhere)" };

            Machine worldMachine = new Machine
            {
                Name = "machine (World)",
                CloneOf = "machine (Nowhere)"
            };

            DatItem nowhereRom = new Rom
            {
                Name = "rom.bin",
                Machine = nowhereMachine
            };

            DatItem worldRom = new Rom
            {
                Name = "rom.nib",
                Machine = worldMachine
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(nowhereRom, statsOnly: false);
            datFile.AddItem(worldRom, statsOnly: false);

            List<string> regions = ["World", "Nowhere"];
            datFile.SetOneGamePerRegion(regions);

            Assert.Empty(datFile.GetItemsForBucket("machine (nowhere)"));

            var actualDatItems = datFile.GetItemsForBucket("machine (world)");
            DatItem actualWorldRom = Assert.Single(actualDatItems);
            Machine? actualWorldMachine = actualWorldRom.Machine;
            Assert.NotNull(actualWorldMachine);
            Assert.Equal("machine (World)", actualWorldMachine.Name);
        }

        [Fact]
        public void SetOneGamePerRegion_ItemsDB()
        {
            Machine nowhereMachine = new Machine { Name = "machine (Nowhere)" };

            Machine worldMachine = new Machine
            {
                Name = "machine (World)",
                CloneOf = "machine (Nowhere)"
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            _ = datFile.AddMachineDB(nowhereMachine);
            _ = datFile.AddMachineDB(worldMachine);

            List<string> regions = ["World", "Nowhere"];
            datFile.SetOneGamePerRegion(regions);

            var actualWorldMachine = Assert.Single(datFile.GetMachinesDB());
            Assert.NotNull(actualWorldMachine.Value);
            Assert.Equal("machine (World)", actualWorldMachine.Value.Name);
        }

        #endregion

        #region StripSceneDatesFromItems

        [Fact]
        public void StripSceneDatesFromItems_Items()
        {
            Source source = new Source(0, source: null);

            Machine machine = new Machine { Name = "10.10.10-machine-name" };

            DatItem datItem = new Rom
            {
                Machine = machine,
                Source = source
            };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            datFile.AddItem(datItem, statsOnly: false);

            datFile.StripSceneDatesFromItems();

            // The name of the bucket is not expected to change
            DatItem actual = Assert.Single(datFile.GetItemsForBucket("10.10.10-machine-name"));
            Machine? actualMachine = actual.Machine;
            Assert.NotNull(actualMachine);
            Assert.Equal("machine-name", actualMachine.Name);
        }

        [Fact]
        public void StripSceneDatesFromItems_ItemsDB()
        {
            Machine machine = new Machine { Name = "10.10.10-machine-name" };

            DatFile datFile = new Logiqx(datFile: null, useGame: false);
            _ = datFile.AddMachineDB(machine);

            datFile.StripSceneDatesFromItems();

            Machine actualMachine = Assert.Single(datFile.GetMachinesDB()).Value;
            Assert.Equal("machine-name", actualMachine.Name);
        }

        #endregion
    }
}
