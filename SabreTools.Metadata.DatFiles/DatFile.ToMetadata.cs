using System.Collections.Generic;
using System.Linq;
using SabreTools.Metadata.DatItems;
using OpenMSXSubType = SabreTools.Data.Models.Metadata.OpenMSXSubType;

namespace SabreTools.Metadata.DatFiles
{
    public partial class DatFile
    {
        #region To Metadata

        /// <summary>
        /// Convert metadata information
        /// </summary>
        internal Data.Models.Metadata.MetadataFile? ConvertToMetadata(bool ignoreblanks = false)
        {
            // If we don't have items, we can't do anything
            if (DatStatistics.TotalCount == 0)
                return null;

            // Create an object to hold the data
            var metadataFile = new Data.Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = Header.GetInternalClone();
            if (header is not null)
                metadataFile.Header = header;

            // Convert and assign the machines
            var machines = ConvertMachines(ignoreblanks);
            if (machines is not null)
                metadataFile.Machine = machines;

            return metadataFile;
        }

        /// <summary>
        /// Convert metadata information
        /// </summary>
        internal Data.Models.Metadata.MetadataFile? ConvertToMetadataDB(bool ignoreblanks = false)
        {
            // If we don't have items, we can't do anything
            if (ItemsDB.DatStatistics.TotalCount == 0)
                return null;

            // Create an object to hold the data
            var metadataFile = new Data.Models.Metadata.MetadataFile();

            // Convert and assign the header
            var header = Header.GetInternalClone();
            if (header is not null)
                metadataFile.Header = header;

            // Convert and assign the machines
            var machines = ConvertMachinesDB(ignoreblanks);
            if (machines is not null)
                metadataFile.Machine = machines;

            return metadataFile;
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        private Data.Models.Metadata.Machine[]? ConvertMachines(bool ignoreblanks = false)
        {
            // Create a machine list to hold all outputs
            List<Data.Models.Metadata.Machine> machines = [];

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.GetItemsForBucket(key, filter: true);
                if (items is null || items.Count == 0)
                    continue;

                // Create a machine to hold everything
                var machine = items[0].Machine!.GetInternalClone();

                // Create mapping dictionaries for the Parts, DataAreas, and DiskAreas associated with this machine
                Dictionary<Data.Models.Metadata.Part, Data.Models.Metadata.DatItem> partMappings = [];
                Dictionary<Data.Models.Metadata.Part, (Data.Models.Metadata.DataArea, Data.Models.Metadata.Rom)> dataAreaMappings = [];
                Dictionary<Data.Models.Metadata.Part, (Data.Models.Metadata.DiskArea, Data.Models.Metadata.Disk)> diskAreaMappings = [];

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case DatItems.Formats.Adjuster adjuster:
                            var adjusterItem = adjuster.GetInternalClone();
                            machine.Adjuster ??= [];
                            machine.Adjuster = [.. machine.Adjuster, adjusterItem];
                            break;
                        case DatItems.Formats.Archive archive:
                            var archiveItem = archive.GetInternalClone();
                            machine.Archive ??= [];
                            machine.Archive = [.. machine.Archive, archiveItem];
                            break;
                        case DatItems.Formats.BiosSet biosSet:
                            var biosSetItem = biosSet.GetInternalClone();
                            machine.BiosSet ??= [];
                            machine.BiosSet = [.. machine.BiosSet, biosSetItem];
                            break;
                        case DatItems.Formats.Chip chip:
                            var chipItem = chip.GetInternalClone();
                            machine.Chip ??= [];
                            machine.Chip = [.. machine.Chip, chipItem];
                            break;
                        case DatItems.Formats.Configuration configuration:
                            var configurationItem = configuration.GetInternalClone();
                            machine.Configuration ??= [];
                            machine.Configuration = [.. machine.Configuration, configurationItem];
                            break;
                        case DatItems.Formats.Device device:
                            var deviceItem = device.GetInternalClone();
                            machine.Device ??= [];
                            machine.Device = [.. machine.Device, deviceItem];
                            break;
                        case DatItems.Formats.DeviceRef deviceRef:
                            var deviceRefItem = deviceRef.GetInternalClone();
                            machine.DeviceRef ??= [];
                            machine.DeviceRef = [.. machine.DeviceRef, deviceRefItem];
                            break;
                        case DatItems.Formats.DipSwitch dipSwitch:
                            var dipSwitchItem = dipSwitch.GetInternalClone();
                            machine.DipSwitch ??= [];
                            machine.DipSwitch = [.. machine.DipSwitch, dipSwitchItem];

                            // Add Part mapping
                            if (dipSwitch.PartInterface is not null || dipSwitch.PartName is not null)
                                partMappings[new Data.Models.Metadata.Part { Interface = dipSwitch.PartInterface, Name = dipSwitch.PartName }] = dipSwitchItem;

                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = disk.GetInternalClone();
                            machine.Disk ??= [];
                            machine.Disk = [.. machine.Disk, diskItem];

                            // Add Part and DiskArea mappings
                            if ((disk.PartInterface is not null || disk.PartName is not null) && disk.DiskAreaName is not null)
                            {
                                var partItemInternal = new Data.Models.Metadata.Part
                                {
                                    Interface = disk.PartInterface,
                                    Name = disk.PartName,
                                };
                                partMappings[partItemInternal] = diskItem;
                                diskAreaMappings[partItemInternal] = (new Data.Models.Metadata.DiskArea { Name = disk.DiskAreaName }, diskItem);
                            }

                            break;
                        case DatItems.Formats.Display display:
                            var displayItem = ProcessItem(display, machine);
                            machine.Display ??= [];
                            machine.Display = [.. machine.Display, displayItem];
                            break;
                        case DatItems.Formats.Driver driver:
                            machine.Driver = driver.GetInternalClone();
                            break;
                        case DatItems.Formats.Feature feature:
                            var featureItem = feature.GetInternalClone();
                            machine.Feature ??= [];
                            machine.Feature = [.. machine.Feature, featureItem];
                            break;
                        case DatItems.Formats.Info info:
                            var infoItem = info.GetInternalClone();
                            machine.Info ??= [];
                            machine.Info = [.. machine.Info, infoItem];
                            break;
                        case DatItems.Formats.Input input:
                            machine.Input = input.GetInternalClone();
                            break;
                        case DatItems.Formats.Media media:
                            var mediaItem = media.GetInternalClone();
                            machine.Media ??= [];
                            machine.Media = [.. machine.Media, mediaItem];
                            break;
                        case DatItems.Formats.PartFeature partFeature:
                            var partFeatureItem = partFeature.GetInternalClone();
                            machine.Feature ??= [];
                            machine.Feature = [.. machine.Feature, partFeatureItem];

                            // Add Part mapping
                            if (partFeature.PartInterface is not null || partFeature.PartName is not null)
                                partMappings[new Data.Models.Metadata.Part { Interface = partFeature.PartInterface, Name = partFeature.PartName }] = partFeatureItem;

                            break;
                        case DatItems.Formats.Port port:
                            var portItem = port.GetInternalClone();
                            machine.Port ??= [];
                            machine.Port = [.. machine.Port, portItem];
                            break;
                        case DatItems.Formats.RamOption ramOption:
                            var ramOptionItem = ramOption.GetInternalClone();
                            machine.RamOption ??= [];
                            machine.RamOption = [.. machine.RamOption, ramOptionItem];
                            break;
                        case DatItems.Formats.Release release:
                            var releaseItem = release.GetInternalClone();
                            machine.Release ??= [];
                            machine.Release = [.. machine.Release, releaseItem];
                            break;
                        case DatItems.Formats.Rom rom:
                            var romItem = ProcessItem(rom, machine, rom.Original);
                            machine.Rom ??= [];
                            machine.Rom = [.. machine.Rom, romItem];

                            // Add Part and DataArea mappings
                            if ((rom.PartInterface is not null || rom.PartName is not null)
                                && (rom.DataAreaEndianness is not null || rom.DataAreaName is not null || rom.DataAreaSize is not null || rom.DataAreaWidth is not null))
                            {
                                var partItemInternal = new Data.Models.Metadata.Part
                                {
                                    Interface = rom.PartInterface,
                                    Name = rom.PartName,
                                };
                                var dataAreaItemInternal = new Data.Models.Metadata.DataArea
                                {
                                    Endianness = rom.DataAreaEndianness,
                                    Name = rom.DataAreaName,
                                    Size = rom.DataAreaSize,
                                    Width = rom.DataAreaWidth,
                                };
                                partMappings[partItemInternal] = romItem;
                                dataAreaMappings[partItemInternal] = (dataAreaItemInternal, romItem);
                            }

                            break;
                        case DatItems.Formats.Sample sample:
                            var sampleItem = sample.GetInternalClone();
                            machine.Sample ??= [];
                            machine.Sample = [.. machine.Sample, sampleItem];
                            break;
                        case DatItems.Formats.SharedFeat sharedFeat:
                            var sharedFeatItem = sharedFeat.GetInternalClone();
                            machine.SharedFeat ??= [];
                            machine.SharedFeat = [.. machine.SharedFeat, sharedFeatItem];
                            break;
                        case DatItems.Formats.Slot slot:
                            var slotItem = slot.GetInternalClone();
                            machine.Slot ??= [];
                            machine.Slot = [.. machine.Slot, slotItem];
                            break;
                        case DatItems.Formats.SoftwareList softwareList:
                            var softwareListItem = softwareList.GetInternalClone();
                            machine.SoftwareList ??= [];
                            machine.SoftwareList = [.. machine.SoftwareList, softwareListItem];
                            break;
                        case DatItems.Formats.Sound sound:
                            machine.Sound = sound.GetInternalClone();
                            break;
                        default:
                            // This should never happen
                            break;
                    }
                }

                // Handle Part, DiskItem, and DatItem mappings, if they exist
                if (partMappings.Count != 0)
                {
                    // Create a collection to hold the inverted Parts
                    Dictionary<string, Data.Models.Metadata.Part> partItems = [];

                    // Loop through the Part mappings
                    foreach (var partMapping in partMappings)
                    {
                        // Get the mapping pieces
                        var partItem = partMapping.Key;
                        var datItem = partMapping.Value;

                        // Get the part name and skip if there's none
                        string? partName = partItem.Name;
                        if (partName is null)
                            continue;

                        // Create the part in the dictionary, if needed
                        if (!partItems.ContainsKey(partName))
                            partItems[partName] = new();

                        // Copy over string values
                        partItems[partName].Name = partName;
                        if (partItems[partName].Interface == null)
                            partItems[partName].Interface = partItem.Interface;

                        // If the item has a DataArea mapping
                        if (dataAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DataArea, Data.Models.Metadata.Rom) dataAreaMap))
                        {
                            // Get the mapped items
                            var (dataArea, romItem) = dataAreaMap;

                            // Get the data area name and skip if there's none
                            string? dataAreaName = dataArea.Name;
                            if (dataAreaName is not null)
                            {
                                // Get existing data areas as a list
                                var dataAreasArr = partItems[partName].DataArea ?? [];
                                List<Data.Models.Metadata.DataArea> dataAreas = [.. dataAreasArr];

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int dataAreaIndex = dataAreas.FindIndex(da => da.Name == dataAreaName);
                                Data.Models.Metadata.DataArea aggregateDataArea;
                                if (dataAreaIndex > -1)
                                {
                                    aggregateDataArea = dataAreas[dataAreaIndex];
                                }
                                else
                                {
                                    aggregateDataArea = new()
                                    {
                                        Endianness = dataArea.Endianness,
                                        Name = dataArea.Name,
                                        Size = dataArea.Size,
                                        Width = dataArea.Width,
                                    };
                                }

                                // Add the rom to the data area
                                aggregateDataArea.Rom ??= [];
                                aggregateDataArea.Rom = [.. aggregateDataArea.Rom, romItem];

                                // Assign back the data area
                                if (dataAreaIndex > -1)
                                    dataAreas[dataAreaIndex] = aggregateDataArea;
                                else
                                    dataAreas.Add(aggregateDataArea);

                                // Assign back the data areas array
                                partItems[partName].DataArea = [.. dataAreas];
                            }
                        }

                        // If the item has a DiskArea mapping
                        if (diskAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DiskArea, Data.Models.Metadata.Disk) diskAreaMap))
                        {
                            // Get the mapped items
                            var (diskArea, diskItem) = diskAreaMap;

                            // Get the disk area name and skip if there's none
                            string? diskAreaName = diskArea.Name;
                            if (diskAreaName is not null)
                            {
                                // Get existing disk areas as a list
                                var diskAreasArr = partItems[partName].DiskArea ?? [];
                                List<Data.Models.Metadata.DiskArea> diskAreas = [.. diskAreasArr];

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int diskAreaIndex = diskAreas.FindIndex(da => da.Name == diskAreaName);
                                Data.Models.Metadata.DiskArea aggregateDiskArea;
                                if (diskAreaIndex > -1)
                                {
                                    aggregateDiskArea = diskAreas[diskAreaIndex];
                                }
                                else
                                {
                                    aggregateDiskArea = new Data.Models.Metadata.DiskArea { Name = diskArea.Name };
                                }

                                // Add the disk to the disk area
                                aggregateDiskArea.Disk ??= [];
                                aggregateDiskArea.Disk = [.. aggregateDiskArea.Disk, diskItem];

                                // Assign back the disk area
                                if (diskAreaIndex > -1)
                                    diskAreas[diskAreaIndex] = aggregateDiskArea;
                                else
                                    diskAreas.Add(aggregateDiskArea);

                                // Assign back the disk areas array
                                partItems[partName].DiskArea = [.. diskAreas];
                            }
                        }

                        // If the item is a DipSwitch
                        if (datItem is Data.Models.Metadata.DipSwitch dipSwitchItem)
                        {
                            partItems[partName].DipSwitch ??= [];
                            partItems[partName].DipSwitch = [.. partItems[partName].DipSwitch!, dipSwitchItem];
                        }

                        // If the item is a Feature
                        else if (datItem is Data.Models.Metadata.Feature featureItem)
                        {
                            partItems[partName].Feature ??= [];
                            partItems[partName].Feature = [.. partItems[partName].Feature!, featureItem];
                        }
                    }

                    // Assign the part array to the machine
                    machine.Part = [.. partItems.Values];
                }

                // Add the machine to the list
                machines.Add(machine);
            }

            // Return the list of machines
            return [.. machines];
        }

        /// <summary>
        /// Convert machines information
        /// </summary>
        private Data.Models.Metadata.Machine[]? ConvertMachinesDB(bool ignoreblanks = false)
        {
            // Create a machine list to hold all outputs
            List<Data.Models.Metadata.Machine> machines = [];

            // Loop through the sorted items and create games for them
            foreach (string key in ItemsDB.SortedKeys)
            {
                var items = GetItemsForBucketDB(key, filter: true);
                if (items is null || items.Count == 0)
                    continue;

                // Create a machine to hold everything
                var machine = GetMachineForItemDB(items.First().Key).Value!.GetInternalClone();

                // Create mapping dictionaries for the Parts, DataAreas, and DiskAreas associated with this machine
                Dictionary<Data.Models.Metadata.Part, Data.Models.Metadata.DatItem> partMappings = [];
                Dictionary<Data.Models.Metadata.Part, (Data.Models.Metadata.DataArea, Data.Models.Metadata.Rom)> dataAreaMappings = [];
                Dictionary<Data.Models.Metadata.Part, (Data.Models.Metadata.DiskArea, Data.Models.Metadata.Disk)> diskAreaMappings = [];

                // Loop through and convert the items to respective lists
                foreach (var kvp in items)
                {
                    // Check for a "null" item
                    var item = new KeyValuePair<long, DatItem>(kvp.Key, ProcessNullifiedItem(kvp.Value));

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item.Value, ignoreblanks))
                        continue;

                    switch (item.Value)
                    {
                        case DatItems.Formats.Adjuster adjuster:
                            var adjusterItem = adjuster.GetInternalClone();
                            machine.Adjuster ??= [];
                            machine.Adjuster = [.. machine.Adjuster, adjusterItem];
                            break;
                        case DatItems.Formats.Archive archive:
                            var archiveItem = archive.GetInternalClone();
                            machine.Archive ??= [];
                            machine.Archive = [.. machine.Archive, archiveItem];
                            break;
                        case DatItems.Formats.BiosSet biosSet:
                            var biosSetItem = biosSet.GetInternalClone();
                            machine.BiosSet ??= [];
                            machine.BiosSet = [.. machine.BiosSet, biosSetItem];
                            break;
                        case DatItems.Formats.Chip chip:
                            var chipItem = chip.GetInternalClone();
                            machine.Chip ??= [];
                            machine.Chip = [.. machine.Chip, chipItem];
                            break;
                        case DatItems.Formats.Configuration configuration:
                            var configurationItem = configuration.GetInternalClone();
                            machine.Configuration ??= [];
                            machine.Configuration = [.. machine.Configuration, configurationItem];
                            break;
                        case DatItems.Formats.Device device:
                            var deviceItem = device.GetInternalClone();
                            machine.Device ??= [];
                            machine.Device = [.. machine.Device, deviceItem];
                            break;
                        case DatItems.Formats.DeviceRef deviceRef:
                            var deviceRefItem = deviceRef.GetInternalClone();
                            machine.DeviceRef ??= [];
                            machine.DeviceRef = [.. machine.DeviceRef, deviceRefItem];
                            break;
                        case DatItems.Formats.DipSwitch dipSwitch:
                            var dipSwitchItem = dipSwitch.GetInternalClone();
                            machine.DipSwitch ??= [];
                            machine.DipSwitch = [.. machine.DipSwitch, dipSwitchItem];

                            // Add Part mapping
                            if (dipSwitch.PartInterface is not null || dipSwitch.PartName is not null)
                                partMappings[new Data.Models.Metadata.Part { Interface = dipSwitch.PartInterface, Name = dipSwitch.PartName }] = dipSwitchItem;

                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = disk.GetInternalClone();
                            machine.Disk ??= [];
                            machine.Disk = [.. machine.Disk, diskItem];

                            // Add Part and DiskArea mappings
                            if ((disk.PartInterface is not null || disk.PartName is not null) && disk.DiskAreaName is not null)
                            {
                                var partItemInternal = new Data.Models.Metadata.Part
                                {
                                    Interface = disk.PartInterface,
                                    Name = disk.PartName,
                                };
                                partMappings[partItemInternal] = diskItem;
                                diskAreaMappings[partItemInternal] = (new Data.Models.Metadata.DiskArea { Name = disk.DiskAreaName }, diskItem);
                            }

                            break;
                        case DatItems.Formats.Display display:
                            var displayItem = ProcessItem(display, machine);
                            machine.Display ??= [];
                            machine.Display = [.. machine.Display, displayItem];
                            break;
                        case DatItems.Formats.Driver driver:
                            machine.Driver = driver.GetInternalClone();
                            break;
                        case DatItems.Formats.Feature feature:
                            var featureItem = feature.GetInternalClone();
                            machine.Feature ??= [];
                            machine.Feature = [.. machine.Feature, featureItem];
                            break;
                        case DatItems.Formats.Info info:
                            var infoItem = info.GetInternalClone();
                            machine.Info ??= [];
                            machine.Info = [.. machine.Info, infoItem];
                            break;
                        case DatItems.Formats.Input input:
                            machine.Input = input.GetInternalClone();
                            break;
                        case DatItems.Formats.Media media:
                            var mediaItem = media.GetInternalClone();
                            machine.Media ??= [];
                            machine.Media = [.. machine.Media, mediaItem];
                            break;
                        case DatItems.Formats.PartFeature partFeature:
                            var partFeatureItem = partFeature.GetInternalClone();
                            machine.Feature ??= [];
                            machine.Feature = [.. machine.Feature, partFeatureItem];

                            // Add Part mapping
                            if (partFeature.PartInterface is not null || partFeature.PartName is not null)
                                partMappings[new Data.Models.Metadata.Part { Interface = partFeature.PartInterface, Name = partFeature.PartName }] = partFeatureItem;

                            break;
                        case DatItems.Formats.Port port:
                            var portItem = port.GetInternalClone();
                            machine.Port ??= [];
                            machine.Port = [.. machine.Port, portItem];
                            break;
                        case DatItems.Formats.RamOption ramOption:
                            var ramOptionItem = ramOption.GetInternalClone();
                            machine.RamOption ??= [];
                            machine.RamOption = [.. machine.RamOption, ramOptionItem];
                            break;
                        case DatItems.Formats.Release release:
                            var releaseItem = release.GetInternalClone();
                            machine.Release ??= [];
                            machine.Release = [.. machine.Release, releaseItem];
                            break;
                        case DatItems.Formats.Rom rom:
                            var romItem = ProcessItem(rom, machine, rom.Original);
                            machine.Rom ??= [];
                            machine.Rom = [.. machine.Rom, romItem];

                            // Add Part and DataArea mappings
                            if ((rom.PartInterface is not null || rom.PartName is not null)
                                && (rom.DataAreaEndianness is not null || rom.DataAreaName is not null || rom.DataAreaSize is not null || rom.DataAreaWidth is not null))
                            {
                                var partItemInternal = new Data.Models.Metadata.Part
                                {
                                    Interface = rom.PartInterface,
                                    Name = rom.PartName,
                                };
                                var dataAreaItemInternal = new Data.Models.Metadata.DataArea
                                {
                                    Endianness = rom.DataAreaEndianness,
                                    Name = rom.DataAreaName,
                                    Size = rom.DataAreaSize,
                                    Width = rom.DataAreaWidth,
                                };
                                partMappings[partItemInternal] = romItem;
                                dataAreaMappings[partItemInternal] = (dataAreaItemInternal, romItem);
                            }

                            break;
                        case DatItems.Formats.Sample sample:
                            var sampleItem = sample.GetInternalClone();
                            machine.Sample ??= [];
                            machine.Sample = [.. machine.Sample, sampleItem];
                            break;
                        case DatItems.Formats.SharedFeat sharedFeat:
                            var sharedFeatItem = sharedFeat.GetInternalClone();
                            machine.SharedFeat ??= [];
                            machine.SharedFeat = [.. machine.SharedFeat, sharedFeatItem];
                            break;
                        case DatItems.Formats.Slot slot:
                            var slotItem = slot.GetInternalClone();
                            machine.Slot ??= [];
                            machine.Slot = [.. machine.Slot, slotItem];
                            break;
                        case DatItems.Formats.SoftwareList softwareList:
                            var softwareListItem = softwareList.GetInternalClone();
                            machine.SoftwareList ??= [];
                            machine.SoftwareList = [.. machine.SoftwareList, softwareListItem];
                            break;
                        case DatItems.Formats.Sound sound:
                            machine.Sound = sound.GetInternalClone();
                            break;
                        default:
                            // This should never happen
                            break;
                    }
                }

                // Handle Part, DiskItem, and DatItem mappings, if they exist
                if (partMappings.Count != 0)
                {
                    // Create a collection to hold the inverted Parts
                    Dictionary<string, Data.Models.Metadata.Part> partItems = [];

                    // Loop through the Part mappings
                    foreach (var partMapping in partMappings)
                    {
                        // Get the mapping pieces
                        var partItem = partMapping.Key;
                        var datItem = partMapping.Value;

                        // Get the part name and skip if there's none
                        string? partName = partItem.Name;
                        if (partName is null)
                            continue;

                        // Create the part in the dictionary, if needed
                        if (!partItems.ContainsKey(partName))
                            partItems[partName] = new();

                        // Copy over string values
                        partItems[partName].Name = partName;
                        if (partItems[partName].Interface == null)
                            partItems[partName].Interface = partItem.Interface;

                        // If the item has a DataArea mapping
                        if (dataAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DataArea, Data.Models.Metadata.Rom) dataAreaMap))
                        {
                            // Get the mapped items
                            var (dataArea, romItem) = dataAreaMap;

                            // Get the data area name and skip if there's none
                            string? dataAreaName = dataArea.Name;
                            if (dataAreaName is not null)
                            {
                                // Get existing data areas as a list
                                var dataAreasArr = partItems[partName].DataArea ?? [];
                                List<Data.Models.Metadata.DataArea> dataAreas = [.. dataAreasArr];

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int dataAreaIndex = dataAreas.FindIndex(da => da.Name == dataAreaName);
                                Data.Models.Metadata.DataArea aggregateDataArea;
                                if (dataAreaIndex > -1)
                                {
                                    aggregateDataArea = dataAreas[dataAreaIndex];
                                }
                                else
                                {
                                    aggregateDataArea = new Data.Models.Metadata.DataArea
                                    {
                                        Endianness = dataArea.Endianness,
                                        Name = dataArea.Name,
                                        Size = dataArea.Size,
                                        Width = dataArea.Width,
                                    };
                                }

                                // Add the rom to the data area
                                aggregateDataArea.Rom ??= [];
                                aggregateDataArea.Rom = [.. aggregateDataArea.Rom, romItem];

                                // Assign back the data area
                                if (dataAreaIndex > -1)
                                    dataAreas[dataAreaIndex] = aggregateDataArea;
                                else
                                    dataAreas.Add(aggregateDataArea);

                                // Assign back the data areas array
                                partItems[partName].DataArea = [.. dataAreas];
                            }
                        }

                        // If the item has a DiskArea mapping
                        if (diskAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DiskArea, Data.Models.Metadata.Disk) diskAreaMap))
                        {
                            // Get the mapped items
                            var (diskArea, diskItem) = diskAreaMap;

                            // Get the disk area name and skip if there's none
                            string? diskAreaName = diskArea.Name;
                            if (diskAreaName is not null)
                            {
                                // Get existing disk areas as a list
                                var diskAreasArr = partItems[partName].DiskArea ?? [];
                                List<Data.Models.Metadata.DiskArea> diskAreas = [.. diskAreasArr];

                                // Find the existing disk area to append to, otherwise create a new disk area
                                int diskAreaIndex = diskAreas.FindIndex(da => da.Name == diskAreaName);
                                Data.Models.Metadata.DiskArea aggregateDiskArea;
                                if (diskAreaIndex > -1)
                                {
                                    aggregateDiskArea = diskAreas[diskAreaIndex];
                                }
                                else
                                {
                                    aggregateDiskArea = new Data.Models.Metadata.DiskArea { Name = diskArea.Name };
                                }

                                // Add the disk to the disk area
                                aggregateDiskArea.Disk ??= [];
                                aggregateDiskArea.Disk = [.. aggregateDiskArea.Disk, diskItem];

                                // Assign back the disk area
                                if (diskAreaIndex > -1)
                                    diskAreas[diskAreaIndex] = aggregateDiskArea;
                                else
                                    diskAreas.Add(aggregateDiskArea);

                                // Assign back the disk areas array
                                partItems[partName].DiskArea = [.. diskAreas];
                            }
                        }

                        // If the item is a DipSwitch
                        if (datItem is Data.Models.Metadata.DipSwitch dipSwitchItem)
                        {
                            partItems[partName].DipSwitch ??= [];
                            partItems[partName].DipSwitch = [.. partItems[partName].DipSwitch!, dipSwitchItem];
                        }

                        // If the item is a Feature
                        else if (datItem is Data.Models.Metadata.Feature featureItem)
                        {
                            partItems[partName].Feature ??= [];
                            partItems[partName].Feature = [.. partItems[partName].Feature!, featureItem];
                        }
                    }

                    // Assign the part array to the machine
                    machine.Part = [.. partItems.Values];
                }

                // Add the machine to the list
                machines.Add(machine);
            }

            // Return the list of machines
            return [.. machines];
        }

        /// <summary>
        /// Convert Display information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Video</param>
        /// <remarks>
        /// This method is required because both a Display and a Video
        /// item might be created and added for a given Display input.
        /// </remarks>
        private static Data.Models.Metadata.Display ProcessItem(DatItems.Formats.Display item, Data.Models.Metadata.Machine machine)
        {
            var displayItem = item.GetInternalClone();

            // Create a Video for any item that has specific fields
            if (displayItem.AspectX != null)
            {
                var videoItem = new Data.Models.Metadata.Video
                {
                    AspectX = displayItem.AspectX,
                    AspectY = displayItem.AspectY,
                    Height = displayItem.Height,
                    Orientation = displayItem.Rotate,
                    Refresh = displayItem.Refresh,
                    Screen = displayItem.DisplayType,
                    Width = displayItem.Width,
                };

                machine.Video ??= [];
                machine.Video = [.. machine.Video, videoItem];
            }

            return displayItem;
        }

        /// <summary>
        /// Convert Rom information
        /// </summary>
        /// <param name="item">Item to convert</param>
        /// <param name="machine">Machine to use for Part and DataArea</param>
        /// <param name="original">Original for OpenMSX</param>
        /// <remarks>
        /// This method is required because both a Rom and a Dump
        /// item might be created and added for a given Rom input.
        /// </remarks>
        private static Data.Models.Metadata.Rom ProcessItem(DatItems.Formats.Rom item, Data.Models.Metadata.Machine machine, DatItems.Formats.Original? original)
        {
            var romItem = item.GetInternalClone();

            // Create a Dump for every Rom that has a subtype
            switch (romItem.OpenMSXMediaType)
            {
                case OpenMSXSubType.Rom:
                    var rom = new Data.Models.Metadata.Rom
                    {
                        Name = romItem.Name,
                        Offset = romItem.Start ?? romItem.Offset,
                        OpenMSXType = romItem.OpenMSXType,
                        Remark = romItem.Remark,
                        SHA1 = romItem.SHA1,
                        Start = romItem.Start ?? romItem.Offset,
                    };

                    var dumpRom = new Data.Models.Metadata.Dump { Rom = rom };
                    if (original is not null)
                    {
                        var newOriginal = new Data.Models.Metadata.Original
                        {
                            Value = original.Value,
                            Content = original.Content,
                        };
                        dumpRom.Original = newOriginal;
                    }

                    machine.Dump ??= [];
                    machine.Dump = [.. machine.Dump, dumpRom];
                    break;

                case OpenMSXSubType.MegaRom:
                    var megaRom = new Data.Models.Metadata.Rom
                    {
                        Name = romItem.Name,
                        Offset = romItem.Start ?? romItem.Offset,
                        OpenMSXType = romItem.OpenMSXType,
                        Remark = romItem.Remark,
                        SHA1 = romItem.SHA1,
                        Start = romItem.Start ?? romItem.Offset,
                    };

                    var dumpMegaRom = new Data.Models.Metadata.Dump { MegaRom = megaRom };
                    if (original is not null)
                    {
                        var newOriginal = new Data.Models.Metadata.Original
                        {
                            Value = original.Value,
                            Content = original.Content,
                        };
                        dumpMegaRom.Original = newOriginal;
                    }

                    machine.Dump ??= [];
                    machine.Dump = [.. machine.Dump, dumpMegaRom];
                    break;

                case OpenMSXSubType.SCCPlusCart:
                    var sccPlusCart = new Data.Models.Metadata.Rom
                    {
                        Name = romItem.Name,
                        Offset = romItem.Start ?? romItem.Offset,
                        OpenMSXType = romItem.OpenMSXType,
                        Remark = romItem.Remark,
                        SHA1 = romItem.SHA1,
                        Start = romItem.Start ?? romItem.Offset,
                    };

                    var dumpSccPlusCart = new Data.Models.Metadata.Dump { SCCPlusCart = sccPlusCart };
                    if (original is not null)
                    {
                        var newOriginal = new Data.Models.Metadata.Original
                        {
                            Value = original.Value,
                            Content = original.Content,
                        };
                        dumpSccPlusCart.Original = newOriginal;
                    }

                    machine.Dump ??= [];
                    machine.Dump = [.. machine.Dump, dumpSccPlusCart];
                    break;
                default:
                    // This should never happen
                    break;
            }

            return romItem;
        }

        #endregion
    }
}
