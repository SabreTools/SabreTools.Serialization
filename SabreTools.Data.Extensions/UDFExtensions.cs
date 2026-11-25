using System;
using System.Collections.Generic;
using System.Text;
using SabreTools.Data.Models.UDF;
using SabreTools.Numerics.Extensions;

namespace SabreTools.Data.Extensions
{
    public static class UDFExtensions
    {
        /// <summary>
        /// Maximum number of Allocation Extent Descriptors followed for one File Entry
        /// </summary>
        private const int MaximumAllocationExtents = 1024;

        /// <summary>
        /// CRC-ITU-T (CCITT, polynomial 0x1021) table for descriptor CRCs
        /// </summary>
        private static readonly ushort[] CrcTable = CreateCrcTable();

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
        /// Get the partition reference number of the Type 1 or Sparable Partition Map for a partition number,
        /// the partition that Virtual and Metadata Partitions are recorded in
        /// </summary>
        /// <param name="lvd">Logical Volume Descriptor holding the partition maps</param>
        /// <param name="partitionNumber">Partition number</param>
        /// <returns>Partition reference number on success, -1 if there is no such map</returns>
        public static int GetPhysicalPartitionReference(this LogicalVolumeDescriptor lvd, ushort partitionNumber)
        {
            for (int i = 0; i < lvd.PartitionMaps.Length; i++)
            {
                var map = lvd.PartitionMaps[i];
                if ((map is Type1PartitionMap type1 && type1.PartitionNumber == partitionNumber)
                    || (map is SparablePartitionMap sparable && sparable.PartitionNumber == partitionNumber))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Map a logical block of a partition to a logical sector of the volume
        /// </summary>
        /// <param name="volume">Volume containing the Volume Descriptor Sequence</param>
        /// <param name="key">Logical block address key, see GetKey</param>
        /// <param name="sector">Logical sector on success</param>
        /// <returns>True if the block could be mapped, false otherwise</returns>
        /// <see cref="TryGetSectorRun"/>
        public static bool TryGetSector(this Volume volume, long key, out long sector)
            => volume.TryGetSectorRun(key, 1, out sector, out _);

        /// <summary>
        /// Map logical blocks of a partition to logical sectors of the volume, as many as are recorded in consecutive sectors
        /// </summary>
        /// <remarks>
        /// Supports Type 1, Sparable, Virtual and Metadata Partitions (the Sparing Table, the Virtual Allocation Table
        /// and the Metadata File's File Entry must be in the volume)
        /// </remarks>
        /// <param name="volume">Volume containing the Volume Descriptor Sequence</param>
        /// <param name="key">Logical block address key of the first block, see GetKey</param>
        /// <param name="blocks">Number of blocks wanted, at least 1</param>
        /// <param name="sector">Logical sector of the first block on success</param>
        /// <param name="run">Number of blocks recorded in consecutive sectors from the first one on success, 1 to blocks</param>
        /// <returns>True if the first block could be mapped, false otherwise</returns>
        public static bool TryGetSectorRun(this Volume volume, long key, uint blocks, out long sector, out uint run)
        {
            sector = -1;
            run = 0;

            var lvd = volume.GetLogicalVolumeDescriptor();
            if (lvd is null || blocks == 0)
                return false;

            int partitionReference = (int)(key >> 32);
            uint block = (uint)key;
            if (partitionReference < 0 || partitionReference >= lvd.PartitionMaps.Length)
                return false;

            var map = lvd.PartitionMaps[partitionReference];
            if (map is Type1PartitionMap type1)
            {
                return TryGetPhysicalRun(volume, type1.PartitionNumber, block, blocks, out sector, out run);
            }
            else if (map is SparablePartitionMap sparable)
            {
                if (!TryGetPhysicalRun(volume, sparable.PartitionNumber, block, blocks, out sector, out run))
                    return false;

                if (volume.SparingTables.TryGetValue((ushort)partitionReference, out var table))
                    ApplySparingTable(table, sparable.PacketLength, block, ref sector, ref run);

                return true;
            }
            else if (map is VirtualPartitionMap virtualMap)
            {
                // The Virtual Allocation Table maps each block to a block of the partition it is recorded in
                var vat = volume.VirtualAllocationTable;
                int physical = lvd.GetPhysicalPartitionReference(virtualMap.PartitionNumber);
                if (vat is null || physical < 0 || block >= vat.Entries.Length)
                    return false;

                // Unused blocks (0xFFFFFFFF) map past the end of the partition
                uint first = vat.Entries[block];
                uint count = 1;
                while (count < blocks && (long)block + count < vat.Entries.Length && vat.Entries[block + count] == first + count)
                {
                    count++;
                }

                return volume.TryGetSectorRun(GetKey((ushort)physical, first), count, out sector, out run);
            }
            else if (map is MetadataPartitionMap metadata)
            {
                // The Metadata File (or its mirror) is recorded in the Type 1 or Sparable partition of the same partition number
                int physical = lvd.GetPhysicalPartitionReference(metadata.PartitionNumber);
                if (physical < 0)
                    return false;

                if (!TryGetMetadataFileBlock(volume, lvd, GetKey((ushort)physical, metadata.MetadataFileLocation), block, out long physicalKey, out uint extentBlocks)
                    && !TryGetMetadataFileBlock(volume, lvd, GetKey((ushort)physical, metadata.MetadataMirrorFileLocation), block, out physicalKey, out extentBlocks))
                {
                    return false;
                }

                return volume.TryGetSectorRun(physicalKey, Math.Min(blocks, extentBlocks), out sector, out run);
            }

            // Unknown partition types are not supported
            return false;
        }

        /// <summary>
        /// Map a recorded extent of a partition to extents of logical sectors of the volume
        /// </summary>
        /// <param name="volume">Volume containing the Volume Descriptor Sequence</param>
        /// <param name="location">Logical block address of the extent</param>
        /// <param name="length">Number of bytes of the extent to map</param>
        /// <returns>
        /// Extents of logical sectors in the order of the extent, one for each run of blocks recorded in consecutive sectors;
        /// null if a block could not be mapped
        /// </returns>
        public static List<ExtentDescriptor>? GetSectorExtents(this Volume volume, LogicalBlockAddress location, uint length)
        {
            List<ExtentDescriptor> extents = [];
            uint block = location.LogicalBlockNumber;
            long done = 0;
            while (done < length)
            {
                uint blocks = (uint)((length - done + Constants.SectorSize - 1) / Constants.SectorSize);
                if (!volume.TryGetSectorRun(GetKey(location.PartitionReferenceNumber, block), blocks, out long sector, out uint run))
                    return null;

                // Logical sector numbers are recorded in 32 bits
                if (sector > uint.MaxValue)
                    return null;

                long runLength = Math.Min(length - done, (long)run * Constants.SectorSize);
                extents.Add(new ExtentDescriptor { ExtentLength = (uint)runLength, ExtentLocation = (uint)sector });
                done += runLength;
                block += run;
            }

            return extents;
        }

        /// <summary>
        /// Map logical blocks of a partition recorded from its starting location
        /// </summary>
        private static bool TryGetPhysicalRun(Volume volume, ushort partitionNumber, uint block, uint blocks, out long sector, out uint run)
        {
            sector = -1;
            run = 0;

            var pd = volume.GetPartitionDescriptor(partitionNumber);
            if (pd is null || block >= pd.PartitionLength)
                return false;

            sector = (long)pd.PartitionStartingLocation + block;
            run = blocks;
            return true;
        }

        /// <summary>
        /// Apply the relocations of a Sparing Table to a run of a Sparable Partition: a relocated packet is a run of its own,
        /// other runs end before the next relocated packet
        /// </summary>
        /// <remarks>
        /// Entries are looked up like Linux does (udf_get_pblock_spar15): in order, until one past the packet;
        /// available and defective entries (0xFFFFFFF0 and up) are past every packet
        /// </remarks>
        private static void ApplySparingTable(SparingTable table, ushort packetLength, uint block, ref long sector, ref uint run)
        {
            if (packetLength == 0)
                return;

            long packet = block - (block % packetLength);
            foreach (var entry in table.MapEntries)
            {
                if (entry.OriginalLocation == packet)
                {
                    sector = entry.MappedLocation + (block - packet);
                    run = (uint)Math.Min(run, packet + packetLength - block);
                    return;
                }

                if (entry.OriginalLocation > packet)
                {
                    // Packets are relocated as a whole, the next one that can be relocated starts at or after this entry
                    long next = ((long)entry.OriginalLocation + packetLength - 1) / packetLength * packetLength;
                    run = (uint)Math.Min(run, next - block);
                    return;
                }
            }
        }

        /// <summary>
        /// Map a logical block of a Metadata Partition through the Metadata File's allocation descriptors
        /// </summary>
        /// <param name="extentBlocks">Number of blocks from the block to the end of its extent on success</param>
        private static bool TryGetMetadataFileBlock(Volume volume, LogicalVolumeDescriptor lvd, long fileKey, uint block, out long physicalKey, out uint extentBlocks)
        {
            physicalKey = -1;
            extentBlocks = 0;
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
                    extentBlocks = (uint)(first + blocks - block);
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

            List<LongAllocationDescriptor> result = [];
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
                    ad.ExtentLength = descriptors.ToUInt32LittleEndian(offset);
                    if (type == AllocationDescriptorType.SHORT)
                    {
                        ad.ExtentLocation.LogicalBlockNumber = descriptors.ToUInt32LittleEndian(offset + 4);
                        ad.ExtentLocation.PartitionReferenceNumber = partition;
                    }
                    else
                    {
                        ad.ExtentLocation.LogicalBlockNumber = descriptors.ToUInt32LittleEndian(offset + 4);
                        ad.ExtentLocation.PartitionReferenceNumber = descriptors.ToUInt16LittleEndian(offset + 8);
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
        /// Check a descriptor tag: checksum, version, CRC (when it fits into the data) and location
        /// </summary>
        /// <remarks>ECMA-167 3/7.2; receiving systems allow descriptor version 2 or 3 (3/7.2.2)</remarks>
        /// <param name="data">Data holding the descriptor</param>
        /// <param name="offset">Offset of the descriptor</param>
        /// <param name="available">Number of bytes of the descriptor available from the offset</param>
        /// <param name="location">Expected tag location (logical sector or logical block number)</param>
        /// <returns>True if the tag is valid, false otherwise</returns>
        public static bool IsValidDescriptor(this byte[] data, int offset, int available, uint location)
        {
            byte checksum = 0;
            for (int i = 0; i < 16; i++)
            {
                if (i != 4)
                    checksum += data[offset + i];
            }

            if (checksum != data[offset + 4])
                return false;

            ushort version = data.ToUInt16LittleEndian(offset + 2);
            if (version != 2 && version != 3)
                return false;

            ushort crcLength = data.ToUInt16LittleEndian(offset + 10);
            if (16 + crcLength <= available)
            {
                if (ComputeCrc(data, offset + 16, crcLength) != data.ToUInt16LittleEndian(offset + 8))
                    return false;
            }

            return data.ToUInt32LittleEndian(offset + 12) == location;
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
                    builder.Append((char)data.ToUInt16BigEndian(i));
                }
            }
            else
            {
                return null;
            }

            return builder.ToString();
        }

        // TODO: Replace with CRC-16/XMODEM from Hashing once its CRC code is faster
        /// <summary>
        /// Compute the CRC-ITU-T of a range of bytes
        /// </summary>
        private static ushort ComputeCrc(byte[] data, int offset, int length)
        {
            ushort crc = 0;
            for (int i = offset; i < offset + length; i++)
            {
                crc = (ushort)(CrcTable[((crc >> 8) ^ data[i]) & 0xFF] ^ (crc << 8));
            }

            return crc;
        }

        /// <summary>
        /// Create the CRC-ITU-T table
        /// </summary>
        private static ushort[] CreateCrcTable()
        {
            var table = new ushort[256];
            for (int i = 0; i < 256; i++)
            {
                ushort crc = (ushort)(i << 8);
                for (int bit = 0; bit < 8; bit++)
                {
                    crc = (crc & 0x8000) != 0 ? (ushort)((crc << 1) ^ 0x1021) : (ushort)(crc << 1);
                }

                table[i] = crc;
            }

            return table;
        }
    }
}
