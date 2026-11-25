using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.ISO9660;
using SabreTools.Data.Models.ISO9660.SystemUseEntries;
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

                // Parse the directories that are only recorded in a path table
                ParsePathTableDirectories(data, sectorLength, volume.VolumeDescriptorSet, ptgs, dirs);

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
            if (IsHighSierraVolumeDescriptor(data))
                return ParseHighSierraVolumeDescriptor(data, sectorLength);

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
        /// Check whether a Stream is positioned at a High Sierra volume descriptor
        /// </summary>
        /// <remarks>A volume descriptor with the ISO 9660 identifier is never read as a High Sierra one</remarks>
        /// <param name="data">Stream to check, positioned at the start of a volume descriptor</param>
        /// <returns>True if the volume descriptor has the High Sierra identifier, false otherwise</returns>
        private static bool IsHighSierraVolumeDescriptor(Stream data)
        {
            if (data.Length - data.Position < 14)
                return false;

            byte[] header = data.PeekBytes(14);
            return !HasIdentifier(header, 1, Constants.StandardIdentifier)
                && HasIdentifier(header, 9, Constants.HighSierraIdentifier);
        }

        /// <summary>
        /// Check whether a volume descriptor header holds an identifier at an offset
        /// </summary>
        /// <param name="header">Start of the volume descriptor</param>
        /// <param name="offset">Offset of the identifier in the header</param>
        /// <param name="identifier">Identifier to compare with</param>
        /// <returns>True if the identifier is present at the offset, false otherwise</returns>
        private static bool HasIdentifier(byte[] header, int offset, byte[] identifier)
        {
            for (int i = 0; i < identifier.Length; i++)
            {
                if (header[offset + i] != identifier[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Parse a Stream into a HighSierraVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled HighSierraVolumeDescriptor</returns>
        public static HighSierraVolumeDescriptor ParseHighSierraVolumeDescriptor(Stream data, short sectorLength)
        {
            // The type follows the logical block number of the volume descriptor
            var type = (VolumeDescriptorType)data.PeekBytes(9)[8];

            return type switch
            {
                // Known High Sierra Volume Descriptors
                VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR => ParseHighSierraPrimaryVolumeDescriptor(data, sectorLength),
                VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR => ParseHighSierraVolumeDescriptorSetTerminator(data, sectorLength),

                // Other and unknown High Sierra Volume Descriptors
                VolumeDescriptorType.BOOT_RECORD_VOLUME_DESCRIPTOR => ParseHighSierraGenericVolumeDescriptor(data, sectorLength),
                VolumeDescriptorType.SUPPLEMENTARY_VOLUME_DESCRIPTOR => ParseHighSierraGenericVolumeDescriptor(data, sectorLength),
                VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR => ParseHighSierraGenericVolumeDescriptor(data, sectorLength),
                _ => ParseHighSierraGenericVolumeDescriptor(data, sectorLength),
            };
        }

        /// <summary>
        /// Parse a Stream into a HighSierraPrimaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled HighSierraPrimaryVolumeDescriptor</returns>
        public static HighSierraPrimaryVolumeDescriptor ParseHighSierraPrimaryVolumeDescriptor(Stream data, short sectorLength)
        {
            var obj = new HighSierraPrimaryVolumeDescriptor();

            obj.LogicalBlockNumber = data.ReadInt32BothEndian();
            obj.Type = (VolumeDescriptorType)data.ReadByteValue();
            obj.Identifier = data.ReadBytes(5);
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
            obj.SecondOptionalPathTableLocationL = data.ReadInt32LittleEndian();
            obj.ThirdOptionalPathTableLocationL = data.ReadInt32LittleEndian();
            obj.PathTableLocationM = data.ReadInt32BigEndian();
            obj.OptionalPathTableLocationM = data.ReadInt32BigEndian();
            obj.SecondOptionalPathTableLocationM = data.ReadInt32BigEndian();
            obj.ThirdOptionalPathTableLocationM = data.ReadInt32BigEndian();

            obj.RootDirectoryRecord = ParseHighSierraRootDirectoryRecord(data);

            obj.VolumeSetIdentifier = data.ReadBytes(128);
            obj.PublisherIdentifier = data.ReadBytes(128);
            obj.DataPreparerIdentifier = data.ReadBytes(128);
            obj.ApplicationIdentifier = data.ReadBytes(128);
            obj.CopyrightFileIdentifier = data.ReadBytes(32);
            obj.AbstractFileIdentifier = data.ReadBytes(32);

            obj.VolumeCreationDateTime = ParseDecDateTime(data, true);
            obj.VolumeModificationDateTime = ParseDecDateTime(data, true);
            obj.VolumeExpirationDateTime = ParseDecDateTime(data, true);
            obj.VolumeEffectiveDateTime = ParseDecDateTime(data, true);

            obj.FileStructureVersion = data.ReadByteValue();
            obj.ReservedByte = data.ReadByteValue();
            obj.ApplicationUse = data.ReadBytes(512);
            obj.Reserved680Bytes = data.ReadBytes(680);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        /// <summary>
        /// Parse the root directory record of a High Sierra volume descriptor
        /// </summary>
        /// <remarks>
        /// Some High Sierra discs record this one directory record in the ISO 9660 layout, with the file flags one byte later.
        /// The directory flag tells the two layouts apart, both have the same length.
        /// </remarks>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryRecord</returns>
        private static DirectoryRecord ParseHighSierraRootDirectoryRecord(Stream data)
        {
            long position = data.Position;
            var obj = ParseDirectoryRecord(data, true, true);
            if ((obj.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY)
                return obj;

            data.SeekIfPossible(position, SeekOrigin.Begin);
            return ParseDirectoryRecord(data, true, false);
        }

        /// <summary>
        /// Parse a Stream into a HighSierraVolumeDescriptorSetTerminator
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled HighSierraVolumeDescriptorSetTerminator</returns>
        public static HighSierraVolumeDescriptorSetTerminator ParseHighSierraVolumeDescriptorSetTerminator(Stream data, short sectorLength)
        {
            var obj = new HighSierraVolumeDescriptorSetTerminator();

            obj.LogicalBlockNumber = data.ReadInt32BothEndian();
            obj.Type = (VolumeDescriptorType)data.ReadByteValue();
            obj.Identifier = data.ReadBytes(5);
            obj.Version = data.ReadByteValue();
            obj.Reserved2033Bytes = data.ReadBytes(2033);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HighSierraGenericVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled HighSierraGenericVolumeDescriptor</returns>
        public static HighSierraGenericVolumeDescriptor ParseHighSierraGenericVolumeDescriptor(Stream data, short sectorLength)
        {
            var obj = new HighSierraGenericVolumeDescriptor();

            obj.LogicalBlockNumber = data.ReadInt32BothEndian();
            obj.Type = (VolumeDescriptorType)data.ReadByteValue();
            obj.Identifier = data.ReadBytes(5);
            obj.Version = data.ReadByteValue();
            obj.Data = data.ReadBytes(2033);

            // Skip remainder of the logical sector
            if (sectorLength > Constants.MinimumSectorSize)
                _ = data.ReadBytes(sectorLength - Constants.MinimumSectorSize);

            return obj;
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
            else if (vd is HighSierraPrimaryVolumeDescriptor hspvd)
            {
                sizeL = hspvd.PathTableSize.LittleEndian;
                sizeB = hspvd.PathTableSize.BigEndian;
                locationL = hspvd.PathTableLocationL;
                locationL2 = hspvd.OptionalPathTableLocationL;
                locationM = hspvd.PathTableLocationM;
                locationM2 = hspvd.OptionalPathTableLocationM;
            }
            else
            {
                return [];
            }

            var groups = new List<PathTableGroup>();
            short blockLength = vd.GetLogicalBlockSize(sectorLength);
            bool highSierra = vd is HighSierraVolumeDescriptor;

            var groupL = new PathTableGroup();
            if (locationL != 0 && ((locationL * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationL * blockLength, SeekOrigin.Begin);
                groupL.PathTableL = ParsePathTable(data, sizeL, true, highSierra);
            }

            if (locationL2 != 0 && ((locationL2 * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationL2 * blockLength, SeekOrigin.Begin);
                groupL.OptionalPathTableL = ParsePathTable(data, sizeL, true, highSierra);
            }

            if (locationM != 0 && ((locationM * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationM * blockLength, SeekOrigin.Begin);
                groupL.PathTableM = ParsePathTable(data, sizeL, false, highSierra);
            }

            if (locationM2 != 0 && ((locationM2 * blockLength) + sizeL) < data.Length)
            {
                data.SeekIfPossible(locationM2 * blockLength, SeekOrigin.Begin);
                groupL.OptionalPathTableM = ParsePathTable(data, sizeL, false, highSierra);
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
                groupB.PathTableL = ParsePathTable(data, sizeB, true, highSierra);
            }

            if (locationL2 != 0 && ((locationL2 * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationL2 * blockLength, SeekOrigin.Begin);
                groupB.OptionalPathTableL = ParsePathTable(data, sizeB, true, highSierra);
            }

            if (locationM != 0 && ((locationM * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationM * blockLength, SeekOrigin.Begin);
                groupB.PathTableM = ParsePathTable(data, sizeB, false, highSierra);
            }

            if (locationM2 != 0 && ((locationM2 * blockLength) + sizeB) < data.Length)
            {
                data.SeekIfPossible(locationM2 * blockLength, SeekOrigin.Begin);
                groupB.OptionalPathTableM = ParsePathTable(data, sizeB, false, highSierra);
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
            => ParsePathTable(data, tableSize, littleEndian, false);

        /// <inheritdoc cref="ParsePathTable(Stream, int, bool)" />
        /// <param name="highSierra">True if the path table is recorded in the High Sierra layout, false otherwise</param>
        public static PathTableRecord[]? ParsePathTable(Stream data, int tableSize, bool littleEndian, bool highSierra)
        {
            var pathTable = new List<PathTableRecord>();

            // TODO: Better deal with invalid path table sizes < 10 (manually detect valid records to determine size)
            // Current status: Trusting path table length field (tableSize)
            int pos = 0;
            while (pos < tableSize)
            {
                var record = new PathTableRecord();

                // High Sierra records the extent location and extended attribute record length first
                if (highSierra)
                {
                    record.ExtentLocation = littleEndian ? data.ReadInt32LittleEndian() : data.ReadInt32BigEndian();
                    record.ExtendedAttributeRecordLength = data.ReadByteValue();
                }

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
                if (!highSierra)
                {
                    record.ExtendedAttributeRecordLength = data.ReadByteValue();
                    record.ExtentLocation = littleEndian ? data.ReadInt32LittleEndian() : data.ReadInt32BigEndian();
                }

                // Read numerics with correct endianness
                record.ParentDirectoryNumber = littleEndian ? data.ReadInt16LittleEndian() : data.ReadInt16BigEndian();

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
                else if (vd is HighSierraPrimaryVolumeDescriptor hspvd)
                    rootDirectoryRecord = hspvd.RootDirectoryRecord;
                else
                    continue;

                // Determine logical block size
                short blockLength = vd.GetLogicalBlockSize(sectorLength);

                // Queue the root directory pointed to from the base volume descriptor
                QueueExtent(data, blockLength, rootDirectoryRecord, vd is HighSierraVolumeDescriptor, true, null, pending);
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
        /// <param name="highSierra">True if the directory records are in the High Sierra layout, false otherwise</param>
        /// <param name="root">True if the extent is the root directory of a directory hierarchy, false otherwise</param>
        /// <param name="systemUseSkip">Number of bytes before the System Use entries of each directory record, null if the directory hierarchy does not use SUSP</param>
        /// <param name="pending">Queued extents, sorted by offset</param>
        private static void QueueExtent(Stream data, short blockLength, DirectoryRecord dr, bool highSierra, bool root, int? systemUseSkip, List<PendingExtent> pending)
        {
            // Only directories and files with an extended attribute record are read
            if ((dr.FileFlags & FileFlags.DIRECTORY) != FileFlags.DIRECTORY && dr.ExtendedAttributeRecordLength == 0)
                return;

            QueueExtent(data, blockLength, dr, false, highSierra, root, systemUseSkip, pending);

            // If the extent location field is ambiguous, also queue the big-endian extent
            if (!dr.ExtentLocation.IsValid)
                QueueExtent(data, blockLength, dr, true, highSierra, root, systemUseSkip, pending);
        }

        /// <summary>
        /// Queue the extent a directory record points to, using one endianness
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="dr">Directory record pointing to the extent</param>
        /// <param name="bigEndian">True if the Big Endian extent location/length should be used</param>
        /// <param name="highSierra">True if the directory records are in the High Sierra layout, false otherwise</param>
        /// <param name="root">True if the extent is the root directory of a directory hierarchy, false otherwise</param>
        /// <param name="systemUseSkip">Number of bytes before the System Use entries of each directory record, null if the directory hierarchy does not use SUSP</param>
        /// <param name="pending">Queued extents, sorted by offset</param>
        private static void QueueExtent(Stream data, short blockLength, DirectoryRecord dr, bool bigEndian, bool highSierra, bool root, int? systemUseSkip, List<PendingExtent> pending)
        {
            if (!TryGetExtent(data, blockLength, dr, bigEndian, out int extentLocation, out long extentOffset, out uint extentLength))
                return;

            var extent = new PendingExtent(extentLocation, extentOffset, extentLength, blockLength, highSierra, root, systemUseSkip, dr);
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
            var probe = new PendingExtent(0, position, 0, 0, false, false, null, null);
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
                var records = ParseDirectoryRecords(data, sectorLength, extent.Length, extent.HighSierra);
                if (records is null)
                    return;

                // Parse the System Use entries, a root directory tells if its directory hierarchy uses SUSP
                int? systemUseSkip = ParseSystemUseEntries(data, extent.BlockLength, records, extent.Root, extent.SystemUseSkip);

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

                    QueueExtent(data, extent.BlockLength, record, extent.HighSierra, false, systemUseSkip, pending);
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
        /// Parse the directories that are recorded in a path table but not reached from the root directory
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="vdSet">Set of volume descriptors for a volume</param>
        /// <param name="pathTableGroups">Path table groups of the volume</param>
        /// <param name="directories">Map of sector numbers to parsed extents, the directories are added to it</param>
        public static void ParsePathTableDirectories(Stream data, short sectorLength, VolumeDescriptor[] vdSet, PathTableGroup[] pathTableGroups, Dictionary<int, FileExtent> directories)
        {
            foreach (VolumeDescriptor vd in vdSet)
            {
                // Only primary and supplementary volume descriptors have a root directory
                if (vd is not BaseVolumeDescriptor bvd)
                    continue;

                // Determine logical block size
                short blockLength = vd.GetLogicalBlockSize(sectorLength);

                foreach (var group in pathTableGroups)
                {
                    foreach (var pathTable in new[] { group.PathTableL, group.OptionalPathTableL, group.PathTableM, group.OptionalPathTableM })
                    {
                        // The path tables of a directory hierarchy start with its root directory
                        if (pathTable is null || pathTable[0].ExtentLocation != bvd.RootDirectoryRecord.ExtentLocation.LittleEndian)
                            continue;

                        foreach (var record in pathTable)
                        {
                            ParsePathTableDirectory(data, sectorLength, blockLength, record.ExtentLocation, directories);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parse a directory recorded in a path table and the directories below it, if it was not parsed yet
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="extentLocation">Logical block number of the directory</param>
        /// <param name="directories">Map of sector numbers to parsed extents</param>
        private static void ParsePathTableDirectory(Stream data, short sectorLength, short blockLength, int extentLocation, Dictionary<int, FileExtent> directories)
        {
            // Directories reached from the root directory are parsed already
            if (directories.ContainsKey(extentLocation))
                return;

            // The first block of the directory has to be within the volume
            long extentOffset = extentLocation * (long)blockLength;
            if (extentLocation < 0 || extentOffset + blockLength > data.Length)
                return;

            // A directory starts with a directory record that points to itself
            data.SeekIfPossible(extentOffset, SeekOrigin.Begin);
            var dr = ParseDirectoryRecord(data, false);
            if (dr.ExtentLocation.LittleEndian != extentLocation)
                return;

            // Parse the directory like a root directory
            var vd = new PrimaryVolumeDescriptor { LogicalBlockSize = blockLength, RootDirectoryRecord = dr };
            var parsed = ParseDirectoryDescriptors(data, sectorLength, [vd]);
            if (parsed is null)
                return;

            foreach (var kvp in parsed)
            {
                if (!directories.ContainsKey(kvp.Key))
                    directories.Add(kvp.Key, kvp.Value);
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

            // A directory fills whole logical blocks, so a length of 0 or one that does not is wrong:
            // take the length from its own "." record if that one is longer and ends within the stream
            bool isDirectory = (dr.FileFlags & FileFlags.DIRECTORY) == FileFlags.DIRECTORY;
            if (isDirectory && (extentLength == 0 || extentLength % blockLength != 0) && extentOffset + blockLength <= data.Length)
            {
                data.SeekIfPossible(extentOffset, SeekOrigin.Begin);
                var currentDirectory = ParseDirectoryRecord(data, false);
                uint currentLength = bigEndian ? currentDirectory.ExtentLength.BigEndian : currentDirectory.ExtentLength.LittleEndian;
                if (currentDirectory.FileIdentifier.EqualsExactly(Constants.CurrentDirectory) && currentLength > extentLength && extentOffset + currentLength <= data.Length)
                    extentLength = currentLength;
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
        /// <param name="highSierra">True if the directory records are in the High Sierra layout, false otherwise</param>
        /// <returns>Filled list of DirectoryRecord on success, null if the directory is invalid</returns>
        private static List<DirectoryRecord>? ParseDirectoryRecords(Stream data, short sectorLength, uint extentLength, bool highSierra)
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

                    continue;
                }

                // Ensure record will end in this extent
                // TODO: Smartly detect record length for invalid record lengths
                pos += recordLength;
                if (pos > extentLength)
                    break;

                // Get the next directory record
                var directoryRecord = ParseDirectoryRecord(data, false, highSierra);

                // Compare recordLength with number of bytes in directoryRecord and ignore the rest of the directory if mismatch
                var readLength = 33 + directoryRecord.FileIdentifier.Length + (directoryRecord.PaddingField is null ? 0 : 1) + directoryRecord.SystemUse.Length;
                if (readLength != recordLength)
                {
                    // Records directly before it with mismatched extent locations are damaged as well
                    while (records.Count > 0 && !records[records.Count - 1].ExtentLocation.IsValid)
                    {
                        records.RemoveAt(records.Count - 1);
                    }

                    return records.Count > 0 ? records : null;
                }

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
            else if (vd is HighSierraPrimaryVolumeDescriptor hspvd)
                current = hspvd.RootDirectoryRecord;
            else
                return null;

            // Determine logical block size
            short blockLength = vd.GetLogicalBlockSize(sectorLength);
            bool highSierra = vd is HighSierraVolumeDescriptor;

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
                var found = FindInDirectory(data, sectorLength, blockLength, current, false, highSierra, encoding, names[i]);
                if (found is null && !current.ExtentLocation.IsValid)
                    found = FindInDirectory(data, sectorLength, blockLength, current, true, highSierra, encoding, names[i]);

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
        /// <param name="highSierra">True if the directory records are in the High Sierra layout, false otherwise</param>
        /// <param name="encoding">Encoding of the file identifiers</param>
        /// <param name="name">Name to find</param>
        /// <returns>Filled array of DirectoryRecord on success, null if the name was not found</returns>
        private static DirectoryRecord[]? FindInDirectory(Stream data, short sectorLength, short blockLength, DirectoryRecord dr, bool bigEndian, bool highSierra, Encoding encoding, string name)
        {
            if (!TryGetExtent(data, blockLength, dr, bigEndian, out _, out long extentOffset, out uint extentLength))
                return null;

            // Move stream to directory location
            data.SeekIfPossible(extentOffset, SeekOrigin.Begin);

            var records = ParseDirectoryRecords(data, sectorLength, extentLength, highSierra);
            if (records is null)
                return null;

            string wanted = TrimIdentifier(name);
            for (int i = 0; i < records.Count; i++)
            {
                // Don't match parent or self
                if (records[i].FileIdentifier.EqualsExactly(Constants.CurrentDirectory) || records[i].FileIdentifier.EqualsExactly(Constants.ParentDirectory))
                    continue;

                // Skip associated files, the file of the same name follows in its own record
                if ((records[i].FileFlags & FileFlags.ASSOCIATED_FILE) == FileFlags.ASSOCIATED_FILE)
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

            if (!TryGetExtent(data, blockLength, dr, bigEndian, out int extentLocation, out long extentOffset, out uint extentLength))
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
                var records = ParseDirectoryRecords(data, sectorLength, extentLength, false);
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
            => ParseDirectoryRecord(data, root, false);

        /// <inheritdoc cref="ParseDirectoryRecord(Stream, bool)" />
        /// <param name="highSierra">True if the directory record is in the High Sierra layout, false otherwise</param>
        /// <remarks>High Sierra records the file flags before a reserved byte</remarks>
        public static DirectoryRecord ParseDirectoryRecord(Stream data, bool root, bool highSierra)
        {
            var obj = new DirectoryRecord();

            obj.DirectoryRecordLength = data.ReadByteValue();
            obj.ExtendedAttributeRecordLength = data.ReadByteValue();
            obj.ExtentLocation = data.ReadInt32BothEndian();
            obj.ExtentLength = data.ReadUInt32BothEndian();

            obj.RecordingDateTime = ParseDirectoryRecordDateTime(data, highSierra);

            obj.FileFlags = (FileFlags)data.ReadByteValue();
            if (highSierra)
                obj.ReservedByte = data.ReadByteValue();

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
            // A record that ends after the file identifier has no padding field
            if (obj.FileIdentifierLength % 2 == 0 && obj.DirectoryRecordLength > 33 + obj.FileIdentifierLength)
                obj.PaddingField = data.ReadByteValue();

            // Root directory within the volume descriptor has no system use bytes, fixed at 34bytes
            if (root)
                return obj;

            // Calculate the size of the system use section (remaining allocated bytes)
            int systemUseLength = obj.DirectoryRecordLength - 33 - obj.FileIdentifierLength;

            // Account for padding field after file identifier
            if (obj.FileIdentifierLength % 2 == 0)
            {
                systemUseLength -= 1;
            }

            // If System Use is empty, or if DirectoryRecordLength is bad, return early
            if (systemUseLength < 1)
            {
                return obj;
            }

            // Read system use field
            obj.SystemUse = data.ReadBytes(systemUseLength);

            return obj;
        }

        /// <summary>
        /// Parse the System Use entries of the directory records of one directory extent
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="records">Directory records of the directory extent</param>
        /// <param name="root">True if the directory extent is a root directory, false otherwise</param>
        /// <param name="systemUseSkip">Number of bytes before the System Use entries of each directory record, null if the directory hierarchy does not use SUSP</param>
        /// <returns>Number of bytes before the System Use entries of each directory record below the directory, null if the directory hierarchy does not use SUSP</returns>
        /// <remarks>
        /// A directory hierarchy uses the System Use Sharing Protocol (SUSP) if the System Use field of the "." record of its
        /// root directory starts with a Sharing Protocol Indicator, after the CD-ROM XA system use information if there is any.
        /// The indicator gives the number of bytes skipped in the System Use fields of the other directory records.
        /// Without SUSP, the entries of other extensions, like the Apple extension for HFS, follow the CD-ROM XA system use
        /// information if there is any, and Continuation Areas are not read.
        /// </remarks>
        private static int? ParseSystemUseEntries(Stream data, short blockLength, List<DirectoryRecord> records, bool root, int? systemUseSkip)
        {
            // The "." record of a root directory tells if the directory hierarchy uses SUSP
            int indicatorOffset = -1;
            if (root && records.Count > 0)
            {
                indicatorOffset = FindSharingProtocolIndicator(records[0].SystemUse);
                systemUseSkip = indicatorOffset < 0 ? null : records[0].SystemUse[indicatorOffset + 6];
            }

            for (int i = 0; i < records.Count; i++)
            {
                byte[] systemUse = records[i].SystemUse;

                // The entries of the "." record of a root directory start with the Sharing Protocol Indicator
                int start;
                if (i == 0 && indicatorOffset >= 0)
                    start = indicatorOffset;
                else if (systemUseSkip is not null)
                    start = systemUseSkip.Value;
                else
                    start = HasXASystemUse(systemUse) ? 14 : 0;

                records[i].SystemUseEntries = ParseSystemUseEntries(data, blockLength, systemUse, start, systemUseSkip is not null);
            }

            return systemUseSkip;
        }

        /// <summary>
        /// Find the Sharing Protocol Indicator in the System Use field of the "." record of a root directory
        /// </summary>
        /// <param name="systemUse">System Use field of the "." record of a root directory</param>
        /// <returns>Offset of the Sharing Protocol Indicator in the System Use field, -1 if there is none</returns>
        private static int FindSharingProtocolIndicator(byte[] systemUse)
        {
            // The indicator follows the CD-ROM XA system use information, if there is any
            int offset = HasXASystemUse(systemUse) ? 14 : 0;
            if (systemUse.Length < offset + 7)
                return -1;

            // The indicator is 7 bytes long, the check bytes tell it from other data
            if (systemUse[offset] != Constants.SharingProtocolIndicatorSignature[0] || systemUse[offset + 1] != Constants.SharingProtocolIndicatorSignature[1])
                return -1;
            if (systemUse[offset + 2] != 7)
                return -1;
            if (systemUse[offset + 4] != Constants.SharingProtocolCheckBytes[0] || systemUse[offset + 5] != Constants.SharingProtocolCheckBytes[1])
                return -1;

            return offset;
        }

        /// <summary>
        /// Indicates if a System Use field starts with the CD-ROM XA system use information
        /// </summary>
        /// <param name="systemUse">System Use field of a directory record</param>
        /// <returns>True if the System Use field starts with the 14 bytes of the CD-ROM XA system use information, false otherwise</returns>
        private static bool HasXASystemUse(byte[] systemUse)
            => systemUse.Length >= 14 && systemUse[6] == 'X' && systemUse[7] == 'A';

        /// <summary>
        /// Parse the System Use entries of a System Use field and of the Continuation Areas it points to
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="systemUse">System Use field of a directory record</param>
        /// <param name="start">Offset of the first System Use entry in the System Use field</param>
        /// <param name="readContinuationAreas">True if Continuation Areas are read, false otherwise</param>
        /// <returns>Filled array of System Use entries in recorded order</returns>
        private static BaseEntry[] ParseSystemUseEntries(Stream data, short blockLength, byte[] systemUse, int start, bool readContinuationAreas)
        {
            // A Continuation Area can point to another one, the limit stops a loop of them
            const int maximumContinuationAreas = 32;

            List<BaseEntry> entries = [];
            var continuationArea = ParseSystemUseArea(systemUse, start, entries);
            for (int i = 0; readContinuationAreas && continuationArea is not null && i < maximumContinuationAreas; i++)
            {
                byte[]? area = ReadContinuationArea(data, blockLength, continuationArea);
                if (area is null)
                    break;

                continuationArea = ParseSystemUseArea(area, 0, entries);
            }

            return [.. entries];
        }

        /// <summary>
        /// Parse the System Use entries of a System Use field or of a Continuation Area
        /// </summary>
        /// <param name="area">System Use field or Continuation Area</param>
        /// <param name="start">Offset of the first System Use entry in the area</param>
        /// <param name="entries">System Use entries, the entries of the area are added to them</param>
        /// <returns>Continuation Area the area points to, null if there is none</returns>
        private static ContinuationArea? ParseSystemUseArea(byte[] area, int start, List<BaseEntry> entries)
        {
            if (start >= area.Length)
                return null;

            // Reading stops at an entry that does not fit, or after a Sharing Protocol Terminator
            var data = new MemoryStream(area, start, area.Length - start);
            ContinuationArea? continuationArea = null;
            var entry = ParseSystemUseEntry(data);
            while (entry is not null)
            {
                entries.Add(entry);
                if (entry is ContinuationArea ce)
                    continuationArea = ce;
                else if (entry is SharingProtocolTerminator)
                    break;

                entry = ParseSystemUseEntry(data);
            }

            return continuationArea;
        }

        /// <summary>
        /// Read the Continuation Area a Continuation Area entry points to
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="blockLength">Number of bytes in a logical block (usually 2048)</param>
        /// <param name="ce">Continuation Area entry</param>
        /// <returns>Bytes of the Continuation Area, null if it is not within one logical block of the volume</returns>
        private static byte[]? ReadContinuationArea(Stream data, short blockLength, ContinuationArea ce)
        {
            int location = ce.BlockLocation.LittleEndian;
            uint offset = ce.Offset.LittleEndian;
            uint length = ce.AreaLength.LittleEndian;

            // A Continuation Area is recorded within one logical block
            long areaOffset = (location * (long)blockLength) + offset;
            if (location < 0 || (long)offset + length > blockLength || areaOffset + length > data.Length)
                return null;

            data.SeekIfPossible(areaOffset, SeekOrigin.Begin);
            return data.ReadBytes((int)length);
        }

        /// <summary>
        /// Parse a Stream into a System Use entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled System Use entry on success, null if no entry fits in the rest of the Stream</returns>
        /// <remarks>
        /// An entry of a known type is only parsed as that type if it has the length its specification gives,
        /// otherwise it is an UnknownEntry that holds its bytes as recorded
        /// </remarks>
        public static BaseEntry? ParseSystemUseEntry(Stream data)
        {
            // Every entry starts with its signature, its length and its version
            if (data.Length - data.Position < 4)
                return null;

            byte[] signature = data.ReadBytes(2);
            byte length = data.ReadByteValue();
            byte version = data.ReadByteValue();
            if (length < 4 || length - 4 > data.Length - data.Position)
                return null;

            byte[] body = data.ReadBytes(length - 4);
            var entry = ParseSystemUseEntry(Encoding.ASCII.GetString(signature), body);
            if (entry is null)
            {
                var unknownEntry = new UnknownEntry();
                unknownEntry.Data = body;
                entry = unknownEntry;
            }

            entry.Signature = signature;
            entry.Length = length;
            entry.Version = version;
            return entry;
        }

        /// <summary>
        /// Parse the bytes after the version of a System Use entry into the entry type of its signature
        /// </summary>
        /// <param name="signature">Signature of the entry</param>
        /// <param name="body">Bytes of the entry after its version</param>
        /// <returns>Filled System Use entry on success, null if the type is unknown or the bytes do not have the length it gives</returns>
        private static BaseEntry? ParseSystemUseEntry(string signature, byte[] body)
        {
            var data = new MemoryStream(body);
            return signature switch
            {
                Constants.ContinuationAreaSignature when body.Length == 24 => ParseContinuationArea(data),
                Constants.PaddingFieldSignature => ParsePaddingField(body),
                Constants.SharingProtocolIndicatorSignature when body.Length == 3 => ParseSharingProtocolIndicator(data),
                Constants.SharingProtocolTerminatorSignature when body.Length == 0 => new SharingProtocolTerminator(),
                Constants.ExtensionsReferenceSignature when body.Length >= 4 => ParseExtensionsReference(data, body.Length),
                Constants.ExtensionSelectorSignature when body.Length == 1 => ParseExtensionSelector(data),
                Constants.PosixFileAttributesSignature when body.Length == 32 || body.Length == 40 => ParsePosixFileAttributes(data, body.Length == 40),
                Constants.PosixDeviceNumberSignature when body.Length == 16 => ParsePosixDeviceNumber(data),
                Constants.SymbolicLinkSignature when body.Length >= 1 => ParseSymbolicLink(data, body.Length),
                Constants.AlternateNameSignature when body.Length >= 1 => ParseAlternateName(data, body.Length),
                Constants.ChildLinkSignature when body.Length == 8 => ParseChildLink(data),
                Constants.ParentLinkSignature when body.Length == 8 => ParseParentLink(data),
                Constants.RelocatedDirectorySignature when body.Length == 0 => new RelocatedDirectory(),
                Constants.TimeStampsSignature when body.Length >= 1 => ParseTimeStamps(data, body.Length),
                Constants.SparseFileSignature when body.Length == 17 => ParseSparseFile(data),
                Constants.ExtensionsInUseSignature when body.Length == 1 => ParseExtensionsInUse(data),
                _ => null,
            };
        }

        /// <summary>
        /// Parse a Stream into a ContinuationArea
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled ContinuationArea</returns>
        private static ContinuationArea ParseContinuationArea(Stream data)
        {
            var obj = new ContinuationArea();

            obj.BlockLocation = data.ReadInt32BothEndian();
            obj.Offset = data.ReadUInt32BothEndian();
            obj.AreaLength = data.ReadUInt32BothEndian();

            return obj;
        }

        /// <summary>
        /// Parse the bytes after the version of an entry into a PaddingField
        /// </summary>
        /// <param name="body">Bytes of the entry after its version</param>
        /// <returns>Filled PaddingField</returns>
        private static PaddingField ParsePaddingField(byte[] body)
        {
            var obj = new PaddingField();

            obj.Padding = body;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SharingProtocolIndicator
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled SharingProtocolIndicator</returns>
        private static SharingProtocolIndicator ParseSharingProtocolIndicator(Stream data)
        {
            var obj = new SharingProtocolIndicator();

            obj.CheckBytes = data.ReadBytes(2);
            obj.BytesSkipped = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ExtensionsReference
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <param name="length">Number of bytes of the entry after its version</param>
        /// <returns>Filled ExtensionsReference on success, null if the identifier, descriptor and source do not fill the entry</returns>
        private static ExtensionsReference? ParseExtensionsReference(Stream data, int length)
        {
            var obj = new ExtensionsReference();

            obj.IdentifierLength = data.ReadByteValue();
            obj.DescriptorLength = data.ReadByteValue();
            obj.SourceLength = data.ReadByteValue();
            obj.ExtensionVersion = data.ReadByteValue();
            if (4 + obj.IdentifierLength + obj.DescriptorLength + obj.SourceLength != length)
                return null;

            obj.Identifier = data.ReadBytes(obj.IdentifierLength);
            obj.Descriptor = data.ReadBytes(obj.DescriptorLength);
            obj.Source = data.ReadBytes(obj.SourceLength);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ExtensionSelector
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled ExtensionSelector</returns>
        private static ExtensionSelector ParseExtensionSelector(Stream data)
        {
            var obj = new ExtensionSelector();

            obj.ExtensionSequence = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PosixFileAttributes
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <param name="serialNumber">True if the entry records the file serial number, false otherwise</param>
        /// <returns>Filled PosixFileAttributes</returns>
        private static PosixFileAttributes ParsePosixFileAttributes(Stream data, bool serialNumber)
        {
            var obj = new PosixFileAttributes();

            obj.FileMode = data.ReadUInt32BothEndian();
            obj.Links = data.ReadUInt32BothEndian();
            obj.UserId = data.ReadUInt32BothEndian();
            obj.GroupId = data.ReadUInt32BothEndian();
            if (serialNumber)
                obj.SerialNumber = data.ReadUInt32BothEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PosixDeviceNumber
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled PosixDeviceNumber</returns>
        private static PosixDeviceNumber ParsePosixDeviceNumber(Stream data)
        {
            var obj = new PosixDeviceNumber();

            obj.DeviceNumberHigh = data.ReadUInt32BothEndian();
            obj.DeviceNumberLow = data.ReadUInt32BothEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SymbolicLink
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <param name="length">Number of bytes of the entry after its version</param>
        /// <returns>Filled SymbolicLink on success, null if the components do not fill the entry</returns>
        private static SymbolicLink? ParseSymbolicLink(Stream data, int length)
        {
            var obj = new SymbolicLink();

            obj.Flags = (SymbolicLinkFlags)data.ReadByteValue();

            // The components fill the rest of the entry
            List<SymbolicLinkComponent> components = [];
            int remaining = length - 1;
            while (remaining > 0)
            {
                var component = ParseSymbolicLinkComponent(data, remaining);
                if (component is null)
                    return null;

                remaining -= 2 + component.ComponentLength;
                components.Add(component);
            }

            obj.Components = [.. components];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SymbolicLinkComponent
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="remaining">Number of bytes left in the entry</param>
        /// <returns>Filled SymbolicLinkComponent on success, null if the component does not fit in the entry</returns>
        private static SymbolicLinkComponent? ParseSymbolicLinkComponent(Stream data, int remaining)
        {
            if (remaining < 2)
                return null;

            var obj = new SymbolicLinkComponent();

            obj.Flags = (ComponentFlags)data.ReadByteValue();
            obj.ComponentLength = data.ReadByteValue();
            if (obj.ComponentLength > remaining - 2)
                return null;

            obj.Content = data.ReadBytes(obj.ComponentLength);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an AlternateName
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <param name="length">Number of bytes of the entry after its version</param>
        /// <returns>Filled AlternateName</returns>
        private static AlternateName ParseAlternateName(Stream data, int length)
        {
            var obj = new AlternateName();

            obj.Flags = (AlternateNameFlags)data.ReadByteValue();
            obj.NameContent = data.ReadBytes(length - 1);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ChildLink
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled ChildLink</returns>
        private static ChildLink ParseChildLink(Stream data)
        {
            var obj = new ChildLink();

            obj.ChildDirectoryLocation = data.ReadInt32BothEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ParentLink
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled ParentLink</returns>
        private static ParentLink ParseParentLink(Stream data)
        {
            var obj = new ParentLink();

            obj.ParentDirectoryLocation = data.ReadInt32BothEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a TimeStamps
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <param name="length">Number of bytes of the entry after its version</param>
        /// <returns>Filled TimeStamps on success, null if the time stamps the flags give do not fill the entry</returns>
        private static TimeStamps? ParseTimeStamps(Stream data, int length)
        {
            var obj = new TimeStamps();

            obj.Flags = (TimeStampFlags)data.ReadByteValue();

            // Each of the 7 lower flag bits records one time stamp
            int count = 0;
            for (int bit = 0; bit < 7; bit++)
            {
                if (((int)obj.Flags & (1 << bit)) != 0)
                    count++;
            }

            bool longForm = (obj.Flags & TimeStampFlags.LONG_FORM) == TimeStampFlags.LONG_FORM;
            if (1 + (count * (longForm ? 17 : 7)) != length)
                return null;

            if (longForm)
            {
                obj.LongFormTimeStamps = new DecDateTime[count];
                for (int i = 0; i < count; i++)
                {
                    obj.LongFormTimeStamps[i] = ParseDecDateTime(data);
                }
            }
            else
            {
                obj.ShortFormTimeStamps = new DirectoryRecordDateTime[count];
                for (int i = 0; i < count; i++)
                {
                    obj.ShortFormTimeStamps[i] = ParseDirectoryRecordDateTime(data);
                }
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SparseFile
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled SparseFile</returns>
        private static SparseFile ParseSparseFile(Stream data)
        {
            var obj = new SparseFile();

            obj.VirtualFileSizeHigh = data.ReadUInt32BothEndian();
            obj.VirtualFileSizeLow = data.ReadUInt32BothEndian();
            obj.TableDepth = data.ReadByteValue();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an ExtensionsInUse
        /// </summary>
        /// <param name="data">Stream to parse, positioned after the version of the entry</param>
        /// <returns>Filled ExtensionsInUse</returns>
        private static ExtensionsInUse ParseExtensionsInUse(Stream data)
        {
            var obj = new ExtensionsInUse();

            obj.Flags = (ExtensionsInUseFlags)data.ReadByteValue();

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
            => ParseDirectoryRecordDateTime(data, false);

        /// <inheritdoc cref="ParseDirectoryRecordDateTime(Stream)" />
        /// <param name="highSierra">True if the date has no time zone offset (High Sierra), false otherwise</param>
        public static DirectoryRecordDateTime ParseDirectoryRecordDateTime(Stream data, bool highSierra)
        {
            var obj = new DirectoryRecordDateTime();

            obj.YearsSince1990 = data.ReadByteValue();
            obj.Month = data.ReadByteValue();
            obj.Day = data.ReadByteValue();
            obj.Hour = data.ReadByteValue();
            obj.Minute = data.ReadByteValue();
            obj.Second = data.ReadByteValue();
            if (!highSierra)
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
            => ParseDecDateTime(data, false);

        /// <inheritdoc cref="ParseDecDateTime(Stream)" />
        /// <param name="highSierra">True if the date has no time zone offset (High Sierra), false otherwise</param>
        public static DecDateTime ParseDecDateTime(Stream data, bool highSierra)
        {
            var obj = new DecDateTime();

            obj.Year = data.ReadBytes(4);
            obj.Month = data.ReadBytes(2);
            obj.Day = data.ReadBytes(2);
            obj.Hour = data.ReadBytes(2);
            obj.Minute = data.ReadBytes(2);
            obj.Second = data.ReadBytes(2);
            obj.Centisecond = data.ReadBytes(2);
            if (!highSierra)
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
            /// True if the directory records of the extent are in the High Sierra layout
            /// </summary>
            public bool HighSierra { get; }

            /// <summary>
            /// True if the extent is the root directory of a directory hierarchy
            /// </summary>
            public bool Root { get; }

            /// <summary>
            /// Number of bytes before the System Use entries of each directory record of the extent,
            /// null if the directory hierarchy does not use SUSP
            /// </summary>
            public int? SystemUseSkip { get; }

            /// <summary>
            /// Directory record pointing to the extent
            /// </summary>
            public DirectoryRecord? Record { get; }

            public PendingExtent(int location, long offset, uint length, short blockLength, bool highSierra, bool root, int? systemUseSkip, DirectoryRecord? record)
            {
                Location = location;
                Offset = offset;
                Length = length;
                BlockLength = blockLength;
                HighSierra = highSierra;
                Root = root;
                SystemUseSkip = systemUseSkip;
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
