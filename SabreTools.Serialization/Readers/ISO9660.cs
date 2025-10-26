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
                volume.SystemArea = data.ReadBytes(Constants.SystemAreaSectors * sectorLength);

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

        #region Volume Descriptor Parsing

        /// <summary>
        /// Parse a Stream into a list of VolumeDescriptor objects
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor[] on success, null on error</returns>
        public static VolumeDescriptor[]? ParseVolumeDescriptorSet(Stream data, int sectorLength)
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
                    data.Position -= sectorLength;
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
        public static VolumeDescriptor? ParseVolumeDescriptor(Stream data, int sectorLength)
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
        public static BootRecordVolumeDescriptor? ParseBootRecordVolumeDescriptor(Stream data, int sectorLength)
        {
            var obj = new BootRecordVolumeDescriptor();

            obj.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.BootSystemIdentifier = data.ReadBytes(32);
            obj.BootIdentifier = data.ReadBytes(32);
            obj.BootSystemUse = data.ReadBytes(1997);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PrimaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled PrimaryVolumeDescriptor on success, null on error</returns>
        public static PrimaryVolumeDescriptor? ParsePrimaryVolumeDescriptor(Stream data, int sectorLength)
        {
            var obj = new PrimaryVolumeDescriptor();

            obj.Type = VolumeDescriptorType.PRIMARY_VOLUME_DESCRIPTOR;
            return (PrimaryVolumeDescriptor?)ParseBaseVolumeDescriptor(data, sectorLength, obj);
        }

        /// <summary>
        /// Parse a Stream into a SupplementaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled SupplementaryVolumeDescriptor on success, null on error</returns>
        public static SupplementaryVolumeDescriptor? ParseSupplementaryVolumeDescriptor(Stream data, int sectorLength)
        {
            var obj = new SupplementaryVolumeDescriptor();

            obj.Type = VolumeDescriptorType.SUPPLEMENTARY_VOLUME_DESCRIPTOR;
            return (SupplementaryVolumeDescriptor?)ParseBaseVolumeDescriptor(data, sectorLength, obj);
        }

        /// <summary>
        /// Parse a Stream into a BaseVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled BaseVolumeDescriptor on success, null on error</returns>
        public static BaseVolumeDescriptor? ParseBaseVolumeDescriptor(Stream data, int sectorLength, BaseVolumeDescriptor obj)
        {
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            obj.Version = data.ReadByteValue();

            // Read the child-specific field
            if (obj is PrimaryVolumeDescriptor objPVD)
                objPVD.UnusedByte = data.ReadByteValue();
            else if (obj is SupplementaryVolumeDescriptor objSVD)
                objSVD.VolumeFlags = (VolumeFlags)data.ReadByteValue();
            else
            {
                // Rewind and return for unknown descriptor
                data.Position -= 8;
                return obj;
            }

            obj.SystemIdentifier = data.ReadBytes(32);
            obj.VolumeIdentifier = data.ReadBytes(32);
            obj.Unused8Bytes = data.ReadBytes(8);
            obj.VolumeSpaceSize = new BothEndianInt32();
            obj.VolumeSpaceSize.LSB = data.ReadInt32LittleEndian();
            obj.VolumeSpaceSize.MSB = data.ReadInt32BigEndian();

            // Read the child-specific field
            if (obj is PrimaryVolumeDescriptor objPVD2)
                objPVD2.Unused32Bytes = data.ReadBytes(32);
            else if (obj is SupplementaryVolumeDescriptor objSVD2)
                objSVD2.EscapeSequences = data.ReadBytes(32);
            else
            {
                // Rewind and return for unknown descriptor
                data.Position -= 120;
                return null;
            }

            obj.VolumeSetSize = new BothEndianInt16();
            obj.VolumeSetSize.LSB = data.ReadInt16LittleEndian();
            obj.VolumeSetSize.MSB = data.ReadInt16BigEndian();
            obj.VolumeSequenceNumber = new BothEndianInt16();
            obj.VolumeSequenceNumber.LSB = data.ReadInt16LittleEndian();
            obj.VolumeSequenceNumber.MSB = data.ReadInt16BigEndian();
            obj.LogicalBlockSize = new BothEndianInt16();
            obj.LogicalBlockSize.LSB = data.ReadInt16LittleEndian();
            obj.LogicalBlockSize.MSB = data.ReadInt16BigEndian();
            obj.PathTableSize = new BothEndianInt32();
            obj.PathTableSize.LSB = data.ReadInt32LittleEndian();
            obj.PathTableSize.MSB = data.ReadInt32BigEndian();
            obj.PathTableLocationLE = data.ReadInt32LittleEndian();
            obj.PathTableLocationLEOptional = data.ReadInt32LittleEndian();
            obj.PathTableLocationBE = data.ReadInt32BigEndian();
            obj.PathTableLocationBEOptional = data.ReadInt32BigEndian();

            obj.RootDirectory = ParseDirectoryRecord(data, true);

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
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a VolumePartitionDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumePartitionDescriptor on success, null on error</returns>
        public static VolumePartitionDescriptor? ParseVolumePartitionDescriptor(Stream data, int sectorLength)
        {
            var obj = new VolumePartitionDescriptor();

            obj.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.SystemIdentifier = data.ReadBytes(32);
            obj.VolumePartitionIdentifier = data.ReadBytes(32);
            obj.VolumePartitionLocation = new BothEndianInt32();
            obj.VolumePartitionLocation.LSB = data.ReadInt32LittleEndian();
            obj.VolumePartitionLocation.MSB = data.ReadInt32BigEndian();
            obj.VolumePartitionSize = new BothEndianInt32();
            obj.VolumePartitionSize.LSB = data.ReadInt32LittleEndian();
            obj.VolumePartitionSize.MSB = data.ReadInt32BigEndian();
            obj.SystemUse = data.ReadBytes(1960);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptorSetTerminator
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptorSetTerminator on success, null on error</returns>
        public static VolumeDescriptorSetTerminator? ParseVolumeDescriptorSetTerminator(Stream data, int sectorLength)
        {
            var obj = new VolumeDescriptorSetTerminator();

            obj.Type = VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.Reserved2041Bytes = data.ReadBytes(2041);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a GenericVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <param name="type">Type </param>
        /// <returns>Filled GenericVolumeDescriptor on success, null on error</returns>
        public static GenericVolumeDescriptor? ParseGenericVolumeDescriptor(Stream data, int sectorLength, VolumeDescriptorType type)
        {
            var obj = new GenericVolumeDescriptor();

            obj.Type = type;
            obj.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (!obj.Identifier.EqualsExactly(Constants.StandardIdentifier))
            {
                data.Position -= 6;
                return null;
            }

            obj.Version = data.ReadByteValue();
            obj.Data = data.ReadBytes(2041);

            // Skip remainder of the logical sector
            if (sectorLength > 2048)
                data.Position += sectorLength - 2048;

            return obj;
        }

        #endregion

        /// <summary>
        /// Parse a Stream into a DecDateTime
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DecDateTime on success, null on error</returns>
        public static DecDateTime? ParseDecDateTime(Stream data)
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

        #region Directory Parsing

        /// <summary>
        /// Parse a Stream into a DirectoryRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="root">true if root directory record, false otherwise</param>
        /// <returns>Filled DirectoryRecord on success, null on error</returns>
        public static DirectoryRecord? ParseDirectoryRecord(Stream data, bool root)
        {
            var obj = new DirectoryRecord();

            obj.DirectoryRecordLength = data.ReadByteValue();
            obj.ExtendedAttributeRecordLength = data.ReadByteValue();
            obj.ExtentLocation = new BothEndianInt32();
            obj.ExtentLocation.LSB = data.ReadInt32LittleEndian();
            obj.ExtentLocation.MSB = data.ReadInt32BigEndian();
            obj.ExtentLength = new BothEndianInt32();
            obj.ExtentLength.LSB = data.ReadInt32LittleEndian();
            obj.ExtentLength.MSB = data.ReadInt32BigEndian();

            obj.RecordingDateTime = ParseDirectoryRecordDateTime(data);

            obj.FileFlags = (FileFlags)data.ReadByteValue();
            obj.FileUnitSize = data.ReadByteValue();
            obj.InterleaveGapSize = data.ReadByteValue();
            obj.VolumeSequenceNumber = new BothEndianInt16();
            obj.VolumeSequenceNumber.LSB = data.ReadInt16LittleEndian();
            obj.VolumeSequenceNumber.MSB = data.ReadInt16BigEndian();
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
            // Calculate the size of the system use section (remaining allocated bytes)
            int totalBytes = 33 + obj.FileIdentifierLength;
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

        /// <summary>
        /// Parse a Stream into a DirectoryExtent
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength"></param>
        /// <returns>Filled DirectoryExtent on success, null on error</returns>
        public static DirectoryExtent? ParseDirectoryExtent(Stream data, int sectorLength)
        {
            return null;
        }

        #endregion
    }
}
