using System;
using System.IO;
using SabreTools.Models.N3DS;
using static SabreTools.Models.N3DS.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public class N3DS : WrapperBase<Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo 3DS Cart Image";

        #endregion

        #region Extension Properties

        /// <summary>
        /// Backup header
        /// </summary>
        public NCCHHeader? BackupHeader => Model.CardInfoHeader?.InitialData?.BackupHeader;

        /// <summary>
        /// Backup Write Wait Time (The time to wait to write save to backup after the card is recognized (0-255
        /// seconds)). NATIVE_FIRM loads this flag from the gamecard NCSD header starting with 6.0.0-11.
        /// </summary>
        public byte BackupWriteWaitTime
        {
            get
            {
                if (Model.Header?.PartitionFlags == null)
                    return default;

                return Model.Header.PartitionFlags[(int)NCSDFlags.BackupWriteWaitTime];
            }
        }

        /// <summary>
        /// Media Card Device (1 = NOR Flash, 2 = None, 3 = BT) (Only SDK 2.X)
        /// </summary>
        public MediaCardDeviceType MediaCardDevice2X
        {
            get
            {
                if (Model.Header?.PartitionFlags == null)
                    return default;

                return (MediaCardDeviceType)Model.Header.PartitionFlags[(int)NCSDFlags.MediaCardDevice2X];
            }
        }

        /// <summary>
        /// Media Card Device (1 = NOR Flash, 2 = None, 3 = BT) (SDK 3.X+)
        /// </summary>
        public MediaCardDeviceType MediaCardDevice3X
        {
            get
            {
                if (Model.Header?.PartitionFlags == null)
                    return default;

                return (MediaCardDeviceType)Model.Header.PartitionFlags[(int)NCSDFlags.MediaCardDevice3X];
            }
        }

        /// <summary>
        /// Media Platform Index (1 = CTR)
        /// </summary>
        public MediaPlatformIndex MediaPlatformIndex
        {
            get
            {
                if (Model.Header?.PartitionFlags == null)
                    return default;

                return (MediaPlatformIndex)Model.Header.PartitionFlags[(int)NCSDFlags.MediaPlatformIndex];
            }
        }

        /// <summary>
        /// Media Type Index (0 = Inner Device, 1 = Card1, 2 = Card2, 3 = Extended Device)
        /// </summary>
        public MediaTypeIndex MediaTypeIndex
        {
            get
            {
                if (Model.Header?.PartitionFlags == null)
                    return default;

                return (MediaTypeIndex)Model.Header.PartitionFlags[(int)NCSDFlags.MediaTypeIndex];
            }
        }

        /// <summary>
        /// Media unit size in bytes
        /// </summary>
        public uint MediaUnitSize
        {
            get
            {
                if (Model.Header?.PartitionFlags == null)
                    return default;

                return (uint)(0x200 * Math.Pow(2, Model.Header.PartitionFlags[(int)NCSDFlags.MediaUnitSize]));
            }
        }

        #region Partition Entries

        /// <summary>
        /// Partition table entry for Executable Content (CXI)
        /// </summary>
        public PartitionTableEntry? ExecutableContentEntry
        {
            get
            {
                if (Model.Header?.PartitionsTable == null)
                    return null;

                return Model.Header.PartitionsTable[0];
            }
        }

        /// <summary>
        /// Partition table entry for E-Manual (CFA)
        /// </summary>
        public PartitionTableEntry? EManualEntry
        {
            get
            {
                if (Model.Header?.PartitionsTable == null)
                    return null;

                return Model.Header.PartitionsTable[1];
            }
        }

        /// <summary>
        /// Partition table entry for Download Play Child container (CFA)
        /// </summary>
        public PartitionTableEntry? DownloadPlayChildContainerEntry
        {
            get
            {
                if (Model.Header?.PartitionsTable == null)
                    return null;

                return Model.Header.PartitionsTable[2];
            }
        }

        /// <summary>
        /// Partition table entry for New3DS Update Data (CFA)
        /// </summary>
        public PartitionTableEntry? New3DSUpdateDataEntry
        {
            get
            {
                if (Model.Header?.PartitionsTable == null)
                    return null;

                return Model.Header.PartitionsTable[6];
            }
        }

        /// <summary>
        /// Partition table entry for Update Data (CFA)
        /// </summary>
        public PartitionTableEntry? UpdateDataEntry
        {
            get
            {
                if (Model.Header?.PartitionsTable == null)
                    return null;

                return Model.Header.PartitionsTable[7];
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
        public N3DS(Cart? model, byte[]? data, int offset)
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
        public N3DS(Cart? model, Stream? data)
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a 3DS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS? Create(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a 3DS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS? Create(Stream? data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Deserializers.N3DS.DeserializeStream(data);
            if (archive == null)
                return null;

            try
            {
                return new N3DS(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Determines if a file header represents a CODE block
        /// </summary>
        public bool IsCodeBinary(int fsIndex, int headerIndex)
        {
            if (Model.ExeFSHeaders == null)
                return false;
            if (fsIndex < 0 || fsIndex >= Model.ExeFSHeaders.Length)
                return false;

            var fsHeader = Model.ExeFSHeaders[fsIndex];
            if (fsHeader?.FileHeaders == null)
                return false;

            if (headerIndex < 0 || headerIndex >= fsHeader.FileHeaders.Length)
                return false;

            var fileHeader = fsHeader.FileHeaders[headerIndex];
            if (fileHeader == null)
                return false;

            return fileHeader.FileName == ".code\0\0\0";
        }

        /// <summary>
        /// Get the initial value for the plain counter
        /// </summary>
        public byte[] PlainIV(int partitionIndex)
        {
            if (Model.Partitions == null)
                return [];
            if (partitionIndex < 0 || partitionIndex >= Model.Partitions.Length)
                return [];

            var header = Model.Partitions[partitionIndex];
            if (header == null)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            return [.. partitionIdBytes, .. PlainCounter];
        }

        /// <summary>
        /// Get the initial value for the ExeFS counter
        /// </summary>
        public byte[] ExeFSIV(int partitionIndex)
        {
            if (Model.Partitions == null)
                return [];
            if (partitionIndex < 0 || partitionIndex >= Model.Partitions.Length)
                return [];

            var header = Model.Partitions[partitionIndex];
            if (header == null)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            return [.. partitionIdBytes, .. ExefsCounter];
        }

        /// <summary>
        /// Get the initial value for the RomFS counter
        /// </summary>
        public byte[] RomFSIV(int partitionIndex)
        {
            if (Model.Partitions == null)
                return [];
            if (partitionIndex < 0 || partitionIndex >= Model.Partitions.Length)
                return [];

            var header = Model.Partitions[partitionIndex];
            if (header == null)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            return [.. partitionIdBytes, .. RomfsCounter];
        }

        /// <summary>
        /// Get if the NoCrypto bit is set
        /// </summary>
        public bool PossiblyDecrypted(int index)
        {
            if (Model.Partitions == null)
                return false;

            if (index < 0 || index >= Model.Partitions.Length)
                return false;

            var partition = Model.Partitions[index];
            if (partition?.Flags == null)
                return false;

#if NET20 || NET35
            return (partition.Flags.BitMasks & BitMasks.NoCrypto) != 0;
#else
            return partition.Flags.BitMasks.HasFlag(BitMasks.NoCrypto);
#endif
        }

        #endregion

        #region Offsets

        /// <summary>
        /// Get the offset of a partition ExeFS
        /// </summary>
        /// <returns>Offset to the ExeFS of the partition, 0 on error</returns>
        public uint GetExeFSOffset(int index)
        {
            // Empty partitions table means no size is available
            var partitionsTable = Model.Header?.PartitionsTable;
            if (partitionsTable == null)
                return 0;

            // Invalid partition table entry means no size is available
            var entry = partitionsTable[index];
            if (entry == null)
                return 0;

            // Empty partitions array means no size is available
            var partitions = Model.Partitions;
            if (partitions == null)
                return 0;

            // Invalid partition means no size is available
            var header = partitions[index];
            if (header == null)
                return 0;

            // If the offset is 0, return 0
            uint exeFsOffsetMU = header.ExeFSOffsetInMediaUnits;
            if (exeFsOffsetMU == 0)
                return 0;

            // Return the adjusted offset
            uint partitionOffsetMU = entry.Offset;
            return (partitionOffsetMU + exeFsOffsetMU) * MediaUnitSize;
        }

        /// <summary>
        /// Get the offset of a partition
        /// </summary>
        /// <returns>Offset to the partition, 0 on error</returns>
        public uint GetPartitionOffset(int index)
        {
            // Empty partitions table means no size is available
            var partitionsTable = Model.Header?.PartitionsTable;
            if (partitionsTable == null)
                return 0;

            // Invalid partition table entry means no size is available
            var entry = partitionsTable[index];
            if (entry == null)
                return 0;

            // Return the adjusted offset
            uint partitionOffsetMU = entry.Offset;
            if (entry.Offset == 0)
                return 0;

            // Return the adjusted offset
            return partitionOffsetMU * MediaUnitSize;
        }

        /// <summary>
        /// Get the offset of a partition RomFS
        /// </summary>
        /// <returns>Offset to the RomFS of the partition, 0 on error</returns>
        public uint GetRomFSOffset(int index)
        {
            // Empty partitions table means no size is available
            var partitionsTable = Model.Header?.PartitionsTable;
            if (partitionsTable == null)
                return 0;

            // Invalid partition table entry means no size is available
            var entry = partitionsTable[index];
            if (entry == null)
                return 0;

            // Empty partitions array means no size is available
            var partitions = Model.Partitions;
            if (partitions == null)
                return 0;

            // Invalid partition means no size is available
            var header = partitions[index];
            if (header == null)
                return 0;

            // If the offset is 0, return 0
            uint romFsOffsetMU = header.RomFSOffsetInMediaUnits;
            if (romFsOffsetMU == 0)
                return 0;

            // Return the adjusted offset
            uint partitionOffsetMU = entry.Offset;
            return (partitionOffsetMU + romFsOffsetMU) * MediaUnitSize;
        }

        #endregion

        #region Sizes

        /// <summary>
        /// Get the size of a partition ExeFS
        /// </summary>
        /// <returns>Size of the partition ExeFS in bytes, 0 on error</returns>
        public uint GetExeFSSize(int index)
        {
            // Empty partitions array means no size is available
            var partitions = Model.Partitions;
            if (partitions == null)
                return 0;

            // Invalid partition header means no size is available
            var header = partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.ExeFSSizeInMediaUnits * MediaUnitSize;
        }

        /// <summary>
        /// Get the size of a partition extended header
        /// </summary>
        /// <returns>Size of the partition extended header in bytes, 0 on error</returns>
        public uint GetExtendedHeaderSize(int index)
        {
            // Empty partitions array means no size is available
            var partitions = Model.Partitions;
            if (partitions == null)
                return 0;

            // Invalid partition header means no size is available
            var header = partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.ExtendedHeaderSizeInBytes;
        }

        /// <summary>
        /// Get the size of a partition RomFS
        /// </summary>
        /// <returns>Size of the partition RomFS in bytes, 0 on error</returns>
        public uint GetRomFSSize(int index)
        {
            // Empty partitions array means no size is available
            var partitions = Model.Partitions;
            if (partitions == null)
                return 0;

            // Invalid partition header means no size is available
            var header = partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.RomFSSizeInMediaUnits * MediaUnitSize;
        }

        #endregion
    }
}