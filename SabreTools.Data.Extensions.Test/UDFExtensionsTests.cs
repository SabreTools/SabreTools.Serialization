using SabreTools.Data.Models.UDF;
using Xunit;

namespace SabreTools.Data.Extensions.Test
{
    public class UDFExtensionsTests
    {
        #region DecodeDCharacters

        [Fact]
        public void DecodeDCharacters_Null_Empty()
        {
            byte[]? data = null;
            Assert.Equal(string.Empty, data.DecodeDCharacters());
        }

        [Fact]
        public void DecodeDCharacters_8Bit_Decoded()
        {
            byte[] data = [8, 0x41, 0x62, 0xE4];
            Assert.Equal("Abä", data.DecodeDCharacters());
        }

        [Fact]
        public void DecodeDCharacters_16Bit_Decoded()
        {
            byte[] data = [16, 0x00, 0x41, 0x30, 0x42];
            Assert.Equal("Aあ", data.DecodeDCharacters());
        }

        [Fact]
        public void DecodeDCharacters_UnknownCompression_Null()
        {
            byte[] data = [1, 0x41];
            Assert.Null(data.DecodeDCharacters());
        }

        [Fact]
        public void DecodeDString_UsesLengthByte()
        {
            byte[] data = [8, 0x41, 0x42, 0x43, 0x00, 3];
            Assert.Equal("AB", data.DecodeDString());
        }

        [Fact]
        public void DecodeDString_ZeroLength_Empty()
        {
            byte[] data = [8, 0x41, 0x00, 0];
            Assert.Equal(string.Empty, data.DecodeDString());
        }

        #endregion

        #region GetSectorSize

        [Fact]
        public void GetSectorSize_LogicalBlockSize()
        {
            var volume = new Volume { VolumeDescriptorSequence = [new LogicalVolumeDescriptor { LogicalBlockSize = 512 }] };
            Assert.Equal(512, volume.GetSectorSize());
        }

        [Fact]
        public void GetSectorSize_NoLogicalVolumeDescriptor_2048()
        {
            Assert.Equal(2048, new Volume().GetSectorSize());
        }

        #endregion

        #region TryGetSector

        [Fact]
        public void TryGetSector_Type1_StartPlusBlock()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 });
            bool actual = volume.TryGetSector(UDFExtensions.GetKey(0, 10), out long sector);
            Assert.True(actual);
            Assert.Equal(1010, sector);
        }

        [Fact]
        public void TryGetSector_Type1_OutsidePartition_False()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 });
            bool actual = volume.TryGetSector(UDFExtensions.GetKey(0, 100), out _);
            Assert.False(actual);
        }

        [Fact]
        public void TryGetSector_UnknownPartitionReference_False()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 });
            bool actual = volume.TryGetSector(UDFExtensions.GetKey(1, 0), out _);
            Assert.False(actual);
        }

        [Fact]
        public void TryGetSector_Metadata_ThroughMetadataFile()
        {
            // Metadata File at block 5 of partition 0: blocks 0-1 of the metadata partition are blocks 20-21,
            // blocks 2-3 are blocks 40-41
            var volume = CreateVolume(
                new Type1PartitionMap { PartitionNumber = 0 },
                new MetadataPartitionMap { PartitionNumber = 0, MetadataFileLocation = 5, MetadataMirrorFileLocation = 6 });
            volume.FileEntries[UDFExtensions.GetKey(0, 5)] = CreateShortAdEntry((4096, 20), (4096, 40));

            Assert.True(volume.TryGetSector(UDFExtensions.GetKey(1, 1), out long first));
            Assert.Equal(1021, first);
            Assert.True(volume.TryGetSector(UDFExtensions.GetKey(1, 2), out long second));
            Assert.Equal(1040, second);
            Assert.False(volume.TryGetSector(UDFExtensions.GetKey(1, 4), out _));
        }

        [Fact]
        public void TryGetSector_Metadata_FallsBackToMirror()
        {
            var volume = CreateVolume(
                new Type1PartitionMap { PartitionNumber = 0 },
                new MetadataPartitionMap { PartitionNumber = 0, MetadataFileLocation = 5, MetadataMirrorFileLocation = 6 });
            volume.FileEntries[UDFExtensions.GetKey(0, 6)] = CreateShortAdEntry((2048, 30));

            Assert.True(volume.TryGetSector(UDFExtensions.GetKey(1, 0), out long sector));
            Assert.Equal(1030, sector);
        }

        #endregion

        #region TryGetSectorRun

        [Fact]
        public void TryGetSectorRun_NoBlocks_False()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 });
            Assert.False(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 10), 0, out _, out _));
        }

        [Fact]
        public void TryGetSectorRun_Type1_AllBlocks()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 });
            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 10), 20, out long sector, out uint run));
            Assert.Equal(1010, sector);
            Assert.Equal(20u, run);
        }

        [Theory]
        [InlineData(13, 1013, 3)]
        [InlineData(17, 501, 3)]
        [InlineData(20, 1020, 8)]
        public void TryGetSectorRun_Sparable_RelocatedPacketIsRunOfItsOwn(uint block, long expectedSector, uint expectedRun)
        {
            // Packet 16 (blocks 16-19) is relocated to sector 500
            var volume = CreateVolume(new SparablePartitionMap { PartitionNumber = 0, PacketLength = 4 });
            volume.SparingTables[0] = CreateSparingTable((16, 500), (0xFFFFFFFF, 0));

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, block), 8, out long sector, out uint run));
            Assert.Equal(expectedSector, sector);
            Assert.Equal(expectedRun, run);
        }

        [Fact]
        public void TryGetSectorRun_Sparable_NoSparingTable_Type1()
        {
            var volume = CreateVolume(new SparablePartitionMap { PartitionNumber = 0, PacketLength = 4 });
            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 17), 8, out long sector, out uint run));
            Assert.Equal(1017, sector);
            Assert.Equal(8u, run);
        }

        [Fact]
        public void TryGetSectorRun_Sparable_NoPacketLength_Type1()
        {
            var volume = CreateVolume(new SparablePartitionMap { PartitionNumber = 0 });
            volume.SparingTables[0] = CreateSparingTable((16, 500));

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 17), 4, out long sector, out uint run));
            Assert.Equal(1017, sector);
            Assert.Equal(4u, run);
        }

        [Theory]
        [InlineData(0xFFFFFFF0u)]
        [InlineData(24u)]
        public void TryGetSectorRun_Sparable_LookupEndsAtEntryPastPacket(uint first)
        {
            // Entries are looked up in order until one past the packet, like Linux does (udf_get_pblock_spar15)
            var volume = CreateVolume(new SparablePartitionMap { PartitionNumber = 0, PacketLength = 4 });
            volume.SparingTables[0] = CreateSparingTable((first, 600), (16, 500));

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 17), 4, out long sector, out _));
            Assert.Equal(1017, sector);
        }

        [Fact]
        public void TryGetSectorRun_Sparable_UnalignedEntry_RunEndsAtNextPacket()
        {
            // An entry inside packet 16 relocates no packet, packet 20 may be relocated by a later entry
            var volume = CreateVolume(new SparablePartitionMap { PartitionNumber = 0, PacketLength = 4 });
            volume.SparingTables[0] = CreateSparingTable((18, 500));

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 13), 10, out long sector, out uint run));
            Assert.Equal(1013, sector);
            Assert.Equal(7u, run);
        }

        [Fact]
        public void TryGetSectorRun_Sparable_UnalignedEntryBeforeAvailableEntries_AllBlocks()
        {
            var volume = CreateVolume(new SparablePartitionMap { PartitionNumber = 0, PacketLength = 32 });
            volume.SparingTables[0] = CreateSparingTable((0xFFFFFFE1, 500));

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(0, 17), 8, out long sector, out uint run));
            Assert.Equal(1017, sector);
            Assert.Equal(8u, run);
        }

        [Theory]
        [InlineData(0, 1005, 3)]
        [InlineData(3, 1020, 1)]
        [InlineData(5, 1030, 2)]
        public void TryGetSectorRun_Virtual_RunOfConsecutiveEntries(uint block, long expectedSector, uint expectedRun)
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 }, new VirtualPartitionMap { PartitionNumber = 0 });
            volume.VirtualAllocationTable = new VirtualAllocationTable { Entries = [5, 6, 7, 20, 0xFFFFFFFF, 30, 31] };

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(1, block), 4, out long sector, out uint run));
            Assert.Equal(expectedSector, sector);
            Assert.Equal(expectedRun, run);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(7)]
        public void TryGetSectorRun_Virtual_UnusedOrPastTable_False(uint block)
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 }, new VirtualPartitionMap { PartitionNumber = 0 });
            volume.VirtualAllocationTable = new VirtualAllocationTable { Entries = [5, 6, 7, 20, 0xFFFFFFFF, 30, 31] };

            Assert.False(volume.TryGetSectorRun(UDFExtensions.GetKey(1, block), 1, out _, out _));
        }

        [Fact]
        public void TryGetSectorRun_Virtual_NoPhysicalMap_False()
        {
            // Enough maps that a missing map (-1) taken as a partition reference (65535) would name a Type 1 map
            var maps = new PartitionMap[ushort.MaxValue + 1];
            for (int i = 0; i < maps.Length; i++)
            {
                maps[i] = new Type1PartitionMap { PartitionNumber = 0 };
            }

            maps[1] = new VirtualPartitionMap { PartitionNumber = 7 };
            var volume = CreateVolume(maps);
            volume.VirtualAllocationTable = new VirtualAllocationTable { Entries = [5] };

            Assert.False(volume.TryGetSectorRun(UDFExtensions.GetKey(1, 0), 1, out _, out _));
        }

        [Fact]
        public void TryGetSectorRun_Virtual_NoVirtualAllocationTable_False()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 }, new VirtualPartitionMap { PartitionNumber = 0 });
            Assert.False(volume.TryGetSectorRun(UDFExtensions.GetKey(1, 0), 1, out _, out _));
        }

        [Fact]
        public void TryGetSectorRun_Metadata_RunEndsWithExtent()
        {
            var volume = CreateVolume(
                new Type1PartitionMap { PartitionNumber = 0 },
                new MetadataPartitionMap { PartitionNumber = 0, MetadataFileLocation = 5, MetadataMirrorFileLocation = 6 });
            volume.FileEntries[UDFExtensions.GetKey(0, 5)] = CreateShortAdEntry((4096, 20), (4096, 40));

            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(1, 0), 4, out long first, out uint firstRun));
            Assert.Equal(1020, first);
            Assert.Equal(2u, firstRun);
            Assert.True(volume.TryGetSectorRun(UDFExtensions.GetKey(1, 2), 4, out long second, out uint secondRun));
            Assert.Equal(1040, second);
            Assert.Equal(2u, secondRun);
        }

        [Fact]
        public void GetPhysicalPartitionReference_Type1OrSparable()
        {
            var lvd = new LogicalVolumeDescriptor
            {
                PartitionMaps =
                [
                    new VirtualPartitionMap { PartitionNumber = 1 },
                    new SparablePartitionMap { PartitionNumber = 1 },
                    new Type1PartitionMap { PartitionNumber = 0 },
                ],
            };

            Assert.Equal(1, lvd.GetPhysicalPartitionReference(1));
            Assert.Equal(2, lvd.GetPhysicalPartitionReference(0));
            Assert.Equal(-1, lvd.GetPhysicalPartitionReference(2));
        }

        #endregion

        #region GetSectorExtents

        [Theory]
        [InlineData(2048u)]
        [InlineData(512u)]
        [InlineData(4096u)]
        public void GetSectorExtents_Virtual_ExtentPerRun(uint sectorSize)
        {
            // Blocks 0-2 are recorded in sectors 1005-1007, block 3 in sector 1020
            var volume = CreateVolume(sectorSize, new Type1PartitionMap { PartitionNumber = 0 }, new VirtualPartitionMap { PartitionNumber = 0 });
            volume.VirtualAllocationTable = new VirtualAllocationTable { Entries = [5, 6, 7, 20, 0xFFFFFFFF, 30, 31] };
            var location = new LogicalBlockAddress { LogicalBlockNumber = 0, PartitionReferenceNumber = 1 };

            var extents = volume.GetSectorExtents(location, (3 * sectorSize) + 1);

            Assert.NotNull(extents);
            Assert.Equal(2, extents.Count);
            Assert.Equal(1005u, extents[0].ExtentLocation);
            Assert.Equal(3 * sectorSize, extents[0].ExtentLength);
            Assert.Equal(1020u, extents[1].ExtentLocation);
            Assert.Equal(1u, extents[1].ExtentLength);
        }

        [Fact]
        public void GetSectorExtents_ZeroLength_Empty()
        {
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 });
            var location = new LogicalBlockAddress { LogicalBlockNumber = 10, PartitionReferenceNumber = 0 };

            var extents = volume.GetSectorExtents(location, 0);

            Assert.NotNull(extents);
            Assert.Empty(extents);
        }

        [Fact]
        public void GetSectorExtents_UnusedBlock_Null()
        {
            // Block 3 is recorded, block 4 is unused
            var volume = CreateVolume(new Type1PartitionMap { PartitionNumber = 0 }, new VirtualPartitionMap { PartitionNumber = 0 });
            volume.VirtualAllocationTable = new VirtualAllocationTable { Entries = [5, 6, 7, 20, 0xFFFFFFFF, 30, 31] };
            var location = new LogicalBlockAddress { LogicalBlockNumber = 3, PartitionReferenceNumber = 1 };

            Assert.Null(volume.GetSectorExtents(location, 2 * 2048));
        }

        [Fact]
        public void GetSectorExtents_SectorPast32Bits_Null()
        {
            var volume = new Volume
            {
                VolumeDescriptorSequence =
                [
                    new PartitionDescriptor { PartitionNumber = 0, PartitionStartingLocation = uint.MaxValue, PartitionLength = 2 },
                    new LogicalVolumeDescriptor { LogicalBlockSize = 2048, PartitionMaps = [new Type1PartitionMap { PartitionNumber = 0 }] },
                ],
            };

            Assert.NotNull(volume.GetSectorExtents(new LogicalBlockAddress { LogicalBlockNumber = 0 }, 2048));
            Assert.Null(volume.GetSectorExtents(new LogicalBlockAddress { LogicalBlockNumber = 1 }, 2048));
        }

        #endregion

        #region GetAllocationDescriptors

        [Fact]
        public void GetAllocationDescriptors_Embedded_Empty()
        {
            var entry = new FileEntry();
            entry.ICBTag.Flags = (ushort)AllocationDescriptorType.EMBEDDED;
            entry.AllocationDescriptors = [1, 2, 3];

            var actual = new Volume().GetAllocationDescriptors(0, entry);
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void GetAllocationDescriptors_ZeroLengthTerminates()
        {
            var entry = CreateShortAdEntry((2048, 20), (0, 99), (2048, 40));

            var actual = new Volume().GetAllocationDescriptors(UDFExtensions.GetKey(2, 7), entry);
            Assert.NotNull(actual);
            var ad = Assert.Single(actual);
            Assert.Equal(20u, ad.ExtentLocation.LogicalBlockNumber);
            Assert.Equal((ushort)2, ad.ExtentLocation.PartitionReferenceNumber);
        }

        [Fact]
        public void GetAllocationDescriptors_MissingAllocationExtent_NullWithKey()
        {
            var entry = CreateShortAdEntry((2048, 20), ((3u << 30) | 2048, 50));

            var actual = new Volume().GetAllocationDescriptors(UDFExtensions.GetKey(0, 7), entry, out long missingKey);
            Assert.Null(actual);
            Assert.Equal(UDFExtensions.GetKey(0, 50), missingKey);
        }

        [Fact]
        public void GetAllocationDescriptors_AllocationExtentLoop_Null()
        {
            var volume = new Volume();
            var entry = CreateShortAdEntry(((3u << 30) | 2048, 50));
            volume.AllocationExtentDescriptors[UDFExtensions.GetKey(0, 50)] = new AllocationExtentDescriptor
            {
                AllocationDescriptors = CreateShortAdEntry(((3u << 30) | 2048, 50)).AllocationDescriptors,
            };

            var actual = volume.GetAllocationDescriptors(UDFExtensions.GetKey(0, 7), entry);
            Assert.Null(actual);
        }

        #endregion

        #region IsValidDescriptor

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void IsValidDescriptor_Valid_True(byte version)
        {
            byte[] data = CreateDescriptor(version);
            Assert.True(data.IsValidDescriptor(0, data.Length, 20));
        }

        [Fact]
        public void IsValidDescriptor_WrongChecksum_False()
        {
            byte[] data = CreateDescriptor(2);
            data[4]++;
            Assert.False(data.IsValidDescriptor(0, data.Length, 20));
        }

        [Fact]
        public void IsValidDescriptor_UnknownVersion_False()
        {
            byte[] data = CreateDescriptor(4);
            Assert.False(data.IsValidDescriptor(0, data.Length, 20));
        }

        [Fact]
        public void IsValidDescriptor_WrongCrc_False()
        {
            byte[] data = CreateDescriptor(2);
            data[16] ^= 0x01;
            Assert.False(data.IsValidDescriptor(0, data.Length, 20));
        }

        [Fact]
        public void IsValidDescriptor_CrcPastAvailable_NotChecked()
        {
            byte[] data = CreateDescriptor(2);
            data[16] ^= 0x01;
            Assert.True(data.IsValidDescriptor(0, 16, 20));
        }

        [Fact]
        public void IsValidDescriptor_WrongLocation_False()
        {
            byte[] data = CreateDescriptor(2);
            Assert.False(data.IsValidDescriptor(0, data.Length, 21));
        }

        #endregion

        /// <summary>
        /// Descriptor with tag location 20 whose 3 bytes after the tag have the CRC #3299 (ECMA-167 3/7.2.6)
        /// </summary>
        private static byte[] CreateDescriptor(byte version)
        {
            byte[] data = new byte[19];
            data[0] = 1;
            data[2] = version;
            data[8] = 0x99;
            data[9] = 0x32;
            data[10] = 3;
            data[12] = 20;
            data[16] = 0x70;
            data[17] = 0x6A;
            data[18] = 0x77;
            for (int i = 0; i < 16; i++)
            {
                if (i != 4)
                    data[4] += data[i];
            }

            return data;
        }

        /// <summary>
        /// Volume with partition 0 at sector 1000 (length 50) and the given partition maps
        /// </summary>
        private static Volume CreateVolume(params PartitionMap[] maps)
            => CreateVolume(2048, maps);

        /// <summary>
        /// Volume with logical sectors of the given size, partition 0 at sector 1000 (length 50) and the given partition maps
        /// </summary>
        private static Volume CreateVolume(uint sectorSize, params PartitionMap[] maps)
        {
            return new Volume
            {
                VolumeDescriptorSequence =
                [
                    new PartitionDescriptor { PartitionNumber = 0, PartitionStartingLocation = 1000, PartitionLength = 50 },
                    new LogicalVolumeDescriptor { LogicalBlockSize = sectorSize, PartitionMaps = maps },
                ],
            };
        }

        /// <summary>
        /// File Entry with the given short allocation descriptors (extent length, position)
        /// </summary>
        private static FileEntry CreateShortAdEntry(params (uint Length, uint Position)[] extents)
        {
            var data = new byte[extents.Length * 8];
            for (int i = 0; i < extents.Length; i++)
            {
                for (int b = 0; b < 4; b++)
                {
                    data[(i * 8) + b] = (byte)(extents[i].Length >> (8 * b));
                    data[(i * 8) + 4 + b] = (byte)(extents[i].Position >> (8 * b));
                }
            }

            var entry = new FileEntry { AllocationDescriptors = data };
            entry.ICBTag.Flags = (ushort)AllocationDescriptorType.SHORT;
            return entry;
        }

        /// <summary>
        /// Sparing Table with the given entries (original location, mapped location)
        /// </summary>
        private static SparingTable CreateSparingTable(params (uint Original, uint Mapped)[] entries)
        {
            var table = new SparingTable { MapEntries = new SparingEntry[entries.Length] };
            for (int i = 0; i < entries.Length; i++)
            {
                table.MapEntries[i] = new SparingEntry { OriginalLocation = entries[i].Original, MappedLocation = entries[i].Mapped };
            }

            return table;
        }
    }
}
