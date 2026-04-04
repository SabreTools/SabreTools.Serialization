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
                metadataFile[Data.Models.Metadata.MetadataFile.HeaderKey] = header;

            // Convert and assign the machines
            var machines = ConvertMachines(ignoreblanks);
            if (machines is not null)
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey] = machines;

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
                metadataFile[Data.Models.Metadata.MetadataFile.HeaderKey] = header;

            // Convert and assign the machines
            var machines = ConvertMachinesDB(ignoreblanks);
            if (machines is not null)
                metadataFile[Data.Models.Metadata.MetadataFile.MachineKey] = machines;

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

                // Handle Trurip object, if it exists
                if (machine.ContainsKey(Data.Models.Metadata.Machine.TruripKey))
                {
                    var trurip = machine.Read<Trurip>(Data.Models.Metadata.Machine.TruripKey);
                    if (trurip is not null)
                    {
                        var truripItem = trurip.ConvertToLogiqx();
                        truripItem.Publisher = machine.Publisher;
                        truripItem.Year = machine.Year;
                        truripItem.Players = machine.Players;
                        truripItem.Source = machine.SourceFile;
                        truripItem.CloneOf = machine.CloneOf;
                        machine[Data.Models.Metadata.Machine.TruripKey] = truripItem;
                    }
                }

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
                            if (dipSwitch.Part is not null)
                                partMappings[dipSwitch.Part.GetInternalClone()] = dipSwitchItem;

                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = disk.GetInternalClone();
                            machine.Disk ??= [];
                            machine.Disk = [.. machine.Disk, diskItem];

                            // Add Part and DiskArea mappings
                            if (disk.Part is not null && disk.DiskArea is not null)
                            {
                                var partItemInternal = disk.Part.GetInternalClone();
                                partMappings[partItemInternal] = diskItem;
                                diskAreaMappings[partItemInternal] = (disk.DiskArea.GetInternalClone(), diskItem);
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
                            if (partFeature.Part is not null)
                                partMappings[partFeature.Part.GetInternalClone()] = partFeatureItem;

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
                            var romItem = ProcessItem(rom, machine);
                            machine.Rom ??= [];
                            machine.Rom = [.. machine.Rom, romItem];

                            // Add Part and DataArea mappings
                            if (rom.Part is not null && rom.DataArea is not null)
                            {
                                var partItemInternal = rom.Part.GetInternalClone();
                                partMappings[partItemInternal] = romItem;
                                dataAreaMappings[partItemInternal] = (rom.DataArea.GetInternalClone(), romItem);
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
                            partItems[partName] = [];

                        // Copy over string values
                        partItems[partName].Name = partName;
                        if (partItems[partName].Interface == null)
                            partItems[partName].Interface = partItem.Interface;

                        // Clear any empty fields
                        ClearEmptyKeys(partItems[partName]);

                        // If the item has a DataArea mapping
                        if (dataAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DataArea, Data.Models.Metadata.Rom) dataAreaMap))
                        {
                            // Get the mapped items
                            var (dataArea, romItem) = dataAreaMap;

                            // Clear any empty fields
                            ClearEmptyKeys(romItem);

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
                                    aggregateDataArea = [];
                                    aggregateDataArea.Endianness = dataArea.Endianness;
                                    aggregateDataArea.Name = dataArea.Name;
                                    aggregateDataArea.Size = dataArea.Size;
                                    aggregateDataArea.Width = dataArea.Width;
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDataArea);

                                // Get existing roms as a list
                                var romsArr = aggregateDataArea.Rom ?? [];
                                List<Data.Models.Metadata.Rom> roms = [.. romsArr];

                                // Add the rom to the data area
                                roms.Add(romItem);

                                // Assign back the roms
                                aggregateDataArea.Rom = roms.ToArray();

                                // Assign back the data area
                                if (dataAreaIndex > -1)
                                    dataAreas[dataAreaIndex] = aggregateDataArea;
                                else
                                    dataAreas.Add(aggregateDataArea);

                                // Assign back the data areas array
                                partItems[partName].DataArea = dataAreas.ToArray();
                            }
                        }

                        // If the item has a DiskArea mapping
                        if (diskAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DiskArea, Data.Models.Metadata.Disk) diskAreaMap))
                        {
                            // Get the mapped items
                            var (diskArea, diskItem) = diskAreaMap;

                            // Clear any empty fields
                            ClearEmptyKeys(diskItem);

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
                                    aggregateDiskArea = [];
                                    aggregateDiskArea.Name = diskArea.Name;
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDiskArea);

                                // Get existing disks as a list
                                var disksArr = aggregateDiskArea.Disk ?? [];
                                List<Data.Models.Metadata.Disk> disks = [.. disksArr];

                                // Add the disk to the data area
                                disks.Add(diskItem);

                                // Assign back the disks
                                aggregateDiskArea.Disk = disks.ToArray();

                                // Assign back the disk area
                                if (diskAreaIndex > -1)
                                    diskAreas[diskAreaIndex] = aggregateDiskArea;
                                else
                                    diskAreas.Add(aggregateDiskArea);

                                // Assign back the disk areas array
                                partItems[partName].DiskArea = diskAreas.ToArray();
                            }
                        }

                        // If the item is a DipSwitch
                        if (datItem is Data.Models.Metadata.DipSwitch dipSwitchItem)
                        {
                            // Get existing dipswitches as a list
                            var dipSwitchesArr = partItems[partName].DipSwitch ?? [];
                            List<Data.Models.Metadata.DipSwitch> dipSwitches = [.. dipSwitchesArr];

                            // Clear any empty fields
                            ClearEmptyKeys(dipSwitchItem);

                            // Add the dipswitch
                            dipSwitches.Add(dipSwitchItem);

                            // Assign back the dipswitches
                            partItems[partName].DipSwitch = dipSwitches.ToArray();
                        }

                        // If the item is a Feature
                        else if (datItem is Data.Models.Metadata.Feature featureItem)
                        {
                            // Get existing features as a list
                            var featuresArr = partItems[partName].Feature ?? [];
                            List<Data.Models.Metadata.Feature> features = [.. featuresArr];

                            // Clear any empty fields
                            ClearEmptyKeys(featureItem);

                            // Add the feature
                            features.Add(featureItem);

                            // Assign back the features
                            partItems[partName].Feature = features.ToArray();
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

                // Handle Trurip object, if it exists
                if (machine.ContainsKey(Data.Models.Metadata.Machine.TruripKey))
                {
                    var trurip = machine.Read<Trurip>(Data.Models.Metadata.Machine.TruripKey);
                    if (trurip is not null)
                    {
                        var truripItem = trurip.ConvertToLogiqx();
                        truripItem.Publisher = machine.Publisher;
                        truripItem.Year = machine.Year;
                        truripItem.Players = machine.Players;
                        truripItem.Source = machine.SourceFile;
                        truripItem.CloneOf = machine.CloneOf;
                        machine[Data.Models.Metadata.Machine.TruripKey] = truripItem;
                    }
                }

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
                            if (dipSwitch.Part is not null)
                                partMappings[dipSwitch.Part.GetInternalClone()] = dipSwitchItem;

                            break;
                        case DatItems.Formats.Disk disk:
                            var diskItem = disk.GetInternalClone();
                            machine.Disk ??= [];
                            machine.Disk = [.. machine.Disk, diskItem];

                            // Add Part and DiskArea mappings
                            if (disk.Part is not null && disk.DiskArea is not null)
                            {
                                var partItemInternal = disk.Part.GetInternalClone();
                                partMappings[partItemInternal] = diskItem;
                                diskAreaMappings[partItemInternal] = (disk.DiskArea.GetInternalClone(), diskItem);
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
                            if (partFeature.Part is not null)
                                partMappings[partFeature.Part.GetInternalClone()] = partFeatureItem;

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
                            var romItem = ProcessItem(rom, machine);
                            machine.Rom ??= [];
                            machine.Rom = [.. machine.Rom, romItem];

                            // Add Part and DataArea mappings
                            if (rom.Part is not null && rom.DataArea is not null)
                            {
                                var partItemInternal = rom.Part.GetInternalClone();
                                partMappings[partItemInternal] = romItem;
                                dataAreaMappings[partItemInternal] = (rom.DataArea.GetInternalClone(), romItem);
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
                            partItems[partName] = [];

                        // Copy over string values
                        partItems[partName].Name = partName;
                        if (partItems[partName].Interface == null)
                            partItems[partName].Interface = partItem.Interface;

                        // Clear any empty fields
                        ClearEmptyKeys(partItems[partName]);

                        // If the item has a DataArea mapping
                        if (dataAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DataArea, Data.Models.Metadata.Rom) dataAreaMap))
                        {
                            // Get the mapped items
                            var (dataArea, romItem) = dataAreaMap;

                            // Clear any empty fields
                            ClearEmptyKeys(romItem);

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
                                    aggregateDataArea = [];
                                    aggregateDataArea.Endianness = dataArea.Endianness;
                                    aggregateDataArea.Name = dataArea.Name;
                                    aggregateDataArea.Size = dataArea.Size;
                                    aggregateDataArea.Width = dataArea.Width;
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDataArea);

                                // Get existing roms as a list
                                var romsArr = aggregateDataArea.Rom ?? [];
                                List<Data.Models.Metadata.Rom> roms = [.. romsArr];

                                // Add the rom to the data area
                                roms.Add(romItem);

                                // Assign back the roms
                                aggregateDataArea.Rom = roms.ToArray();

                                // Assign back the data area
                                if (dataAreaIndex > -1)
                                    dataAreas[dataAreaIndex] = aggregateDataArea;
                                else
                                    dataAreas.Add(aggregateDataArea);

                                // Assign back the data areas array
                                partItems[partName].DataArea = dataAreas.ToArray();
                            }
                        }

                        // If the item has a DiskArea mapping
                        if (diskAreaMappings.TryGetValue(partItem, out (Data.Models.Metadata.DiskArea, Data.Models.Metadata.Disk) diskAreaMap))
                        {
                            // Get the mapped items
                            var (diskArea, diskItem) = diskAreaMap;

                            // Clear any empty fields
                            ClearEmptyKeys(diskItem);

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
                                    aggregateDiskArea = [];
                                    aggregateDiskArea.Name = diskArea.Name;
                                }

                                // Clear any empty fields
                                ClearEmptyKeys(aggregateDiskArea);

                                // Get existing disks as a list
                                var disksArr = aggregateDiskArea.Disk ?? [];
                                List<Data.Models.Metadata.Disk> disks = [.. disksArr];

                                // Add the disk to the data area
                                disks.Add(diskItem);

                                // Assign back the disks
                                aggregateDiskArea.Disk = disks.ToArray();

                                // Assign back the disk area
                                if (diskAreaIndex > -1)
                                    diskAreas[diskAreaIndex] = aggregateDiskArea;
                                else
                                    diskAreas.Add(aggregateDiskArea);

                                // Assign back the disk areas array
                                partItems[partName].DiskArea = diskAreas.ToArray();
                            }
                        }

                        // If the item is a DipSwitch
                        if (datItem is Data.Models.Metadata.DipSwitch dipSwitchItem)
                        {
                            // Get existing dipswitches as a list
                            var dipSwitchesArr = partItems[partName].DipSwitch ?? [];
                            List<Data.Models.Metadata.DipSwitch> dipSwitches = [.. dipSwitchesArr];

                            // Clear any empty fields
                            ClearEmptyKeys(dipSwitchItem);

                            // Add the dipswitch
                            dipSwitches.Add(dipSwitchItem);

                            // Assign back the dipswitches
                            partItems[partName].DipSwitch = dipSwitches.ToArray();
                        }

                        // If the item is a Feature
                        else if (datItem is Data.Models.Metadata.Feature featureItem)
                        {
                            // Get existing features as a list
                            var featuresArr = partItems[partName].Feature ?? [];
                            List<Data.Models.Metadata.Feature> features = [.. featuresArr];

                            // Clear any empty fields
                            ClearEmptyKeys(featureItem);

                            // Add the feature
                            features.Add(featureItem);

                            // Assign back the features
                            partItems[partName].Feature = features.ToArray();
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
        /// <remarks>
        /// This method is required because both a Rom and a Dump
        /// item might be created and added for a given Rom input.
        /// </remarks>
        private static Data.Models.Metadata.Rom ProcessItem(DatItems.Formats.Rom item, Data.Models.Metadata.Machine machine)
        {
            var romItem = item.GetInternalClone();

            // Create a Dump for every Rom that has a subtype
            switch (romItem.OpenMSXMediaType)
            {
                case OpenMSXSubType.Rom:
                    var dumpRom = new Data.Models.Metadata.Dump();
                    var rom = new Data.Models.Metadata.Rom();

                    rom.Name = romItem.Name;
                    rom[Data.Models.Metadata.Rom.OffsetKey] = romItem.ReadString(Data.Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Data.Models.Metadata.Rom.OffsetKey);
                    rom[Data.Models.Metadata.Rom.OpenMSXType] = romItem.ReadString(Data.Models.Metadata.Rom.OpenMSXType);
                    rom[Data.Models.Metadata.Rom.RemarkKey] = romItem.ReadString(Data.Models.Metadata.Rom.RemarkKey);
                    rom[Data.Models.Metadata.Rom.SHA1Key] = romItem.ReadString(Data.Models.Metadata.Rom.SHA1Key);
                    rom[Data.Models.Metadata.Rom.StartKey] = romItem.ReadString(Data.Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Data.Models.Metadata.Rom.OffsetKey);

                    dumpRom.Rom = rom;

                    var romOriginal = romItem.Read<DatItems.Formats.Original>("ORIGINAL");
                    if (romOriginal is not null)
                    {
                        var newOriginal = new Data.Models.Metadata.Original
                        {
                            Value = romOriginal.Value,
                            Content = romOriginal.Content,
                        };
                        dumpRom.Original = newOriginal;
                    }

                    machine.Dump ??= [];
                    machine.Dump = [.. machine.Dump, dumpRom];
                    break;

                case OpenMSXSubType.MegaRom:
                    var dumpMegaRom = new Data.Models.Metadata.Dump();
                    var megaRom = new Data.Models.Metadata.Rom();

                    megaRom.Name = romItem.Name;
                    megaRom[Data.Models.Metadata.Rom.OffsetKey] = romItem.ReadString(Data.Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Data.Models.Metadata.Rom.OffsetKey);
                    megaRom[Data.Models.Metadata.Rom.OpenMSXType] = romItem.ReadString(Data.Models.Metadata.Rom.OpenMSXType);
                    megaRom[Data.Models.Metadata.Rom.RemarkKey] = romItem.ReadString(Data.Models.Metadata.Rom.RemarkKey);
                    megaRom[Data.Models.Metadata.Rom.SHA1Key] = romItem.ReadString(Data.Models.Metadata.Rom.SHA1Key);
                    megaRom[Data.Models.Metadata.Rom.StartKey] = romItem.ReadString(Data.Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Data.Models.Metadata.Rom.OffsetKey);

                    dumpMegaRom.MegaRom = megaRom;

                    var megaRomOriginal = romItem.Read<DatItems.Formats.Original>("ORIGINAL");
                    if (megaRomOriginal is not null)
                    {
                        var newOriginal = new Data.Models.Metadata.Original
                        {
                            Value = megaRomOriginal.Value,
                            Content = megaRomOriginal.Content,
                        };
                        dumpMegaRom.Original = newOriginal;
                    }

                    machine.Dump ??= [];
                    machine.Dump = [.. machine.Dump, dumpMegaRom];
                    break;

                case OpenMSXSubType.SCCPlusCart:
                    var dumpSccPlusCart = new Data.Models.Metadata.Dump();
                    var sccPlusCart = new Data.Models.Metadata.Rom();

                    sccPlusCart.Name = romItem.Name;
                    sccPlusCart[Data.Models.Metadata.Rom.OffsetKey] = romItem.ReadString(Data.Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Data.Models.Metadata.Rom.OffsetKey);
                    sccPlusCart[Data.Models.Metadata.Rom.OpenMSXType] = romItem.ReadString(Data.Models.Metadata.Rom.OpenMSXType);
                    sccPlusCart[Data.Models.Metadata.Rom.RemarkKey] = romItem.ReadString(Data.Models.Metadata.Rom.RemarkKey);
                    sccPlusCart[Data.Models.Metadata.Rom.SHA1Key] = romItem.ReadString(Data.Models.Metadata.Rom.SHA1Key);
                    sccPlusCart[Data.Models.Metadata.Rom.StartKey] = romItem.ReadString(Data.Models.Metadata.Rom.StartKey) ?? romItem.ReadString(Data.Models.Metadata.Rom.OffsetKey);

                    dumpSccPlusCart.SCCPlusCart = sccPlusCart;

                    var sccPlusCartOriginal = romItem.Read<DatItems.Formats.Original>("ORIGINAL");
                    if (sccPlusCartOriginal is not null)
                    {
                        var newOriginal = new Data.Models.Metadata.Original
                        {
                            Value = sccPlusCartOriginal.Value,
                            Content = sccPlusCartOriginal.Content,
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

        /// <summary>
        /// Clear empty keys from a DictionaryBase object
        /// </summary>
        private static void ClearEmptyKeys(Data.Models.Metadata.DictionaryBase obj)
        {
            string[] fieldNames = [.. obj.Keys];
            foreach (string fieldName in fieldNames)
            {
                if (obj[fieldName] is null)
                    obj.Remove(fieldName);
            }
        }

        #endregion
    }
}
