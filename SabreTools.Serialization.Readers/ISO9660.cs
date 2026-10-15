using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Numerics.Extensions;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class ISO9660 : BaseBinaryReader<Volume>
    {
        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data)
            => Deserialize(data, Constants.MinimumSectorSize);

        /// <inheritdoc cref="Deserialize(Stream?)" />
        /// <param name="sectorLength">Size of the logical sector used in the volume</param>
        public Volume? Deserialize(Stream? data, short sectorLength)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            // Ensure the logical sector size is valid (2^n where n>=11)
            if (sectorLength < Constants.MinimumSectorSize || (sectorLength & (sectorLength - 1)) != 0)
                return null;

            // Simple check for a valid stream length
            if (sectorLength * (Constants.SystemAreaSectors + 2) > data.Length - data.Position)
                return null;

            try
            {
                // Create a new Volume to fill
                var volume = new Volume();

                // Read the System Area
                volume.SystemArea = data.ReadBytes(Constants.SystemAreaSectors * sectorLength);

                // Read the set of Volume Descriptors
                var vdSet = ParseVolumeDescriptorSet(data, sectorLength);
                if (vdSet.Length == 0)
                    return null;

                volume.VolumeDescriptorSet = vdSet;

                // Parse the path table group(s) for each base volume descriptor
                var ptgs = ParsePathTableGroups(data, sectorLength, volume.VolumeDescriptorSet);
                if (ptgs is null || ptgs.Length == 0)
                    return null;

                volume.PathTableGroups = ptgs;

                // Parse the root directory descriptor(s) for each base volume descriptor
                var dirs = ParseDirectoryDescriptors(data, sectorLength, volume.VolumeDescriptorSet);
                if (dirs is null || dirs.Count == 0)
                    return null;

                volume.DirectoryDescriptors = dirs;

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        #region Volume Descriptor Parsing

        /// <summary>
        /// Parse a Stream into an array of VolumeDescriptor objects
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor[] on success, null on error</returns>
        public static VolumeDescriptor[] ParseVolumeDescriptorSet(Stream data, short sectorLength)
        {
            var obj = new List<VolumeDescriptor>();

            bool setTerminated = false;
            while (data.Position < data.Length)
            {
                var volumeDescriptor = ParseVolumeDescriptor(data, sectorLength);

                // If no valid volume descriptor could be read, return the current set
                if (volumeDescriptor is null)
                    return [.. obj];

                // If the set has already been terminated and the returned volume descriptor is not another terminator,
                // assume the read volume descriptor is not a valid volume descriptor and return the current set
                if (setTerminated && volumeDescriptor.Type != VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)
                {
                    // Reset stream to before the just-read volume descriptor
                    data.SeekIfPossible(-sectorLength, SeekOrigin.Current);
                    return [.. obj];
                }

                // Add the valid read volume descriptor to the set
                obj.Add(volumeDescriptor);

                // If the set terminator was read, set the set terminated flag (further set terminators may be present)
                if (!setTerminated && volumeDescriptor.Type == VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)
                    setTerminated = true;
            }

            return [.. obj];
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor on success, null on error</returns>
        public static VolumeDescriptor? ParseVolumeDescriptor(Stream data, short sectorLength)
        {
            var type = (VolumeDescriptorType)data.ReadByteValue();

            return type switch
            {
                // Known Volume Descriptors defined by ISO9660
                VolumeDescriptorType.BOOT_RECORD_VOLUME_DESCRIPTOR => ParseBootRecordVolumeDescriptor(data, sectorLength),
                VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR => ParsePrimaryVolumeDescriptor(data, sectorLength),
                VolumeDescriptorType.SUPPLEMENTARY_VOLUME_DESCRIPTOR => ParseSupplementaryVolumeDescriptor(data, sectorLength),
                VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR => ParseVolumePartitionDescriptor(data, sectorLength),
                VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR => ParseVolumeDescriptorSetTerminator(data, sectorLength),

                // Unknown Volume Descriptor
                _ => ParseGenericVolumeDescriptor(data, sectorLength, type),
            };
        }

        /// <summary>
        /// Parse a Stream into a BootRecordVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled BootRecordVolumeDescriptor on success, null on error</returns>
        public static BootRecordVolumeDescriptor? ParseBootRecordVolumeDescriptor(Stream data, short sectorLength)
        {
            var obj = new BootRecordVolumeDescriptor();

            obj.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.SeekIfPossible(-6, SeekOrigin.Current);
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.BootSystemIdentifier = data.ReadBytes(32);
            obj.BootIdentifier = data.ReadBytes(32);
            obj.BootSystemUse = data.ReadBytes(1977);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                data.SeekIfPossible(sectorLength - Constants.MinimumSectorSize, SeekOrigin.Current);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PrimaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled PrimaryVolumeDescriptor on success, null on error</returns>
        public static PrimaryVolumeDescriptor? ParsePrimaryVolumeDescriptor(Stream data, short sectorLength)
        {
            var obj = new PrimaryVolumeDescriptor();

            obj.Type = VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.SeekIfPossible(-6, SeekOrigin.Current);
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.UnusedByte = data.ReadByteValue();
            obj.SystemIdentifier = data.ReadBytes(32);
            obj.VolumeIdentifier = data.ReadBytes(32);
            obj.Unused8Bytes = data.ReadBytes(8);
            obj.VolumeSpaceSize = data.ReadInt32BothEndian();
            obj.Unused32Bytes = data.ReadBytes(32);
            obj.VolumeSetSize = data.ReadInt16BothEndian();
            obj.VolumeSequenceNumber = data.ReadInt16BothEndian();
            obj.LogicalBlockSize = data.ReadInt16BothEndian();
            obj.PathTableSize = data.ReadInt32BothEndian();
            obj.PathTableLocationL = data.ReadInt32LittleEndian();
            obj.OptionalPathTableLocationL = data.ReadInt32LittleEndian();
            obj.PathTableLocationM = data.ReadInt32BigEndian();
            obj.OptionalPathTableLocationM = data.ReadInt32BigEndian();

            obj.RootDirectoryRecord = ParseDirectoryRecord(data, true);

            obj.VolumeSetIdentifier = data.ReadBytes(128);
            obj.PublisherIdentifier = data.ReadBytes(128);
            obj.DataPreparerIdentifier = data.ReadBytes(128);
            obj.ApplicationIdentifier = data.ReadBytes(128);
            obj.CopyrightFileIdentifier = data.ReadBytes(37);
            obj.AbstractFileIdentifier = data.ReadBytes(37);
            obj.BibliographicFileIdentifier = data.ReadBytes(37);

            obj.VolumeCreationDateTime = ParseDecDateTime(data);
            obj.VolumeModificationDateTime = ParseDecDateTime(data);
            obj.VolumeExpirationDateTime = ParseDecDateTime(data);
            obj.VolumeEffectiveDateTime = ParseDecDateTime(data);

            obj.FileStructureVersion = data.ReadByteValue();
            obj.ReservedByte = data.ReadByteValue();
            obj.ApplicationUse = data.ReadBytes(512);
            obj.Reserved653Bytes = data.ReadBytes(653);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SupplementaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled SupplementaryVolumeDescriptor on success, null on error</returns>
        public static SupplementaryVolumeDescriptor? ParseSupplementaryVolumeDescriptor(Stream data, short sectorLength)
        {
            var obj = new SupplementaryVolumeDescriptor();

            obj.Type = VolumeDescriptorType.SUPPLEMENTARY_VOLUME_DESCRIPTOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.SeekIfPossible(-6, SeekOrigin.Current);
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.VolumeFlags = (VolumeFlags)data.ReadByteValue();
            obj.SystemIdentifier = data.ReadBytes(32);
            obj.VolumeIdentifier = data.ReadBytes(32);
            obj.Unused8Bytes = data.ReadBytes(8);
            obj.VolumeSpaceSize = data.ReadInt32BothEndian();
            obj.EscapeSequences = data.ReadBytes(32);
            obj.VolumeSetSize = data.ReadInt16BothEndian();
            obj.VolumeSequenceNumber = data.ReadInt16BothEndian();
            obj.LogicalBlockSize = data.ReadInt16BothEndian();
            obj.PathTableSize = data.ReadInt32BothEndian();
            obj.PathTableLocationL = data.ReadInt32LittleEndian();
            obj.OptionalPathTableLocationL = data.ReadInt32LittleEndian();
            obj.PathTableLocationM = data.ReadInt32BigEndian();
            obj.OptionalPathTableLocationM = data.ReadInt32BigEndian();

            obj.RootDirectoryRecord = ParseDirectoryRecord(data, true);

            obj.VolumeSetIdentifier = data.ReadBytes(128);
            obj.PublisherIdentifier = data.ReadBytes(128);
            obj.DataPreparerIdentifier = data.ReadBytes(128);
            obj.ApplicationIdentifier = data.ReadBytes(128);
            obj.CopyrightFileIdentifier = data.ReadBytes(37);
            obj.AbstractFileIdentifier = data.ReadBytes(37);
            obj.BibliographicFileIdentifier = data.ReadBytes(37);

            obj.VolumeCreationDateTime = ParseDecDateTime(data);
            obj.VolumeModificationDateTime = ParseDecDateTime(data);
            obj.VolumeExpirationDateTime = ParseDecDateTime(data);
            obj.VolumeEffectiveDateTime = ParseDecDateTime(data);

            obj.FileStructureVersion = data.ReadByteValue();
            obj.ReservedByte = data.ReadByteValue();
            obj.ApplicationUse = data.ReadBytes(512);
            obj.Reserved653Bytes = data.ReadBytes(653);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a VolumePartitionDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumePartitionDescriptor on success, null on error</returns>
        public static VolumePartitionDescriptor? ParseVolumePartitionDescriptor(Stream data, short sectorLength)
        {
            var obj = new VolumePartitionDescriptor();

            obj.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.SeekIfPossible(-6, SeekOrigin.Current);
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.UnusedByte = data.ReadByteValue();
            obj.SystemIdentifier = data.ReadBytes(32);
            obj.VolumePartitionIdentifier = data.ReadBytes(32);
            obj.VolumePartitionLocation = data.ReadInt32BothEndian();
            obj.VolumePartitionSize = data.ReadInt32BothEndian();
            obj.SystemUse = data.ReadBytes(1960);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptorSetTerminator
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptorSetTerminator on success, null on error</returns>
        public static VolumeDescriptorSetTerminator? ParseVolumeDescriptorSetTerminator(Stream data, short sectorLength)
        {
            var obj = new VolumeDescriptorSetTerminator();

            obj.Type = VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.SeekIfPossible(-6, SeekOrigin.Current);
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.Reserved2041Bytes = data.ReadBytes(2041);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a GenericVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="type">Type </param>
        /// <returns>Filled GenericVolumeDescriptor on success, null on error</returns>
        public static GenericVolumeDescriptor? ParseGenericVolumeDescriptor(Stream data, short sectorLength, VolumeDescriptorType type)
        {
            var obj = new GenericVolumeDescriptor();

            obj.Type = type;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.SeekIfPossible(-6, SeekOrigin.Current);
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.Data = data.ReadBytes(2041);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        #endregion

        #region Path Table Parsing

        /// <summary>
        /// Parse a Stream into an array of PathTableGroup
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="vd">Primary/Supplementary/Enhanced Volume Descriptor pointing to path table(s)</param>
        /// <returns>Filled PathTableGroup[] on success, null on error</returns>
        public static PathTableGroup[]? ParsePathTableGroups(Stream data, short sectorLength, VolumeDescriptor[] vdSet)
        {
            var groups = new List<PathTableGroup>();
            foreach (VolumeDescriptor vd in vdSet)
            {
                // Parse the path table group in the base volume descriptor
                var pathTableGroups = ParsePathTableGroup(data, sectorLength, vd);
                if (pathTableGroups.Count > 0)
                    groups.AddRange(pathTableGroups);
            }

            // Return error (null) if no valid path table groups were found
            if (groups.Count == 0)
                return null;

            return [.. groups];
        }

        /// <summary>
        /// Parse a Stream into a list of PathTableGroup
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="vd">Primary/Supplementary/Enhanced Volume Descriptor pointing to path table(s)</param>
        /// <returns>Filled list of PathTableGroup on success, null on error</returns>
        public static List<PathTableGroup> ParsePathTableGroup(Stream data, short sectorLength, VolumeDescriptor vd)
        {
            int sizeL, sizeB, locationL, locationL2, locationM, locationM2;
            if (vd is PrimaryVolumeDescriptor pvd)
            {
                sizeL = pvd.PathTableSize.LittleEndian;
                sizeB = pvd.PathTableSize.BigEndian;
                locationL = pvd.PathTableLocationL;
                locationL2 = pvd.OptionalPathTableLocationL;
                locationM = pvd.PathTableLocationM;
                locationM2 = pvd.OptionalPathTableLocationM;
            }
            else if (vd is SupplementaryVolumeDescriptor svd)
            {
                sizeL = svd.PathTableSize.LittleEndian;
                sizeB = svd.PathTableSize.BigEndian;
                locationL = svd.PathTableLocationL;
                locationL2 = svd.OptionalPathTableLocationL;
                locationM = svd.PathTableLocationM;
                locationM2 = svd.OptionalPathTableLocationM;
            }
            else
            {
                return [];
            }

            var groups = new List<PathTableGroup>();
            short blockLength = vd.GetLogicalBlockSize(sectorLength);

            var groupL = new PathTableGroup();
            if (locationL != 0 && ((locationL * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationL * blockLength, SeekOrigin.Begin);
                groupL.PathTableL = ParsePathTable(data, sizeL, true);
            }

            if (locationL2 != 0 && ((locationL2 * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationL2 * blockLength, SeekOrigin.Begin);
                groupL.OptionalPathTableL = ParsePathTable(data, sizeL, true);
            }

            if (locationM != 0 && ((locationM * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationM * blockLength, SeekOrigin.Begin);
                groupL.PathTableM = ParsePathTable(data, sizeL, false);
            }

            if (locationM2 != 0 && ((locationM2 * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationM2 * blockLength, SeekOrigin.Begin);
                groupL.OptionalPathTableM = ParsePathTable(data, sizeL, false);
            }

            // If no valid path tables were found, don't add the table group
            if (groupL.PathTableL is not null || groupL.OptionalPathTableL is not null || groupL.PathTableM is not null || groupL.OptionalPathTableM is not null)
                groups.Add(groupL);

            // If the both-endian path table size value is consistent, return the single path table group
            if (sizeL == sizeB)
                return groups;

            // Get the other-sized path table group
            var groupB = new PathTableGroup();
            if (locationL != 0 && ((locationL * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationL * blockLength, SeekOrigin.Begin);
                groupB.PathTableL = ParsePathTable(data, sizeB, true);
            }

            if (locationL2 != 0 && ((locationL2 * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationL2 * blockLength, SeekOrigin.Begin);
                groupB.OptionalPathTableL = ParsePathTable(data, sizeB, true);
            }

            if (locationM != 0 && ((locationM * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationM * blockLength, SeekOrigin.Begin);
                groupB.PathTableM = ParsePathTable(data, sizeB, false);
            }

            if (locationM2 != 0 && ((locationM2 * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationM2 * blockLength, SeekOrigin.Begin);
                groupB.OptionalPathTableM = ParsePathTable(data, sizeB, false);
            }

            // If no valid path tables were found, don't add the table group
            if (groupB.PathTableL is not null || groupB.OptionalPathTableL is not null || groupB.PathTableM is not null || groupB.OptionalPathTableM is not null)
                groups.Add(groupB);

            return groups;
        }

        /// <summary>
        /// Parse a Stream into an array of path table records
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="tableSize">Size of the path table</param>
        /// <param name="littleEndian">True if path table is little endian, false if big endian</param>
        /// <returns>Filled array of path table records on success, null on error</returns>
        public static PathTableRecord[]? ParsePathTable(Stream data, int tableSize, bool littleEndian)
        {
            var pathTable = new List<PathTableRecord>();

            // TODO: Better deal with invalid path table sizes < 10 (manually detect valid records to determine size)
            // Current status: Trusting path table length field (tableSize)
            int pos = 0;
            while (pos < tableSize)
            {
                var record = new PathTableRecord();
                var directoryIdentifierLength = data.ReadByteValue();

                // Check that the current record can fit within the current path table size
                pos += 8 + directoryIdentifierLength;
                if (directoryIdentifierLength % 2 != 0)
                    pos += 1;
                if (pos > tableSize)
                {
                    // Invalid record length, quit early
                    // TODO: Try detect record length and recover?
                    break;
                }

                record.DirectoryIdentifierLength = directoryIdentifierLength;
                record.ExtendedAttributeRecordLength = data.ReadByteValue();

                // Read numerics with correct endianness
                if (littleEndian)
                {
                    record.ExtentLocation = data.ReadInt32LittleEndian();
                    record.ParentDirectoryNumber = data.ReadInt16LittleEndian();
                }
                else
                {
                    record.ExtentLocation = data.ReadInt32BigEndian();
                    record.ParentDirectoryNumber = data.ReadInt16BigEndian();
                }

                // Read the directory identifier
                record.DirectoryIdentifier = data.ReadBytes(record.DirectoryIdentifierLength);

                // Padding field is present is directory identifier length is odd
                if (record.DirectoryIdentifierLength % 2 != 0)
                    record.PaddingField = data.ReadByteValue();

                pathTable.Add(record);
            }

            // Return error (null) if no valid path table records were found
            if (pathTable.Count == 0)
                return null;

            return [.. pathTable];
        }

        #endregion

        #region Directory Descriptor Parsing

        /// <summary>
        /// Parse a Stream into a map of sector numbers to Directory
        /// </summary>
        /// <remarks>
        /// The directory trees of all volume descriptors are read together, in the order of their extents on the volume:
        /// always the next known extent at or after the last one read, wrapping around to the lowest one at the end
        /// </remarks>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="vd">Set of volume descriptors for a volume</param>
        /// <returns>Filled Dictionary of int to FileExtent on success, null on error</returns>
        public static Dictionary<int, FileExtent>? ParseDirectoryDescriptors(Stream data, short sectorLength, VolumeDescriptor[] vdSet)
        {
            var directories = new Dictionary<int, FileExtent>();
            var parsed = new Dictionary<long, bool>();
            var pending = new List<PendingExtent>();
            foreach (VolumeDescriptor vd in vdSet)
            {
                DirectoryRecord rootDirectoryRecord;
                if (vd is PrimaryVolumeDescriptor pvd)
                    rootDirectoryRecord = pvd.RootDirectoryRecord;
                else if (vd is SupplementaryVolumeDescriptor svd)
                    rootDirectoryRecord = svd.RootDirectoryRecord;
                else
                    continue;

                // Determine logical block size
                short blockLength = vd.GetLogicalBlockSize(sectorLength);

                // Queue the root directory pointed to from the base volume descriptor
                QueueExtent(data, blockLength, rootDirectoryRecord, pending);
            }

            // Read the queued extents in volume order
            long position = 0;
            while (pending.Count > 0)
            {
                int index = FindNextExtent(pending, position);
                var extent = pending[index];
                pending.RemoveAt(index);
                position = extent.Offset + extent.Length;

                // Each extent is read once
                if (parsed.ContainsKey(extent.Offset))
                    continue;

                parsed[extent.Offset] = true;
                ParseExtent(data, sectorLength, extent, directories, pending);
            }

            // Return error (null) if no valid directory descriptors were found
            if (directories.Count == 0)
                return null;

            return directories;
        }

        /// <summary>
        /// Queue the extent a directory record points to, if it needs to be read
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the extent</param>
        /// <param name="pending">Queued extents, sorted by offset</param>
        private static void QueueExtent(Stream data, short blockLength, DirectoryRecord dr, List<PendingExtent> pending)
        {
            // Only directories and files with an extended attribute record are read
            if ((dr.FileFlags & FileFlags.DIRECTORY) != FileFlags.DIRECTORY && dr.ExtendedAttributeRecordLength == 0)
                return;

            QueueExtent(data, blockLength, dr, false, pending);

            // If the extent location field is ambiguous, also queue the big-endian extent
            if (!dr.ExtentLocation.IsValid)
                QueueExtent(data, blockLength, dr, true, pending);
        }

        /// <summary>
        /// Queue the extent a directory record points to, using one endianness
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the extent</param>
        /// <param name="bigEndian">True if the Big Endian extent location/length should be used</param>
        /// <param name="pending">Queued extents, sorted by offset</param>
        private static void QueueExtent(Stream data, short blockLength, DirectoryRecord dr, bool bigEndian, List<PendingExtent> pending)
        {
            if (!TryGetExtent(data, blockLength, dr, bigEndian, out int extentLocation, out long extentOffset, out uint extentLength))
                return;

            var extent = new PendingExtent(extentLocation, extentOffset, extentLength, blockLength, dr);
            int index = pending.BinarySearch(extent, PendingExtentComparer.Instance);
            pending.Insert(index < 0 ? ~index : index, extent);
        }

        /// <summary>
        /// Find the first queued extent at or after a position, or the first one if there is none
        /// </summary>
        /// <param name="pending">Queued extents, sorted by offset</param>
        /// <param name="position">Offset just after the last extent read</param>
        /// <returns>Index of the next extent to read</returns>
        private static int FindNextExtent(List<PendingExtent> pending, long position)
        {
            var probe = new PendingExtent(0, position, 0, 0, null);
            int index = pending.BinarySearch(probe, PendingExtentComparer.Instance);
            if (index < 0)
            {
                index = ~index;
            }
            else
            {
                while (index > 0 && pending[index - 1].Offset == position)
                {
                    index--;
                }
            }

            return index < pending.Count ? index : 0;
        }

        /// <summary>
        /// Parse one queued extent and queue the extents its directory records point to
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="extent">Extent to parse</param>
        /// <param name="directories">Map of sector numbers to parsed extents</param>
        /// <param name="pending">Queued extents, sorted by offset</param>
        private static void ParseExtent(Stream data, short sectorLength, PendingExtent extent, Dictionary<int, FileExtent> directories, List<PendingExtent> pending)
        {
            var dr = extent.Record!;

            // Move stream to extent location
            data.SeekIfPossible(extent.Offset, SeekOrigin.Begin);

            // Check if the current extent is a directory
            if ((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
            {
                // Read all directory records in this directory
                var records = ParseDirectoryRecords(data, sectorLength, extent.Length);
                if (records is null)
                    return;

                // Add current directory to dictionary
                var currentDirectory = new DirectoryExtent();
                currentDirectory.DirectoryRecords = [.. records];
                if (!directories.ContainsKey(extent.Location))
                    directories.Add(extent.Location, currentDirectory);

                // Queue all child extents
                foreach (var record in records)
                {
                    // Don't traverse to parent or self
                    if (record.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || record.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                        continue;

                    QueueExtent(data, extent.BlockLength, record, pending);
                }
            }
            else if (dr.ExtendedAttributeRecordLength > 0)
            {
                // Extent is a file, parse the Extended Attribute Record
                var fileExtent = new FileExtent();
                var ear = ParseExtendedAttributeRecord(data);
                if (ear is not null)
                    fileExtent.ExtendedAttributeRecord = ear;

                // Put the file extent is the dictionary
                if (!directories.ContainsKey(extent.Location))
                    directories.Add(extent.Location, fileExtent);
            }
        }

        /// <summary>
        /// Get the extent a directory record points to, using one endianness
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the extent</param>
        /// <param name="bigEndian">True if the Big Endian extent location/length should be used</param>
        /// <param name="extentLocation">Logical block number of the extent</param>
        /// <param name="extentOffset">Offset of the extent in the stream</param>
        /// <param name="extentLength">Number of bytes in the extent</param>
        /// <returns>True if the extent is valid, false otherwise</returns>
        private static bool TryGetExtent(Stream data, short blockLength, DirectoryRecord dr, bool bigEndian, out int extentLocation, out long extentOffset, out uint extentLength)
        {
            // Use provided extent endinanness
            extentLocation = bigEndian ? dr.ExtentLocation.BigEndian : dr.ExtentLocation.LittleEndian;
            extentLength = bigEndian ? dr.ExtentLength.BigEndian : dr.ExtentLength.LittleEndian;
            extentOffset = extentLocation * (long)blockLength;
            long extentFinal = extentOffset + extentLength;

            // Deal with extent length ambiguity
            if (!dr.ExtentLength.IsValid)
            {
                // If provided extent length is invalid, use the other value
                if (extentLength <= 0 || extentFinal > data.Length)
                    extentLength = bigEndian ? dr.ExtentLength.LittleEndian : dr.ExtentLength.BigEndian;
            }

            // Validate extent length
            return extentLength > 0 && extentFinal <= data.Length;
        }

        /// <summary>
        /// Parse the directory records of one directory extent
        /// </summary>
        /// <param name="data">Stream to parse, positioned at the start of the directory extent</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="extentLength">Number of bytes in the directory extent</param>
        /// <returns>Filled list of DirectoryRecord on success, null if the directory is invalid</returns>
        private static List<DirectoryRecord>? ParseDirectoryRecords(Stream data, short sectorLength, uint extentLength)
        {
            // Start of directory should not be 0
            int firstRecordLength = data.PeekByteValue();
            if (firstRecordLength == 0)
                return null;

            // Read all directory records in this directory
            var records = new List<DirectoryRecord>();
            uint pos = 0;
            while (pos < extentLength)
            {
                // Peek next byte to check whether the next record length is not greater than the end of the dir extent
                var recordLength = data.PeekByteValue();

                // If record length of 0x00, next record begins in next sector
                if (recordLength == 0)
                {
                    int paddingLength = (int)((uint)sectorLength - (pos % (uint)sectorLength));
                    pos += (uint)paddingLength;
                    _ = data.ReadBytes(paddingLength);

                    // Finish parsing records if end reached
                    if (pos >= extentLength)
                        break;

                    // Start of sector should not be 0, ignore entire directory
                    int nextRecordLength = data.PeekByteValue();
                    if (nextRecordLength <= 0)
                        return null;

                    continue;
                }

                // Ensure record will end in this extent
                // TODO: Smartly detect record length for invalid record lengths
                pos += recordLength;
                if (pos > extentLength)
                    break;

                // Get the next directory record
                var directoryRecord = ParseDirectoryRecord(data, false);

                // Compare recordLength with number of bytes in directoryRecord and ignore the directory if mismatch
                var readLength = 33 + directoryRecord.FileIdentifier.Length + (directoryRecord.PaddingField is null ? 0 : 1) + directoryRecord.SystemUse.Length;
                if (readLength != recordLength)
                    return null;

                records.Add(directoryRecord);
            }

            return records;
        }

        /// <summary>
        /// Find the directory records of one path, reading only the directories along the path
        /// </summary>
        /// <remarks>
        /// Names are compared case-insensitively and without a version suffix (";1") or a trailing period.
        /// A file recorded in several extents returns one directory record per extent, in recorded order.
        /// </remarks>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="vd">Primary or supplementary volume descriptor whose directory hierarchy is searched</param>
        /// <param name="path">Path from the root directory, separated by '/' or '\'</param>
        /// <returns>Filled array of DirectoryRecord on success, null if the path was not found</returns>
        public static DirectoryRecord[]? FindDirectoryRecords(Stream data, short sectorLength, VolumeDescriptor vd, string path)
        {
            DirectoryRecord current;
            if (vd is PrimaryVolumeDescriptor pvd)
                current = pvd.RootDirectoryRecord;
            else if (vd is SupplementaryVolumeDescriptor svd)
                current = svd.RootDirectoryRecord;
            else
                return null;

            // Determine logical block size
            short blockLength = vd.GetLogicalBlockSize(sectorLength);

            // TODO: Better encoding detection (EscapeSequences)
            var encoding = vd is SupplementaryVolumeDescriptor ? Encoding.BigEndianUnicode : Encoding.UTF8;

            string[] names = path.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
            if (names.Length == 0)
                return [current];

            for (int i = 0; i < names.Length; i++)
            {
                // Only a directory can contain the next name
                if ((current.FileFlags & FileFlags.DIRECTORY) != FileFlags.DIRECTORY)
                    return null;

                // If the extent location field is ambiguous, also search the big-endian directory extent
                var found = FindInDirectory(data, sectorLength, blockLength, current, false, encoding, names[i]);
                if (found is null && !current.ExtentLocation.IsValid)
                    found = FindInDirectory(data, sectorLength, blockLength, current, true, encoding, names[i]);

                if (found is null)
                    return null;

                if (i == names.Length - 1)
                    return found;

                current = found[0];
            }

            return null;
        }

        /// <summary>
        /// Find the directory records of one name in one directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the directory extent</param>
        /// <param name="bigEndian">True if the Big Endian extent location/length should be used</param>
        /// <param name="encoding">Encoding of the file identifiers</param>
        /// <param name="name">Name to find</param>
        /// <returns>Filled array of DirectoryRecord on success, null if the name was not found</returns>
        private static DirectoryRecord[]? FindInDirectory(Stream data, short sectorLength, short blockLength, DirectoryRecord dr, bool bigEndian, Encoding encoding, string name)
        {
            if (!TryGetExtent(data, blockLength, dr, bigEndian, out _, out long extentOffset, out uint extentLength))
                return null;

            // Move stream to directory location
            data.SeekIfPossible(extentOffset, SeekOrigin.Begin);

            var records = ParseDirectoryRecords(data, sectorLength, extentLength);
            if (records is null)
                return null;

            string wanted = TrimIdentifier(name);
            for (int i = 0; i < records.Count; i++)
            {
                // Don't match parent or self
                if (records[i].FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || records[i].FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                    continue;

                if (!string.Equals(TrimIdentifier(encoding.GetString(records[i].FileIdentifier)), wanted, StringComparison.OrdinalIgnoreCase))
                    continue;

                // A multi-extent file continues in the following records, up to the final one
                var matches = new List<DirectoryRecord> { records[i] };
                while ((records[i].FileFlags & FileFlags.MULTI_EXTENT) == FileFlags.MULTI_EXTENT && i + 1 < records.Count)
                {
                    i++;
                    matches.Add(records[i]);
                }

                return [.. matches];
            }

            return null;
        }

        /// <summary>
        /// Remove the version suffix and a trailing period from a file identifier
        /// </summary>
        /// <param name="identifier">File identifier as recorded or requested</param>
        /// <returns>File identifier without version suffix and trailing period</returns>
        private static string TrimIdentifier(string identifier)
        {
            int versionSeparator = identifier.LastIndexOf(';');
            if (versionSeparator >= 0)
                identifier = identifier.Substring(0, versionSeparator);

            if (identifier.EndsWith("."))
                identifier = identifier.Substring(0, identifier.Length - 1);

            return identifier;
        }

        /// <summary>
        /// Parse a Stream into a map of sector numbers to Directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the directory extent</param>
        /// <param name="bigEndian">True if the Big Endian extent location/length should be parsed</param>
        /// <returns>Filled Dictionary of int to FileExtent on success, null on error</returns>
        public static Dictionary<int, FileExtent>? ParseDirectory(Stream data, short sectorLength, short blockLength, DirectoryRecord dr, bool bigEndian)
        {
            var directories = new Dictionary<int, FileExtent>();
            int blocksPerSector = sectorLength / blockLength;

            // Use provided extent endinanness
            int extentLocation = bigEndian ? dr.ExtentLocation.BigEndian : dr.ExtentLocation.LittleEndian;
            uint extentLength = bigEndian ? dr.ExtentLength.BigEndian : dr.ExtentLength.LittleEndian;
            long extentOffset = extentLocation * (long)blockLength;
            long extentFinal = extentOffset + extentLength;

            // Deal with extent length ambiguity
            if (!dr.ExtentLength.IsValid)
            {
                // If provided extent length is invalid, use the other value
                if (extentLength <= 0 || extentFinal > data.Length)
                    extentLength = bigEndian ? dr.ExtentLength.LittleEndian : dr.ExtentLength.BigEndian;
            }

            // Validate extent length
            if (extentLength <= 0 || extentFinal > data.Length)
                return null;

            // Move stream to directory location
            data.SeekIfPossible(extentOffset, SeekOrigin.Begin);

            // Check if the current extent is a directory
#if NET20 || NET35
            if ((dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
#else
            if (dr.FileFlags.HasFlag(FileFlags.DIRECTORY))
#endif
            {
                // Read all directory records in this directory
                var records = ParseDirectoryRecords(data, sectorLength, extentLength);
                if (records is null)
                    return null;

                // Add current directory to dictionary
                var currentDirectory = new DirectoryExtent();
                currentDirectory.DirectoryRecords = [.. records];
                directories.Add(extentLocation, currentDirectory);

                // Add all child directories to dictionary recursively
                foreach (var record in records)
                {
                    // Don't traverse to parent or self
                    if (record.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || record.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                        continue;

                    // Recursively parse child directory
                    int sectorNum = record.ExtentLocation * blocksPerSector;
                    var dir = ParseDirectory(data, sectorLength, blockLength, record, false);
                    if (dir is null)
                        continue;

                    // Add new directories to dictionary
                    foreach (var kvp in dir)
                    {
                        if (!directories.ContainsKey(kvp.Key))
                            directories.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            else
            {
                // Extent is a file, parse the Extended Attribute Record
                var fileExtent = new FileExtent();
                if (dr.ExtendedAttributeRecordLength > 0)
                {
                    var ear = ParseExtendedAttributeRecord(data);
                    if (ear is not null)
                    {
                        fileExtent.ExtendedAttributeRecord = ear;
                    }
                    // Do not parse file data into file extent, too large

                    // Put the file extent is the dictionary
#if NETCOREAPP
                    directories.TryAdd(extentLocation, fileExtent);
#else
                    if (!directories.ContainsKey(extentLocation))
                        directories.Add(extentLocation, fileExtent);
#endif
                }
            }

            // If the extent location field is ambiguous, also parse the big-endian directory extent
            if (!bigEndian && !dr.ExtentLocation.IsValid)
            {
                var bigEndianDir = ParseDirectory(data, sectorLength, blockLength, dr, true);
                if (bigEndianDir is not null)
                {
                    // Add new directories to dictionary
                    foreach (var kvp in bigEndianDir)
                    {
                        if (!directories.ContainsKey(kvp.Key))
                            directories.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return directories;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="root">true if root directory record, false otherwise</param>
        /// <returns>Filled DirectoryRecord on success, null on error</returns>
        public static DirectoryRecord ParseDirectoryRecord(Stream data, bool root)
        {
            var obj = new DirectoryRecord();

            obj.DirectoryRecordLength = data.ReadByteValue();
            obj.ExtendedAttributeRecordLength = data.ReadByteValue();
            obj.ExtentLocation = data.ReadInt32BothEndian();
            obj.ExtentLength = data.ReadUInt32BothEndian();

            obj.RecordingDateTime = ParseDirectoryRecordDateTime(data);

            obj.FileFlags = (FileFlags)data.ReadByteValue();
            obj.FileUnitSize = data.ReadByteValue();
            obj.InterleaveGapSize = data.ReadByteValue();
            obj.VolumeSequenceNumber = data.ReadInt16BothEndian();
            obj.FileIdentifierLength = data.ReadByteValue();

            // Root directory within the volume descriptor has a single byte file identifier
            if (root)
                obj.FileIdentifier = data.ReadBytes(1);
            else if (obj.FileIdentifierLength > 0)
                obj.FileIdentifier = data.ReadBytes(obj.FileIdentifierLength);

            // If file identifier length is even, there is a padding field byte
            if (obj.FileIdentifierLength % 2 == 0)
                obj.PaddingField = data.ReadByteValue();

            // Root directory within the volume descriptor has no system use bytes, fixed at 34bytes
            if (root)
                return obj;

            // Calculate actual size of record
            int totalBytes = 33 + obj.FileIdentifierLength;

            // Calculate the size of the system use section (remaining allocated bytes)
            int systemUseLength = obj.DirectoryRecordLength - 33 - obj.FileIdentifierLength;

            // Account for padding field after file identifier
            if (obj.FileIdentifierLength % 2 == 0)
            {
                totalBytes += 1;
                systemUseLength -= 1;
            }

            // If System Use is empty, or if DirectoryRecordLength is bad, return early
            if (systemUseLength < 1)
            {
                // Total record size must be even, read a padding byte
                if (totalBytes % 2 != 0)
                    obj.SystemUse = data.ReadBytes(1);

                return obj;
            }

            // Total used bytes must be even, read a padding byte
            totalBytes += systemUseLength;
            if (totalBytes % 2 != 0)
                systemUseLength += 1;

            // Read system use field
            obj.SystemUse = data.ReadBytes(systemUseLength);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExtendedAttributeRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="root">true if root directory record, false otherwise</param>
        /// <returns>Filled ExtendedAttributeRecord on success, null on error</returns>
        public static ExtendedAttributeRecord ParseExtendedAttributeRecord(Stream data)
        {
            var obj = new ExtendedAttributeRecord();

            obj.OwnerIdentification = data.ReadInt16BothEndian();
            obj.GroupIdentification = data.ReadInt16BothEndian();
            obj.Permissions = (Permissions)data.ReadUInt16LittleEndian();
            obj.FileCreationDateTime = ParseDecDateTime(data);
            obj.FileModificationDateTime = ParseDecDateTime(data);
            obj.FileExpirationDateTime = ParseDecDateTime(data);
            obj.FileEffectiveDateTime = ParseDecDateTime(data);
            obj.RecordFormat = (RecordFormat)data.ReadByteValue();
            obj.RecordAttributes = (RecordAttributes)data.ReadByteValue();
            obj.RecordLength = data.ReadInt16BothEndian();
            obj.SystemIdentifier = data.ReadBytes(32);
            obj.SystemUse = data.ReadBytes(64);
            obj.ExtendedAttributeRecordVersion = data.ReadByteValue();
            obj.EscapeSequencesLength = data.ReadByteValue();
            obj.Reserved64Bytes = data.ReadBytes(64);
            obj.ApplicationLength = data.ReadInt16BothEndian();

            if (obj.ApplicationLength > 0)
                obj.ApplicationUse = data.ReadBytes(obj.ApplicationLength);

            if (obj.EscapeSequencesLength > 0)
                obj.EscapeSequences = data.ReadBytes(obj.EscapeSequencesLength);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryRecordDateTime
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryRecordDateTime on success, null on error</returns>
        public static DirectoryRecordDateTime ParseDirectoryRecordDateTime(Stream data)
        {
            var obj = new DirectoryRecordDateTime();

            obj.YearsSince1990 = data.ReadByteValue();
            obj.Month = data.ReadByteValue();
            obj.Day = data.ReadByteValue();
            obj.Hour = data.ReadByteValue();
            obj.Minute = data.ReadByteValue();
            obj.Second = data.ReadByteValue();
            obj.TimezoneOffset = data.ReadByteValue();

            return obj;
        }

        #endregion

        /// <summary>
        /// Parse a Stream into a DecDateTime
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DecDateTime on success, null on error</returns>
        public static DecDateTime ParseDecDateTime(Stream data)
        {
            var obj = new DecDateTime();

            obj.Year = data.ReadBytes(4);
            obj.Month = data.ReadBytes(2);
            obj.Day = data.ReadBytes(2);
            obj.Hour = data.ReadBytes(2);
            obj.Minute = data.ReadBytes(2);
            obj.Second = data.ReadBytes(2);
            obj.Centisecond = data.ReadBytes(2);
            obj.TimezoneOffset = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Extent queued for reading
        /// </summary>
        private sealed class PendingExtent
        {
            /// <summary>
            /// Logical block number of the extent
            /// </summary>
            public int Location { get; }

            /// <summary>
            /// Byte offset of the extent
            /// </summary>
            public long Offset { get; }

            /// <summary>
            /// Length of the extent in bytes
            /// </summary>
            public uint Length { get; }

            /// <summary>
            /// Number of bytes in a logical block of the volume
            /// </summary>
            public short BlockLength { get; }

            /// <summary>
            /// Directory record pointing to the extent
            /// </summary>
            public DirectoryRecord? Record { get; }

            public PendingExtent(int location, long offset, uint length, short blockLength, DirectoryRecord? record)
            {
                Location = location;
                Offset = offset;
                Length = length;
                BlockLength = blockLength;
                Record = record;
            }
        }

        /// <summary>
        /// Orders queued extents by offset
        /// </summary>
        private sealed class PendingExtentComparer : IComparer<PendingExtent>
        {
            /// <summary>
            /// Shared instance
            /// </summary>
            public static readonly PendingExtentComparer Instance = new();

            /// <inheritdoc/>
            public int Compare(PendingExtent? x, PendingExtent? y)
                => (x?.Offset ?? 0).CompareTo(y?.Offset ?? 0);
        }
    }
}
