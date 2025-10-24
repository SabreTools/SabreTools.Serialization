using System;
using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    public class ISO9660 : BaseBinaryReader<Volume>
    {
        /// <inheritdoc/>
        public override Volume? Deserialize(Stream? data) => Deserialize(data, 2048);

        /// <inheritdoc cref="Deserialize(Stream?)" />
        /// <param name="sectorLength">Size of the logical sector used in the volume</param>
        public Volume? Deserialize(Stream? data, int sectorLength)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            // Ensure the logical sector size is valid (2^n where n>=11)
            if (sectorLength < 2048 || (sectorLength & (sectorLength - 1)) != 0)
                return null;

            // Simple check for a valid stream length
            if (sectorLength * (Constants.SystemAreaSectors + 2) > data.Length - data.Position)
                return null;

            try
            {
                // Create a new Volume to fill
                var volume = new Volume();

                // Read the System Area
                volume.SystemArea = ParseSystemArea(data, sectorLength);

                // Read the set of Volume Descriptors
                volume.VolumeDescriptorSet = ParseVolumeDescriptorSet(data, sectorLength);

                // Read the remainder of the Data Area
                volume.RootDirectoryExtent = ParseDirectoryExtent(data, sectorLength);

                return volume;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a SystemArea byte array (16 logical sectors)
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled byte array on success, null on error</returns>
        public static byte[]? ParseSystemArea(Stream data, int sectorLength)
        {
            return data.ReadBytes(Constants.SystemAreaSectors * sectorLength);
        }

        /// <summary>
        /// Parse a Stream into a list of VolumeDescriptor objects
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor[] on success, null on error</returns>
        public static VolumeDescriptor[]? ParseVolumeDescriptorSet(Stream data, int sectorLength)
        {
            var volumeDescriptorSet = new List<VolumeDescriptor>();
            bool setTerminated = false;
            while (true)
            {
                var volumeDescriptor = ParseVolumeDescriptor(data, sectorLength);

                // If no valid volume descriptor could be read, return the current set
                if (volumeDescriptor == null)
                    return volumeDescriptorSet.ToArray();
                
                // If the set has already been terminated and the returned volume descriptor is not another terminator,
                // assume the read volume descriptor is not a valid volume descriptor and return the current set
                if (setTerminated && volumeDescriptor.Type != VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)
                    return volumeDescriptorSet.ToArray();
                
                // Add the valid read volume descriptor to the set
                volumeDescriptorSet.Add(volumeDescriptor);

                // If the set terminator was read, set the set terminated flag (further set terminators may be present)
                if (!setTerminated && volumeDescriptor.Type == VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR)
                    setTerminated = true;
            }
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor on success, null on error</returns>
        public static VolumeDescriptor? ParseVolumeDescriptor(Stream data, int sectorLength)
        {
            // Read the first byte of the volume descriptor
            byte? type = data.ReadByteValue();
            if (type == null)
                return null;

            VolumeDescriptorType volumeDescriptorType = (VolumeDescriptorType)type;
            return volumeDescriptorType switch
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
        public static BootRecordVolumeDescriptor? ParseBootRecordVolumeDescriptor(Stream data, int sectorLength)
        {
            var bootRecordVolumeDescriptor = new BootRecordVolumeDescriptor();
            bootRecordVolumeDescriptor.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            bootRecordVolumeDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!bootRecordVolumeDescriptor.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            bootRecordVolumeDescriptor.Version = data.ReadByteValue();
            bootRecordVolumeDescriptor.BootSystemIdentifier = data.ReadBytes(32);
            bootRecordVolumeDescriptor.BootIdentifier = data.ReadBytes(32);
            bootRecordVolumeDescriptor.BootSystemUse = data.ReadBytes(1997);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return bootRecordVolumeDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a PrimaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled PrimaryVolumeDescriptor on success, null on error</returns>
        public static PrimaryVolumeDescriptor? ParsePrimaryVolumeDescriptor(Stream data, int sectorLength)
        {
            var primaryVolumeDescriptor = new PrimaryVolumeDescriptor();
            primaryVolumeDescriptor.Type = VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR;
            return (PrimaryVolumeDescriptor?)ParseBaseVolumeDescriptor(data, sectorLength, primaryVolumeDescriptor);
        }

        /// <summary>
        /// Parse a Stream into a SupplementaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled SupplementaryVolumeDescriptor on success, null on error</returns>
        public static SupplementaryVolumeDescriptor? ParseSupplementaryVolumeDescriptor(Stream data, int sectorLength)
        {
            var supplementaryVolumeDescriptor = new SupplementaryVolumeDescriptor();
            supplementaryVolumeDescriptor.Type = VolumeDescriptorType.SUPPLEMENTARY_VOLUME_DESCRIPTOR;
            return (SupplementaryVolumeDescriptor?)ParseBaseVolumeDescriptor(data, sectorLength, supplementaryVolumeDescriptor);
        }

        /// <summary>
        /// Parse a Stream into a BaseVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled BaseVolumeDescriptor on success, null on error</returns>
        public static BaseVolumeDescriptor? ParseBaseVolumeDescriptor(Stream data, int sectorLength, BaseVolumeDescriptor baseVolumeDescriptor)
        {
            baseVolumeDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!baseVolumeDescriptor.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            baseVolumeDescriptor.Version = data.ReadByteValue();

            // Read the child-specific field
            if (baseVolumeDescriptor is PrimaryVolumeDescriptor primaryVolumeDescriptor)
                primaryVolumeDescriptor.UnusedByte = data.ReadByteValue();
            else if (baseVolumeDescriptor is SupplementaryVolumeDescriptor supplementaryVolumeDescriptor)
                supplementaryVolumeDescriptor.VolumeFlags = (VolumeFlags?)data.ReadByteValue();
            else
            {
                // Rewind and return for unknown descriptor
                data.Position -= 8;
                return null;
            }

            baseVolumeDescriptor.SystemIdentifier = data.ReadBytes(32);
            baseVolumeDescriptor.VolumeIdentifier = data.ReadBytes(32);
            baseVolumeDescriptor.Unused8Bytes = data.ReadBytes(8);
            baseVolumeDescriptor.VolumeSpaceSize = new BothEndianInt32();
            baseVolumeDescriptor.VolumeSpaceSize.LSB = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.VolumeSpaceSize.MSB = data.ReadInt32BigEndian();

            // Read the child-specific field
            if (baseVolumeDescriptor is PrimaryVolumeDescriptor primaryVolumeDescriptor2)
                primaryVolumeDescriptor2.Unused32Bytes = data.ReadBytes(32);
            else if (baseVolumeDescriptor is SupplementaryVolumeDescriptor supplementaryVolumeDescriptor2)
                supplementaryVolumeDescriptor2.EscapeSequences = data.ReadBytes(32);
            else
            {
                // Rewind and return for unknown descriptor
                data.Position -= 120;
                return null;
            }

            baseVolumeDescriptor.VolumeSetSize = new BothEndianInt16();
            baseVolumeDescriptor.VolumeSetSize.LSB = data.ReadInt16LittleEndian();
            baseVolumeDescriptor.VolumeSetSize.MSB = data.ReadInt16BigEndian();
            baseVolumeDescriptor.VolumeSequenceNumber = new BothEndianInt16();
            baseVolumeDescriptor.VolumeSequenceNumber.LSB = data.ReadInt16LittleEndian();
            baseVolumeDescriptor.VolumeSequenceNumber.MSB = data.ReadInt16BigEndian();
            baseVolumeDescriptor.LogicalBlockSize = new BothEndianInt16();
            baseVolumeDescriptor.LogicalBlockSize.LSB = data.ReadInt16LittleEndian();
            baseVolumeDescriptor.LogicalBlockSize.MSB = data.ReadInt16BigEndian();
            baseVolumeDescriptor.PathTableSize = new BothEndianInt32();
            baseVolumeDescriptor.PathTableSize.LSB = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.PathTableSize.MSB = data.ReadInt32BigEndian();
            baseVolumeDescriptor.PathTableLocationLE = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.PathTableLocationLEOptional = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.PathTableLocationBE = data.ReadInt32BigEndian();
            baseVolumeDescriptor.PathTableLocationBEOptional = data.ReadInt32BigEndian();

            baseVolumeDescriptor.RootDirectory = ParseDirectoryRecord(data, true);

            baseVolumeDescriptor.VolumeSetIdentifier = data.ReadBytes(128);
            baseVolumeDescriptor.PublisherIdentifier = data.ReadBytes(128);
            baseVolumeDescriptor.DataPreparerIdentifier = data.ReadBytes(128);
            baseVolumeDescriptor.ApplicationIdentifier = data.ReadBytes(128);
            baseVolumeDescriptor.CopyrightFileIdentifier = data.ReadBytes(37);
            baseVolumeDescriptor.AbstractFileIdentifier = data.ReadBytes(37);
            baseVolumeDescriptor.BibliographicFileIdentifier = data.ReadBytes(37);

            baseVolumeDescriptor.VolumeCreationDateTime = ParseDecDateTime(data);
            baseVolumeDescriptor.VolumeModificationDateTime = ParseDecDateTime(data);
            baseVolumeDescriptor.VolumeExpirationDateTime = ParseDecDateTime(data);
            baseVolumeDescriptor.VolumeEffectiveDateTime = ParseDecDateTime(data);

            baseVolumeDescriptor.FileStructureVersion = data.ReadByteValue();
            baseVolumeDescriptor.ReservedByte = data.ReadByteValue();
            baseVolumeDescriptor.ApplicationUse = data.ReadBytes(512);
            baseVolumeDescriptor.Reserved653Bytes = data.ReadBytes(653);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return baseVolumeDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a VolumePartitionDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumePartitionDescriptor on success, null on error</returns>
        public static VolumePartitionDescriptor? ParseVolumePartitionDescriptor(Stream data, int sectorLength)
        {
            var volumePartitionDescriptor = new VolumePartitionDescriptor();
            volumePartitionDescriptor.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            volumePartitionDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!volumePartitionDescriptor.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            volumePartitionDescriptor.Version = data.ReadByteValue();
            volumePartitionDescriptor.SystemIdentifier = data.ReadBytes(32);
            volumePartitionDescriptor.VolumePartitionIdentifier = data.ReadBytes(32);
            volumePartitionDescriptor.VolumePartitionLocation = new BothEndianInt32();
            volumePartitionDescriptor.VolumePartitionLocation.LSB = data.ReadInt32LittleEndian();
            volumePartitionDescriptor.VolumePartitionLocation.MSB = data.ReadInt32BigEndian();
            volumePartitionDescriptor.VolumePartitionSize = new BothEndianInt32();
            volumePartitionDescriptor.VolumePartitionSize.LSB = data.ReadInt32LittleEndian();
            volumePartitionDescriptor.VolumePartitionSize.MSB = data.ReadInt32BigEndian();
            volumePartitionDescriptor.SystemUse = data.ReadBytes(1960);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return volumePartitionDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptorSetTerminator
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptorSetTerminator on success, null on error</returns>
        public static VolumeDescriptorSetTerminator? ParseVolumeDescriptorSetTerminator(Stream data, int sectorLength)
        {
            var volumeDescriptorSetTerminator = new VolumeDescriptorSetTerminator();
            volumeDescriptorSetTerminator.Type = VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR;
            volumeDescriptorSetTerminator.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!volumeDescriptorSetTerminator.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            volumeDescriptorSetTerminator.Version = data.ReadByteValue();
            volumeDescriptorSetTerminator.Reserved2041Bytes = data.ReadBytes(2041);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return volumeDescriptorSetTerminator;
        }

        /// <summary>
        /// Parse a Stream into a GenericVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled GenericVolumeDescriptor on success, null on error</returns>
        public static GenericVolumeDescriptor? ParseGenericVolumeDescriptor(Stream data, int sectorLength, byte? type)
        {
            var genericVolumeDescriptor = new GenericVolumeDescriptor();
            genericVolumeDescriptor.Type = (VolumeDescriptorType?)type;
            genericVolumeDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!genericVolumeDescriptor.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            genericVolumeDescriptor.Version = data.ReadByteValue();
            genericVolumeDescriptor.Data = data.ReadBytes(2041);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return genericVolumeDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a DecDateTime
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DecDateTime on success, null on error</returns>
        public static DecDateTime? ParseDecDateTime(Stream data)
        {
            var decDateTime = new DecDateTime();
            decDateTime.Year = data.ReadBytes(4);
            decDateTime.Month = data.ReadBytes(2);
            decDateTime.Day = data.ReadBytes(2);
            decDateTime.Hour = data.ReadBytes(2);
            decDateTime.Minute = data.ReadBytes(2);
            decDateTime.Second = data.ReadBytes(2);
            decDateTime.Centisecond = data.ReadBytes(2);
            decDateTime.TimezoneOffset = data.ReadByteValue();
            return decDateTime;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="root">true if root directory record, false otherwise</param>
        /// <returns>Filled DirectoryRecord on success, null on error</returns>
        public static DirectoryRecord? ParseDirectoryRecord(Stream data, bool root)
        {
            var directoryRecord = new DirectoryRecord();

            directoryRecord.DirectoryRecordLength = data.ReadByteValue();
            directoryRecord.ExtendedAttributeRecordLength = data.ReadByteValue();
            directoryRecord.ExtentLocation = new BothEndianInt32();
            directoryRecord.ExtentLocation.LSB = data.ReadInt32LittleEndian();
            directoryRecord.ExtentLocation.MSB = data.ReadInt32BigEndian();
            directoryRecord.ExtentLength = new BothEndianInt32();
            directoryRecord.ExtentLength.LSB = data.ReadInt32LittleEndian();
            directoryRecord.ExtentLength.MSB = data.ReadInt32BigEndian();

            directoryRecord.RecordingDateTime = ParseDirectoryRecordDateTime(data);

            directoryRecord.FileFlags = (FileFlags)data.ReadByteValue();
            directoryRecord.FileUnitSize = data.ReadByteValue();
            directoryRecord.InterleaveGapSize = data.ReadByteValue();
            directoryRecord.VolumeSequenceNumber = new BothEndianInt16();
            directoryRecord.VolumeSequenceNumber.LSB = data.ReadInt16LittleEndian();
            directoryRecord.VolumeSequenceNumber.MSB = data.ReadInt16BigEndian();
            directoryRecord.FileIdentifierLength = data.ReadByteValue();

            // Root directory within the volume descriptor has a single byte file identifier
            if (root)
                directoryRecord.FileIdentifier = [data.ReadByteValue()];
            else if (directoryRecord.FileIdentifierLength > 0)
                directoryRecord.FileIdentifier = data.ReadBytes(directoryRecord.FileIdentifierLength);

            if (directoryRecord.FileIdentifierLength % 2 != 0)
                directoryRecord.PaddingField = data.ReadByteValue();

            // Root directory within the volume descriptor has no system use bytes
            if (root)
                return directoryRecord;

            // Calculate the size of the system use section
            int systemUseLength = directoryRecord.DirectoryRecordLength - 33 - directoryRecord.FileIdentifierLength;
            // Account for padding field
            if (directoryRecord.FileIdentifierLength % 2 != 0)
                systemUseLength -= 1;

            // If the system use is empty, return
            if (systemUseLength < 1)
                return directoryRecord;

            // System use field must be even size
            if (systemUseLength % 2 != 0)
                systemUseLength += 1;

            // Read system use field 
            directoryRecord.SystemUse = data.ReadBytes(systemUseLength);

            return directoryRecord;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryRecordDateTime
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryRecordDateTime on success, null on error</returns>
        public static DirectoryRecordDateTime? ParseDirectoryRecordDateTime(Stream data)
        {
            directoryRecordDateTime = new DirectoryRecordDateTime();
            directoryRecordDateTime.YearsSince1990 = data.ReadByteValue();
            directoryRecordDateTime.Month = data.ReadByteValue();
            directoryRecordDateTime.Day = data.ReadByteValue();
            directoryRecordDateTime.Hour = data.ReadByteValue();
            directoryRecordDateTime.Minute = data.ReadByteValue();
            directoryRecordDateTime.Second = data.ReadByteValue();
            directoryRecordDateTime.TimezoneOffset = data.ReadByteValue();
        }

        /// <summary>
        /// Parse a Stream into a DirectoryExtent
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="volumeDescriptorSet"></param>
        /// <returns>Filled DirectoryExtent on success, null on error</returns>
        public static DirectoryExtent? ParseDirectoryExtent(Stream data, int sectorLength)
        {
            return null;
        }
    }
}

