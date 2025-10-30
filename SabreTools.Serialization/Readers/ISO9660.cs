using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

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
            if (data == null || !data.CanRead)
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
                if (vdSet == null || vdSet.Length == 0)
                    return null;

                volume.VolumeDescriptorSet = vdSet;

                // Parse the path table group(s) for each base volume descriptor
                var ptgs = ParsePathTableGroups(data, sectorLength, volume.VolumeDescriptorSet);
                if (ptgs == null || ptgs.Length == 0)
                    return null;

                volume.PathTableGroups = ptgs;

                // Parse the root directory descriptor(s) for each base volume descriptor
                var dirs = ParseDirectoryDescriptors(data, sectorLength, volume.VolumeDescriptorSet);
                if (dirs == null || dirs.Count == 0)
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
        public static VolumeDescriptor[]? ParseVolumeDescriptorSet(Stream data, short sectorLength)
        {
            var obj = new List<VolumeDescriptor>();

            bool setTerminated = false;
            while (data.Position < data.Length)
            {
                var volumeDescriptor = ParseVolumeDescriptor(data, sectorLength);

                // If no valid volume descriptor could be read, return the current set
                if (volumeDescriptor == null)
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

            var dr = ParseDirectoryRecord(data, true);
            if (dr == null)
                return null;

            obj.RootDirectoryRecord = dr;

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

            var dr = ParseDirectoryRecord(data, true);
            if (dr == null)
                return null;

            obj.RootDirectoryRecord = dr;

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
                if (vd is not BaseVolumeDescriptor bvd)
                    continue;

                // Parse the path table group in the base volume descriptor
                var pathTableGroups = ParsePathTableGroup(data, sectorLength, bvd);
                if (pathTableGroups != null && pathTableGroups.Count > 0)
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
        public static List<PathTableGroup>? ParsePathTableGroup(Stream data, short sectorLength, BaseVolumeDescriptor vd)
        {
            var groups = new List<PathTableGroup>();

            int sizeL = vd.PathTableSize.LittleEndian;
            int sizeB = vd.PathTableSize.BigEndian;
            int locationL = vd.PathTableLocationL;
            int locationL2 = vd.OptionalPathTableLocationL;
            int locationM = vd.PathTableLocationM;
            int locationM2 = vd.OptionalPathTableLocationM;

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
            if (groupL.PathTableL != null || groupL.OptionalPathTableL != null || groupL.PathTableM != null || groupL.OptionalPathTableM != null)
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
            if (groupB.PathTableL != null || groupB.OptionalPathTableL != null || groupB.PathTableM != null || groupB.OptionalPathTableM != null)
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
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="vd">Set of volume descriptors for a volume</param>
        /// <returns>Filled Dictionary of int to Directory on success, null on error</returns>
        public static Dictionary<int, DirectoryExtent>? ParseDirectoryDescriptors(Stream data, short sectorLength, VolumeDescriptor[] vdSet)
        {
            var directories = new Dictionary<int, DirectoryExtent>();
            foreach (VolumeDescriptor vd in vdSet)
            {
                if (vd is not BaseVolumeDescriptor bvd)
                    continue;

                // Determine logical block size
                short blockLength = bvd.GetLogicalBlockSize(sectorLength);

                // Parse the root directory pointed to from the base volume descriptor
                var descriptors = ParseDirectory(data, sectorLength, blockLength, bvd.RootDirectoryRecord, false);
                if (descriptors == null || descriptors.Count == 0)
                    continue;

                // Merge dictionaries
                foreach (var kvp in descriptors)
                {
                    if (!directories.ContainsKey(kvp.Key))
                        directories.Add(kvp.Key, kvp.Value);
                }
            }

            // Return error (null) if no valid directory descriptors were found
            if (directories.Count == 0)
                return null;

            return directories;
        }

        /// <summary>
        /// Parse a Stream into a map of sector numbers to Directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the directory extent</param>
        /// <param name="bigEndian">True if the Big Endian extent location/length should be parsed</param>
        /// <returns>Filled Dictionary of int to Directory on success, null on error</returns>
        public static Dictionary<int, DirectoryExtent>? ParseDirectory(Stream data, short sectorLength, short blockLength, DirectoryRecord dr, bool bigEndian)
        {
            // Do not parse file extents
#if NET20 || NET35
            if ((dr.FileFlags & FileFlags.DIRECTORY) == 0)
                return null;
#else
            if (!dr.FileFlags.HasFlag(FileFlags.DIRECTORY))
                return null;
#endif

            int blocksPerSector = sectorLength / blockLength;

            // Validate both-endian extent location
            // TODO: Validate both-endian extent length (use the longest / non-zero one)
            int extentLocation = bigEndian ? dr.ExtentLocation.LittleEndian : dr.ExtentLocation.BigEndian;
            int extentLength = bigEndian ? dr.ExtentLength.LittleEndian : dr.ExtentLength.BigEndian;

            // Validate extent within data stream
            if ((extentLocation * blockLength) + extentLength > data.Length)
                return null;

            // Move stream to directory location
            data.SeekIfPossible(extentLocation * blockLength, SeekOrigin.Begin);

            // Read all directory records in this directory
            var records = new List<DirectoryRecord>();
            int pos = 0;
            while (pos < extentLength)
            {
                // Peek next byte to check whether the next record length is not greater than the end of the dir extent
                var recordLength = data.PeekByteValue();

                // If record length of 0x00, next record begins in next sector
                if (recordLength == 0)
                {
                    int paddingLength = sectorLength - (pos % sectorLength);
                    pos += paddingLength;
                    _ = data.ReadBytes(paddingLength);
                    continue;
                }

                // Ensure record will end in this extent
                // TODO: Smartly detect record length for invalid record lengths
                pos += recordLength;
                if (pos > extentLength)
                    break;

                // Get the next directory record
                var directoryRecord = ParseDirectoryRecord(data, false);
                records.Add(directoryRecord);
            }

            // Add current directory to dictionary
            var directories = new Dictionary<int, DirectoryExtent>();
            var currentDirectory = new DirectoryExtent();
            currentDirectory.DirectoryRecords = [.. records];
            directories.Add(extentLocation * blocksPerSector, currentDirectory);

            // Add all child directories to dictionary recursively
            foreach (var record in records)
            {
                // Don't traverse to parent or self
                if (record.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || record.FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                    continue;

                // Recursively parse child directory
                int sectorNum = record.ExtentLocation * blocksPerSector;
                var dir = ParseDirectory(data, sectorLength, blockLength, record, false);
                if (dir == null)
                    continue;

                // Add new directories to dictionary
                foreach (var kvp in dir)
                {
                    if (!directories.ContainsKey(kvp.Key))
                        directories.Add(kvp.Key, kvp.Value);
                }
            }

            // If the extent location field is ambiguous, also parse the big-endian directory extent
            if (!dr.ExtentLocation.IsValid)
            {
                var bigEndianDir = ParseDirectory(data, sectorLength, blockLength, dr, true);
                if (bigEndianDir != null)
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
            obj.ExtentLength = data.ReadInt32BothEndian();

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
        /// Parse a Stream into a DirectoryRecordDateTime
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryRecordDateTime on success, null on error</returns>
        public static DirectoryRecordDateTime? ParseDirectoryRecordDateTime(Stream data)
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
    }
}
