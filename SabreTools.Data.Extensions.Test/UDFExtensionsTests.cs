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

        /// <summary>
        /// Volume with partition 0 at sector 1000 (length 50) and the given partition maps
        /// </summary>
        private static Volume CreateVolume(params PartitionMap[] maps)
        {
            return new Volume
            {
                VolumeDescriptorSequence =
                [
                    new PartitionDescriptor { PartitionNumber = 0, PartitionStartingLocation = 1000, PartitionLength = 50 },
                    new LogicalVolumeDescriptor { LogicalBlockSize = 2048, PartitionMaps = maps },
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
    }
}
