using System.Collections.Generic;
using SabreTools.Hashing;
using SabreTools.Metadata.DatItems;
using SabreTools.Metadata.DatItems.Formats;
using ItemStatus = SabreTools.Data.Models.Metadata.ItemStatus;

namespace SabreTools.Metadata.DatFiles
{
    /// <summary>
    /// Statistics wrapper for outputting
    /// </summary>
    public class DatStatistics
    {
        #region Private instance variables

        /// <summary>
        /// Number of items for each hash type
        /// </summary>
        private readonly Dictionary<HashType, long> _hashCounts = [];

        /// <summary>
        /// Number of items for each item type
        /// </summary>
        private readonly Dictionary<Data.Models.Metadata.ItemType, long> _itemCounts = [];

        /// <summary>
        /// Number of items for each item status
        /// </summary>
        private readonly Dictionary<ItemStatus, long> _statusCounts = [];

        /// <summary>
        /// Lock for statistics calculation
        /// </summary>
#if NET9_0_OR_GREATER
        private readonly System.Threading.Lock statsLock = new();
#else
        private readonly object statsLock = new();
#endif

        #endregion

        #region Properties

        /// <summary>
        /// Overall item count
        /// </summary>
        public long TotalCount { get; private set; } = 0;

        /// <summary>
        /// Number of machines
        /// </summary>
        /// <remarks>Special count only used by statistics output</remarks>
        public long GameCount { get; set; } = 0;

        /// <summary>
        /// Total uncompressed size
        /// </summary>
        public long TotalSize { get; private set; } = 0;

        /// <summary>
        /// Number of items with the remove flag
        /// </summary>
        public long RemovedCount { get; private set; } = 0;

        /// <summary>
        /// Name to display on output
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Total machine count to use on output
        /// </summary>
        public long MachineCount { get; set; }

        /// <summary>
        /// Determines if statistics are for a directory or not
        /// </summary>
        public readonly bool IsDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatStatistics()
        {
            DisplayName = null;
            MachineCount = 0;
            IsDirectory = false;
        }

        /// <summary>
        /// Constructor for aggregate data
        /// </summary>
        public DatStatistics(string? displayName, bool isDirectory)
        {
            DisplayName = displayName;
            MachineCount = 0;
            IsDirectory = isDirectory;
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Add to the statistics for a given DatItem
        /// </summary>
        /// <param name="item">Item to add info from</param>
        public void AddItemStatistics(DatItem item)
        {
            lock (statsLock)
            {
                // No matter what the item is, we increment the count
                TotalCount++;

                // Increment removal count
                if (item.RemoveFlag)
                    RemovedCount++;

                // Increment the item count for the type
                ModifyItemCount(item.ItemType, 1);

                // Some item types require special processing
                switch (item)
                {
                    case Disk disk:
                        AddItemStatistics(disk);
                        break;
                    case File file:
                        AddItemStatistics(file);
                        break;
                    case Media media:
                        AddItemStatistics(media);
                        break;
                    case Rom rom:
                        AddItemStatistics(rom);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Add statistics from another DatStatistics object
        /// </summary>
        /// <param name="stats">DatStatistics object to add from</param>
        public void AddStatistics(DatStatistics stats)
        {
            TotalCount += stats.TotalCount;

            // Loop through and add stats for all items
            foreach (var itemCountKvp in stats._itemCounts)
            {
                ModifyItemCount(itemCountKvp.Key, itemCountKvp.Value);
            }

            GameCount += stats.GameCount;

            TotalSize += stats.TotalSize;

            // Individual hash counts
            foreach (var hashCountKvp in stats._hashCounts)
            {
                ModifyHashCount(hashCountKvp.Key, hashCountKvp.Value);
            }

            // Individual status counts
            foreach (var statusCountKvp in stats._statusCounts)
            {
                ModifyStatusCount(statusCountKvp.Key, statusCountKvp.Value);
            }

            RemovedCount += stats.RemovedCount;
        }

        /// <summary>
        /// Get the item count for a given hash type, defaulting to 0 if it does not exist
        /// </summary>
        /// <param name="hashType">Hash type to retrieve</param>
        /// <returns>The number of items with that hash, if it exists</returns>
        public long GetHashCount(HashType hashType)
        {
            lock (_hashCounts)
            {
                if (!_hashCounts.TryGetValue(hashType, out long value))
                    return 0;

                return value;
            }
        }

        /// <summary>
        /// Get the item count for a given item type, defaulting to 0 if it does not exist
        /// </summary>
        /// <param name="itemType">Item type to retrieve</param>
        /// <returns>The number of items of that type, if it exists</returns>
        public long GetItemCount(Data.Models.Metadata.ItemType itemType)
        {
            lock (_itemCounts)
            {
                if (!_itemCounts.TryGetValue(itemType, out long value))
                    return 0;

                return value;
            }
        }

        /// <summary>
        /// Get the item count for a given item status, defaulting to 0 if it does not exist
        /// </summary>
        /// <param name="itemStatus">Item status to retrieve</param>
        /// <returns>The number of items of that type, if it exists</returns>
        public long GetStatusCount(ItemStatus itemStatus)
        {
            lock (_statusCounts)
            {
                if (!_statusCounts.TryGetValue(itemStatus, out long value))
                    return 0;

                return value;
            }
        }

        /// <summary>
        /// Remove from the statistics given a DatItem
        /// </summary>
        /// <param name="item">Item to remove info for</param>
        public void RemoveItemStatistics(DatItem item)
        {
            // If we have a null item, we can't do anything
            if (item is null)
                return;

            lock (statsLock)
            {
                // No matter what the item is, we decrease the count
                TotalCount--;

                // Decrement removal count
                if (item.RemoveFlag)
                    RemovedCount--;

                // Decrement the item count for the type
                ModifyItemCount(item.ItemType, -1);

                // Some item types require special processing
                switch (item)
                {
                    case Disk disk:
                        RemoveItemStatistics(disk);
                        break;
                    case File file:
                        RemoveItemStatistics(file);
                        break;
                    case Media media:
                        RemoveItemStatistics(media);
                        break;
                    case Rom rom:
                        RemoveItemStatistics(rom);
                        break;

                    default:
                        // Item type requires no special processing
                        break;
                }
            }
        }

        /// <summary>
        /// Reset all statistics
        /// </summary>
        public void ResetStatistics()
        {
            _hashCounts.Clear();
            _itemCounts.Clear();
            _statusCounts.Clear();

            TotalCount = 0;
            GameCount = 0;
            TotalSize = 0;
            RemovedCount = 0;
        }

        /// <summary>
        /// Add to the statistics for a given Disk
        /// </summary>
        /// <param name="disk">Item to add info from</param>
        private void AddItemStatistics(Disk disk)
        {
            ItemStatus? status = disk.Status;
            if (status != ItemStatus.Nodump)
            {
                ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(disk.MD5) ? 0 : 1);
                ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(disk.SHA1) ? 0 : 1);
            }

            ModifyStatusCount(ItemStatus.BadDump, status == ItemStatus.BadDump ? 1 : 0);
            ModifyStatusCount(ItemStatus.Good, status == ItemStatus.Good ? 1 : 0);
            ModifyStatusCount(ItemStatus.Nodump, status == ItemStatus.Nodump ? 1 : 0);
            ModifyStatusCount(ItemStatus.Verified, status == ItemStatus.Verified ? 1 : 0);
        }

        /// <summary>
        /// Add to the statistics for a given File
        /// </summary>
        /// <param name="file">Item to add info from</param>
        private void AddItemStatistics(File file)
        {
            TotalSize += file.Size ?? 0;
            ModifyHashCount(HashType.CRC32, string.IsNullOrEmpty(file.CRC) ? 0 : 1);
            ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(file.MD5) ? 0 : 1);
            ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(file.SHA1) ? 0 : 1);
            ModifyHashCount(HashType.SHA256, string.IsNullOrEmpty(file.SHA256) ? 0 : 1);
        }

        /// <summary>
        /// Add to the statistics for a given Media
        /// </summary>
        /// <param name="media">Item to add info from</param>
        private void AddItemStatistics(Media media)
        {
            ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(media.MD5) ? 0 : 1);
            ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(media.SHA1) ? 0 : 1);
            ModifyHashCount(HashType.SHA256, string.IsNullOrEmpty(media.SHA256) ? 0 : 1);
            ModifyHashCount(HashType.SpamSum, string.IsNullOrEmpty(media.SpamSum) ? 0 : 1);
        }

        /// <summary>
        /// Add to the statistics for a given Rom
        /// </summary>
        /// <param name="rom">Item to add info from</param>
        private void AddItemStatistics(Rom rom)
        {
            ItemStatus? status = rom.Status;
            if (status != ItemStatus.Nodump)
            {
                TotalSize += rom.Size ?? 0;
                ModifyHashCount(HashType.CRC16, string.IsNullOrEmpty(rom.CRC16) ? 0 : 1);
                ModifyHashCount(HashType.CRC32, string.IsNullOrEmpty(rom.CRC32) ? 0 : 1);
                ModifyHashCount(HashType.CRC64, string.IsNullOrEmpty(rom.CRC64) ? 0 : 1);
                ModifyHashCount(HashType.MD2, string.IsNullOrEmpty(rom.MD2) ? 0 : 1);
                ModifyHashCount(HashType.MD4, string.IsNullOrEmpty(rom.MD4) ? 0 : 1);
                ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(rom.MD5) ? 0 : 1);
                ModifyHashCount(HashType.RIPEMD128, string.IsNullOrEmpty(rom.RIPEMD128) ? 0 : 1);
                ModifyHashCount(HashType.RIPEMD160, string.IsNullOrEmpty(rom.RIPEMD160) ? 0 : 1);
                ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(rom.SHA1) ? 0 : 1);
                ModifyHashCount(HashType.SHA256, string.IsNullOrEmpty(rom.SHA256) ? 0 : 1);
                ModifyHashCount(HashType.SHA384, string.IsNullOrEmpty(rom.SHA384) ? 0 : 1);
                ModifyHashCount(HashType.SHA512, string.IsNullOrEmpty(rom.SHA512) ? 0 : 1);
                ModifyHashCount(HashType.SpamSum, string.IsNullOrEmpty(rom.SpamSum) ? 0 : 1);
            }

            ModifyStatusCount(ItemStatus.BadDump, status == ItemStatus.BadDump ? 1 : 0);
            ModifyStatusCount(ItemStatus.Good, status == ItemStatus.Good ? 1 : 0);
            ModifyStatusCount(ItemStatus.Nodump, status == ItemStatus.Nodump ? 1 : 0);
            ModifyStatusCount(ItemStatus.Verified, status == ItemStatus.Verified ? 1 : 0);
        }

        /// <summary>
        /// Remove from the statistics given a Disk
        /// </summary>
        /// <param name="disk">Item to remove info for</param>
        private void RemoveItemStatistics(Disk disk)
        {
            ItemStatus? status = disk.Status;
            if (status != ItemStatus.Nodump)
            {
                ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(disk.MD5) ? 0 : -1);
                ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(disk.SHA1) ? 0 : -1);
            }

            ModifyStatusCount(ItemStatus.BadDump, status == ItemStatus.BadDump ? -1 : 0);
            ModifyStatusCount(ItemStatus.Good, status == ItemStatus.Good ? -1 : 0);
            ModifyStatusCount(ItemStatus.Nodump, status == ItemStatus.Nodump ? -1 : 0);
            ModifyStatusCount(ItemStatus.Verified, status == ItemStatus.Verified ? -1 : 0);
        }

        /// <summary>
        /// Remove from the statistics given a File
        /// </summary>
        /// <param name="file">Item to remove info for</param>
        private void RemoveItemStatistics(File file)
        {
            TotalSize -= file.Size ?? 0;
            ModifyHashCount(HashType.CRC32, string.IsNullOrEmpty(file.CRC) ? 0 : -1);
            ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(file.MD5) ? 0 : -1);
            ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(file.SHA1) ? 0 : -1);
            ModifyHashCount(HashType.SHA256, string.IsNullOrEmpty(file.SHA256) ? 0 : -1);
        }

        /// <summary>
        /// Remove from the statistics given a Media
        /// </summary>
        /// <param name="media">Item to remove info for</param>
        private void RemoveItemStatistics(Media media)
        {
            ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(media.MD5) ? 0 : -1);
            ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(media.SHA1) ? 0 : -1);
            ModifyHashCount(HashType.SHA256, string.IsNullOrEmpty(media.SHA256) ? 0 : -1);
            ModifyHashCount(HashType.SpamSum, string.IsNullOrEmpty(media.SpamSum) ? 0 : -1);
        }

        /// <summary>
        /// Remove from the statistics given a Rom
        /// </summary>
        /// <param name="rom">Item to remove info for</param>
        private void RemoveItemStatistics(Rom rom)
        {
            ItemStatus? status = rom.Status;
            if (status != ItemStatus.Nodump)
            {
                TotalSize -= rom.Size ?? 0;
                ModifyHashCount(HashType.CRC16, string.IsNullOrEmpty(rom.CRC16) ? 0 : -1);
                ModifyHashCount(HashType.CRC32, string.IsNullOrEmpty(rom.CRC32) ? 0 : -1);
                ModifyHashCount(HashType.CRC64, string.IsNullOrEmpty(rom.CRC64) ? 0 : -1);
                ModifyHashCount(HashType.MD2, string.IsNullOrEmpty(rom.MD2) ? 0 : -1);
                ModifyHashCount(HashType.MD4, string.IsNullOrEmpty(rom.MD4) ? 0 : -1);
                ModifyHashCount(HashType.MD5, string.IsNullOrEmpty(rom.MD5) ? 0 : -1);
                ModifyHashCount(HashType.RIPEMD128, string.IsNullOrEmpty(rom.RIPEMD128) ? 0 : -1);
                ModifyHashCount(HashType.RIPEMD160, string.IsNullOrEmpty(rom.RIPEMD160) ? 0 : -1);
                ModifyHashCount(HashType.SHA1, string.IsNullOrEmpty(rom.SHA1) ? 0 : -1);
                ModifyHashCount(HashType.SHA256, string.IsNullOrEmpty(rom.SHA256) ? 0 : -1);
                ModifyHashCount(HashType.SHA384, string.IsNullOrEmpty(rom.SHA384) ? 0 : -1);
                ModifyHashCount(HashType.SHA512, string.IsNullOrEmpty(rom.SHA512) ? 0 : -1);
                ModifyHashCount(HashType.SpamSum, string.IsNullOrEmpty(rom.SpamSum) ? 0 : -1);
            }

            ModifyStatusCount(ItemStatus.BadDump, status == ItemStatus.BadDump ? -1 : 0);
            ModifyStatusCount(ItemStatus.Good, status == ItemStatus.Good ? -1 : 0);
            ModifyStatusCount(ItemStatus.Nodump, status == ItemStatus.Nodump ? -1 : 0);
            ModifyStatusCount(ItemStatus.Verified, status == ItemStatus.Verified ? -1 : 0);
        }

        /// <summary>
        /// Modify the hash count for a given hash type
        /// </summary>
        /// <param name="hashType">Hash type to change</param>
        /// <param name="interval">Amount to change by</param>
        private void ModifyHashCount(HashType hashType, long interval)
        {
            // Skip if the interval is 0
            if (interval == 0)
                return;

            lock (_hashCounts)
            {
                if (!_hashCounts.ContainsKey(hashType))
                    _hashCounts[hashType] = 0;

                _hashCounts[hashType] += interval;
                if (_hashCounts[hashType] < 0)
                    _hashCounts[hashType] = 0;
            }
        }

        /// <summary>
        /// Increment the item count for a given item type
        /// </summary>
        /// <param name="itemType">Item type to change</param>
        /// <param name="interval">Amount to change by</param>
        private void ModifyItemCount(Data.Models.Metadata.ItemType itemType, long interval)
        {
            // Skip if the interval is 0
            if (interval == 0)
                return;

            lock (_itemCounts)
            {
                if (!_itemCounts.ContainsKey(itemType))
                    _itemCounts[itemType] = 0;

                _itemCounts[itemType] += interval;
                if (_itemCounts[itemType] < 0)
                    _itemCounts[itemType] = 0;
            }
        }

        /// <summary>
        /// Increment the item count for a given item status
        /// </summary>
        /// <param name="itemStatus">Item type to change</param>
        /// <param name="interval">Amount to change by</param>
        private void ModifyStatusCount(ItemStatus itemStatus, long interval)
        {
            // Skip if the interval is 0
            if (interval == 0)
                return;

            lock (_statusCounts)
            {
                if (!_statusCounts.ContainsKey(itemStatus))
                    _statusCounts[itemStatus] = 0;

                _statusCounts[itemStatus] += interval;
                if (_statusCounts[itemStatus] < 0)
                    _statusCounts[itemStatus] = 0;
            }
        }

        #endregion
    }
}
