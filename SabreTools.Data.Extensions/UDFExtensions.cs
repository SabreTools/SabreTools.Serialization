using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Models.UDF;

namespace SabreTools.Data.Extensions
{
    public static class UDFExtensions
    {
        /// <summary>
        /// Maximum number of Allocation Extent Descriptors followed for one File Entry
        /// </summary>
        private const int MaximumAllocationExtents = 1024;

        /// <summary>
        /// Get the dictionary key of a logical block address
        /// </summary>
        /// <param name="address">Logical block address</param>
        /// <returns>((long)PartitionReferenceNumber &lt;&lt; 32) | LogicalBlockNumber</returns>
        public static long GetKey(this LogicalBlockAddress address)
            => GetKey(address.PartitionReferenceNumber, address.LogicalBlockNumber);

        /// <summary>
        /// Get the dictionary key of a logical block address
        /// </summary>
        /// <param name="partitionReferenceNumber">Index of the partition map</param>
        /// <param name="logicalBlockNumber">Logical block number within the partition</param>
        /// <returns>((long)partitionReferenceNumber &lt;&lt; 32) | logicalBlockNumber</returns>
        public static long GetKey(ushort partitionReferenceNumber, uint logicalBlockNumber)
            => ((long)partitionReferenceNumber << 32) | logicalBlockNumber;

        /// <summary>
        /// Get the extent type from an allocation descriptor's extent length
        /// </summary>
        public static ExtentType GetExtentType(uint extentLength)
            => (ExtentType)(extentLength >> Constants.ExtentTypeShift);

        /// <summary>
        /// Get the extent length in bytes from an allocation descriptor's extent length
        /// </summary>
        public static uint GetExtentLength(uint extentLength)
            => extentLength & Constants.ExtentLengthMask;

        /// <summary>
        /// Get the type of the allocation descriptors of an ICB
        /// </summary>
        public static AllocationDescriptorType GetAllocationDescriptorType(this ICBTag tag)
            => (AllocationDescriptorType)(tag.Flags & Constants.AllocationDescriptorTypeMask);

        /// <summary>
        /// Get the prevailing Logical Volume Descriptor (highest Volume Descriptor Sequence Number)
        /// </summary>
        /// <param name="volume">Volume to search</param>
        /// <returns>Logical Volume Descriptor on success, null if none was recorded</returns>
        public static LogicalVolumeDescriptor? GetLogicalVolumeDescriptor(this Volume volume)
        {
            LogicalVolumeDescriptor? prevailing = null;
            foreach (var descriptor in volume.VolumeDescriptorSequence)
            {
                if (descriptor is LogicalVolumeDescriptor lvd
                    && (prevailing is null || lvd.VolumeDescriptorSequenceNumber > prevailing.VolumeDescriptorSequenceNumber))
                {
                    prevailing = lvd;
                }
            }

            return prevailing;
        }

        /// <summary>
        /// Get the prevailing Partition Descriptor of a partition (highest Volume Descriptor Sequence Number)
        /// </summary>
        /// <param name="volume">Volume to search</param>
        /// <param name="partitionNumber">Partition number</param>
        /// <returns>Partition Descriptor on success, null if none was recorded</returns>
        public static PartitionDescriptor? GetPartitionDescriptor(this Volume volume, ushort partitionNumber)
        {
            PartitionDescriptor? prevailing = null;
            foreach (var descriptor in volume.VolumeDescriptorSequence)
            {
                if (descriptor is PartitionDescriptor pd
                    && pd.PartitionNumber == partitionNumber
                    && (prevailing is null || pd.VolumeDescriptorSequenceNumber > prevailing.VolumeDescriptorSequenceNumber))
                {
                    prevailing = pd;
                }
            }

            return prevailing;
        }

        /// <summary>
        /// Get the partition reference number of the Type 1 partition map for a partition number
        /// </summary>
        /// <param name="lvd">Logical Volume Descriptor holding the partition maps</param>
        /// <param name="partitionNumber">Partition number</param>
        /// <returns>Partition reference number on success, -1 if there is no such map</returns>
        public static int GetType1PartitionReference(this LogicalVolumeDescriptor lvd, ushort partitionNumber)
        {
            for (int i = 0; i < lvd.PartitionMaps.Length; i++)
            {
                if (lvd.PartitionMaps[i] is Type1PartitionMap map && map.PartitionNumber == partitionNumber)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Map a logical block of a partition to a logical sector of the volume
        /// Supports Type 1 partitions and Metadata Partitions (the Metadata File's File Entry must be in FileEntries)
        /// </summary>
        /// <param name="volume">Volume containing the Volume Descriptor Sequence</param>
        /// <param name="key">Logical block address key, see GetKey</param>
        /// <param name="sector">Logical sector on success</param>
        /// <returns>True if the block could be mapped, false otherwise</returns>
        public static bool TryGetSector(this Volume volume, long key, out long sector)
        {
            sector = -1;

            var lvd = volume.GetLogicalVolumeDescriptor();
            if (lvd is null)
                return false;

            int partitionReference = (int)(key >> 32);
            uint block = (uint)key;
            if (partitionReference < 0 || partitionReference >= lvd.PartitionMaps.Length)
                return false;

            var map = lvd.PartitionMaps[partitionReference];
            if (map is Type1PartitionMap type1)
            {
                var pd = volume.GetPartitionDescriptor(type1.PartitionNumber);
                if (pd is null || block >= pd.PartitionLength)
                    return false;

                sector = (long)pd.PartitionStartingLocation + block;
                return true;
            }
            else if (map is MetadataPartitionMap metadata)
            {
                // The Metadata File (or its mirror) is recorded in the Type 1 partition of the same partition number
                int physical = lvd.GetType1PartitionReference(metadata.PartitionNumber);
                if (physical < 0)
                    return false;

                if (!TryGetMetadataFileBlock(volume, lvd, GetKey((ushort)physical, metadata.MetadataFileLocation), block, out long physicalKey)
                    && !TryGetMetadataFileBlock(volume, lvd, GetKey((ushort)physical, metadata.MetadataMirrorFileLocation), block, out physicalKey))
                {
                    return false;
                }

                return volume.TryGetSector(physicalKey, out sector);
            }

            // Virtual and Sparable partitions are not supported
            return false;
        }

        /// <summary>
        /// Map a logical block of a Metadata Partition through the Metadata File's allocation descriptors
        /// </summary>
        private static bool TryGetMetadataFileBlock(Volume volume, LogicalVolumeDescriptor lvd, long fileKey, uint block, out long physicalKey)
        {
            physicalKey = -1;
            if (!volume.FileEntries.TryGetValue(fileKey, out var fileEntry))
                return false;

            var descriptors = volume.GetAllocationDescriptors(fileKey, fileEntry);
            if (descriptors is null)
                return false;

            uint blockSize = lvd.LogicalBlockSize;
            long first = 0;
            foreach (var ad in descriptors)
            {
                long blocks = (GetExtentLength(ad.ExtentLength) + blockSize - 1) / blockSize;
                if (block < first + blocks)
                {
                    if (GetExtentType(ad.ExtentLength) != ExtentType.RECORDED_AND_ALLOCATED)
                        return false;

                    physicalKey = GetKey(ad.ExtentLocation.PartitionReferenceNumber, (uint)(ad.ExtentLocation.LogicalBlockNumber + (block - first)));
                    return true;
                }

                first += blocks;
            }

            return false;
        }

        /// <summary>
        /// Get the allocation descriptors of a File Entry, including those continued in Allocation Extent Descriptors,
        /// as long allocation descriptors (short allocation descriptors get the partition of the File Entry)
        /// </summary>
        /// <param name="volume">Volume containing the Allocation Extent Descriptors</param>
        /// <param name="key">Logical block address key of the File Entry</param>
        /// <param name="fileEntry">File Entry</param>
        /// <returns>Allocation descriptors (empty for embedded data), null if they are not supported or incomplete</returns>
        public static List<LongAllocationDescriptor>? GetAllocationDescriptors(this Volume volume, long key, FileEntry fileEntry)
            => volume.GetAllocationDescriptors(key, fileEntry, out _);

        /// <summary>
        /// Get the allocation descriptors of a File Entry, including those continued in Allocation Extent Descriptors,
        /// as long allocation descriptors (short allocation descriptors get the partition of the File Entry)
        /// </summary>
        /// <param name="volume">Volume containing the Allocation Extent Descriptors</param>
        /// <param name="key">Logical block address key of the File Entry</param>
        /// <param name="fileEntry">File Entry</param>
        /// <param name="missingKey">Key of the first Allocation Extent Descriptor that is not in the volume, -1 if none</param>
        /// <returns>Allocation descriptors (empty for embedded data), null if they are not supported or incomplete</returns>
        public static List<LongAllocationDescriptor>? GetAllocationDescriptors(this Volume volume, long key, FileEntry fileEntry, out long missingKey)
        {
            missingKey = -1;

            var type = fileEntry.ICBTag.GetAllocationDescriptorType();
            if (type == AllocationDescriptorType.EMBEDDED)
                return [];

            // Extended allocation descriptors are not allowed by UDF
            if (type != AllocationDescriptorType.SHORT && type != AllocationDescriptorType.LONG)
                return null;

            var result = new List<LongAllocationDescriptor>();
            ushort partition = (ushort)(key >> 32);
            byte[] descriptors = fileEntry.AllocationDescriptors;
            int length = descriptors.Length;

            for (int extents = 0; extents <= MaximumAllocationExtents; extents++)
            {
                long nextKey = -1;
                int size = type == AllocationDescriptorType.SHORT ? 8 : 16;
                for (int offset = 0; offset + size <= length; offset += size)
                {
                    var ad = new LongAllocationDescriptor();
                    ad.ExtentLength = ReadUInt32(descriptors, offset);
                    if (type == AllocationDescriptorType.SHORT)
                    {
                        ad.ExtentLocation.LogicalBlockNumber = ReadUInt32(descriptors, offset + 4);
                        ad.ExtentLocation.PartitionReferenceNumber = partition;
                    }
                    else
                    {
                        ad.ExtentLocation.LogicalBlockNumber = ReadUInt32(descriptors, offset + 4);
                        ad.ExtentLocation.PartitionReferenceNumber = (ushort)(descriptors[offset + 8] | (descriptors[offset + 9] << 8));
                        for (int i = 0; i < 6; i++)
                        {
                            ad.ImplementationUse[i] = descriptors[offset + 10 + i];
                        }
                    }

                    // A zero length terminates the allocation descriptors
                    if (GetExtentLength(ad.ExtentLength) == 0)
                        return result;

                    if (GetExtentType(ad.ExtentLength) == ExtentType.NEXT_EXTENT)
                    {
                        nextKey = ad.ExtentLocation.GetKey();
                        break;
                    }

                    result.Add(ad);
                }

                if (nextKey < 0)
                    return result;

                if (!volume.AllocationExtentDescriptors.TryGetValue(nextKey, out var aed))
                {
                    missingKey = nextKey;
                    return null;
                }

                partition = (ushort)(nextKey >> 32);
                descriptors = aed.AllocationDescriptors;
                length = descriptors.Length;
            }

            // Too many continuation extents, assume a loop
            return null;
        }

        /// <summary>
        /// Decode OSTA Compressed Unicode d-characters (first byte = compression ID)
        /// </summary>
        /// <param name="data">Encoded characters</param>
        /// <returns>Decoded string, empty for no characters, null for an unknown compression ID</returns>
        public static string? DecodeDCharacters(this byte[]? data)
            => DecodeDCharacters(data, data?.Length ?? 0);

        /// <summary>
        /// Decode an OSTA Compressed Unicode dstring (last byte = number of used bytes)
        /// </summary>
        /// <param name="data">Encoded dstring field</param>
        /// <returns>Decoded string, empty for no characters, null for an unknown compression ID</returns>
        public static string? DecodeDString(this byte[]? data)
        {
            if (data is null || data.Length == 0)
                return string.Empty;

            int length = data[data.Length - 1];
            if (length > data.Length - 1)
                length = data.Length - 1;

            return DecodeDCharacters(data, length);
        }

        /// <summary>
        /// Decode the first length bytes of OSTA Compressed Unicode d-characters
        /// </summary>
        private static string? DecodeDCharacters(byte[]? data, int length)
        {
            if (data is null || length == 0)
                return string.Empty;

            // 254 and 255 are the same encodings, used by UDF for deleted entries
            byte compressionId = data[0];
            var builder = new StringBuilder(length);
            if (compressionId == Constants.CompressionId8 || compressionId == 254)
            {
                for (int i = 1; i < length; i++)
                {
                    builder.Append((char)data[i]);
                }
            }
            else if (compressionId == Constants.CompressionId16 || compressionId == 255)
            {
                for (int i = 1; i + 1 < length; i += 2)
                {
                    builder.Append((char)((data[i] << 8) | data[i + 1]));
                }
            }
            else
            {
                return null;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Read a little-endian UInt32 from a byte array
        /// </summary>
        private static uint ReadUInt32(byte[] data, int offset)
            => (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));
    }
}
