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
        /// ExeFS headers
        /// </summary>
        public ExeFSHeader[] ExeFSHeaders => Model.ExeFSHeaders ?? [];

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

        /// <summary>
        /// Partitions data table
        /// </summary>
        public NCCHHeader[] Partitions => Model.Partitions ?? [];

        /// <summary>
        /// Partitions header table
        /// </summary>
        public PartitionTableEntry[] PartitionsTable => Model.Header?.PartitionsTable ?? [];

        #region Named Partition Entries

        /// <summary>
        /// Partition table entry for Executable Content (CXI)
        /// </summary>
        public PartitionTableEntry? ExecutableContentEntry
        {
            get
            {
                if (PartitionsTable == null || PartitionsTable.Length == 0)
                    return null;

                return PartitionsTable[0];
            }
        }

        /// <summary>
        /// Partition table entry for E-Manual (CFA)
        /// </summary>
        public PartitionTableEntry? EManualEntry
        {
            get
            {
                if (PartitionsTable == null || PartitionsTable.Length == 0)
                    return null;

                return PartitionsTable[1];
            }
        }

        /// <summary>
        /// Partition table entry for Download Play Child container (CFA)
        /// </summary>
        public PartitionTableEntry? DownloadPlayChildContainerEntry
        {
            get
            {
                if (PartitionsTable == null || PartitionsTable.Length == 0)
                    return null;

                return PartitionsTable[2];
            }
        }

        /// <summary>
        /// Partition table entry for New3DS Update Data (CFA)
        /// </summary>
        public PartitionTableEntry? New3DSUpdateDataEntry
        {
            get
            {
                if (PartitionsTable == null || PartitionsTable.Length == 0)
                    return null;

                return PartitionsTable[6];
            }
        }

        /// <summary>
        /// Partition table entry for Update Data (CFA)
        /// </summary>
        public PartitionTableEntry? UpdateDataEntry
        {
            get
            {
                if (PartitionsTable == null || PartitionsTable.Length == 0)
                    return null;

                return PartitionsTable[7];
            }
        }

        #endregion

        /// <summary>
        /// Partitions flags
        /// </summary>
        public byte[] PartitionFlags => Model.Header?.PartitionFlags ?? [];

        #region Partition Flags

        /// <summary>
        /// Backup Write Wait Time (The time to wait to write save to backup after the card is recognized (0-255
        /// seconds)). NATIVE_FIRM loads this flag from the gamecard NCSD header starting with 6.0.0-11.
        /// </summary>
        public byte BackupWriteWaitTime
        {
            get
            {
                if (PartitionFlags == null || PartitionFlags.Length == 0)
                    return default;

                return PartitionFlags[(int)NCSDFlags.BackupWriteWaitTime];
            }
        }

        /// <summary>
        /// Media Card Device (1 = NOR Flash, 2 = None, 3 = BT) (Only SDK 2.X)
        /// </summary>
        public MediaCardDeviceType MediaCardDevice2X
        {
            get
            {
                if (PartitionFlags == null || PartitionFlags.Length == 0)
                    return default;

                return (MediaCardDeviceType)PartitionFlags[(int)NCSDFlags.MediaCardDevice2X];
            }
        }

        /// <summary>
        /// Media Card Device (1 = NOR Flash, 2 = None, 3 = BT) (SDK 3.X+)
        /// </summary>
        public MediaCardDeviceType MediaCardDevice3X
        {
            get
            {
                if (PartitionFlags == null || PartitionFlags.Length == 0)
                    return default;

                return (MediaCardDeviceType)PartitionFlags[(int)NCSDFlags.MediaCardDevice3X];
            }
        }

        /// <summary>
        /// Media Platform Index (1 = CTR)
        /// </summary>
        public MediaPlatformIndex MediaPlatformIndex
        {
            get
            {
                if (PartitionFlags == null || PartitionFlags.Length == 0)
                    return default;

                return (MediaPlatformIndex)PartitionFlags[(int)NCSDFlags.MediaPlatformIndex];
            }
        }

        /// <summary>
        /// Media Type Index (0 = Inner Device, 1 = Card1, 2 = Card2, 3 = Extended Device)
        /// </summary>
        public MediaTypeIndex MediaTypeIndex
        {
            get
            {
                if (PartitionFlags == null || PartitionFlags.Length == 0)
                    return default;

                return (MediaTypeIndex)PartitionFlags[(int)NCSDFlags.MediaTypeIndex];
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
            if (data == null || data.Length == 0)
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
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long currentOffset = data.Position;

                var model = Deserializers.N3DS.DeserializeStream(data);
                if (model == null)
                    return null;

                data.Seek(currentOffset, SeekOrigin.Begin);
                return new N3DS(model, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Get the bit masks for a partition
        /// </summary>
        public BitMasks GetBitMasks(int index)
        {
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            var partition = Partitions[index];
            if (partition?.Flags == null)
                return 0;

            return partition.Flags.BitMasks;
        }

        /// <summary>
        /// Get the crypto method for a partition
        /// </summary>
        public CryptoMethod GetCryptoMethod(int index)
        {
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            var partition = Partitions[index];
            if (partition?.Flags == null)
                return 0;

            return partition.Flags.CryptoMethod;
        }

        /// <summary>
        /// Determines if a file header represents a CODE block
        /// </summary>
        public bool IsCodeBinary(int fsIndex, int headerIndex)
        {
            if (ExeFSHeaders == null)
                return false;
            if (fsIndex < 0 || fsIndex >= ExeFSHeaders.Length)
                return false;

            var fsHeader = ExeFSHeaders[fsIndex];
            if (fsHeader?.FileHeaders == null)
                return false;

            if (headerIndex < 0 || headerIndex >= fsHeader.FileHeaders.Length)
                return false;

            var fileHeader = fsHeader.FileHeaders[headerIndex];
            if (fileHeader == null)
                return false;

            return fileHeader.FileName == ".code";
        }

        /// <summary>
        /// Get if the NoCrypto bit is set
        /// </summary>
        public bool PossiblyDecrypted(int index)
        {
            var bitMasks = GetBitMasks(index);
#if NET20 || NET35
            return (bitMasks & BitMasks.NoCrypto) != 0;
#else
            return bitMasks.HasFlag(BitMasks.NoCrypto);
#endif
        }

        #endregion

        #region Encryption

        /// <summary>
        /// Get the initial value for the ExeFS counter
        /// </summary>
        public byte[] ExeFSIV(int index)
        {
            if (Partitions == null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. ExefsCounter];
        }

        /// <summary>
        /// Get the initial value for the plain counter
        /// </summary>
        public byte[] PlainIV(int index)
        {
            if (Partitions == null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. PlainCounter];
        }

        /// <summary>
        /// Get the initial value for the RomFS counter
        /// </summary>
        public byte[] RomFSIV(int index)
        {
            if (Partitions == null)
                return [];
            if (index < 0 || index >= Partitions.Length)
                return [];

            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return [];

            byte[] partitionIdBytes = BitConverter.GetBytes(header.PartitionId);
            Array.Reverse(partitionIdBytes);
            return [.. partitionIdBytes, .. RomfsCounter];
        }

        #endregion

        #region Offsets

        /// <summary>
        /// Get the offset of a partition ExeFS
        /// </summary>
        /// <returns>Offset to the ExeFS of the partition, 0 on error</returns>
        public uint GetExeFSOffset(int index)
        {
            // No partitions means no size is available
            if (PartitionsTable == null || Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition means no size is available
            var entry = PartitionsTable[index];
            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
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
        /// Get the offset of a partition logo region
        /// </summary>
        /// <returns>Offset to the logo region of the partition, 0 on error</returns>
        public uint GetLogoRegionOffset(int index)
        {
            // No partitions means no size is available
            if (PartitionsTable == null || Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition means no size is available
            var entry = PartitionsTable[index];
            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return 0;

            // If the offset is 0, return 0
            uint logoOffsetMU = header.LogoRegionOffsetInMediaUnits;
            if (logoOffsetMU == 0)
                return 0;

            // Return the adjusted offset
            uint partitionOffsetMU = entry.Offset;
            return (partitionOffsetMU + logoOffsetMU) * MediaUnitSize;
        }

        /// <summary>
        /// Get the offset of a partition
        /// </summary>
        /// <returns>Offset to the partition, 0 on error</returns>
        public uint GetPartitionOffset(int index)
        {
            // No partitions means no size is available
            if (PartitionsTable == null)
                return 0;
            if (index < 0 || index >= PartitionsTable.Length)
                return 0;

            // Return the adjusted offset
            var entry = PartitionsTable[index];
            uint partitionOffsetMU = entry.Offset;
            if (entry.Offset == 0)
                return 0;

            // Return the adjusted offset
            return partitionOffsetMU * MediaUnitSize;
        }

        /// <summary>
        /// Get the offset of a partition plain region
        /// </summary>
        /// <returns>Offset to the plain region of the partition, 0 on error</returns>
        public uint GetPlainRegionOffset(int index)
        {
            // No partitions means no size is available
            if (PartitionsTable == null || Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition means no size is available
            var entry = PartitionsTable[index];
            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
                return 0;

            // If the offset is 0, return 0
            uint prOffsetMU = header.PlainRegionOffsetInMediaUnits;
            if (prOffsetMU == 0)
                return 0;

            // Return the adjusted offset
            uint partitionOffsetMU = entry.Offset;
            return (partitionOffsetMU + prOffsetMU) * MediaUnitSize;
        }

        /// <summary>
        /// Get the offset of a partition RomFS
        /// </summary>
        /// <returns>Offset to the RomFS of the partition, 0 on error</returns>
        public uint GetRomFSOffset(int index)
        {
            // No partitions means no size is available
            if (PartitionsTable == null || Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition means no size is available
            var entry = PartitionsTable[index];
            var header = Partitions[index];
            if (header == null || header.MagicID != NCCHMagicNumber)
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
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition header means no size is available
            var header = Partitions[index];
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
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition header means no size is available
            var header = Partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.ExtendedHeaderSizeInBytes;
        }

        /// <summary>
        /// Get the size of a partition logo region
        /// </summary>
        /// <returns>Size of the partition logo region in bytes, 0 on error</returns>
        public uint GetLogoRegionSize(int index)
        {
            // Empty partitions array means no size is available
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition header means no size is available
            var header = Partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.LogoRegionSizeInMediaUnits * MediaUnitSize;
        }

        /// <summary>
        /// Get the size of a partition plain region
        /// </summary>
        /// <returns>Size of the partition plain region in bytes, 0 on error</returns>
        public uint GetPlainRegionSize(int index)
        {
            // Empty partitions array means no size is available
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition header means no size is available
            var header = Partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.PlainRegionSizeInMediaUnits * MediaUnitSize;
        }

        /// <summary>
        /// Get the size of a partition RomFS
        /// </summary>
        /// <returns>Size of the partition RomFS in bytes, 0 on error</returns>
        public uint GetRomFSSize(int index)
        {
            // Empty partitions array means no size is available
            if (Partitions == null)
                return 0;
            if (index < 0 || index >= Partitions.Length)
                return 0;

            // Invalid partition header means no size is available
            var header = Partitions[index];
            if (header == null)
                return 0;

            // Return the adjusted size
            return header.RomFSSizeInMediaUnits * MediaUnitSize;
        }

        #endregion
    }
}
