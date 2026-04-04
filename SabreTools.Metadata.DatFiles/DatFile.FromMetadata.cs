using System;
using System.Collections.Generic;
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.Threading.Tasks;
#endif
using SabreTools.Metadata.Filter;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using MergingFlag = SabreTools.Data.Models.Metadata.MergingFlag;
using NodumpFlag = SabreTools.Data.Models.Metadata.NodumpFlag;
using PackingFlag = SabreTools.Data.Models.Metadata.PackingFlag;

#pragma warning disable IDE0056 // Use index operator
#pragma warning disable IDE0060 // Remove unused parameter
namespace SabreTools.Metadata.DatFiles
{
    public partial class DatFile
    {
        #region From Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        /// <param name="item">Metadata file to convert</param>
        /// <param name="filename">Name of the file to be parsed</param>
        /// <param name="indexId">Index ID for the DAT</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        internal void ConvertFromMetadata(Data.Models.Metadata.MetadataFile? item,
            string filename,
            int indexId,
            bool keep,
            bool statsOnly,
            FilterRunner? filterRunner)
        {
            // If the metadata file is invalid, we can't do anything
            if (item is null || item.Count == 0)
                return;

            // Create an internal source and add to the dictionary
            var source = new Source(indexId, filename);
            // long sourceIndex = AddSourceDB(source);

            // Get the header from the metadata
            var header = item.Read<Data.Models.Metadata.Header>(Data.Models.Metadata.MetadataFile.HeaderKey);
            if (header is not null)
                ConvertHeader(header, keep);

            // Get the machines from the metadata
            var machines = item.ReadArray<Data.Models.Metadata.Machine>(Data.Models.Metadata.MetadataFile.MachineKey);
            if (machines is not null)
                ConvertMachines(machines, source, sourceIndex: 0, statsOnly, filterRunner);
        }

        /// <summary>
        /// Convert header information
        /// </summary>
        /// <param name="item">Header to convert</param>
        /// <param name="keep">True if full pathnames are to be kept, false otherwise</param>
        private void ConvertHeader(Data.Models.Metadata.Header? item, bool keep)
        {
            // If the header is invalid, we can't do anything
            if (item is null || item.Count == 0)
                return;

            // Create an internal header
            var header = new DatHeader(item);
            Header.Name = header.Name;

            // Convert subheader values
            var canOpen = item.CanOpen;
            if (canOpen?.Extension is not null)
                Header.CanOpen = canOpen;

            var images = item.Images;
            if (images is not null)
                Header.Images = images;

            var infos = item.Infos;
            if (infos is not null)
                Header.Infos = infos;

            var newDat = item.NewDat;
            if (newDat is not null)
                Header.NewDat = newDat;

            var search = item.Search;
            if (search is not null)
                Header.Search = search;

            // Selectively set all possible fields -- TODO: Figure out how to make this less manual
            if (Header.Author is null)
                Header.Author = header.Author;
            if (Header.BiosMode == MergingFlag.None)
                Header.BiosMode = header.BiosMode;
            if (Header.Build is null)
                Header.Build = header.Build;
            if (Header.Category is null)
                Header.Category = header.Category;
            if (Header.Comment is null)
                Header.Comment = header.Comment;
            if (Header.Date is null)
                Header.Date = header.Date;
            if (Header.DatVersion is null)
                Header.DatVersion = header.DatVersion;
            if (Header.Debug is null)
                Header.Debug = header.Debug;
            if (Header.Description is null)
                Header.Description = header.Description;
            if (Header.Email is null)
                Header.Email = header.Email;
            if (Header.EmulatorVersion is null)
                Header.EmulatorVersion = header.EmulatorVersion;
            if (Header.ForceMerging == MergingFlag.None)
                Header.ForceMerging = header.ForceMerging;
            if (Header.ForceNodump == NodumpFlag.None)
                Header.ForceNodump = header.ForceNodump;
            if (Header.ForcePacking == PackingFlag.None)
                Header.ForcePacking = header.ForcePacking;
            if (Header.ForceZipping is null)
                Header.ForceZipping = header.ForceZipping;
            if (Header.HeaderSkipper is null)
                Header.HeaderSkipper = header.HeaderSkipper;
            if (Header.Homepage is null)
                Header.Homepage = header.Homepage;
            if (Header.Id is null)
                Header.Id = header.Id;
            if (Header.ImFolder is null)
                Header.ImFolder = header.ImFolder;
            if (Header.LockBiosMode is null)
                Header.LockBiosMode = header.LockBiosMode;
            if (Header.LockRomMode is null)
                Header.LockRomMode = header.LockRomMode;
            if (Header.LockSampleMode is null)
                Header.LockSampleMode = header.LockSampleMode;
            if (Header.MameConfig is null)
                Header.MameConfig = header.MameConfig;
            if (Header.Name is null)
                Header.Name = header.Name;
            if (Header.Notes is null)
                Header.Notes = header.Notes;
            if (Header.Plugin is null)
                Header.Plugin = header.Plugin;
            if (Header.RefName is null)
                Header.RefName = header.RefName;
            if (Header.RomMode == MergingFlag.None)
                Header.RomMode = header.RomMode;
            if (Header.RomTitle is null)
                Header.RomTitle = header.RomTitle;
            if (Header.RootDir is null)
                Header.RootDir = header.RootDir;
            if (Header.SampleMode == MergingFlag.None)
                Header.SampleMode = header.SampleMode;
            if (Header.ReadString(Data.Models.Metadata.Header.SchemaLocationKey) is null)
                Header.Write<string?>(Data.Models.Metadata.Header.SchemaLocationKey, header.ReadString(Data.Models.Metadata.Header.SchemaLocationKey));
            if (Header.ScreenshotsHeight is null)
                Header.ScreenshotsHeight = header.ScreenshotsHeight;
            if (Header.ScreenshotsWidth is null)
                Header.ScreenshotsWidth = header.ScreenshotsWidth;
            if (Header.System is null)
                Header.System = header.System;
            if (Header.Timestamp is null)
                Header.Timestamp = header.Timestamp;
            if (Header.Type is null)
                Header.Type = header.Type;
            if (Header.Url is null)
                Header.Url = header.Url;
            if (Header.Version is null)
                Header.Version = header.Version;

            // Handle implied SuperDAT
            if (Header.Name?.Contains(" - SuperDAT") == true && keep)
            {
                if (Header.Type is null)
                    Header.Type = "SuperDAT";
            }
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        /// <param name="items">Machine array to convert</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ConvertMachines(Data.Models.Metadata.Machine[]? items,
            Source source,
            long sourceIndex,
            bool statsOnly,
            FilterRunner? filterRunner)
        {
            // If the array is invalid, we can't do anything
            if (items is null || items.Length == 0)
                return;

            // Loop through the machines and add
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            Parallel.ForEach(items, machine =>
#else
            foreach (var machine in items)
#endif
            {
                ConvertMachine(machine, source, sourceIndex, statsOnly, filterRunner);
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            });
#else
            }
#endif
        }

        /// <summary>
        /// Convert machine information
        /// </summary>
        /// <param name="item">Machine to convert</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ConvertMachine(Data.Models.Metadata.Machine? item,
            Source source,
            long sourceIndex,
            bool statsOnly,
            FilterRunner? filterRunner)
        {
            // If the machine is invalid, we can't do anything
            if (item is null)
                return;

            // If the machine doesn't pass the filter
            if (filterRunner is not null && !filterRunner.Run(item))
                return;

            // Create an internal machine and add to the dictionary
            var machine = new Machine(item);
            // long machineIndex = AddMachineDB(machine);

            // Convert items in the machine
            if (item.Adjuster is not null)
            {
                var items = item.Adjuster ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Adjuster(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Archive is not null)
            {
                var items = item.Archive ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Archive(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.BiosSet is not null)
            {
                var items = item.BiosSet ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new BiosSet(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Chip is not null)
            {
                var items = item.Chip ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Chip(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Configuration is not null)
            {
                var items = item.Configuration ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Configuration(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Device is not null)
            {
                var items = item.Device ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Device(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.DeviceRef is not null)
            {
                var items = item.DeviceRef ?? [];
                // Do not filter these due to later use
                Array.ForEach(items, item =>
                {
                    var datItem = new DeviceRef(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.DipSwitch is not null)
            {
                var items = item.DipSwitch ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new DipSwitch(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Disk is not null)
            {
                var items = item.Disk ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Disk(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Display is not null)
            {
                var items = item.Display ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Display(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Driver is not null && filterRunner?.Run(item.Driver) != false)
            {
                var datItem = new Driver(item.Driver, machine, source);
                AddItem(datItem, statsOnly);
                // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
            }

            if (item.Dump is not null)
            {
                var items = item.Dump ?? [];
                for (int i = 0; i < items.Length; i++)
                {
                    var datItem = new Rom(items[i], machine, source, i);
                    if (datItem.Name is not null)
                    {
                        AddItem(datItem, statsOnly);
                        // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                    }
                }
            }

            if (item.Feature is not null)
            {
                var items = item.Feature ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Feature(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Info is not null)
            {
                var items = item.Info ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Info(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Input is not null && filterRunner?.Run(item.Input) != false)
            {
                var datItem = new Input(item.Input, machine, source);
                AddItem(datItem, statsOnly);
                // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
            }

            if (item.Media is not null)
            {
                var items = item.Media ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Media(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Part is not null)
            {
                var items = item.Part ?? [];
                ProcessItems(items, machine, machineIndex: 0, source, sourceIndex, statsOnly, filterRunner);
            }

            if (item.Port is not null)
            {
                var items = item.Port ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Port(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.RamOption is not null)
            {
                var items = item.RamOption ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new RamOption(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Release is not null)
            {
                var items = item.Release ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Release(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Rom is not null)
            {
                var items = item.Rom ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Rom(item, machine, source);
                    datItem.Source = source;
                    datItem.CopyMachineInformation(machine);

                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Sample is not null)
            {
                var items = item.Sample ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Sample(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.SharedFeat is not null)
            {
                var items = item.SharedFeat ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new SharedFeat(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Slot is not null)
            {
                var items = item.Slot ?? [];
                // Do not filter these due to later use
                Array.ForEach(items, item =>
                {
                    var datItem = new Slot(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.SoftwareList is not null)
            {
                var items = item.SoftwareList ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new SoftwareList(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.Sound is not null && filterRunner?.Run(item.Sound) != false)
            {
                var datItem = new Sound(item.Sound, machine, source);
                AddItem(datItem, statsOnly);
                // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
            }

            if (item.Video is not null)
            {
                var items = item.Video ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Display(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }
        }

        /// <summary>
        /// Convert Part information
        /// </summary>
        /// <param name="items">Array of internal items to convert</param>
        /// <param name="machine">Machine to use with the converted items</param>
        /// <param name="machineIndex">Index of the Machine to use with the converted items</param>
        /// <param name="source">Source to use with the converted items</param>
        /// <param name="sourceIndex">Index of the Source to use with the converted items</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <param name="filterRunner">Optional FilterRunner to filter items on parse</param>
        private void ProcessItems(Data.Models.Metadata.Part[] items,
            Machine machine,
            long machineIndex,
            Source source,
            long sourceIndex,
            bool statsOnly,
            FilterRunner? filterRunner)
        {
            // If the array is null or empty, return without processing
            if (items.Length == 0)
                return;

            // Loop through the items and add
            foreach (var item in items)
            {
                var partItem = new Part(item, machine, source);

                // Handle subitems
                var dataAreas = item.DataArea;
                if (dataAreas is not null)
                {
                    foreach (var dataArea in dataAreas)
                    {
                        var dataAreaItem = new DataArea(dataArea, machine, source);
                        var roms = dataArea.Rom;
                        if (roms is null)
                            continue;

                        // Handle "offset" roms
                        List<Rom> addRoms = [];
                        foreach (var rom in roms)
                        {
                            // If the item doesn't pass the filter
                            if (filterRunner is not null && !filterRunner.Run(rom))
                                continue;

                            // Convert the item
                            var romItem = new Rom(rom, machine, source);
                            long? size = romItem.Size;

                            // If the rom is a continue or ignore
                            Data.Models.Metadata.LoadFlag? loadFlag = rom.LoadFlag;
                            if (loadFlag is not null
                                && (loadFlag == Data.Models.Metadata.LoadFlag.Continue
                                    || loadFlag == Data.Models.Metadata.LoadFlag.Ignore))
                            {
                                var lastRom = addRoms[addRoms.Count - 1];
                                long? lastSize = lastRom.Size;
                                lastRom.Size = lastSize + size;
                                continue;
                            }

                            romItem.DataArea = dataAreaItem;
                            romItem.Part = partItem;

                            addRoms.Add(romItem);
                        }

                        // Add all of the adjusted roms
                        foreach (var romItem in addRoms)
                        {
                            AddItem(romItem, statsOnly);
                            // AddItemDB(romItem, machineIndex, sourceIndex, statsOnly);
                        }
                    }
                }

                var diskAreas = item.DiskArea;
                if (diskAreas is not null)
                {
                    foreach (var diskArea in diskAreas)
                    {
                        var diskAreaitem = new DiskArea(diskArea, machine, source);
                        var disks = diskArea.Disk;
                        if (disks is null)
                            continue;

                        foreach (var disk in disks)
                        {
                            // If the item doesn't pass the filter
                            if (filterRunner is not null && !filterRunner.Run(disk))
                                continue;

                            var diskItem = new Disk(disk, machine, source)
                            {
                                DiskArea = diskAreaitem,
                                Part = partItem,
                            };

                            AddItem(diskItem, statsOnly);
                            // AddItemDB(diskItem, machineIndex, sourceIndex, statsOnly);
                        }
                    }
                }

                var dipSwitches = item.DipSwitch;
                if (dipSwitches is not null)
                {
                    foreach (var dipSwitch in dipSwitches)
                    {
                        // If the item doesn't pass the filter
                        if (filterRunner is not null && !filterRunner.Run(dipSwitch))
                            continue;

                        var dipSwitchItem = new DipSwitch(dipSwitch, machine, source) { Part = partItem };

                        AddItem(dipSwitchItem, statsOnly);
                        // AddItemDB(dipSwitchItem, machineIndex, sourceIndex, statsOnly);
                    }
                }

                var partFeatures = item.Feature;
                if (partFeatures is not null)
                {
                    foreach (var partFeature in partFeatures)
                    {
                        // If the item doesn't pass the filter
                        if (filterRunner is not null && !filterRunner.Run(partFeature))
                            continue;

                        var partFeatureItem = new PartFeature(partFeature)
                        {
                            Part = partItem,
                            Source = source,
                        };
                        partFeatureItem.CopyMachineInformation(machine);

                        AddItem(partFeatureItem, statsOnly);
                        // AddItemDB(partFeatureItem, machineIndex, sourceIndex, statsOnly);
                    }
                }
            }
        }

        #endregion
    }
}
