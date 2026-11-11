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

        /// <summary>
        /// Stream that throws an IOException for reads touching a range, like a drive with an unreadable sector
        /// </summary>
        private sealed class ThrowingStream(byte[] data, long badStart, long badEnd) : MemoryStream(data)
        {
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (Position < badEnd && Position + count > badStart)
                    throw new IOException("Unreadable sector");

                return base.Read(buffer, offset, count);
            }
        }
    }
}
