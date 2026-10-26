using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.UDF;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class UDF : BaseBinaryReader<Volume>
    {
        /// <summary>
        /// Maximum number of Volume Descriptor Pointers followed in a Volume Descriptor Sequence
        /// </summary>
        private const int MaximumVolumeDescriptorPointers = 16;

        /// <summary>
        /// CRC-ITU-T (CCITT, polynomial 0x1021) table for descriptor CRCs
        /// </summary>
        private static readonly ushort[] CrcTable = CreateCrcTable();

        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data)
            => Deserialize(data, includeDirectories: true);

        /// <inheritdoc cref="Deserialize(Stream?)" />
        /// <param name="includeDirectories">
        /// True to parse the whole directory hierarchy, false to parse only the volume and file set structures
        /// (use FindFileEntry to look up single paths afterwards)
        /// </param>
        /// <remarks>Logical sectors are addressed from the start of the stream</remarks>
        public Volume? Deserialize(Stream? data, bool includeDirectories)
        {
            // If the data is invalid
            if (data is null || !data.CanRead || !data.CanSeek)
                return null;

            try
            {
                // Create a new Volume to fill
                var volume = new Volume();

                // Read the Volume Recognition Sequence
                volume.VolumeRecognitionSequence = ParseVolumeRecognitionSequence(data);

                // Read the Anchor Volume Descriptor Pointer at sector 256, then at N - 256 and N (anchor points, ECMA-167 3/8.4.2.1)
                var anchor = ParseAnchorVolumeDescriptorPointer(data, Constants.AnchorVolumeDescriptorPointerSector);
                long lastSector = (data.Length / Constants.SectorSize) - 1;
                if (anchor is null && lastSector > Constants.AnchorVolumeDescriptorPointerSector)
                    anchor = ParseAnchorVolumeDescriptorPointer(data, lastSector - Constants.AnchorVolumeDescriptorPointerSector);
                if (anchor is null && lastSector > Constants.AnchorVolumeDescriptorPointerSector)
                    anchor = ParseAnchorVolumeDescriptorPointer(data, lastSector);
                if (anchor is null)
                    return null;

                volume.AnchorVolumeDescriptorPointer = anchor;

                // Read the Main Volume Descriptor Sequence, then the equivalent Reserve one (ECMA-167 3/8.4.2.2) if the main one is unusable
                volume.VolumeDescriptorSequence = ParseVolumeDescriptorSequence(data, anchor.MainVolumeDescriptorSequenceExtent);
                if (!HasLogicalVolume(volume))
                    volume.VolumeDescriptorSequence = ParseVolumeDescriptorSequence(data, anchor.ReserveVolumeDescriptorSequenceExtent);
                if (!HasLogicalVolume(volume))
                    return null;

                // Read the File Entries of Metadata Files, needed to map Metadata Partition blocks
                var lvd = volume.GetLogicalVolumeDescriptor()!;
                foreach (var map in lvd.PartitionMaps)
                {
                    if (map is not MetadataPartitionMap metadata)
                        continue;

                    int physical = lvd.GetType1PartitionReference(metadata.PartitionNumber);
                    if (physical < 0)
                        continue;

                    var state = new HierarchyState(data, volume, includeDirectories: false);
                    state.QueueEntry(UDFExtensions.GetKey((ushort)physical, metadata.MetadataFileLocation));
                    state.QueueEntry(UDFExtensions.GetKey((ushort)physical, metadata.MetadataMirrorFileLocation));
                    state.Run();
                }

                // Read the File Set Descriptor
                var fsd = ParseFileSetDescriptor(data, volume, lvd.LogicalVolumeContentsUse.ExtentLocation);
                if (fsd is null)
                    return null;

                volume.FileSetDescriptor = fsd;

                // Read the directory hierarchy from the root directory
                if (includeDirectories)
                {
                    var state = new HierarchyState(data, volume, includeDirectories: true);
                    state.QueueEntry(fsd.RootDirectoryICB.ExtentLocation.GetKey());
                    state.Run();
                }

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        #region Lookup

        /// <summary>
        /// Find the File Entry of a path by walking the directories on the path only
        /// Names are compared exactly first, then case-insensitively; deleted entries are ignored
        /// File Entries and directories read on the way are added to the volume (reused by later lookups)
        /// </summary>
        /// <param name="data">Stream containing the volume</param>
        /// <param name="volume">Volume parsed by Deserialize (directories may be excluded)</param>
        /// <param name="path">Path from the root directory, separated by '/' or '\'</param>
        /// <param name="key">Logical block address key of the File Entry on success, -1 otherwise</param>
        /// <returns>File Entry on success, null if the path does not exist or could not be read</returns>
        public static FileEntry? FindFileEntry(Stream data, Volume volume, string path, out long key)
        {
            key = -1;
            if (volume.FileSetDescriptor is null)
                return null;

            long current = volume.FileSetDescriptor.RootDirectoryICB.ExtentLocation.GetKey();
            var entry = LoadEntry(data, volume, current);

            string[] names = path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in names)
            {
                if (entry is null || entry.ICBTag.FileType != FileType.DIRECTORY)
                    return null;

                if (!volume.Directories.TryGetValue(current, out var fids))
                    return null;

                var fid = FindInDirectory(fids, name);
                if (fid is null)
                    return null;

                current = fid.ICB.ExtentLocation.GetKey();
                entry = LoadEntry(data, volume, current);
            }

            if (entry is null)
                return null;

            key = current;
            return entry;
        }

        /// <summary>
        /// Read a File Entry (with its Allocation Extent Descriptors, and its directory if it is one) unless already read
        /// </summary>
        private static FileEntry? LoadEntry(Stream data, Volume volume, long key)
        {
            if (!volume.FileEntries.TryGetValue(key, out var entry)
                || (entry.ICBTag.FileType == FileType.DIRECTORY && !volume.Directories.ContainsKey(key)))
            {
                var state = new HierarchyState(data, volume, includeDirectories: false);
                state.QueueEntry(key);
                state.Run();
                volume.FileEntries.TryGetValue(key, out entry);
            }

            return entry;
        }

        /// <summary>
        /// Find a name in the File Identifier Descriptors of a directory, exact match first, then case-insensitive
        /// </summary>
        private static FileIdentifierDescriptor? FindInDirectory(FileIdentifierDescriptor[] fids, string name)
        {
            FileIdentifierDescriptor? caseInsensitive = null;
            foreach (var fid in fids)
            {
                if ((fid.FileCharacteristics & (FileCharacteristics.DELETED | FileCharacteristics.PARENT)) != 0)
                    continue;

                string? identifier = fid.FileIdentifier.DecodeDCharacters();
                if (identifier is null)
                    continue;

                if (string.Equals(identifier, name, StringComparison.Ordinal))
                    return fid;

                if (caseInsensitive is null && string.Equals(identifier, name, StringComparison.OrdinalIgnoreCase))
                    caseInsensitive = fid;
            }

            return caseInsensitive;
        }

        #endregion

        #region Volume Structure Parsing

        /// <summary>
        /// Parse the Volume Recognition Sequence starting at sector 16
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Volume Structure Descriptors up to the first unknown one</returns>
        public static VolumeStructureDescriptor[] ParseVolumeRecognitionSequence(Stream data)
        {
            var obj = new List<VolumeStructureDescriptor>();

            long sector = Constants.VolumeRecognitionSequenceSector;
            while (true)
            {
                byte[] block = ReadSectors(data, sector, 1);

                int offset = 0;
                var vsd = new VolumeStructureDescriptor();
                vsd.StructureType = block.ReadByteValue(ref offset);
                vsd.StandardIdentifier = block.ReadBytes(ref offset, 5);
                vsd.StructureVersion = block.ReadByteValue(ref offset);
                vsd.StructureData = block.ReadBytes(ref offset, Constants.SectorSize - 7);

                if (!IsKnownStandardIdentifier(vsd.StandardIdentifier))
                    break;

                obj.Add(vsd);
                sector++;

                // The extended area ends with the Terminating Extended Area Descriptor
                if (vsd.StandardIdentifier.EqualsExactly(Constants.TerminatingExtendedAreaDescriptor))
                    break;
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse an Anchor Volume Descriptor Pointer
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sector">Logical sector of the anchor</param>
        /// <returns>Filled AnchorVolumeDescriptorPointer on success, null on error</returns>
        public static AnchorVolumeDescriptorPointer? ParseAnchorVolumeDescriptorPointer(Stream data, long sector)
        {
            byte[] block = ReadSectors(data, sector, 1);
            if (!IsValidDescriptor(block, 0, block.Length, (uint)sector))
                return null;

            int offset = 0;
            var tag = ParseDescriptorTag(block, ref offset);
            if (tag.TagIdentifier != TagIdentifier.ANCHOR_VOLUME_DESCRIPTOR_POINTER)
                return null;

            var obj = new AnchorVolumeDescriptorPointer();
            obj.DescriptorTag = tag;
            obj.MainVolumeDescriptorSequenceExtent = ParseExtentDescriptor(block, ref offset);
            obj.ReserveVolumeDescriptorSequenceExtent = ParseExtentDescriptor(block, ref offset);
            obj.Reserved = block.ReadBytes(ref offset, 480);
            return obj;
        }

        /// <summary>
        /// Parse a Volume Descriptor Sequence, following Volume Descriptor Pointers
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="extent">Extent of the sequence</param>
        /// <returns>Descriptors up to the Terminating Descriptor or the first invalid descriptor</returns>
        public static Descriptor[] ParseVolumeDescriptorSequence(Stream data, ExtentDescriptor extent)
        {
            var obj = new List<Descriptor>();

            long sector = extent.ExtentLocation;
            long end = sector + (extent.ExtentLength / Constants.SectorSize);
            int pointers = 0;
            while (sector < end)
            {
                byte[] block = ReadSectors(data, sector, 1);
                if (!IsValidDescriptor(block, 0, block.Length, (uint)sector))
                    break;

                var descriptor = ParseVolumeDescriptor(block);
                obj.Add(descriptor);

                TagIdentifier tagIdentifier = descriptor.DescriptorTag.TagIdentifier;
                if (tagIdentifier == TagIdentifier.TERMINATING_DESCRIPTOR)
                    break;

                // Continue the sequence in the extent of a Volume Descriptor Pointer
                if (descriptor is VolumeDescriptorPointer pointer)
                {
                    if (++pointers > MaximumVolumeDescriptorPointers)
                        break;

                    sector = pointer.NextVolumeDescriptorSequenceExtent.ExtentLocation;
                    end = sector + (pointer.NextVolumeDescriptorSequenceExtent.ExtentLength / Constants.SectorSize);
                    continue;
                }

                sector++;
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a validated descriptor of the Volume Descriptor Sequence
        /// </summary>
        /// <param name="block">Logical sector holding the descriptor</param>
        /// <returns>Filled Descriptor</returns>
        public static Descriptor ParseVolumeDescriptor(byte[] block)
        {
            int offset = 0;
            var tag = ParseDescriptorTag(block, ref offset);
            return tag.TagIdentifier switch
            {
                TagIdentifier.PRIMARY_VOLUME_DESCRIPTOR => ParsePrimaryVolumeDescriptor(block, ref offset, tag),
                TagIdentifier.VOLUME_DESCRIPTOR_POINTER => ParseVolumeDescriptorPointer(block, ref offset, tag),
                TagIdentifier.PARTITION_DESCRIPTOR => ParsePartitionDescriptor(block, ref offset, tag),
                TagIdentifier.LOGICAL_VOLUME_DESCRIPTOR => ParseLogicalVolumeDescriptor(block, ref offset, tag),

                // Other descriptors are kept as raw data
                TagIdentifier.ANCHOR_VOLUME_DESCRIPTOR_POINTER
                    or TagIdentifier.IMPLEMENTATION_USE_VOLUME_DESCRIPTOR
                    or TagIdentifier.UNALLOCATED_SPACE_DESCRIPTOR
                    or TagIdentifier.TERMINATING_DESCRIPTOR
                    or TagIdentifier.LOGICAL_VOLUME_INTEGRITY_DESCRIPTOR
                    or TagIdentifier.FILE_SET_DESCRIPTOR
                    or TagIdentifier.FILE_IDENTIFIER_DESCRIPTOR
                    or TagIdentifier.ALLOCATION_EXTENT_DESCRIPTOR
                    or TagIdentifier.INDIRECT_ENTRY
                    or TagIdentifier.TERMINAL_ENTRY
                    or TagIdentifier.FILE_ENTRY
                    or TagIdentifier.EXTENDED_ATTRIBUTE_HEADER_DESCRIPTOR
                    or TagIdentifier.UNALLOCATED_SPACE_ENTRY
                    or TagIdentifier.SPACE_BITMAP_DESCRIPTOR
                    or TagIdentifier.PARTITION_INTEGRITY_ENTRY
                    or TagIdentifier.EXTENDED_FILE_ENTRY => ParseGenericDescriptor(block, ref offset, tag),
                _ => ParseGenericDescriptor(block, ref offset, tag),
            };
        }

        /// <summary>
        /// Parse a descriptor that is not parsed further after its tag
        /// </summary>
        private static GenericDescriptor ParseGenericDescriptor(byte[] data, ref int offset, DescriptorTag tag)
        {
            var obj = new GenericDescriptor();

            obj.DescriptorTag = tag;
            obj.Data = data.ReadBytes(ref offset, data.Length - offset);

            return obj;
        }

        /// <summary>
        /// Parse a Primary Volume Descriptor after its tag
        /// </summary>
        private static PrimaryVolumeDescriptor ParsePrimaryVolumeDescriptor(byte[] data, ref int offset, DescriptorTag tag)
        {
            var obj = new PrimaryVolumeDescriptor();

            obj.DescriptorTag = tag;
            obj.VolumeDescriptorSequenceNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.PrimaryVolumeDescriptorNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.VolumeIdentifier = data.ReadBytes(ref offset, 32);
            obj.VolumeSequenceNumber = data.ReadUInt16LittleEndian(ref offset);
            obj.MaximumVolumeSequenceNumber = data.ReadUInt16LittleEndian(ref offset);
            obj.InterchangeLevel = data.ReadUInt16LittleEndian(ref offset);
            obj.MaximumInterchangeLevel = data.ReadUInt16LittleEndian(ref offset);
            obj.CharacterSetList = data.ReadUInt32LittleEndian(ref offset);
            obj.MaximumCharacterSetList = data.ReadUInt32LittleEndian(ref offset);
            obj.VolumeSetIdentifier = data.ReadBytes(ref offset, 128);
            obj.DescriptorCharacterSet = ParseCharacterSetSpecification(data, ref offset);
            obj.ExplanatoryCharacterSet = ParseCharacterSetSpecification(data, ref offset);
            obj.VolumeAbstract = ParseExtentDescriptor(data, ref offset);
            obj.VolumeCopyrightNotice = ParseExtentDescriptor(data, ref offset);
            obj.ApplicationIdentifier = ParseEntityIdentifier(data, ref offset);
            obj.RecordingDateAndTime = ParseTimestamp(data, ref offset);
            obj.ImplementationIdentifier = ParseEntityIdentifier(data, ref offset);
            obj.ImplementationUse = data.ReadBytes(ref offset, 64);
            obj.PredecessorVolumeDescriptorSequenceLocation = data.ReadUInt32LittleEndian(ref offset);
            obj.Flags = data.ReadUInt16LittleEndian(ref offset);
            obj.Reserved = data.ReadBytes(ref offset, 22);

            return obj;
        }

        /// <summary>
        /// Parse a Volume Descriptor Pointer after its tag
        /// </summary>
        private static VolumeDescriptorPointer ParseVolumeDescriptorPointer(byte[] data, ref int offset, DescriptorTag tag)
        {
            var obj = new VolumeDescriptorPointer();

            obj.DescriptorTag = tag;
            obj.VolumeDescriptorSequenceNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.NextVolumeDescriptorSequenceExtent = ParseExtentDescriptor(data, ref offset);
            obj.Reserved = data.ReadBytes(ref offset, 484);

            return obj;
        }

        /// <summary>
        /// Parse a Partition Descriptor after its tag
        /// </summary>
        private static PartitionDescriptor ParsePartitionDescriptor(byte[] data, ref int offset, DescriptorTag tag)
        {
            var obj = new PartitionDescriptor();

            obj.DescriptorTag = tag;
            obj.VolumeDescriptorSequenceNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.PartitionFlags = data.ReadUInt16LittleEndian(ref offset);
            obj.PartitionNumber = data.ReadUInt16LittleEndian(ref offset);
            obj.PartitionContents = ParseEntityIdentifier(data, ref offset);
            obj.PartitionContentsUse = data.ReadBytes(ref offset, 128);
            obj.AccessType = data.ReadUInt32LittleEndian(ref offset);
            obj.PartitionStartingLocation = data.ReadUInt32LittleEndian(ref offset);
            obj.PartitionLength = data.ReadUInt32LittleEndian(ref offset);
            obj.ImplementationIdentifier = ParseEntityIdentifier(data, ref offset);
            obj.ImplementationUse = data.ReadBytes(ref offset, 128);
            obj.Reserved = data.ReadBytes(ref offset, 156);

            return obj;
        }

        /// <summary>
        /// Parse a Logical Volume Descriptor after its tag
        /// </summary>
        private static LogicalVolumeDescriptor ParseLogicalVolumeDescriptor(byte[] data, ref int offset, DescriptorTag tag)
        {
            var obj = new LogicalVolumeDescriptor();

            obj.DescriptorTag = tag;
            obj.VolumeDescriptorSequenceNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.DescriptorCharacterSet = ParseCharacterSetSpecification(data, ref offset);
            obj.LogicalVolumeIdentifier = data.ReadBytes(ref offset, 128);
            obj.LogicalBlockSize = data.ReadUInt32LittleEndian(ref offset);
            obj.DomainIdentifier = ParseEntityIdentifier(data, ref offset);
            obj.LogicalVolumeContentsUse = ParseLongAllocationDescriptor(data, ref offset);
            obj.MapTableLength = data.ReadUInt32LittleEndian(ref offset);
            obj.NumberOfPartitionMaps = data.ReadUInt32LittleEndian(ref offset);
            obj.ImplementationIdentifier = ParseEntityIdentifier(data, ref offset);
            obj.ImplementationUse = data.ReadBytes(ref offset, 128);
            obj.IntegritySequenceExtent = ParseExtentDescriptor(data, ref offset);

            // Partition maps, bounded by the map table length and the logical sector
            var maps = new List<PartitionMap>();
            int end = Math.Min(data.Length, offset + (int)Math.Min(obj.MapTableLength, (uint)data.Length));
            for (uint i = 0; i < obj.NumberOfPartitionMaps && offset + 2 <= end; i++)
            {
                int start = offset;
                byte length = data[offset + 1];
                if (length < 2)
                    break;

                maps.Add(ParsePartitionMap(data, ref offset));
                offset = start + length;
            }

            obj.PartitionMaps = [.. maps];

            return obj;
        }

        /// <summary>
        /// Parse a Partition Map
        /// </summary>
        private static PartitionMap ParsePartitionMap(byte[] data, ref int offset)
        {
            int start = offset;
            byte type = data[offset];
            byte length = data[offset + 1];

            if (type == Constants.Type1PartitionMap && length >= 6)
            {
                var type1 = new Type1PartitionMap();
                type1.PartitionMapType = data.ReadByteValue(ref offset);
                type1.PartitionMapLength = data.ReadByteValue(ref offset);
                type1.VolumeSequenceNumber = data.ReadUInt16LittleEndian(ref offset);
                type1.PartitionNumber = data.ReadUInt16LittleEndian(ref offset);
                return type1;
            }

            if (type == Constants.Type2PartitionMap && length >= 64)
            {
                offset += 4;
                var identifier = ParseEntityIdentifier(data, ref offset);
                offset = start;

                string? name = DecodeIdentifier(identifier.Identifier);
                Type2PartitionMap type2 = name == Constants.MetadataPartitionIdentifier
                    ? new MetadataPartitionMap()
                    : new Type2PartitionMap();

                type2.PartitionMapType = data.ReadByteValue(ref offset);
                type2.PartitionMapLength = data.ReadByteValue(ref offset);
                type2.Reserved = data.ReadBytes(ref offset, 2);
                type2.PartitionTypeIdentifier = ParseEntityIdentifier(data, ref offset);
                type2.VolumeSequenceNumber = data.ReadUInt16LittleEndian(ref offset);
                type2.PartitionNumber = data.ReadUInt16LittleEndian(ref offset);
                if (type2 is MetadataPartitionMap metadata)
                {
                    metadata.MetadataFileLocation = data.ReadUInt32LittleEndian(ref offset);
                    metadata.MetadataMirrorFileLocation = data.ReadUInt32LittleEndian(ref offset);
                    metadata.MetadataBitmapFileLocation = data.ReadUInt32LittleEndian(ref offset);
                    metadata.AllocationUnitSize = data.ReadUInt32LittleEndian(ref offset);
                    metadata.AlignmentUnitSize = data.ReadUInt16LittleEndian(ref offset);
                    metadata.Flags = data.ReadByteValue(ref offset);
                    metadata.Reserved2 = data.ReadBytes(ref offset, 5);
                }
                else
                {
                    type2.PartitionTypeData = data.ReadBytes(ref offset, 24);
                }

                return type2;
            }

            // Unknown partition map type, keep the raw data
            var unknown = new Type2PartitionMap();
            unknown.PartitionMapType = data.ReadByteValue(ref offset);
            unknown.PartitionMapLength = data.ReadByteValue(ref offset);
            unknown.PartitionTypeData = data.ReadBytes(ref offset, Math.Max(0, length - 2));
            return unknown;
        }

        #endregion

        #region File Structure Parsing

        /// <summary>
        /// Parse the File Set Descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="volume">Volume containing the Volume Descriptor Sequence</param>
        /// <param name="location">Logical block address of the File Set Descriptor</param>
        /// <returns>Filled FileSetDescriptor on success, null on error</returns>
        public static FileSetDescriptor? ParseFileSetDescriptor(Stream data, Volume volume, LogicalBlockAddress location)
        {
            if (!volume.TryGetSector(location.GetKey(), out long sector))
                return null;

            byte[] block = ReadSectors(data, sector, 1);
            if (!IsValidDescriptor(block, 0, block.Length, location.LogicalBlockNumber))
                return null;

            int offset = 0;
            var tag = ParseDescriptorTag(block, ref offset);
            if (tag.TagIdentifier != TagIdentifier.FILE_SET_DESCRIPTOR)
                return null;

            var obj = new FileSetDescriptor();

            obj.DescriptorTag = tag;
            obj.RecordingDateAndTime = ParseTimestamp(block, ref offset);
            obj.InterchangeLevel = block.ReadUInt16LittleEndian(ref offset);
            obj.MaximumInterchangeLevel = block.ReadUInt16LittleEndian(ref offset);
            obj.CharacterSetList = block.ReadUInt32LittleEndian(ref offset);
            obj.MaximumCharacterSetList = block.ReadUInt32LittleEndian(ref offset);
            obj.FileSetNumber = block.ReadUInt32LittleEndian(ref offset);
            obj.FileSetDescriptorNumber = block.ReadUInt32LittleEndian(ref offset);
            obj.LogicalVolumeIdentifierCharacterSet = ParseCharacterSetSpecification(block, ref offset);
            obj.LogicalVolumeIdentifier = block.ReadBytes(ref offset, 128);
            obj.FileSetCharacterSet = ParseCharacterSetSpecification(block, ref offset);
            obj.FileSetIdentifier = block.ReadBytes(ref offset, 32);
            obj.CopyrightFileIdentifier = block.ReadBytes(ref offset, 32);
            obj.AbstractFileIdentifier = block.ReadBytes(ref offset, 32);
            obj.RootDirectoryICB = ParseLongAllocationDescriptor(block, ref offset);
            obj.DomainIdentifier = ParseEntityIdentifier(block, ref offset);
            obj.NextExtent = ParseLongAllocationDescriptor(block, ref offset);
            obj.SystemStreamDirectoryICB = ParseLongAllocationDescriptor(block, ref offset);
            obj.Reserved = block.ReadBytes(ref offset, 32);

            return obj;
        }

        /// <summary>
        /// Parse a File Entry or Extended File Entry
        /// </summary>
        /// <param name="block">Logical block holding the entry</param>
        /// <param name="logicalBlockNumber">Logical block number the entry was read from</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry? ParseFileEntry(byte[] block, uint logicalBlockNumber)
        {
            if (!IsValidDescriptor(block, 0, block.Length, logicalBlockNumber))
                return null;

            int offset = 0;
            var tag = ParseDescriptorTag(block, ref offset);
            bool extended = tag.TagIdentifier == TagIdentifier.EXTENDED_FILE_ENTRY;
            if (!extended && tag.TagIdentifier != TagIdentifier.FILE_ENTRY)
                return null;

            var obj = extended ? new ExtendedFileEntry() : new FileEntry();

            obj.DescriptorTag = tag;
            obj.ICBTag = ParseICBTag(block, ref offset);
            obj.Uid = block.ReadUInt32LittleEndian(ref offset);
            obj.Gid = block.ReadUInt32LittleEndian(ref offset);
            obj.Permissions = block.ReadUInt32LittleEndian(ref offset);
            obj.FileLinkCount = block.ReadUInt16LittleEndian(ref offset);
            obj.RecordFormat = block.ReadByteValue(ref offset);
            obj.RecordDisplayAttributes = block.ReadByteValue(ref offset);
            obj.RecordLength = block.ReadUInt32LittleEndian(ref offset);
            obj.InformationLength = block.ReadUInt64LittleEndian(ref offset);
            if (obj is ExtendedFileEntry efe)
            {
                efe.ObjectSize = block.ReadUInt64LittleEndian(ref offset);
                efe.LogicalBlocksRecorded = block.ReadUInt64LittleEndian(ref offset);
                efe.AccessDateAndTime = ParseTimestamp(block, ref offset);
                efe.ModificationDateAndTime = ParseTimestamp(block, ref offset);
                efe.CreationDateAndTime = ParseTimestamp(block, ref offset);
                efe.AttributeDateAndTime = ParseTimestamp(block, ref offset);
                efe.Checkpoint = block.ReadUInt32LittleEndian(ref offset);
                efe.Reserved = block.ReadUInt32LittleEndian(ref offset);
                efe.ExtendedAttributeICB = ParseLongAllocationDescriptor(block, ref offset);
                efe.StreamDirectoryICB = ParseLongAllocationDescriptor(block, ref offset);
            }
            else
            {
                obj.LogicalBlocksRecorded = block.ReadUInt64LittleEndian(ref offset);
                obj.AccessDateAndTime = ParseTimestamp(block, ref offset);
                obj.ModificationDateAndTime = ParseTimestamp(block, ref offset);
                obj.AttributeDateAndTime = ParseTimestamp(block, ref offset);
                obj.Checkpoint = block.ReadUInt32LittleEndian(ref offset);
                obj.ExtendedAttributeICB = ParseLongAllocationDescriptor(block, ref offset);
            }

            obj.ImplementationIdentifier = ParseEntityIdentifier(block, ref offset);
            obj.UniqueID = block.ReadUInt64LittleEndian(ref offset);
            obj.LengthOfExtendedAttributes = block.ReadUInt32LittleEndian(ref offset);
            obj.LengthOfAllocationDescriptors = block.ReadUInt32LittleEndian(ref offset);

            // Both fields must fit into the logical block
            if (obj.LengthOfExtendedAttributes > block.Length - offset
                || obj.LengthOfAllocationDescriptors > block.Length - offset - obj.LengthOfExtendedAttributes)
            {
                return null;
            }

            obj.ExtendedAttributes = block.ReadBytes(ref offset, (int)obj.LengthOfExtendedAttributes);
            obj.AllocationDescriptors = block.ReadBytes(ref offset, (int)obj.LengthOfAllocationDescriptors);

            return obj;
        }

        /// <summary>
        /// Parse an Allocation Extent Descriptor
        /// </summary>
        /// <param name="block">Logical block holding the descriptor</param>
        /// <param name="logicalBlockNumber">Logical block number the descriptor was read from</param>
        /// <returns>Filled AllocationExtentDescriptor on success, null on error</returns>
        public static AllocationExtentDescriptor? ParseAllocationExtentDescriptor(byte[] block, uint logicalBlockNumber)
        {
            if (!IsValidDescriptor(block, 0, block.Length, logicalBlockNumber))
                return null;

            int offset = 0;
            var tag = ParseDescriptorTag(block, ref offset);
            if (tag.TagIdentifier != TagIdentifier.ALLOCATION_EXTENT_DESCRIPTOR)
                return null;

            var obj = new AllocationExtentDescriptor();

            obj.DescriptorTag = tag;
            obj.PreviousAllocationExtentLocation = block.ReadUInt32LittleEndian(ref offset);
            obj.LengthOfAllocationDescriptors = block.ReadUInt32LittleEndian(ref offset);
            obj.AllocationDescriptors = block.ReadBytes(ref offset, (int)obj.LengthOfAllocationDescriptors);

            return obj;
        }

        /// <summary>
        /// Parse the File Identifier Descriptors of a directory
        /// </summary>
        /// <param name="data">Directory contents</param>
        /// <param name="length">Length of the directory contents in bytes</param>
        /// <returns>File Identifier Descriptors on success, null if the directory is invalid</returns>
        public static FileIdentifierDescriptor[]? ParseDirectory(byte[] data, int length)
        {
            var obj = new List<FileIdentifierDescriptor>();

            int offset = 0;
            while (offset < length)
            {
                var tag = ParseDescriptorTag(data, ref offset);
                if (tag.TagIdentifier != TagIdentifier.FILE_IDENTIFIER_DESCRIPTOR)
                    return null;

                var fid = new FileIdentifierDescriptor();
                fid.DescriptorTag = tag;
                fid.FileVersionNumber = data.ReadUInt16LittleEndian(ref offset);
                fid.FileCharacteristics = (FileCharacteristics)data.ReadByteValue(ref offset);
                fid.LengthOfFileIdentifier = data.ReadByteValue(ref offset);
                fid.ICB = ParseLongAllocationDescriptor(data, ref offset);
                fid.LengthOfImplementationUse = data.ReadUInt16LittleEndian(ref offset);

                // The descriptor is padded to 4 bytes
                int used = 38 + fid.LengthOfImplementationUse + fid.LengthOfFileIdentifier;
                int padded = (used + 3) & ~3;
                fid.ImplementationUse = data.ReadBytes(ref offset, fid.LengthOfImplementationUse);
                fid.FileIdentifier = data.ReadBytes(ref offset, fid.LengthOfFileIdentifier);
                fid.Padding = data.ReadBytes(ref offset, padded - used);

                obj.Add(fid);
            }

            return [.. obj];
        }

        #endregion

        #region Common Structures

        /// <summary>
        /// Parse a Descriptor Tag
        /// </summary>
        public static DescriptorTag ParseDescriptorTag(byte[] data, ref int offset)
        {
            var obj = new DescriptorTag();

            obj.TagIdentifier = (TagIdentifier)data.ReadUInt16LittleEndian(ref offset);
            obj.DescriptorVersion = data.ReadUInt16LittleEndian(ref offset);
            obj.TagChecksum = data.ReadByteValue(ref offset);
            obj.Reserved = data.ReadByteValue(ref offset);
            obj.TagSerialNumber = data.ReadUInt16LittleEndian(ref offset);
            obj.DescriptorCRC = data.ReadUInt16LittleEndian(ref offset);
            obj.DescriptorCRCLength = data.ReadUInt16LittleEndian(ref offset);
            obj.TagLocation = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse an Extent Descriptor (extent_ad)
        /// </summary>
        public static ExtentDescriptor ParseExtentDescriptor(byte[] data, ref int offset)
        {
            var obj = new ExtentDescriptor();

            obj.ExtentLength = data.ReadUInt32LittleEndian(ref offset);
            obj.ExtentLocation = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Long Allocation Descriptor (long_ad)
        /// </summary>
        public static LongAllocationDescriptor ParseLongAllocationDescriptor(byte[] data, ref int offset)
        {
            var obj = new LongAllocationDescriptor();

            obj.ExtentLength = data.ReadUInt32LittleEndian(ref offset);
            obj.ExtentLocation.LogicalBlockNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.ExtentLocation.PartitionReferenceNumber = data.ReadUInt16LittleEndian(ref offset);
            obj.ImplementationUse = data.ReadBytes(ref offset, 6);

            return obj;
        }

        /// <summary>
        /// Parse an Entity Identifier (regid)
        /// </summary>
        public static EntityIdentifier ParseEntityIdentifier(byte[] data, ref int offset)
        {
            var obj = new EntityIdentifier();

            obj.Flags = data.ReadByteValue(ref offset);
            obj.Identifier = data.ReadBytes(ref offset, 23);
            obj.IdentifierSuffix = data.ReadBytes(ref offset, 8);

            return obj;
        }

        /// <summary>
        /// Parse a Character Set Specification (charspec)
        /// </summary>
        public static CharacterSetSpecification ParseCharacterSetSpecification(byte[] data, ref int offset)
        {
            var obj = new CharacterSetSpecification();

            obj.CharacterSetType = data.ReadByteValue(ref offset);
            obj.CharacterSetInformation = data.ReadBytes(ref offset, 63);

            return obj;
        }

        /// <summary>
        /// Parse a Timestamp
        /// </summary>
        public static Timestamp ParseTimestamp(byte[] data, ref int offset)
        {
            var obj = new Timestamp();

            obj.TypeAndTimezone = data.ReadUInt16LittleEndian(ref offset);
            obj.Year = data.ReadInt16LittleEndian(ref offset);
            obj.Month = data.ReadByteValue(ref offset);
            obj.Day = data.ReadByteValue(ref offset);
            obj.Hour = data.ReadByteValue(ref offset);
            obj.Minute = data.ReadByteValue(ref offset);
            obj.Second = data.ReadByteValue(ref offset);
            obj.Centiseconds = data.ReadByteValue(ref offset);
            obj.HundredsOfMicroseconds = data.ReadByteValue(ref offset);
            obj.Microseconds = data.ReadByteValue(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse an ICB Tag (icbtag)
        /// </summary>
        public static ICBTag ParseICBTag(byte[] data, ref int offset)
        {
            var obj = new ICBTag();

            obj.PriorRecordedNumberOfDirectEntries = data.ReadUInt32LittleEndian(ref offset);
            obj.StrategyType = data.ReadUInt16LittleEndian(ref offset);
            obj.StrategyParameter = data.ReadBytes(ref offset, 2);
            obj.MaximumNumberOfEntries = data.ReadUInt16LittleEndian(ref offset);
            obj.Reserved = data.ReadByteValue(ref offset);
            obj.FileType = (FileType)data.ReadByteValue(ref offset);
            obj.ParentICBLocation.LogicalBlockNumber = data.ReadUInt32LittleEndian(ref offset);
            obj.ParentICBLocation.PartitionReferenceNumber = data.ReadUInt16LittleEndian(ref offset);
            obj.Flags = data.ReadUInt16LittleEndian(ref offset);

            return obj;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check a descriptor tag: checksum, version, CRC (when it fits into the data) and location
        /// </summary>
        /// <remarks>ECMA-167 3/7.2; receiving systems allow descriptor version 2 or 3 (3/7.2.2)</remarks>
        /// <param name="data">Data holding the descriptor</param>
        /// <param name="offset">Offset of the descriptor</param>
        /// <param name="available">Number of bytes of the descriptor available from the offset</param>
        /// <param name="location">Expected tag location (logical sector or logical block number)</param>
        /// <returns>True if the tag is valid, false otherwise</returns>
        public static bool IsValidDescriptor(byte[] data, int offset, int available, uint location)
        {
            byte checksum = 0;
            for (int i = 0; i < 16; i++)
            {
                if (i != 4)
                    checksum += data[offset + i];
            }

            if (checksum != data[offset + 4])
                return false;

            int version = data[offset + 2] | (data[offset + 3] << 8);
            if (version != 2 && version != 3)
                return false;

            int crcLength = data[offset + 10] | (data[offset + 11] << 8);
            if (16 + crcLength <= available)
            {
                int crc = data[offset + 8] | (data[offset + 9] << 8);
                if (ComputeCrc(data, offset + 16, crcLength) != crc)
                    return false;
            }

            uint tagLocation = (uint)(data[offset + 12] | (data[offset + 13] << 8) | (data[offset + 14] << 16) | (data[offset + 15] << 24));
            return tagLocation == location;
        }

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

        /// <summary>
        /// Check whether the volume has a Logical Volume Descriptor with a supported logical block size
        /// </summary>
        private static bool HasLogicalVolume(Volume volume)
        {
            var lvd = volume.GetLogicalVolumeDescriptor();
            return lvd is not null && lvd.LogicalBlockSize == Constants.SectorSize;
        }

        /// <summary>
        /// Check whether a standard identifier belongs to the Volume Recognition Sequence
        /// </summary>
        private static bool IsKnownStandardIdentifier(byte[] identifier)
        {
            return identifier.EqualsExactly(Constants.BeginningExtendedAreaDescriptor)
                || identifier.EqualsExactly(Constants.NSR02)
                || identifier.EqualsExactly(Constants.NSR03)
                || identifier.EqualsExactly(Constants.TerminatingExtendedAreaDescriptor)
                || identifier.EqualsExactly(Constants.CD001)
                || identifier.EqualsExactly(Constants.BOOT2)
                || identifier.EqualsExactly(Constants.CDW02);
        }

        /// <summary>
        /// Decode an Entity Identifier's identifier field (ASCII, padded with 0x00)
        /// </summary>
        private static string? DecodeIdentifier(byte[] identifier)
        {
            int length = Array.IndexOf(identifier, (byte)0);
            if (length < 0)
                length = identifier.Length;

            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)identifier[i];
            }

            return new string(chars);
        }

        /// <summary>
        /// Read whole logical sectors
        /// </summary>
        private static byte[] ReadSectors(Stream data, long sector, int count)
        {
            data.SeekIfPossible(sector * Constants.SectorSize, SeekOrigin.Begin);
            return data.ReadBytes(count * Constants.SectorSize);
        }

        #endregion

        #region Directory Hierarchy

        /// <summary>
        /// Kind of a pending read
        /// </summary>
        private enum ReadKind
        {
            /// <summary>
            /// File Entry or Extended File Entry
            /// </summary>
            Entry,

            /// <summary>
            /// Allocation Extent Descriptor of a File Entry
            /// </summary>
            AllocationExtent,

            /// <summary>
            /// Part of the contents of a directory
            /// </summary>
            DirectoryData,
        }

        /// <summary>
        /// Contents of a directory being assembled from its extents
        /// </summary>
        private sealed class DirectoryContents
        {
            public long Key;
            public byte[] Data = [];
            public int Pending;
        }

        /// <summary>
        /// A read waiting for the elevator
        /// </summary>
        private sealed class PendingRead
        {
            public long Sector;
            public int Count;
            public ReadKind Kind;
            public long Key;
            public DirectoryContents? Directory;
            public int DirectoryOffset;
            public int Length;
        }

        /// <summary>
        /// Orders pending reads by sector
        /// </summary>
        private sealed class PendingReadComparer : IComparer<PendingRead>
        {
            public static readonly PendingReadComparer Instance = new();

            public int Compare(PendingRead? x, PendingRead? y)
                => (x?.Sector ?? 0).CompareTo(y?.Sector ?? 0);
        }

        /// <summary>
        /// Reads File Entries, Allocation Extent Descriptors and directories in ascending sector order (C-SCAN),
        /// merging adjacent sectors into one request; structures found in the last request are served from it
        /// </summary>
        private sealed class HierarchyState
        {
            private readonly Stream _data;
            private readonly Volume _volume;
            private readonly bool _includeDirectories;

            /// <summary>
            /// Pending reads ordered by sector
            /// </summary>
            private readonly List<PendingRead> _pending = [];

            /// <summary>
            /// Reads served from the last request, processed before the next request
            /// </summary>
            private readonly Queue<KeyValuePair<PendingRead, byte[]>> _ready = new();

            /// <summary>
            /// Keys of Allocation Extent Descriptors already queued, with the File Entries waiting for them
            /// </summary>
            private readonly Dictionary<long, List<long>> _queuedExtents = [];

            /// <summary>
            /// Sector after the last request
            /// </summary>
            private long _head = 0;

            /// <summary>
            /// Data and first sector of the last request
            /// </summary>
            private byte[] _lastRun = [];
            private long _lastRunSector = -1;

            /// <param name="data">Stream containing the volume</param>
            /// <param name="volume">Volume to fill</param>
            /// <param name="includeDirectories">True to read the entries of every directory read</param>
            public HierarchyState(Stream data, Volume volume, bool includeDirectories)
            {
                _data = data;
                _volume = volume;
                _includeDirectories = includeDirectories;
            }

            /// <summary>
            /// Queue a File Entry, or resolve it if it was already read
            /// </summary>
            public void QueueEntry(long key)
            {
                if (_volume.FileEntries.TryGetValue(key, out var entry))
                {
                    ResolveEntry(key, entry);
                    return;
                }

                if (_volume.TryGetSector(key, out long sector))
                    Queue(new PendingRead { Sector = sector, Count = 1, Kind = ReadKind.Entry, Key = key });
            }

            /// <summary>
            /// Process pending reads until none are left
            /// </summary>
            public void Run()
            {
                while (true)
                {
                    while (_ready.Count > 0)
                    {
                        var ready = _ready.Dequeue();
                        Process(ready.Key, ready.Value);
                    }

                    if (_pending.Count == 0)
                        return;

                    // Next pending read at or after the head, wrapping around to the lowest sector
                    int first = FirstAtOrAfter(_head);
                    if (first >= _pending.Count)
                        first = 0;

                    // Merge adjacent or overlapping reads into one request
                    long runStart = _pending[first].Sector;
                    long runEnd = runStart + _pending[first].Count;
                    int last = first;
                    while (last + 1 < _pending.Count && _pending[last + 1].Sector <= runEnd)
                    {
                        last++;
                        runEnd = Math.Max(runEnd, _pending[last].Sector + _pending[last].Count);
                    }

                    var run = _pending.GetRange(first, last - first + 1);
                    _pending.RemoveRange(first, last - first + 1);
                    _head = runEnd;

                    byte[]? buffer = null;
                    try
                    {
                        buffer = ReadSectors(_data, runStart, (int)(runEnd - runStart));
                    }
                    catch (IOException)
                    {
                        // Unreadable sectors: retry each read alone below
                    }

                    if (buffer is not null)
                    {
                        _lastRun = buffer;
                        _lastRunSector = runStart;
                    }

                    foreach (var read in run)
                    {
                        byte[]? sectors = buffer is not null
                            ? Slice(buffer, (int)(read.Sector - runStart) * Constants.SectorSize, read.Count * Constants.SectorSize)
                            : TryReadSectors(read.Sector, read.Count);
                        if (sectors is not null)
                            Process(read, sectors);
                    }
                }
            }

            /// <summary>
            /// Add a read to the pending reads, or to the ready reads if the last request contains it
            /// </summary>
            private void Queue(PendingRead read)
            {
                long runEnd = _lastRunSector + (_lastRun.Length / Constants.SectorSize);
                if (_lastRunSector >= 0 && read.Sector >= _lastRunSector && read.Sector + read.Count <= runEnd)
                {
                    int offset = (int)(read.Sector - _lastRunSector) * Constants.SectorSize;
                    _ready.Enqueue(new KeyValuePair<PendingRead, byte[]>(read, Slice(_lastRun, offset, read.Count * Constants.SectorSize)));
                    return;
                }

                int index = _pending.BinarySearch(read, PendingReadComparer.Instance);
                if (index < 0)
                    index = ~index;

                // Keep reads of the same sector in queue order
                while (index < _pending.Count && _pending[index].Sector == read.Sector)
                {
                    index++;
                }

                _pending.Insert(index, read);
            }

            /// <summary>
            /// Index of the first pending read at or after a sector
            /// </summary>
            private int FirstAtOrAfter(long sector)
            {
                int low = 0, high = _pending.Count;
                while (low < high)
                {
                    int middle = low + ((high - low) / 2);
                    if (_pending[middle].Sector < sector)
                        low = middle + 1;
                    else
                        high = middle;
                }

                return low;
            }

            /// <summary>
            /// Process the data of a read
            /// </summary>
            private void Process(PendingRead read, byte[] sectors)
            {
                switch (read.Kind)
                {
                    case ReadKind.Entry:
                        var entry = ParseFileEntry(sectors, (uint)read.Key);
                        if (entry is null)
                            return;

                        _volume.FileEntries[read.Key] = entry;
                        ResolveEntry(read.Key, entry);
                        break;

                    case ReadKind.AllocationExtent:
                        var aed = ParseAllocationExtentDescriptor(sectors, (uint)read.Key);
                        if (aed is null)
                            return;

                        _volume.AllocationExtentDescriptors[read.Key] = aed;
                        if (_queuedExtents.TryGetValue(read.Key, out var owners))
                        {
                            foreach (long owner in owners)
                            {
                                if (_volume.FileEntries.TryGetValue(owner, out var ownerEntry))
                                    ResolveEntry(owner, ownerEntry);
                            }
                        }

                        break;

                    case ReadKind.DirectoryData:
                        var directory = read.Directory!;
                        Array.Copy(sectors, 0, directory.Data, read.DirectoryOffset, Math.Min(read.Length, sectors.Length));
                        if (--directory.Pending == 0)
                            CompleteDirectory(directory.Key, directory.Data);

                        break;

                    default:
                        // All read kinds are handled above
                        break;
                }
            }

            /// <summary>
            /// Queue what a File Entry still needs: its Allocation Extent Descriptors, and its contents if it is a directory
            /// </summary>
            private void ResolveEntry(long key, FileEntry entry)
            {
                var descriptors = _volume.GetAllocationDescriptors(key, entry, out long missingKey);
                if (descriptors is null)
                {
                    if (missingKey >= 0)
                        QueueAllocationExtent(missingKey, key);

                    return;
                }

                if (entry.ICBTag.FileType != FileType.DIRECTORY || _volume.Directories.ContainsKey(key))
                    return;

                int length = (int)entry.InformationLength;
                if (entry.ICBTag.GetAllocationDescriptorType() == AllocationDescriptorType.EMBEDDED)
                {
                    if (length <= entry.AllocationDescriptors.Length)
                        CompleteDirectory(key, entry.AllocationDescriptors.Length == length ? entry.AllocationDescriptors : Slice(entry.AllocationDescriptors, 0, length));

                    return;
                }

                // Queue the recorded extents; extents that are not recorded read as zeros
                var directory = new DirectoryContents { Key = key, Data = new byte[length] };
                var reads = new List<PendingRead>();
                int offset = 0;
                foreach (var ad in descriptors)
                {
                    if (offset >= length)
                        break;

                    int extentLength = (int)Math.Min(UDFExtensions.GetExtentLength(ad.ExtentLength), (uint)(length - offset));
                    if (UDFExtensions.GetExtentType(ad.ExtentLength) == ExtentType.RECORDED_AND_ALLOCATED)
                    {
                        if (!_volume.TryGetSector(ad.ExtentLocation.GetKey(), out long sector))
                            return;

                        int count = (extentLength + Constants.SectorSize - 1) / Constants.SectorSize;
                        reads.Add(new PendingRead
                        {
                            Sector = sector,
                            Count = count,
                            Kind = ReadKind.DirectoryData,
                            Key = key,
                            Directory = directory,
                            DirectoryOffset = offset,
                            Length = extentLength,
                        });
                    }

                    offset += extentLength;
                }

                directory.Pending = reads.Count;
                if (reads.Count == 0)
                {
                    CompleteDirectory(key, directory.Data);
                    return;
                }

                foreach (var read in reads)
                {
                    Queue(read);
                }
            }

            /// <summary>
            /// Queue an Allocation Extent Descriptor needed by a File Entry
            /// </summary>
            private void QueueAllocationExtent(long key, long owner)
            {
                if (_queuedExtents.TryGetValue(key, out var owners))
                {
                    if (!owners.Contains(owner))
                        owners.Add(owner);

                    return;
                }

                _queuedExtents[key] = [owner];
                if (_volume.TryGetSector(key, out long sector))
                    Queue(new PendingRead { Sector = sector, Count = 1, Kind = ReadKind.AllocationExtent, Key = key });
            }

            /// <summary>
            /// Parse the contents of a directory and queue the File Entries it identifies
            /// </summary>
            private void CompleteDirectory(long key, byte[] contents)
            {
                var fids = ParseDirectory(contents, contents.Length);
                if (fids is null)
                    return;

                _volume.Directories[key] = fids;
                if (!_includeDirectories)
                    return;

                foreach (var fid in fids)
                {
                    if ((fid.FileCharacteristics & (FileCharacteristics.DELETED | FileCharacteristics.PARENT)) != 0)
                        continue;

                    QueueEntry(fid.ICB.ExtentLocation.GetKey());
                }
            }

            /// <summary>
            /// Read sectors alone, null if they cannot be read
            /// </summary>
            private byte[]? TryReadSectors(long sector, int count)
            {
                try
                {
                    return ReadSectors(_data, sector, count);
                }
                catch (IOException)
                {
                    return null;
                }
            }

            /// <summary>
            /// Copy part of an array
            /// </summary>
            private static byte[] Slice(byte[] data, int offset, int length)
            {
                var slice = new byte[length];
                Array.Copy(data, offset, slice, 0, length);
                return slice;
            }
        }

        #endregion
    }
}
