using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.UDF;
using Xunit;

namespace SabreTools.Serialization.Readers.Test
{
    public class UDFTests
    {
        [Fact]
        public void NullArray_Null()
        {
            byte[]? data = null;
            int offset = 0;
            var deserializer = new UDF();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyArray_Null()
        {
            byte[]? data = [];
            int offset = 0;
            var deserializer = new UDF();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidArray_Null()
        {
            byte[]? data = [.. Enumerable.Repeat<byte>(0xFF, 1024)];
            int offset = 0;
            var deserializer = new UDF();

            var actual = deserializer.Deserialize(data, offset);
            Assert.Null(actual);
        }

        [Fact]
        public void NullStream_Null()
        {
            Stream? data = null;
            var deserializer = new UDF();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void EmptyStream_Null()
        {
            Stream? data = new MemoryStream([]);
            var deserializer = new UDF();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void InvalidStream_Null()
        {
            Stream? data = new MemoryStream([.. Enumerable.Repeat<byte>(0xFF, 1024)]);
            var deserializer = new UDF();

            var actual = deserializer.Deserialize(data);
            Assert.Null(actual);
        }

        [Fact]
        public void ValidImage_VolumeStructures()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build()));

            Assert.NotNull(volume);
            Assert.Equal(3, volume.VolumeRecognitionSequence.Length);
            Assert.NotNull(volume.AnchorVolumeDescriptorPointer);
            Assert.Equal(4, volume.VolumeDescriptorSequence.Length);
            var terminator = Assert.IsType<GenericDescriptor>(volume.VolumeDescriptorSequence[3]);
            Assert.Equal(TagIdentifier.TERMINATING_DESCRIPTOR, terminator.DescriptorTag.TagIdentifier);

            var lvd = volume.GetLogicalVolumeDescriptor();
            Assert.NotNull(lvd);
            Assert.Equal("TESTVOLUME", lvd.LogicalVolumeIdentifier.DecodeDString());
            Assert.IsType<Type1PartitionMap>(Assert.Single(lvd.PartitionMaps));

            Assert.NotNull(volume.FileSetDescriptor);
            Assert.Equal("TESTSET", volume.FileSetDescriptor.FileSetIdentifier.DecodeDString());
            Assert.Equal(2u, volume.FileSetDescriptor.RootDirectoryICB.ExtentLocation.LogicalBlockNumber);
        }

        [Fact]
        public void ValidImage_DirectoryHierarchy()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build()));

            Assert.NotNull(volume);

            // Root, "README.TXT", "Sub", "data.bin", "Media"; the deleted entry is not followed
            Assert.Equal(5, volume.FileEntries.Count);
            Assert.Equal(2, volume.Directories.Count);
            Assert.Single(volume.AllocationExtentDescriptors);

            // The last root entry has no padding and ends exactly at the end of the directory
            var root = volume.Directories[UDFExtensions.GetKey(0, 2)];
            Assert.Equal(5, root.Length);
            Assert.Equal("Media", root[4].FileIdentifier.DecodeDCharacters());
            Assert.Empty(root[4].Padding);

            var sub = volume.Directories[UDFExtensions.GetKey(0, 5)];
            Assert.Equal("data.bin", sub[1].FileIdentifier.DecodeDCharacters());
        }

        [Fact]
        public void FindFileEntry_EmbeddedData()
        {
            var data = new MemoryStream(UDFImageBuilder.Build());
            var volume = new UDF().Deserialize(data);
            Assert.NotNull(volume);

            var entry = UDF.FindFileEntry(data, volume, "README.TXT", out long key);

            Assert.NotNull(entry);
            Assert.Equal(UDFExtensions.GetKey(0, 4), key);
            Assert.Equal(AllocationDescriptorType.EMBEDDED, entry.ICBTag.GetAllocationDescriptorType());
            Assert.Equal("hello world", Encoding.ASCII.GetString(entry.AllocationDescriptors, 0, (int)entry.InformationLength));
        }

        [Fact]
        public void FindFileEntry_CaseInsensitive()
        {
            var data = new MemoryStream(UDFImageBuilder.Build());
            var volume = new UDF().Deserialize(data);
            Assert.NotNull(volume);

            var entry = UDF.FindFileEntry(data, volume, "/sub\\DATA.BIN", out long key);

            Assert.NotNull(entry);
            Assert.Equal(UDFExtensions.GetKey(0, 7), key);
        }

        [Fact]
        public void FindFileEntry_DeletedOrMissing_Null()
        {
            var data = new MemoryStream(UDFImageBuilder.Build());
            var volume = new UDF().Deserialize(data);
            Assert.NotNull(volume);

            Assert.Null(UDF.FindFileEntry(data, volume, "deleted.txt", out long deletedKey));
            Assert.Equal(-1, deletedKey);
            Assert.Null(UDF.FindFileEntry(data, volume, "Sub/missing.bin", out _));
            Assert.Null(UDF.FindFileEntry(data, volume, "README.TXT/data.bin", out _));
        }

        [Fact]
        public void FindFileEntry_WithoutDirectories_ReadsOnlyThePath()
        {
            var data = new MemoryStream(UDFImageBuilder.Build());
            var volume = new UDF().Deserialize(data, includeDirectories: false);
            Assert.NotNull(volume);
            Assert.Empty(volume.FileEntries);
            Assert.Empty(volume.Directories);

            var entry = UDF.FindFileEntry(data, volume, "Sub/data.bin", out long key);

            Assert.NotNull(entry);
            Assert.Equal(UDFExtensions.GetKey(0, 7), key);

            // Root, "Sub" and "data.bin" only
            Assert.Equal(3, volume.FileEntries.Count);
            Assert.Equal(2, volume.Directories.Count);
            Assert.False(volume.FileEntries.ContainsKey(UDFExtensions.GetKey(0, 4)));
        }

        [Fact]
        public void AllocationExtent_Followed()
        {
            byte[] image = UDFImageBuilder.Build();
            var data = new MemoryStream(image);
            var volume = new UDF().Deserialize(data);
            Assert.NotNull(volume);

            long key = UDFExtensions.GetKey(0, 7);
            var descriptors = volume.GetAllocationDescriptors(key, volume.FileEntries[key]);

            Assert.NotNull(descriptors);
            Assert.Equal(2, descriptors.Count);
            Assert.Equal(8u, descriptors[0].ExtentLocation.LogicalBlockNumber);
            Assert.Equal(9u, descriptors[1].ExtentLocation.LogicalBlockNumber);
            Assert.Equal((uint)UDFImageBuilder.DataBinLength, UDFExtensions.GetExtentLength(descriptors[0].ExtentLength) + UDFExtensions.GetExtentLength(descriptors[1].ExtentLength));

            Assert.True(volume.TryGetSector(descriptors[1].ExtentLocation.GetKey(), out long sector));
            Assert.Equal(UDFImageBuilder.PartitionStart + 9, sector);
        }

        [Fact]
        public void NotRecordedExtent_Kept()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build()));
            Assert.NotNull(volume);

            long key = UDFExtensions.GetKey(0, 11);
            var descriptors = volume.GetAllocationDescriptors(key, volume.FileEntries[key]);

            Assert.NotNull(descriptors);
            var ad = Assert.Single(descriptors);
            Assert.Equal(ExtentType.NOT_RECORDED_BUT_ALLOCATED, UDFExtensions.GetExtentType(ad.ExtentLength));
            Assert.Equal(100u, UDFExtensions.GetExtentLength(ad.ExtentLength));
        }

        [Fact]
        public void CorruptRootEntry_NoDirectories()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(corruptRootEntry: true)));

            Assert.NotNull(volume);
            Assert.NotNull(volume.FileSetDescriptor);
            Assert.Empty(volume.FileEntries);
            Assert.Empty(volume.Directories);
        }

        [Fact]
        public void CorruptRootEntryCrc_NoDirectories()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(corruptRootEntryCrc: true)));

            Assert.NotNull(volume);
            Assert.NotNull(volume.FileSetDescriptor);
            Assert.Empty(volume.FileEntries);
            Assert.Empty(volume.Directories);
        }

        [Fact]
        public void EmptyPartitionMap_EndsMapTable()
        {
            var data = new MemoryStream(UDFImageBuilder.Build(emptyPartitionMap: true));
            var anchor = UDF.ParseAnchorVolumeDescriptorPointer(data, Constants.AnchorVolumeDescriptorPointerSector);
            Assert.NotNull(anchor);

            var sequence = UDF.ParseVolumeDescriptorSequence(data, anchor.MainVolumeDescriptorSequenceExtent);

            var lvd = Assert.Single(sequence.OfType<LogicalVolumeDescriptor>());
            Assert.Empty(lvd.PartitionMaps);
        }

        [Fact]
        public void VolumeDescriptorPointerLoop_Stops()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(pointerLoop: true)));

            Assert.NotNull(volume);
            Assert.Equal(5, volume.FileEntries.Count);
        }

        [Fact]
        public void UnreadableSector_OtherEntriesRead()
        {
            // The "Sub" File Entry (block 5) cannot be read; "README.TXT" (block 4) is read in the same request
            byte[] image = UDFImageBuilder.Build();
            long bad = UDFImageBuilder.Block(5);
            var volume = new UDF().Deserialize(new ThrowingStream(image, bad, bad + UDFImageBuilder.SectorSize));

            // Root, "README.TXT" and "Media"
            long[] expected = [UDFExtensions.GetKey(0, 2), UDFExtensions.GetKey(0, 4), UDFExtensions.GetKey(0, 11)];
            Assert.NotNull(volume);
            Assert.Equal(expected, volume.FileEntries.Keys.OrderBy(k => k));
            Assert.Single(volume.Directories);
        }

        [Fact]
        public void DirectoryPastPartition_OtherEntriesRead()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(subPastPartition: true)));

            // The "Sub" contents cannot be mapped, so its entry "data.bin" (block 7) is not found
            long[] expected = [UDFExtensions.GetKey(0, 2), UDFExtensions.GetKey(0, 4), UDFExtensions.GetKey(0, 5), UDFExtensions.GetKey(0, 11)];
            Assert.NotNull(volume);
            Assert.Equal(expected, volume.FileEntries.Keys.OrderBy(k => k));
            Assert.Single(volume.Directories);
        }

        [Fact]
        public void Sparable_SparingTableRead()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(layout: UDFLayout.Sparable)));

            Assert.NotNull(volume);
            var map = Assert.IsType<SparablePartitionMap>(Assert.Single(volume.GetLogicalVolumeDescriptor()!.PartitionMaps));
            uint[] locations = [40, 44, 0, 0];
            Assert.Equal(UDFImageBuilder.PacketLength, map.PacketLength);
            Assert.Equal(2, map.NumberOfSparingTables);
            Assert.Equal(locations, map.LocationsOfSparingTables);

            var table = volume.SparingTables[0];
            Assert.Equal(4, table.MapEntries.Length);
            Assert.Equal(4u, table.MapEntries[0].OriginalLocation);
            Assert.Equal(104u, table.MapEntries[0].MappedLocation);

            // Root, "README.TXT", "Sub", "data.bin", "Media" and "big.bin", some of them in relocated packets
            Assert.Equal(6, volume.FileEntries.Count);
            Assert.Equal(2, volume.Directories.Count);
        }

        [Fact]
        public void Sparable_SparingTableAcrossSectors()
        {
            // 300 entries, 2456 bytes
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(layout: UDFLayout.Sparable, sparingTableLength: 300)));

            Assert.NotNull(volume);
            Assert.Equal(300, volume.SparingTables[0].MapEntries.Length);
            Assert.Equal(6, volume.FileEntries.Count);
        }

        [Theory]
        [InlineData(SparingTableDamage.Checksum)]
        [InlineData(SparingTableDamage.TagIdentifier)]
        [InlineData(SparingTableDamage.SparingIdentifier)]
        public void Sparable_FirstSparingTableDamaged_SecondUsed(SparingTableDamage damage)
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(layout: UDFLayout.Sparable, firstSparingTable: damage)));

            Assert.NotNull(volume);
            Assert.Equal(104u, volume.SparingTables[0].MapEntries[0].MappedLocation);
            Assert.Equal(6, volume.FileEntries.Count);
        }

        [Fact]
        public void Sparable_FirstSparingTableUnreadable_SecondUsed()
        {
            byte[] image = UDFImageBuilder.Build(layout: UDFLayout.Sparable);
            long bad = UDFImageBuilder.SparingTableSectors[0] * UDFImageBuilder.SectorSize;
            var volume = new UDF().Deserialize(new ThrowingStream(image, bad, bad + UDFImageBuilder.SectorSize));

            Assert.NotNull(volume);
            Assert.Equal(104u, volume.SparingTables[0].MapEntries[0].MappedLocation);
            Assert.Equal(6, volume.FileEntries.Count);
        }

        [Fact]
        public void Sparable_NoReadableSparingTable_MoreTablesThanLocations()
        {
            // The map has 4 locations for its 5 tables; the tables at sectors 40 and 44 cannot be read
            byte[] image = UDFImageBuilder.Build(layout: UDFLayout.Sparable, numberOfSparingTables: 5);
            long start = UDFImageBuilder.SparingTableSectors[0] * UDFImageBuilder.SectorSize;
            long end = (UDFImageBuilder.SparingTableSectors[1] + 1) * UDFImageBuilder.SectorSize;
            var volume = new UDF().Deserialize(new ThrowingStream(image, start, end));

            // Without a table, the relocated packets are read where they were: "README.TXT" (block 4) is not found
            Assert.NotNull(volume);
            Assert.Empty(volume.SparingTables);
            Assert.False(volume.FileEntries.ContainsKey(UDFExtensions.GetKey(0, 4)));
        }

        [Fact]
        public void Virtual_VirtualAllocationTableRead()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(layout: UDFLayout.Virtual)));

            Assert.NotNull(volume);
            var maps = volume.GetLogicalVolumeDescriptor()!.PartitionMaps;
            Assert.Equal(2, maps.Length);
            Assert.IsType<Type1PartitionMap>(maps[0]);
            Assert.IsType<VirtualPartitionMap>(maps[1]);

            var vat = volume.VirtualAllocationTable;
            Assert.NotNull(vat);
            Assert.Null(vat.Identifier);
            Assert.Equal(152, vat.LengthOfHeader);
            Assert.Equal("TESTVOLUME", vat.LogicalVolumeIdentifier.DecodeDString());
            Assert.Equal(4u, vat.NumberOfFiles);
            Assert.Equal(2u, vat.NumberOfDirectories);
            Assert.Equal(0x0200, vat.MinimumUDFReadRevision);
            Assert.Equal(UDFImageBuilder.VirtualBlocks, vat.Entries.Length);
            Assert.Equal(0xFFFFFFFFu, vat.Entries[1]);

            // Root, "README.TXT", "Sub", "data.bin", "Media", "big.bin" and the table's File Entry in the last sector
            Assert.Equal(7, volume.FileEntries.Count);
            Assert.True(volume.FileEntries.ContainsKey(UDFExtensions.GetKey(0, 39)));
            Assert.Equal(2, volume.Directories.Count);
        }

        [Fact]
        public void Virtual150_VirtualAllocationTableRead()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(layout: UDFLayout.Virtual150)));

            Assert.NotNull(volume);
            var vat = volume.VirtualAllocationTable;
            Assert.NotNull(vat);
            Assert.NotNull(vat.Identifier);
            Assert.Equal(0xFFFFFFFFu, vat.PreviousVATICBLocation);
            Assert.Equal(UDFImageBuilder.VirtualBlocks, vat.Entries.Length);
            Assert.Equal(UDFImageBuilder.PhysicalBlock(20), vat.Entries[20]);
            Assert.Equal(7, volume.FileEntries.Count);
        }

        [Fact]
        public void Virtual_LastSectorUnreadable_VirtualAllocationTableBeforeIt()
        {
            byte[] image = UDFImageBuilder.Build(layout: UDFLayout.Virtual, vatCandidate: VatCandidate.NotATable);
            long last = (UDFImageBuilder.Sectors - 1) * UDFImageBuilder.SectorSize;
            var volume = new UDF().Deserialize(new ThrowingStream(image, last, last + UDFImageBuilder.SectorSize));

            Assert.NotNull(volume);
            Assert.NotNull(volume.VirtualAllocationTable);
            Assert.True(volume.FileEntries.ContainsKey(UDFExtensions.GetKey(0, 38)));
            Assert.Equal(7, volume.FileEntries.Count);
        }

        [Fact]
        public void Virtual_CandidateContentsUnreadable_NotKept()
        {
            // The File Entry in the last sector can be read, the contents it records in block 36 cannot
            byte[] image = UDFImageBuilder.Build(layout: UDFLayout.Virtual, vatCandidate: VatCandidate.NotATable);
            long bad = UDFImageBuilder.Block(36);
            var volume = new UDF().Deserialize(new ThrowingStream(image, bad, bad + UDFImageBuilder.SectorSize));

            Assert.NotNull(volume);
            Assert.NotNull(volume.VirtualAllocationTable);
            Assert.False(volume.FileEntries.ContainsKey(UDFExtensions.GetKey(0, 39)));
            Assert.Equal(7, volume.FileEntries.Count);
        }

        [Theory]
        [InlineData(VatCandidate.NotATable)]
        [InlineData(VatCandidate.TooLarge)]
        [InlineData(VatCandidate.Truncated)]
        [InlineData(VatCandidate.ExtendedDescriptors)]
        [InlineData(VatCandidate.PastPartition)]
        public void Virtual_CandidateInLastSector_VirtualAllocationTableBeforeIt(VatCandidate candidate)
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(layout: UDFLayout.Virtual, vatCandidate: candidate)));

            Assert.NotNull(volume);
            Assert.NotNull(volume.VirtualAllocationTable);
            Assert.Equal(UDFImageBuilder.VirtualBlocks, volume.VirtualAllocationTable.Entries.Length);
            Assert.False(volume.FileEntries.ContainsKey(UDFExtensions.GetKey(0, 39)));
            Assert.Equal(7, volume.FileEntries.Count);
        }

        [Fact]
        public void Virtual_RegularFileInLastSector_ContentsNotRead()
        {
            byte[] image = UDFImageBuilder.Build(layout: UDFLayout.Virtual, vatCandidate: VatCandidate.RegularFile);
            long contents = UDFImageBuilder.Block(36);
            var stream = new ThrowingStream(image, contents, contents + UDFImageBuilder.SectorSize);
            var volume = new UDF().Deserialize(stream);

            Assert.NotNull(volume);
            Assert.NotNull(volume.VirtualAllocationTable);
            Assert.False(stream.Touched);
        }

        [Fact]
        public void VirtualMapWithoutPartitionDescriptor_Ignored()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.Build(danglingVirtualMap: true)));

            Assert.NotNull(volume);
            Assert.Equal(3, volume.GetLogicalVolumeDescriptor()!.PartitionMaps.Length);
            Assert.Null(volume.VirtualAllocationTable);
            Assert.Equal(5, volume.FileEntries.Count);
        }

        [Fact]
        public void Session_LaterSession_SectorsFromDiscStart()
        {
            // The second session starts at sector 400: its anchor is in sector 656, its partition starts at sector 660
            byte[] image = UDFImageBuilder.BuildSessions(UDFImageBuilder.Build(subName: "First"), UDFImageBuilder.Build(subName: "Second"), 400);
            var volume = new UDF().Deserialize(new MemoryStream(image), 400, includeDirectories: true);

            Assert.NotNull(volume);
            Assert.Equal("NSR03", Encoding.ASCII.GetString(volume.VolumeRecognitionSequence[1].StandardIdentifier));
            Assert.NotNull(volume.AnchorVolumeDescriptorPointer);
            Assert.Equal(656u, volume.AnchorVolumeDescriptorPointer.DescriptorTag.TagLocation);
            Assert.Equal(660u, volume.GetPartitionDescriptor(0)!.PartitionStartingLocation);
            Assert.Equal("Second", volume.Directories[UDFExtensions.GetKey(0, 2)][3].FileIdentifier.DecodeDCharacters());
            Assert.Equal(5, volume.FileEntries.Count);
        }

        [Fact]
        public void Session_LaterSessionVirtual_VirtualAllocationTableRead()
        {
            byte[] image = UDFImageBuilder.BuildSessions(UDFImageBuilder.Build(), UDFImageBuilder.Build(layout: UDFLayout.Virtual), 400);
            var volume = new UDF().Deserialize(new MemoryStream(image), 400, includeDirectories: true);

            Assert.NotNull(volume);
            Assert.NotNull(volume.VirtualAllocationTable);
            Assert.Equal(UDFImageBuilder.VirtualBlocks, volume.VirtualAllocationTable.Entries.Length);
            Assert.Equal(7, volume.FileEntries.Count);
        }

        [Fact]
        public void Session_NegativeStart_Null()
        {
            // A start that misses the first anchor still finds the one in the last sector, a negative start does not
            byte[] image = UDFImageBuilder.Build(anchorInLastSector: true);

            Assert.NotNull(new UDF().Deserialize(new MemoryStream(image), 1, includeDirectories: true));
            Assert.Null(new UDF().Deserialize(new MemoryStream(image), -1, includeDirectories: true));
        }

        [Theory]
        [InlineData(512)]
        [InlineData(1024)]
        [InlineData(2048)]
        [InlineData(4096)]
        public void SectorSize_DirectoryHierarchy(int sectorSize)
        {
            var data = new MemoryStream(UDFImageBuilder.BuildWithSectorSize(sectorSize));
            var volume = new UDF().Deserialize(data);

            Assert.NotNull(volume);
            Assert.Equal(sectorSize, volume.GetSectorSize());
            Assert.Equal(3, volume.VolumeRecognitionSequence.Length);
            Assert.NotNull(volume.AnchorVolumeDescriptorPointer);
            Assert.Equal(256u, volume.AnchorVolumeDescriptorPointer.DescriptorTag.TagLocation);
            Assert.NotNull(volume.FileSetDescriptor);
            Assert.Equal("TESTSET", volume.FileSetDescriptor.FileSetIdentifier.DecodeDString());

            var entry = UDF.FindFileEntry(data, volume, "data.bin", out long key);
            Assert.NotNull(entry);
            Assert.Equal(UDFExtensions.GetKey(0, 3), key);
            Assert.Equal((ulong)UDFImageBuilder.SectorSizeDataLength, entry.InformationLength);
        }

        [Theory]
        [InlineData(512)]
        [InlineData(1024)]
        [InlineData(4096)]
        public void SectorSize_NoNSRDescriptor_Null(int sectorSize)
        {
            // Like Linux, other sizes than 2048 bytes are only looked for in a volume recognized as UDF
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.BuildWithSectorSize(sectorSize, nsrDescriptor: false)));

            Assert.Null(volume);
        }

        [Fact]
        public void SectorSize2048_NoNSRDescriptor_Read()
        {
            var volume = new UDF().Deserialize(new MemoryStream(UDFImageBuilder.BuildWithSectorSize(2048, nsrDescriptor: false)));

            Assert.NotNull(volume);
            Assert.Equal(2, volume.VolumeRecognitionSequence.Length);
            Assert.Equal(2, volume.FileEntries.Count);
        }

        [Theory]
        [InlineData(512, 2048)]
        [InlineData(2048, 512)]
        public void SectorSize_OtherLogicalBlockSize_Null(int sectorSize, int logicalBlockSize)
        {
            // UDF records the logical sector size as the logical block size, which the volume is read with
            var data = new MemoryStream(UDFImageBuilder.BuildWithSectorSize(sectorSize, logicalBlockSize: logicalBlockSize));

            Assert.Null(new UDF().Deserialize(data));
        }

        [Theory]
        [InlineData(512, 2048)]
        [InlineData(2048, 512)]
        public void SectorSize_OtherLogicalBlockSize_ReserveSequence(int sectorSize, int logicalBlockSize)
        {
            // A Main Volume Descriptor Sequence whose Logical Volume Descriptor records another block size is unusable
            var data = new MemoryStream(UDFImageBuilder.BuildWithSectorSize(sectorSize, logicalBlockSize: logicalBlockSize, reserveSequence: true));
            var volume = new UDF().Deserialize(data);

            Assert.NotNull(volume);
            Assert.Equal(sectorSize, volume.GetSectorSize());
            Assert.NotNull(UDF.FindFileEntry(data, volume, "data.bin", out _));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(64)]
        public void SectorSize4096_DescriptorEvery2048Bytes_DirectoryHierarchy(int sessionStart)
        {
            // Windows records the sequence on 4096-byte sectors with one descriptor every 2048 bytes, which Linux reads too
            var data = new MemoryStream(UDFImageBuilder.BuildWithSectorSize(4096, sessionStart: sessionStart, spacing: 2048));
            var volume = new UDF().Deserialize(data, sessionStart, includeDirectories: true);

            Assert.NotNull(volume);
            Assert.Equal(4096, volume.GetSectorSize());
            Assert.Equal(3, volume.VolumeRecognitionSequence.Length);
            Assert.NotNull(UDF.FindFileEntry(data, volume, "data.bin", out _));
        }

        [Theory]
        [InlineData(512)]
        [InlineData(1024)]
        [InlineData(4096)]
        public void SectorSize_Session_DirectoryHierarchy(int sectorSize)
        {
            // The session start counts sectors of the size the volume is looked for with
            var data = new MemoryStream(UDFImageBuilder.BuildWithSectorSize(sectorSize, sessionStart: 64));
            var volume = new UDF().Deserialize(data, 64, includeDirectories: true);

            Assert.NotNull(volume);
            Assert.Equal(sectorSize, volume.GetSectorSize());
            Assert.NotNull(volume.AnchorVolumeDescriptorPointer);
            Assert.Equal(320u, volume.AnchorVolumeDescriptorPointer.DescriptorTag.TagLocation);
            Assert.NotNull(UDF.FindFileEntry(data, volume, "data.bin", out _));
        }

        [Theory]
        [InlineData(512, 2048, 1)]
        [InlineData(1024, 2048, 1)]
        [InlineData(2048, 2048, 1)]
        [InlineData(4096, 4096, 2)]
        [InlineData(4096, 2048, 2)]
        public void SectorSize_VolumeRecognitionSequence_ReadOncePerSpacing(int sectorSize, int spacing, int reads)
        {
            // From sector 0, every sector size records the sequence at the same bytes, with one descriptor every 2048 bytes
            // or every sector, which only sectors larger than 2048 bytes do
            var data = new ReadCountingStream(UDFImageBuilder.BuildWithSectorSize(sectorSize, spacing: spacing));

            Assert.NotNull(new UDF().Deserialize(data));
            Assert.Equal(reads, data.ReadsAt(32768));
        }

        [Fact]
        public void ParseSparingTable_SectorSize_AcrossSectors()
        {
            // 100 entries, 856 bytes over 2 sectors of 512 bytes
            var data = new MemoryStream(UDFImageBuilder.BuildSparingTable(512, 3, 100));
            var table = UDF.ParseSparingTable(data, 3, 512);

            Assert.NotNull(table);
            Assert.Equal(100, table.MapEntries.Length);
            Assert.Equal(104u, table.MapEntries[0].MappedLocation);
        }

        [Fact]
        public void ParseVirtualAllocationTable_Header_EntriesAfterIt()
        {
            // Header with 8 bytes of implementation use, then 2 entries and 3 bytes
            var data = new byte[160 + 8 + 3];
            data[0] = 160;
            data[2] = 8;
            data[160] = 5;
            data[164] = 7;

            var vat = UDF.ParseVirtualAllocationTable(data, FileType.VIRTUAL_ALLOCATION_TABLE);

            uint[] entries = [5, 7];
            Assert.NotNull(vat);
            Assert.Equal(8, vat.ImplementationUse.Length);
            Assert.Equal(entries, vat.Entries);
        }

        [Theory]
        [InlineData(151)]
        [InlineData(201)]
        public void ParseVirtualAllocationTable_HeaderLengthInvalid_Null(int headerLength)
        {
            var data = new byte[200];
            data[0] = (byte)headerLength;

            Assert.Null(UDF.ParseVirtualAllocationTable(data, FileType.VIRTUAL_ALLOCATION_TABLE));
        }

        [Fact]
        public void ParseVirtualAllocationTable_Trailer_EntriesBeforeIt()
        {
            var data = new byte[8 + 36];
            data[0] = 5;
            data[4] = 7;
            Encoding.ASCII.GetBytes(Constants.VirtualAllocationTableIdentifier, 0, 22, data, 9);
            data[40] = 3;

            var vat = UDF.ParseVirtualAllocationTable(data, FileType.UNSPECIFIED);

            uint[] entries = [5, 7];
            Assert.NotNull(vat);
            Assert.Equal(entries, vat.Entries);
            Assert.Equal(3u, vat.PreviousVATICBLocation);
        }

        [Fact]
        public void ParseVirtualAllocationTable_TrailerWithoutIdentifier_Null()
        {
            Assert.Null(UDF.ParseVirtualAllocationTable(new byte[44], FileType.UNSPECIFIED));
        }

        [Theory]
        [InlineData(FileType.VIRTUAL_ALLOCATION_TABLE)]
        [InlineData(FileType.UNSPECIFIED)]
        public void ParseVirtualAllocationTable_Empty_Null(FileType fileType)
        {
            Assert.Null(UDF.ParseVirtualAllocationTable([], fileType));
        }

        [Fact]
        public void ParseVirtualAllocationTable_OtherFileType_Null()
        {
            var data = new byte[200];
            data[0] = 152;

            Assert.Null(UDF.ParseVirtualAllocationTable(data, FileType.FILE));
        }

        /// <summary>
        /// Stream that throws an IOException for reads touching a range, like a drive with an unreadable sector
        /// </summary>
        private sealed class ThrowingStream(byte[] data, long badStart, long badEnd) : MemoryStream(data)
        {
            /// <summary>
            /// True once a read touched the range
            /// </summary>
            public bool Touched { get; private set; }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (Position < badEnd && Position + count > badStart)
                {
                    Touched = true;
                    throw new IOException("Unreadable sector");
                }

                return base.Read(buffer, offset, count);
            }
        }

        /// <summary>
        /// Memory stream that counts the reads starting at each position
        /// </summary>
        private sealed class ReadCountingStream(byte[] data) : MemoryStream(data)
        {
            private readonly Dictionary<long, int> _reads = [];

            /// <summary>
            /// Number of reads that started at a position
            /// </summary>
            public int ReadsAt(long position)
                => _reads.TryGetValue(position, out int count) ? count : 0;

            public override int Read(byte[] buffer, int offset, int count)
            {
                _reads[Position] = ReadsAt(Position) + 1;
                return base.Read(buffer, offset, count);
            }
        }
    }
}
