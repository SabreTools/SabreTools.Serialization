using System;
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using SabreTools.Hashing;
using SabreTools.Logging;
using SabreTools.Text.Compare;
using SabreTools.Text.Extensions;
using ItemStatus = SabreTools.Data.Models.Metadata.ItemStatus;

/*
 * Planning Notes:
 *
 * In order for this in-memory "database" design to work, there need to be a few things:
 * - Feature parity with all existing item dictionary operations
 * - A way to transition between the two item dictionaries (a flag?)
 * - Helper methods that target the "database" version instead of assuming the standard dictionary
 *
 * Notable changes include:
 * - Separation of Machine from DatItem, leading to a mapping instead
 * - Adding machines to the dictionary distinctly from the items
 * - Having a separate "bucketing" system that only reorders indicies and not full items; quicker?
 * - Non-key-based add/remove of values; use explicit methods instead of dictionary-style accessors
*/

namespace SabreTools.Metadata.DatFiles
{
    /// <summary>
    /// Item dictionary with statistics, bucketing, and sorting
    /// </summary>
    [JsonObject("items"), XmlRoot("items")]
    public class ItemDatabase
    {
        #region Private Classes

        /// <summary>
        /// Represents a table with a string key and a list of indexes as a value
        /// </summary>
        private class GroupingTable
        {
            #region Private Fields

#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            private readonly ConcurrentDictionary<string, List<long>> _groupings = [];
#else
            private readonly Dictionary<string, List<long>> _groupings = [];
#endif

            #endregion

            #region Properties

            /// <summary>
            /// Indicates what the grouping keys represent
            /// </summary>
            public ItemKey GroupedBy { get; set; } = ItemKey.NULL;

            /// <summary>
            /// All grouping keys
            /// </summary>
            public string[] Keys => [.. _groupings.Keys];

            #endregion

            #region Accessors

            /// <summary>
            /// Add an index to a grouping
            /// </summary>
            public void Add(string key, long index)
            {
                lock (key)
                {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                    // If the key is missing from the dictionary, add it
                    _groupings.GetOrAdd(key, []);
                    if (!_groupings.TryGetValue(key, out var grouping) || grouping is null)
                        return;

                    grouping.Add(index);
#else
                    // If the key is missing from the dictionary, add it
                    if (!_groupings.ContainsKey(key))
                        _groupings[key] = [];

                    _groupings[key].Add(index);
#endif
                }
            }

            /// <summary>
            /// Remove all groupings
            /// </summary>
            public void Clear() => _groupings.Clear();

            /// <summary>
            /// Try to set a grouping by key and list of values
            /// </summary>
            /// <remarks>This overwrites anything existing</remarks>
            public bool TryAdd(string key, List<long> value)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                return _groupings.TryAdd(key, value);
#else
                _groupings[key] = value;
                return true;
#endif
            }

            /// <summary>
            /// Try to get a value by key, returning success
            /// </summary>
            public bool TryGet(string? key, out List<long>? value)
            {
                if (key is null)
                {
                    value = null;
                    return false;
                }

                return _groupings.TryGetValue(key, out value);
            }

            /// <summary>
            /// Try to remove a value by key, returning success
            /// </summary>
            public bool TryRemove(string key, out List<long>? value)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                return _groupings.TryRemove(key, out value);
#else
                if (!_groupings.ContainsKey(key))
                {
                    value = default;
                    return false;
                }

                value = _groupings[key];
                return _groupings.Remove(key);
#endif
            }

            #endregion
        }

        /// <summary>
        /// Represents a table with an incremental index as the key
        /// </summary>
        /// <typeparam name="T">Type of the row values</typeparam>
        private class IndexedTable<T>
        {
            #region Private Fields

            /// <summary>
            /// Internal dictionary for the table
            /// </summary>
            [JsonIgnore, XmlIgnore]
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            private readonly ConcurrentDictionary<long, T> _table = [];
#else
            private readonly Dictionary<long, T> _table = [];
#endif

            /// <summary>
            /// Current highest available index
            /// </summary>
            [JsonIgnore, XmlIgnore]
            private long _tableIndex = 0;

            #endregion

            #region Properties

            /// <summary>
            /// Indicates if the table is empty
            /// </summary>
#if NET20 || NET35
            public bool IsEmpty => _table.Count == 0;
#else
            public bool IsEmpty => _table.IsEmpty;
#endif

            /// <summary>
            /// Direct access to the internal table
            /// </summary>
            /// TODO: Investigate ways of avoiding this being needed
            public IDictionary<long, T> Table => _table;

            /// <summary>
            /// All currently used indexes
            /// </summary>
            public long[] Indexes => [.. _table.Keys];

            /// <summary>
            /// All values in the table
            /// </summary>
            public T[] Values => [.. _table.Values];

            #endregion

            #region Accessors

            /// <summary>
            /// Add a value to the table, returning the insert index
            /// </summary>
            public long Add(T value)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                long index = Interlocked.Increment(ref _tableIndex) - 1;
                _table.TryAdd(index, value);
                return index;
#else
                long index = _tableIndex++ - 1;
                _table[index] = value;
                return index;
#endif
            }

            /// <summary>
            /// Get a value from the table, null on error
            /// </summary>
            public T? Get(long index)
            {
                if (_table.TryGetValue(index, out var value))
                    return value;

                return default;
            }

            /// <summary>
            /// Remove a value from the table, returning success
            /// </summary>
            public bool Remove(long index)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                return _table.TryRemove(index, out var _);
#else
                return _table.Remove(index);
#endif
            }

            /// <summary>
            /// Remove all values that match a function
            /// </summary>
            public void RemoveAll(Func<T, bool> func)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                Parallel.For(0, Indexes.Length, i =>
#else
                for (int i = 0; i < Indexes.Length; i++)
#endif
                {
                    if (func(_table[i]))
                        Remove(i);
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                });
#else
                }
#endif
            }

            /// <summary>
            /// Set an indexed value directly
            /// </summary>
            /// <remarks>This does not increment the index so values may be overwritten</remarks>
            public bool Set(long index, T value)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                return _table.TryAdd(index, value);
#else
                _table[index] = value;
                return true;
#endif
            }

            /// <summary>
            /// Try to get a value by index, returning success
            /// </summary>
            public bool TryGet(long index, out T? value)
                => _table.TryGetValue(index, out value);

            /// <summary>
            /// Try to remove a value by index, returning success
            /// </summary>
            public bool TryRemove(long index, out T? value)
            {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                return _table.TryRemove(index, out value);
#else
                if (!_table.ContainsKey(index))
                {
                    value = default;
                    return false;
                }

                value = _table[index];
                return _table.Remove(index);
#endif
            }

            #endregion

            #region Search

            /// <summary>
            /// Indicates if an index is valid
            /// </summary>
            public bool ContainsIndex(long index) => _table.ContainsKey(index);

            /// <summary>
            /// Find an item based on a supplied function
            /// </summary>
            public KeyValuePair<long, T?> Find(Func<T, bool> func)
            {
                foreach (long i in Indexes)
                {
                    if (func(_table[i]))
                        return new KeyValuePair<long, T?>(i, _table[i]);
                }

                return new KeyValuePair<long, T?>(-1, default);
            }

            #endregion
        }

        #endregion

        #region Private instance variables

        /// <summary>
        /// Internal table for all items
        /// </summary>
        [JsonIgnore, XmlIgnore]
        private readonly IndexedTable<DatItem> _items = new();

        /// <summary>
        /// Internal table for all machines
        /// </summary>
        [JsonIgnore, XmlIgnore]
        private readonly IndexedTable<Machine> _machines = new();

        /// <summary>
        /// Internal table for all sources
        /// </summary>
        [JsonIgnore, XmlIgnore]
        private readonly IndexedTable<Source> _sources = new();

        /// <summary>
        /// Internal dictionary representing the current buckets
        /// </summary>
        [JsonIgnore, XmlIgnore]
        private readonly GroupingTable _buckets = new();

        /// <summary>
        /// Logging object
        /// </summary>
        private readonly Logger _logger;

        #endregion

        #region Properties

        /// <summary>
        /// Get the keys in sorted order from the file dictionary
        /// </summary>
        /// <returns>List of the keys in sorted order</returns>
        [JsonIgnore, XmlIgnore]
        public string[] SortedKeys
        {
            get
            {
                List<string> keys = [.. _buckets.Keys];
                keys.Sort(new NaturalComparer());
                return [.. keys];
            }
        }

        /// <summary>
        /// DAT statistics
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public DatStatistics DatStatistics { get; } = new DatStatistics();

        #endregion

        #region Constructors

        /// <summary>
        /// Generic constructor
        /// </summary>
        public ItemDatabase()
        {
            _logger = new Logger(this);
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Add a DatItem to the dictionary after validation
        /// </summary>
        /// <param name="item">Item data to validate</param>
        /// <param name="statsOnly">True to only add item statistics while parsing, false otherwise</param>
        /// <returns>The index for the added item, -1 on error</returns>
        public long AddItem(DatItem item, bool statsOnly)
        {
            // If we have a Disk, File, Media, or Rom, clean the hash data
            if (item is Disk disk)
            {
                // If the file has aboslutely no hashes, skip and log
                if (disk.Status != ItemStatus.Nodump
                    && string.IsNullOrEmpty(disk.MD5)
                    && string.IsNullOrEmpty(disk.SHA1))
                {
                    _logger.Verbose($"Incomplete entry for '{disk.Name}' will be output as nodump");
                    disk.Status = ItemStatus.Nodump;
                }

                item = disk;
            }
            else if (item is DatItems.Formats.File file)
            {
                // If the file has aboslutely no hashes, skip and log
                if (string.IsNullOrEmpty(file.CRC)
                    && string.IsNullOrEmpty(file.MD5)
                    && string.IsNullOrEmpty(file.SHA1)
                    && string.IsNullOrEmpty(file.SHA256))
                {
                    _logger.Verbose($"Incomplete entry for '{file.GetName()}' will be output as nodump");
                }

                item = file;
            }
            else if (item is Media media)
            {
                // If the file has aboslutely no hashes, skip and log
                if (string.IsNullOrEmpty(media.MD5)
                    && string.IsNullOrEmpty(media.SHA1)
                    && string.IsNullOrEmpty(media.SHA256)
                    && string.IsNullOrEmpty(media.SpamSum))
                {
                    _logger.Verbose($"Incomplete entry for '{media.Name}' will be output as nodump");
                }

                item = media;
            }
            else if (item is Rom rom)
            {
                long? size = rom.Size;

                // If we have the case where there is SHA-1 and nothing else, we don't fill in any other part of the data
                if (size is null && !string.IsNullOrEmpty(rom.SHA1))
                {
                    // No-op, just catch it so it doesn't go further
                    //logger.Verbose($"{Header.GetStringFieldValue(DatHeader.FileNameKey)}: Entry with only SHA-1 found - '{rom.Name}'");
                }

                // If we have a rom and it's missing size AND the hashes match a 0-byte file, fill in the rest of the info
                else if ((size == 0 || size is null)
                    && (string.IsNullOrEmpty(rom.CRC32) || rom.HasZeroHash()))
                {
                    rom.Size = 0;
                    rom.CRC16 = null; // HashType.CRC16.ZeroString
                    rom.CRC32 = HashType.CRC32.ZeroString;
                    rom.CRC64 = null; // HashType.CRC64.ZeroString
                    rom.MD2 = null; // HashType.MD2.ZeroString
                    rom.MD4 = null; // HashType.MD4.ZeroString
                    rom.MD5 = HashType.MD5.ZeroString;
                    rom.RIPEMD128 = null; // HashType.RIPEMD128.ZeroString
                    rom.RIPEMD160 = null; // HashType.RIPEMD160.ZeroString
                    rom.SHA1 = HashType.SHA1.ZeroString;
                    rom.SHA256 = null; // HashType.SHA256.ZeroString;
                    rom.SHA384 = null; // HashType.SHA384.ZeroString;
                    rom.SHA512 = null; // HashType.SHA512.ZeroString;
                    rom.SpamSum = null; // HashType.SpamSum.ZeroString;
                }

                // If the file has no size and it's not the above case, skip and log
                else if (rom.Status != ItemStatus.Nodump && (size == 0 || size is null))
                {
                    //logger.Verbose($"{Header.GetStringFieldValue(DatHeader.FileNameKey)}: Incomplete entry for '{rom.Name}' will be output as nodump");
                    rom.Status = ItemStatus.Nodump;
                }

                // If the file has a size but aboslutely no hashes, skip and log
                else if (rom.Status != ItemStatus.Nodump
                    && size is not null && size > 0
                    && !rom.HasHashes())
                {
                    //logger.Verbose($"{Header.GetStringFieldValue(DatHeader.FileNameKey)}: Incomplete entry for '{rom.Name}' will be output as nodump");
                    rom.Status = ItemStatus.Nodump;
                }

                item = rom;
            }

            // If only adding statistics, we add just item stats
            if (statsOnly)
            {
                DatStatistics.AddItemStatistics(item);
                return -1;
            }
            else
            {
                return AddItemInternal(item);
            }
        }

        /// <summary>
        /// Add a machine, returning the insert index
        /// </summary>
        public long AddMachine(Machine machine) => _machines.Add(machine);

        /// <summary>
        /// Add a source, returning the insert index
        /// </summary>
        public long AddSource(Source source) => _sources.Add(source);

        /// <summary>
        /// Remove all items marked for removal
        /// </summary>
        public void ClearMarked() => _items.RemoveAll(d => d.RemoveFlag);

        /// <summary>
        /// Get a item based on the index
        /// </summary>
        public DatItem? GetItem(long index) => _items.Get(index);

        /// <summary>
        /// Get the indices and items associated with a bucket name
        /// </summary>
        public Dictionary<long, DatItem> GetItemsForBucket(string? bucketName, bool filter = false)
        {
            // Get item indexes for the bucket
            if (!_buckets.TryGet(bucketName, out var itemIds) || itemIds is null)
                return [];

            // Get the items based on index
            var datItems = new Dictionary<long, DatItem>();
            foreach (long itemId in itemIds)
            {
                // Ignore missing IDs
                if (!_items.TryGet(itemId, out var datItem) || datItem is null)
                    continue;

                if (!filter || !datItem.RemoveFlag)
                    datItems[itemId] = datItem;
            }

            return datItems;
        }

        /// <summary>
        /// Get a machine based on the index
        /// </summary>
        public KeyValuePair<long, Machine?> GetMachine(long index)
        {
            if (!_machines.TryGet(index, out var machine))
                return new KeyValuePair<long, Machine?>(-1, null);

            return new KeyValuePair<long, Machine?>(index, machine);
        }

        /// <summary>
        /// Get a machine based on the name
        /// </summary>
        /// <remarks>This assume that all machines have unique names</remarks>
        public KeyValuePair<long, Machine?> GetMachine(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return new KeyValuePair<long, Machine?>(-1, null);

            var machine = _machines.Find(m => m.Name == name);
            return new KeyValuePair<long, Machine?>(machine.Key, machine.Value);
        }

        /// <summary>
        /// Get all machines and their indicies
        /// </summary>
        public Machine[] GetMachines() => _machines.Values;

        /// <summary>
        /// Get a source based on the index
        /// </summary>
        public KeyValuePair<long, Source?> GetSource(long index)
        {
            if (!_sources.TryGet(index, out var source))
                return new KeyValuePair<long, Source?>(-1, null);

            return new KeyValuePair<long, Source?>(index, source);
        }

        /// <summary>
        /// Remove a key from the file dictionary if it exists
        /// </summary>
        /// <param name="key">Key in the dictionary to remove</param>
        public bool RemoveBucket(string key)
        {
            bool removed = _buckets.TryRemove(key, out var list);
            if (list is null)
                return removed;

            foreach (var index in list)
            {
                if (!_items.TryGet(index, out var datItem) || datItem is null)
                    continue;

                RemoveItem(index);
            }

            return removed;
        }

        /// <summary>
        /// Remove an item, returning if it could be removed
        /// </summary>
        public bool RemoveItem(long itemIndex)
        {
            // If the key doesn't exist, return
            if (!_items.TryRemove(itemIndex, out var datItem))
                return false;

            // Remove statistics, if possible
            if (datItem is not null)
                DatStatistics.RemoveItemStatistics(datItem);

            return true;
        }

        /// <summary>
        /// Remove a machine, returning if it could be removed
        /// </summary>
        public bool RemoveMachine(long machineIndex)
        {
            if (!_machines.TryRemove(machineIndex, out _))
                return false;

            _items.RemoveAll(d => d.MachineIndex == machineIndex);
            return true;
        }

        /// <summary>
        /// Remove a machine, returning if it could be removed
        /// </summary>
        public bool RemoveMachine(string machineName)
        {
            if (string.IsNullOrEmpty(machineName))
                return false;

            var machine = _machines.Find(m => string.Equals(m.Name, machineName, StringComparison.OrdinalIgnoreCase));
            return RemoveMachine(machine.Key);
        }

        /// <summary>
        /// Add an item, returning the insert index
        /// </summary>
        internal long AddItemInternal(DatItem item)
        {
            // Add the item with a new index
            long index = _items.Add(item);

            // Add the item statistics
            DatStatistics.AddItemStatistics(item);

            // Add the item to the default bucket
            PerformItemBucketing(new KeyValuePair<long, DatItem>(index, item), _buckets.GroupedBy, lower: true, norename: true);

            // Return the used index
            return index;
        }

        #endregion

        #region Bucketing

        /// <summary>
        /// Update the bucketing dictionary
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased (default), false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// <returns></returns>
        public void BucketBy(ItemKey bucketBy, bool lower = true, bool norename = true)
        {
            // If the sorted type isn't the same, we want to sort the dictionary accordingly
            if (_buckets.GroupedBy != bucketBy && bucketBy != ItemKey.NULL)
            {
                _logger.User($"Organizing roms by {bucketBy}");
                PerformBucketing(bucketBy, lower, norename);
            }

            // Sort the dictionary to be consistent
            _logger.User($"Sorting roms by {bucketBy}");
            PerformSorting(norename);
        }

        /// <summary>
        /// Perform deduplication on the current sorted dictionary
        /// </summary>
        public void Deduplicate()
        {
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            Parallel.ForEach(SortedKeys, key =>
#else
            foreach (var key in SortedKeys)
#endif
            {
                // Get the possibly unsorted list
                List<KeyValuePair<long, DatItem>> sortedList = [.. GetItemsForBucket(key)];

                // Sort and merge the list
                Sort(ref sortedList, false);
                sortedList = Merge(sortedList);

                // Add the list back to the dictionary
                RemoveBucket(key);
                sortedList.ForEach(item => AddItemInternal(item.Value));
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            });
#else
            }
#endif
        }

        /// <summary>
        /// Return the duplicate status of two items
        /// </summary>
        /// <param name="selfItem">Current DatItem</param>
        /// <param name="selfSource">Source associated with this item</param>
        /// <param name="lastItem">DatItem to check against</param>
        /// <param name="lastSource">Source associated with the last item</param>
        /// <returns>The DupeType corresponding to the relationship between the two</returns>
        public DupeType GetDuplicateStatus(KeyValuePair<long, DatItem>? selfItem, Source? selfSource, KeyValuePair<long, DatItem>? lastItem, Source? lastSource)
        {
            DupeType output = 0x00;

            // If either item is null
            if (selfItem is null || lastItem is null)
                return output;

            // If we don't have a duplicate at all, return none
            if (!selfItem.Value.Value.Equals(lastItem.Value.Value))
                return output;

            // Get the machines for comparison
            var selfMachine = GetMachine(selfItem.Value.Value.MachineIndex).Value;
            string? selfMachineName = selfMachine?.Name;
            var lastMachine = GetMachine(lastItem.Value.Value.MachineIndex).Value;
            string? lastMachineName = lastMachine?.Name;

            // If the duplicate is external already
#if NET20 || NET35
            if ((lastItem.Value.Value.DupeType & DupeType.External) != 0)
#else
            if (lastItem.Value.Value.DupeType.HasFlag(DupeType.External))
#endif
                output |= DupeType.External;

            // If the duplicate should be external
            else if (lastSource?.Index != selfSource?.Index)
                output |= DupeType.External;

            // Otherwise, it's considered an internal dupe
            else
                output |= DupeType.Internal;

            // If the item and machine names match
            if (lastMachineName == selfMachineName && lastItem.Value.Value.GetName() == selfItem.Value.Value.GetName())
                output |= DupeType.All;

            // Otherwise, hash match is assumed
            else
                output |= DupeType.Hash;

            return output;
        }

        /// <summary>
        /// List all duplicates found in a DAT based on a DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>List of matched DatItem objects</returns>
        /// <remarks>This also sets the remove flag on any duplicates found</remarks>
        /// TODO: Figure out if removal should be a flag or just removed entirely
        internal Dictionary<long, DatItem> GetDuplicates(KeyValuePair<long, DatItem> datItem, bool sorted = false)
        {
            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return [];

            // We want to get the proper key for the DatItem, ignoring the index
            _ = SortAndGetKey(datItem, sorted);
            var machine = GetMachine(datItem.Value.MachineIndex);
            var source = GetSource(datItem.Value.SourceIndex);
            string key = datItem.Value.GetKey(_buckets.GroupedBy, machine.Value, source.Value);

            // If the key doesn't exist, return the empty list
            var items = GetItemsForBucket(key);
            if (items.Count == 0)
                return [];

            // Try to find duplicates
            Dictionary<long, DatItem> output = [];
            foreach (var rom in items)
            {
                // Skip items marked for removal
                if (rom.Value.RemoveFlag)
                    continue;

                // Mark duplicates for future removal
                if (datItem.Value.Equals(rom.Value))
                {
                    rom.Value.RemoveFlag = true;
                    output[rom.Key] = rom.Value;
                }
            }

            // Return any matching items
            return output;
        }

        /// <summary>
        /// Check if a DAT contains the given DatItem
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>True if it contains the rom, false otherwise</returns>
        internal bool HasDuplicates(KeyValuePair<long, DatItem> datItem, bool sorted = false)
        {
            // Check for an empty rom list first
            if (DatStatistics.TotalCount == 0)
                return false;

            // We want to get the proper key for the DatItem, ignoring the index
            _ = SortAndGetKey(datItem, sorted);
            var machine = GetMachine(datItem.Value.MachineIndex);
            var source = GetSource(datItem.Value.SourceIndex);
            string key = datItem.Value.GetKey(_buckets.GroupedBy, machine.Value, source.Value);

            // If the key doesn't exist
            var roms = GetItemsForBucket(key);
            if (roms is null || roms.Count == 0)
                return false;

            // Try to find duplicates
            return roms.Values.Any(datItem.Value.Equals);
        }

        /// <summary>
        /// Merge an arbitrary set of item pairs based on the supplied information
        /// </summary>
        /// <param name="itemMappings">List of pairs representing the items to be merged</param>
        private List<KeyValuePair<long, DatItem>> Merge(List<KeyValuePair<long, DatItem>> itemMappings)
        {
            // Check for null or blank roms first
            if (itemMappings is null || itemMappings.Count == 0)
                return [];

            // Create output list
            List<KeyValuePair<long, DatItem>> output = [];

            // Then deduplicate them by checking to see if data matches previous saved roms
            int nodumpCount = 0;
            foreach (var kvp in itemMappings)
            {
                long itemIndex = kvp.Key;
                DatItem datItem = kvp.Value;

                // If we don't have a Disk, File, Media, or Rom, we skip checking for duplicates
                if (datItem is not Disk && datItem is not DatItems.Formats.File && datItem is not Media && datItem is not Rom)
                    continue;

                // If it's a nodump, add and skip
                if (datItem is Rom rom && rom.Status == ItemStatus.Nodump)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    nodumpCount++;
                    continue;
                }
                else if (datItem is Disk disk && disk.Status == ItemStatus.Nodump)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    nodumpCount++;
                    continue;
                }

                // If it's the first non-nodump rom in the list, don't touch it
                if (output.Count == nodumpCount)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    continue;
                }

                // Find the index of the first duplicate, if one exists
                var datItemSource = GetSource(datItem.SourceIndex);
                int pos = output.FindIndex(lastItem =>
                {
                    var lastItemSource = GetSource(lastItem.Value.SourceIndex);
                    return GetDuplicateStatus(kvp, datItemSource.Value, lastItem, lastItemSource.Value) != 0x00;
                });
                if (pos < 0)
                {
                    output.Add(new KeyValuePair<long, DatItem>(itemIndex, datItem));
                    continue;
                }

                // Get the duplicate item
                long savedIndex = output[pos].Key;
                DatItem savedItem = output[pos].Value;
                var savedItemSource = GetSource(savedItem.SourceIndex);
                DupeType dupetype = GetDuplicateStatus(kvp, datItemSource.Value, output[pos], savedItemSource.Value);

                // Disks, Media, and Roms have more information to fill
                if (datItem is Disk diskItem && savedItem is Disk savedDisk)
                    savedDisk.FillMissingInformation(diskItem);
                else if (datItem is DatItems.Formats.File fileItem && savedItem is DatItems.Formats.File savedFile)
                    savedFile.FillMissingInformation(fileItem);
                else if (datItem is Media mediaItem && savedItem is Media savedMedia)
                    savedMedia.FillMissingInformation(mediaItem);
                else if (datItem is Rom romItem && savedItem is Rom savedRom)
                    savedRom.FillMissingInformation(romItem);

                savedItem.DupeType = dupetype;

                // Get the sources associated with the items
                var savedSource = GetSource(savedItem.SourceIndex);
                var itemSource = GetSource(datItem.SourceIndex);

                // Get the machines associated with the items
                var savedMachine = GetMachine(savedItem.MachineIndex);
                var itemMachine = GetMachine(datItem.MachineIndex);

                // If the current source has a lower ID than the saved, use the saved source
                if (itemSource.Value?.Index < savedSource.Value?.Index)
                {
                    datItem.SourceIndex = savedItem.SourceIndex;
                    _machines.Set(savedMachine.Key, (itemMachine.Value!.Clone() as Machine)!);
                    savedItem.SetName(datItem.GetName());
                }

                // If the saved machine is a child of the current machine, use the current machine instead
                if (savedMachine.Value!.CloneOf == itemMachine.Value!.Name
                    || savedMachine.Value!.RomOf == itemMachine.Value!.Name)
                {
                    _machines.Set(savedMachine.Key, (itemMachine.Value!.Clone() as Machine)!);
                    savedItem.SetName(datItem.GetName());
                }

                // Replace the original item in the list
                output.RemoveAt(pos);
                output.Insert(pos, new KeyValuePair<long, DatItem>(savedIndex, savedItem));
            }

            return output;
        }

        /// <summary>
        /// Get the highest-order Field value that represents the statistics
        /// </summary>
        private ItemKey GetBestAvailable()
        {
            // Get the required counts
            long diskCount = DatStatistics.GetItemCount(Data.Models.Metadata.ItemType.Disk);
            long mediaCount = DatStatistics.GetItemCount(Data.Models.Metadata.ItemType.Media);
            long romCount = DatStatistics.GetItemCount(Data.Models.Metadata.ItemType.Rom);
            long nodumpCount = DatStatistics.GetStatusCount(ItemStatus.Nodump);

            // If all items are supposed to have a SHA-512, we bucket by that
            if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA512))
                return ItemKey.SHA512;

            // If all items are supposed to have a SHA-384, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA384))
                return ItemKey.SHA384;

            // If all items are supposed to have a SHA-256, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA256))
                return ItemKey.SHA256;

            // If all items are supposed to have a SHA-1, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.SHA1))
                return ItemKey.SHA1;

            // If all items are supposed to have a RIPEMD160, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.RIPEMD160))
                return ItemKey.RIPEMD160;

            // If all items are supposed to have a RIPEMD128, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.RIPEMD128))
                return ItemKey.RIPEMD128;

            // If all items are supposed to have a MD5, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD5))
                return ItemKey.MD5;

            // If all items are supposed to have a MD4, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD4))
                return ItemKey.MD4;

            // If all items are supposed to have a MD2, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.MD2))
                return ItemKey.MD2;

            // If all items are supposed to have a CRC64, we bucket by that
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.CRC64))
                return ItemKey.CRC64;

            // If all items are supposed to have a CRC16, we bucket by that
            // TODO: This should really come after normal CRC
            else if (diskCount + mediaCount + romCount - nodumpCount == DatStatistics.GetHashCount(HashType.CRC16))
                return ItemKey.CRC16;

            // Otherwise, we bucket by CRC32
            else
                return ItemKey.CRC32;
        }

        /// <summary>
        /// Get the bucketing key for a given item index
        /// <param name="datItem">Current item</param>
        /// <param name="bucketBy">ItemKey value representing what key to get</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        /// </summary>
        private string GetBucketKey(DatItem datItem, ItemKey bucketBy, bool lower, bool norename)
        {
            var source = GetSource(datItem.SourceIndex);
            var machine = GetMachine(datItem.MachineIndex);

            // Treat NULL like machine
            if (bucketBy == ItemKey.NULL)
                bucketBy = ItemKey.Machine;

            // Get the bucket key
            return datItem.GetKey(bucketBy, machine.Value, source.Value, lower, norename);
        }

        /// <summary>
        /// Perform bucketing based on the item key provided
        /// </summary>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        private void PerformBucketing(ItemKey bucketBy, bool lower, bool norename)
        {
            // Reset the bucketing values
            _buckets.GroupedBy = bucketBy;
            _buckets.Clear();

            // Get the current list of item indicies
            long[] itemIndexes = _items.Indexes;

#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            Parallel.For(0, itemIndexes.Length, i =>
#else
            for (int i = 0; i < itemIndexes.Length; i++)
#endif
            {
                var datItem = GetItem(i);
                if (datItem is null)
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                    return;
#else
                    continue;
#endif

                PerformItemBucketing(new KeyValuePair<long, DatItem>(i, datItem), bucketBy, lower, norename);
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            });
#else
            }
#endif
        }

        /// <summary>
        /// Bucket a single DatItem
        /// </summary>
        /// <param name="datItem">Item to bucket</param>
        /// <param name="bucketBy">ItemKey enum representing how to bucket the individual items</param>
        /// <param name="lower">True if the key should be lowercased, false otherwise</param>
        /// <param name="norename">True if games should only be compared on game and file name, false if system and source are counted</param>
        private void PerformItemBucketing(KeyValuePair<long, DatItem> datItem, ItemKey bucketBy, bool lower, bool norename)
        {
            string? bucketKey = GetBucketKey(datItem.Value, bucketBy, lower, norename);
            _buckets.Add(bucketKey, datItem.Key);
        }

        /// <summary>
        /// Sort existing buckets for consistency
        /// </summary>
        private void PerformSorting(bool norename)
        {
            // Get the current list of bucket keys
            string[] bucketKeys = _buckets.Keys;

#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            Parallel.For(0, bucketKeys.Length, i =>
#else
            for (int i = 0; i < bucketKeys.Length; i++)
#endif
            {
                _buckets.TryGet(bucketKeys[i], out var itemIndices);
                if (itemIndices is null || itemIndices.Count == 0)
                {
                    _buckets.TryRemove(bucketKeys[i], out _);
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
                    return;
#else
                    continue;
#endif
                }

                var datItems = itemIndices
                    .FindAll(i => _items.ContainsIndex(i))
                    .ConvertAll(i => new KeyValuePair<long, DatItem>(i, _items.Get(i)!));

                Sort(ref datItems, norename);
                _buckets.TryAdd(bucketKeys[i], datItems.ConvertAll(kvp => kvp.Key));
#if NET40_OR_GREATER || NETCOREAPP || NETSTANDARD2_0_OR_GREATER
            });
#else
            }
#endif
        }

        /// <summary>
        /// Sort a list of item pairs by SourceID, Game, and Name (in order)
        /// </summary>
        /// <param name="itemMappings">List of pairs representing the items to be sorted</param>
        /// <param name="norename">True if files are not renamed, false otherwise</param>
        /// <returns>True if it sorted correctly, false otherwise</returns>
        private bool Sort(ref List<KeyValuePair<long, DatItem>> itemMappings, bool norename)
        {
            // Create the comparer extenal to the delegate
            var nc = new NaturalComparer();

            itemMappings.Sort(delegate (KeyValuePair<long, DatItem> x, KeyValuePair<long, DatItem> y)
            {
                try
                {
                    // Compare on source if renaming
                    if (!norename)
                    {
                        int xSourceIndex = GetSource(x.Value.SourceIndex).Value?.Index ?? 0;
                        int ySourceIndex = GetSource(y.Value.SourceIndex).Value?.Index ?? 0;
                        if (xSourceIndex != ySourceIndex)
                            return xSourceIndex - ySourceIndex;
                    }

                    // Get the machines
                    Machine? xMachine = GetMachine(x.Value.MachineIndex).Value;
                    Machine? yMachine = GetMachine(y.Value.MachineIndex).Value;

                    // If machine names don't match
                    string? xMachineName = xMachine?.Name;
                    string? yMachineName = yMachine?.Name;
                    if (xMachineName != yMachineName)
                        return nc.Compare(xMachineName, yMachineName);

                    // If types don't match
                    Data.Models.Metadata.ItemType xType = x.Value.ItemType;
                    Data.Models.Metadata.ItemType yType = y.Value.ItemType;
                    if (xType != yType)
                        return xType - yType;

                    // If directory names don't match
                    string? xDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName()));
                    string? yDirectoryName = Path.GetDirectoryName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName()));
                    if (xDirectoryName != yDirectoryName)
                        return nc.Compare(xDirectoryName, yDirectoryName);

                    // If item names don't match
                    string? xName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(x.Value.GetName()));
                    string? yName = Path.GetFileName(TextHelper.RemovePathUnsafeCharacters(y.Value.GetName()));
                    return nc.Compare(xName, yName);
                }
                catch
                {
                    // Absorb the error
                    return 0;
                }
            });

            return true;
        }

        /// <summary>
        /// Sort the input DAT and get the key to be used by the item
        /// </summary>
        /// <param name="datItem">Item to try to match</param>
        /// <param name="sorted">True if the DAT is already sorted accordingly, false otherwise (default)</param>
        /// <returns>Key to try to use</returns>
        private string SortAndGetKey(KeyValuePair<long, DatItem> datItem, bool sorted = false)
        {
            // If we're not already sorted, take care of it
            if (!sorted)
                BucketBy(GetBestAvailable());

            // Now that we have the sorted type, we get the proper key
            return GetBucketKey(datItem.Value, _buckets.GroupedBy, lower: true, norename: true);
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Recalculate the statistics for the Dat
        /// </summary>
        public void RecalculateStats()
        {
            // Wipe out any stats already there
            DatStatistics.ResetStatistics();

            // If there are no items
            if (_items.IsEmpty)
                return;

            // Loop through and add
            foreach (var item in _items.Values)
            {
                if (item is null)
                    continue;

                DatStatistics.AddItemStatistics(item);
            }
        }

        #endregion
    }
}
