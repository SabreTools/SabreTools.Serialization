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
            var canOpen = item.Read<Data.Models.OfflineList.CanOpen>(Data.Models.Metadata.Header.CanOpenKey);
            if (canOpen?.Extension is not null)
                Header.Write<string[]?>(Data.Models.Metadata.Header.CanOpenKey, canOpen.Extension);

            var images = item.Read<Data.Models.OfflineList.Images>(Data.Models.Metadata.Header.ImagesKey);
            if (images is not null)
                Header.Write<Data.Models.OfflineList.Images?>(Data.Models.Metadata.Header.ImagesKey, images);

            var infos = item.Read<Data.Models.OfflineList.Infos>(Data.Models.Metadata.Header.InfosKey);
            if (infos is not null)
                Header.Write<Data.Models.OfflineList.Infos?>(Data.Models.Metadata.Header.InfosKey, infos);

            var newDat = item.Read<Data.Models.OfflineList.NewDat>(Data.Models.Metadata.Header.NewDatKey);
            if (newDat is not null)
                Header.Write<Data.Models.OfflineList.NewDat?>(Data.Models.Metadata.Header.NewDatKey, newDat);

            var search = item.Read<Data.Models.OfflineList.Search>(Data.Models.Metadata.Header.SearchKey);
            if (search is not null)
                Header.Write<Data.Models.OfflineList.Search?>(Data.Models.Metadata.Header.SearchKey, search);

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
            if (Header.ReadString(Data.Models.Metadata.Header.HeaderKey) is null)
                Header.Write<string?>(Data.Models.Metadata.Header.HeaderKey, header.ReadString(Data.Models.Metadata.Header.HeaderKey));
            if (Header.Homepage is null)
                Header.Homepage = header.Homepage;
            if (Header.Id is null)
                Header.Id = header.Id;
            if (Header.ReadString(Data.Models.Metadata.Header.ImFolderKey) is null)
                Header.Write<string?>(Data.Models.Metadata.Header.ImFolderKey, header.ReadString(Data.Models.Metadata.Header.ImFolderKey));
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
            if (Header.ReadString(Data.Models.Metadata.Header.ScreenshotsHeightKey) is null)
                Header.Write<string?>(Data.Models.Metadata.Header.ScreenshotsHeightKey, header.ReadString(Data.Models.Metadata.Header.ScreenshotsHeightKey));
            if (Header.ReadString(Data.Models.Metadata.Header.ScreenshotsWidthKey) is null)
                Header.Write<string?>(Data.Models.Metadata.Header.ScreenshotsWidthKey, header.ReadString(Data.Models.Metadata.Header.ScreenshotsWidthKey));
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
            if (item is null || item.Count == 0)
                return;

            // If the machine doesn't pass the filter
            if (filterRunner is not null && !filterRunner.Run(item))
                return;

            // Create an internal machine and add to the dictionary
            var machine = new Machine(item);
            // long machineIndex = AddMachineDB(machine);

            // Convert items in the machine
            if (item.ContainsKey(Data.Models.Metadata.Machine.AdjusterKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Adjuster>(Data.Models.Metadata.Machine.AdjusterKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Adjuster(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.ArchiveKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Archive>(Data.Models.Metadata.Machine.ArchiveKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Archive(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.BiosSetKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.BiosSet>(Data.Models.Metadata.Machine.BiosSetKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new BiosSet(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.ChipKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Chip>(Data.Models.Metadata.Machine.ChipKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Chip(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.ConfigurationKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Configuration>(Data.Models.Metadata.Machine.ConfigurationKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Configuration(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DeviceKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Device>(Data.Models.Metadata.Machine.DeviceKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Device(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DeviceRefKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.DeviceRef>(Data.Models.Metadata.Machine.DeviceRefKey) ?? [];
                // Do not filter these due to later use
                Array.ForEach(items, item =>
                {
                    var datItem = new DeviceRef(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DipSwitchKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.DipSwitch>(Data.Models.Metadata.Machine.DipSwitchKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new DipSwitch(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DiskKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Disk>(Data.Models.Metadata.Machine.DiskKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Disk(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DisplayKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Display>(Data.Models.Metadata.Machine.DisplayKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Display(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DriverKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Driver>(Data.Models.Metadata.Machine.DriverKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Driver(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.DumpKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Dump>(Data.Models.Metadata.Machine.DumpKey) ?? [];
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

            if (item.ContainsKey(Data.Models.Metadata.Machine.FeatureKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Feature>(Data.Models.Metadata.Machine.FeatureKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Feature(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.InfoKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Info>(Data.Models.Metadata.Machine.InfoKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Info(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.InputKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Input>(Data.Models.Metadata.Machine.InputKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Input(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.MediaKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Media>(Data.Models.Metadata.Machine.MediaKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Media(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.PartKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Part>(Data.Models.Metadata.Machine.PartKey) ?? [];
                ProcessItems(items, machine, machineIndex: 0, source, sourceIndex, statsOnly, filterRunner);
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.PortKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Port>(Data.Models.Metadata.Machine.PortKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Port(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.RamOptionKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.RamOption>(Data.Models.Metadata.Machine.RamOptionKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new RamOption(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.ReleaseKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Release>(Data.Models.Metadata.Machine.ReleaseKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Release(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.RomKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Rom>(Data.Models.Metadata.Machine.RomKey) ?? [];
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

            if (item.ContainsKey(Data.Models.Metadata.Machine.SampleKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Sample>(Data.Models.Metadata.Machine.SampleKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Sample(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.SharedFeatKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.SharedFeat>(Data.Models.Metadata.Machine.SharedFeatKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new SharedFeat(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.SlotKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Slot>(Data.Models.Metadata.Machine.SlotKey) ?? [];
                // Do not filter these due to later use
                Array.ForEach(items, item =>
                {
                    var datItem = new Slot(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.SoftwareListKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.SoftwareList>(Data.Models.Metadata.Machine.SoftwareListKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new SoftwareList(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.SoundKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Sound>(Data.Models.Metadata.Machine.SoundKey) ?? [];
                var filtered = filterRunner is null ? items : Array.FindAll(items, i => filterRunner.Run(item));
                Array.ForEach(filtered, item =>
                {
                    var datItem = new Sound(item, machine, source);
                    AddItem(datItem, statsOnly);
                    // AddItemDB(datItem, machineIndex, sourceIndex, statsOnly);
                });
            }

            if (item.ContainsKey(Data.Models.Metadata.Machine.VideoKey))
            {
                var items = item.ReadArray<Data.Models.Metadata.Video>(Data.Models.Metadata.Machine.VideoKey) ?? [];
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
                var dataAreas = item.ReadArray<Data.Models.Metadata.DataArea>(Data.Models.Metadata.Part.DataAreaKey);
                if (dataAreas is not null)
                {
                    foreach (var dataArea in dataAreas)
                    {
                        var dataAreaItem = new DataArea(dataArea, machine, source);
                        var roms = dataArea.ReadArray<Data.Models.Metadata.Rom>(Data.Models.Metadata.DataArea.RomKey);
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
                            long? size = romItem.ReadLong(Data.Models.Metadata.Rom.SizeKey);

                            // If the rom is a continue or ignore
                            Data.Models.Metadata.LoadFlag? loadFlag = rom.LoadFlag;
                            if (loadFlag is not null
                                && (loadFlag == Data.Models.Metadata.LoadFlag.Continue
                                    || loadFlag == Data.Models.Metadata.LoadFlag.Ignore))
                            {
                                var lastRom = addRoms[addRoms.Count - 1];
                                long? lastSize = lastRom.ReadLong(Data.Models.Metadata.Rom.SizeKey);
                                lastRom.Write(Data.Models.Metadata.Rom.SizeKey, lastSize + size);
                                continue;
                            }

                            romItem.Write<DataArea?>(Rom.DataAreaKey, dataAreaItem);
                            romItem.Write<Part?>(Rom.PartKey, partItem);

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

                var diskAreas = item.ReadArray<Data.Models.Metadata.DiskArea>(Data.Models.Metadata.Part.DiskAreaKey);
                if (diskAreas is not null)
                {
                    foreach (var diskArea in diskAreas)
                    {
                        var diskAreaitem = new DiskArea(diskArea, machine, source);
                        var disks = diskArea.ReadArray<Data.Models.Metadata.Disk>(Data.Models.Metadata.DiskArea.DiskKey);
                        if (disks is null)
                            continue;

                        foreach (var disk in disks)
                        {
                            // If the item doesn't pass the filter
                            if (filterRunner is not null && !filterRunner.Run(disk))
                                continue;

                            var diskItem = new Disk(disk, machine, source);
                            diskItem.Write<DiskArea?>(Disk.DiskAreaKey, diskAreaitem);
                            diskItem.Write<Part?>(Disk.PartKey, partItem);

                            AddItem(diskItem, statsOnly);
                            // AddItemDB(diskItem, machineIndex, sourceIndex, statsOnly);
                        }
                    }
                }

                var dipSwitches = item.ReadArray<Data.Models.Metadata.DipSwitch>(Data.Models.Metadata.Part.DipSwitchKey);
                if (dipSwitches is not null)
                {
                    foreach (var dipSwitch in dipSwitches)
                    {
                        // If the item doesn't pass the filter
                        if (filterRunner is not null && !filterRunner.Run(dipSwitch))
                            continue;

                        var dipSwitchItem = new DipSwitch(dipSwitch, machine, source);
                        dipSwitchItem.Write<Part?>(DipSwitch.PartKey, partItem);

                        AddItem(dipSwitchItem, statsOnly);
                        // AddItemDB(dipSwitchItem, machineIndex, sourceIndex, statsOnly);
                    }
                }

                var partFeatures = item.ReadArray<Data.Models.Metadata.Feature>(Data.Models.Metadata.Part.FeatureKey);
                if (partFeatures is not null)
                {
                    foreach (var partFeature in partFeatures)
                    {
                        // If the item doesn't pass the filter
                        if (filterRunner is not null && !filterRunner.Run(partFeature))
                            continue;

                        var partFeatureItem = new PartFeature(partFeature);
                        partFeatureItem.Write<Part?>(DipSwitch.PartKey, partItem);
                        partFeatureItem.Source = source;
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
