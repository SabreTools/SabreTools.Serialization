using SabreTools.Hashing;
using SabreTools.Metadata.DatItems.Formats;
using Xunit;
using ItemStatus = SabreTools.Data.Models.Metadata.ItemStatus;

namespace SabreTools.Metadata.DatFiles.Test
{
    public class DatStatisticsTests
    {
        #region Constructor

        [Fact]
        public void DefaultConstructorTest()
        {
            var stats = new DatStatistics();

            Assert.Null(stats.DisplayName);
            Assert.Equal(0, stats.MachineCount);
            Assert.False(stats.IsDirectory);
        }

        [Fact]
        public void NamedConstructorTest()
        {
            var stats = new DatStatistics("name", isDirectory: true);

            Assert.Equal("name", stats.DisplayName);
            Assert.Equal(0, stats.MachineCount);
            Assert.True(stats.IsDirectory);
        }

        #endregion

        #region End to End

        [Fact]
        public void AddRemoveStatisticsTest()
        {
            // Get items for testing
            var disk = CreateDisk();
            var file = CreateFile();
            var media = CreateMedia();
            var rom = CreateRom();
            var sample = CreateSample();

            // Create an empty stats object
            var stats = new DatStatistics();

            // Validate pre-add values
            Assert.Equal(0, stats.TotalCount);
            Assert.Equal(0, stats.TotalSize);
            Assert.Equal(0, stats.GetHashCount(HashType.CRC32));
            Assert.Equal(0, stats.GetHashCount(HashType.MD2));
            Assert.Equal(0, stats.GetHashCount(HashType.MD4));
            Assert.Equal(0, stats.GetHashCount(HashType.MD5));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA1));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA256));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA384));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA512));
            Assert.Equal(0, stats.GetHashCount(HashType.SpamSum));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Disk));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.File));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Media));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Sample));
            Assert.Equal(0, stats.GetStatusCount(ItemStatus.Good));

            // AddItemStatistics
            stats.AddItemStatistics(disk);
            stats.AddItemStatistics(file);
            stats.AddItemStatistics(media);
            stats.AddItemStatistics(rom);
            stats.AddItemStatistics(sample);

            // Validate post-add values
            Assert.Equal(5, stats.TotalCount);
            Assert.Equal(2, stats.TotalSize);
            Assert.Equal(2, stats.GetHashCount(HashType.CRC32));
            Assert.Equal(1, stats.GetHashCount(HashType.MD2));
            Assert.Equal(1, stats.GetHashCount(HashType.MD4));
            Assert.Equal(4, stats.GetHashCount(HashType.MD5));
            Assert.Equal(1, stats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(1, stats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(4, stats.GetHashCount(HashType.SHA1));
            Assert.Equal(3, stats.GetHashCount(HashType.SHA256));
            Assert.Equal(1, stats.GetHashCount(HashType.SHA384));
            Assert.Equal(1, stats.GetHashCount(HashType.SHA512));
            Assert.Equal(2, stats.GetHashCount(HashType.SpamSum));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Disk));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.File));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Media));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Sample));
            Assert.Equal(2, stats.GetStatusCount(ItemStatus.Good));

            // RemoveItemStatistics
            stats.RemoveItemStatistics(disk);
            stats.RemoveItemStatistics(file);
            stats.RemoveItemStatistics(media);
            stats.RemoveItemStatistics(rom);
            stats.RemoveItemStatistics(sample);

            // Validate post-remove values
            Assert.Equal(0, stats.TotalCount);
            Assert.Equal(0, stats.TotalSize);
            Assert.Equal(0, stats.GetHashCount(HashType.CRC32));
            Assert.Equal(0, stats.GetHashCount(HashType.MD2));
            Assert.Equal(0, stats.GetHashCount(HashType.MD4));
            Assert.Equal(0, stats.GetHashCount(HashType.MD5));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA1));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA256));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA384));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA512));
            Assert.Equal(0, stats.GetHashCount(HashType.SpamSum));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Disk));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.File));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Media));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Sample));
            Assert.Equal(0, stats.GetStatusCount(ItemStatus.Good));
        }

        [Fact]
        public void ResetStatisticsTest()
        {
            // Get items for testing
            var disk = CreateDisk();
            var file = CreateFile();
            var media = CreateMedia();
            var rom = CreateRom();
            var sample = CreateSample();

            // Create an empty stats object
            var stats = new DatStatistics();

            // Validate pre-add values
            Assert.Equal(0, stats.TotalCount);
            Assert.Equal(0, stats.TotalSize);
            Assert.Equal(0, stats.GetHashCount(HashType.CRC32));
            Assert.Equal(0, stats.GetHashCount(HashType.MD2));
            Assert.Equal(0, stats.GetHashCount(HashType.MD4));
            Assert.Equal(0, stats.GetHashCount(HashType.MD5));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA1));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA256));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA384));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA512));
            Assert.Equal(0, stats.GetHashCount(HashType.SpamSum));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Disk));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.File));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Media));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Sample));
            Assert.Equal(0, stats.GetStatusCount(ItemStatus.Good));

            // AddItemStatistics
            stats.AddItemStatistics(disk);
            stats.AddItemStatistics(file);
            stats.AddItemStatistics(media);
            stats.AddItemStatistics(rom);
            stats.AddItemStatistics(sample);

            // Validate post-add values
            Assert.Equal(5, stats.TotalCount);
            Assert.Equal(2, stats.TotalSize);
            Assert.Equal(2, stats.GetHashCount(HashType.CRC32));
            Assert.Equal(1, stats.GetHashCount(HashType.MD2));
            Assert.Equal(1, stats.GetHashCount(HashType.MD4));
            Assert.Equal(4, stats.GetHashCount(HashType.MD5));
            Assert.Equal(1, stats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(1, stats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(4, stats.GetHashCount(HashType.SHA1));
            Assert.Equal(3, stats.GetHashCount(HashType.SHA256));
            Assert.Equal(1, stats.GetHashCount(HashType.SHA384));
            Assert.Equal(1, stats.GetHashCount(HashType.SHA512));
            Assert.Equal(2, stats.GetHashCount(HashType.SpamSum));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Disk));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.File));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Media));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(1, stats.GetItemCount(Data.Models.Metadata.ItemType.Sample));
            Assert.Equal(2, stats.GetStatusCount(ItemStatus.Good));

            // ResetStatistics
            stats.ResetStatistics();

            // Validate post-reset values
            Assert.Equal(0, stats.TotalCount);
            Assert.Equal(0, stats.TotalSize);
            Assert.Equal(0, stats.GetHashCount(HashType.CRC32));
            Assert.Equal(0, stats.GetHashCount(HashType.MD2));
            Assert.Equal(0, stats.GetHashCount(HashType.MD4));
            Assert.Equal(0, stats.GetHashCount(HashType.MD5));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(0, stats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA1));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA256));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA384));
            Assert.Equal(0, stats.GetHashCount(HashType.SHA512));
            Assert.Equal(0, stats.GetHashCount(HashType.SpamSum));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Disk));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.File));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Media));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(0, stats.GetItemCount(Data.Models.Metadata.ItemType.Sample));
            Assert.Equal(0, stats.GetStatusCount(ItemStatus.Good));
        }

        #endregion

        #region AddStatistics

        [Fact]
        public void AddStatisticsTest()
        {
            var rom = CreateRom();
            var origStats = new DatStatistics();
            origStats.AddItemStatistics(rom);

            var newStats = new DatStatistics();
            newStats.AddStatistics(origStats);

            Assert.Equal(1, newStats.TotalCount);
            Assert.Equal(1, newStats.TotalSize);
            Assert.Equal(1, newStats.GetHashCount(HashType.CRC32));
            Assert.Equal(1, newStats.GetHashCount(HashType.MD2));
            Assert.Equal(1, newStats.GetHashCount(HashType.MD4));
            Assert.Equal(1, newStats.GetHashCount(HashType.MD5));
            Assert.Equal(1, newStats.GetHashCount(HashType.RIPEMD128));
            Assert.Equal(1, newStats.GetHashCount(HashType.RIPEMD160));
            Assert.Equal(1, newStats.GetHashCount(HashType.SHA1));
            Assert.Equal(1, newStats.GetHashCount(HashType.SHA256));
            Assert.Equal(1, newStats.GetHashCount(HashType.SHA384));
            Assert.Equal(1, newStats.GetHashCount(HashType.SHA512));
            Assert.Equal(1, newStats.GetHashCount(HashType.SpamSum));
            Assert.Equal(1, newStats.GetItemCount(Data.Models.Metadata.ItemType.Rom));
            Assert.Equal(1, newStats.GetStatusCount(ItemStatus.Good));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Create a Disk for testing
        /// </summary>
        private static Disk CreateDisk()
        {
            var disk = new Disk { Status = ItemStatus.Good };
            disk.Write<string?>(Data.Models.Metadata.Disk.MD5Key, HashType.MD5.ZeroString);
            disk.Write<string?>(Data.Models.Metadata.Disk.SHA1Key, HashType.SHA1.ZeroString);

            return disk;
        }

        /// <summary>
        /// Create a File for testing
        /// </summary>
        private static File CreateFile()
        {
            var file = new File
            {
                Size = 1,
                CRC = HashType.CRC32.ZeroString,
                MD5 = HashType.MD5.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = HashType.SHA256.ZeroString
            };

            return file;
        }

        /// <summary>
        /// Create a Media for testing
        /// </summary>
        private static Media CreateMedia()
        {
            var media = new Media
            {
                MD5 = HashType.MD5.ZeroString,
                SHA1 = HashType.SHA1.ZeroString,
                SHA256 = HashType.SHA256.ZeroString,
                SpamSum = HashType.SpamSum.ZeroString
            };

            return media;
        }

        /// <summary>
        /// Create a Rom for testing
        /// </summary>
        private static Rom CreateRom()
        {
            var rom = new Rom
            {
                Status = ItemStatus.Good,
                Size = 1
            };
            rom.Write<string?>(Data.Models.Metadata.Rom.CRC16Key, HashType.CRC16.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.CRCKey, HashType.CRC32.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.CRC64Key, HashType.CRC64.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.MD2Key, HashType.MD2.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.MD4Key, HashType.MD4.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.MD5Key, HashType.MD5.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.RIPEMD128Key, HashType.RIPEMD128.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.RIPEMD160Key, HashType.RIPEMD160.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA1Key, HashType.SHA1.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA256Key, HashType.SHA256.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA384Key, HashType.SHA384.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.SHA512Key, HashType.SHA512.ZeroString);
            rom.Write<string?>(Data.Models.Metadata.Rom.SpamSumKey, HashType.SpamSum.ZeroString);

            return rom;
        }

        /// <summary>
        /// Create a Sample for testing
        /// </summary>
        private static Sample CreateSample() => new();

        #endregion
    }
}
