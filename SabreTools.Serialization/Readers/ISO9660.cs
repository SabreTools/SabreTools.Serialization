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
        public override Volume? Deserialize(Stream? data) => Deserialize(Stream, 2048);

        /// <inheritdoc cref="Deserialize(Stream?)" />
        /// <param name="sectorLength">Size of the logical sector used in the volume</param>
        public Volume? Deserialize(Stream? data, uint sectorLength)
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

                // Read the initial System Area
                volume.SystemArea = ParseSystemArea(data, sectorLength);

                // Read the set of Volume Descriptors
                volume.VolumeDescriptorSet = ParseVolumeDescriptorSet(data, sectorLength);

                // Read the remainder of the Data Area
                volume.RootDirectoryExtent = ParseDirectoryExtent(volume, volume.VolumeDescriptorSet);

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
        public static byte[]? ParseSystemArea(Stream data, uint sectorLength)
        {
            return data.ReadBytes(Constants.SystemAreaSectors * sectorLength);
        }

        /// <summary>
        /// Parse a Stream into a list of VolumeDescriptor objects
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor[] on success, null on error</returns>
        public static VolumeDescriptor[]? ParseVolumeDescriptorSet(Stream data, uint sectorLength)
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
                if (setTerminated && volumeDescriptor.Type != VolumeDescriptorType.VolumeDescriptorSetTerminator)
                    return volumeDescriptorSet.ToArray();
                
                // Add the valid read volume descriptor to the set
                volumeDescriptorSet.Add(volumeDescriptor);

                // If the set terminator was read, set the set terminated flag (further set terminators may be present)
                if (!setTerminated && volumeDescriptor.Type == VolumeDescriptorType.VolumeDescriptorSetTerminator)
                    setTerminated = true;
            }
        }

        /// <summary>
        /// Parse a Stream into a VolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumeDescriptor on success, null on error</returns>
        public static VolumeDescriptor? ParseVolumeDescriptor(Stream data, uint sectorLength)
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
        public static BootRecordVolumeDescriptor? ParseBootRecordVolumeDescriptor(Stream data, uint sectorLength)
        {
            var bootRecordVolumeDescriptor = new BootRecordVolumeDescriptor();
            bootRecordVolumeDescriptor.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            bootRecordVolumeDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (false)
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
        public static PrimaryVolumeDescriptor? ParsePrimaryVolumeDescriptor(Stream data, uint sectorLength)
        {
            var primaryVolumeDescriptor = new PrimaryVolumeDescriptor();
            return ParseBaseVolumeDescriptor(data, sectorLength, primaryVolumeDescriptor);
        }

        /// <summary>
        /// Parse a Stream into a SupplementaryVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled SupplementaryVolumeDescriptor on success, null on error</returns>
        public static SupplementaryVolumeDescriptor? ParseSupplementaryVolumeDescriptor(Stream data, uint sectorLength)
        {
            var supplementaryVolumeDescriptor = new SupplementaryVolumeDescriptor();
            return ParseBaseVolumeDescriptor(data, sectorLength, supplementaryVolumeDescriptor);
        }

        /// <summary>
        /// Parse a Stream into a BaseVolumeDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled BaseVolumeDescriptor on success, null on error</returns>
        public static BaseVolumeDescriptor? ParseBaseVolumeDescriptor(Stream data, uint sectorLength, BaseVolumeDescriptor baseVolumeDescriptor)
        {
            baseVolumeDescriptor.Type = type;
            baseVolumeDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (false)
            {
                data.Position -= 6;
                return null;
            }

            baseVolumeDescriptor.Version = data.ReadByteValue();

            // Read the child-specific field
            if (baseVolumeDescriptor is PrimaryVolumeDescriptor primaryVolumeDescriptor)
                primaryVolumeDescriptor.UnusedByte = data.ReadByteValue();
            else if (baseVolumeDescriptor is SupplementaryVolumeDescriptor supplementaryVolumeDescriptor)
                supplementaryVolumeDescriptor.VolumeFlags = data.ReadByteValue();
            else
            {
                // Rewind and return for unknown descriptor
                data.Position -= 8;
                return null;
            }

            baseVolumeDescriptor.SystemIdentifier = data.ReadBytes(32);
            baseVolumeDescriptor.VolumePartitionIdentifier = data.ReadBytes(32);
            baseVolumeDescriptor.Unused8Bytes = data.ReadBytes(8);
            baseVolumeDescriptor.VolumeSpaceSize = new Types.BothEndianInt32();
            baseVolumeDescriptor.VolumeSpaceSize.LSB = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.VolumeSpaceSize.MSB = data.ReadInt32BigEndian();

            // Read the child-specific field
            if (baseVolumeDescriptor is PrimaryVolumeDescriptor primaryVolumeDescriptor)
                primaryVolumeDescriptor.Unused32Bytes = data.ReadBytes(32);
            else if (baseVolumeDescriptor is SupplementaryVolumeDescriptor supplementaryVolumeDescriptor)
                supplementaryVolumeDescriptor.VolEscapeSequencesumeFlags = data.ReadBytes(32);
            else
            {
                // Rewind and return for unknown descriptor
                data.Position -= 120;
                return null;
            }

            baseVolumeDescriptor.VolumeSetSize = new Types.BothEndianInt16();
            baseVolumeDescriptor.VolumeSetSize.LSB = data.ReadInt16LittleEndian();
            baseVolumeDescriptor.VolumeSetSize.MSB = data.ReadInt16BigEndian();
            baseVolumeDescriptor.VolumeSequenceNumber = new Types.BothEndianInt16();
            baseVolumeDescriptor.VolumeSequenceNumber.LSB = data.ReadInt16LittleEndian();
            baseVolumeDescriptor.VolumeSequenceNumber.MSB = data.ReadInt16BigEndian();
            baseVolumeDescriptor.LogicalBlockSize = new Types.BothEndianInt16();
            baseVolumeDescriptor.LogicalBlockSize.LSB = data.ReadInt16LittleEndian();
            baseVolumeDescriptor.LogicalBlockSize.MSB = data.ReadInt16BigEndian();
            baseVolumeDescriptor.PathTableSize = new Types.BothEndianInt32();
            baseVolumeDescriptor.PathTableSize.LSB = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.PathTableSize.MSB = data.ReadInt32BigEndian();
            baseVolumeDescriptor.PathTableLocationLE = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.PathTableLocationLEOptional = data.ReadInt32LittleEndian();
            baseVolumeDescriptor.PathTableLocationBE = data.ReadInt32BigEndian();
            baseVolumeDescriptor.PathTableLocationBEOptional = data.ReadInt32BigEndian();

            baseVolumeDescriptor.RootDirectory = ParseDirectoryRecord(data);

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

            return primaryVolumeDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a VolumePartitionDescriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="sectorLength">Number of bytes in a logical sector (usually 2048)</param>
        /// <returns>Filled VolumePartitionDescriptor on success, null on error</returns>
        public static VolumePartitionDescriptor? ParseVolumePartitionDescriptor(Stream data, uint sectorLength)
        {
            var volumePartitionDescriptor = new VolumePartitionDescriptor();
            volumePartitionDescriptor.Type = VolumeDescriptorType.VOLUME_PARTITION_DESCRIPTOR;
            volumePartitionDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (false)
            {
                data.Position -= 6;
                return null;
            }

            volumePartitionDescriptor.Version = data.ReadByteValue();
            bootRecordVolumeDescriptor.SystemIdentifier = data.ReadBytes(32);
            bootRecordVolumeDescriptor.VolumePartitionIdentifier = data.ReadBytes(32);
            bootRecordVolumeDescriptor.VolumePartitionLocation = new Types.BothEndianInt32();
            bootRecordVolumeDescriptor.VolumePartitionLocation.LSB = data.ReadInt32LittleEndian();
            bootRecordVolumeDescriptor.VolumePartitionLocation.MSB = data.ReadInt32BigEndian();
            bootRecordVolumeDescriptor.VolumePartitionSize = new Types.BothEndianInt32();
            bootRecordVolumeDescriptor.VolumePartitionSize.LSB = data.ReadInt32LittleEndian();
            bootRecordVolumeDescriptor.VolumePartitionSize.MSB = data.ReadInt32BigEndian();
            bootRecordVolumeDescriptor.SystemUse = data.ReadBytes(1960);

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
        public static VolumeDescriptorSetTerminator? ParseVolumeDescriptorSetTerminator(Stream data, uint sectorLength)
        {
            var volumeDescriptorSetTerminator = new VolumeDescriptorSetTerminator();
            volumeDescriptorSetTerminator.Type = VolumeDescriptorType.VOLUME_DESCRIPTOR_SET_TERMINATOR;
            volumeDescriptorSetTerminator.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (false)
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
        public static GenericVolumeDescriptor? ParseGenericVolumeDescriptor(Stream data, uint sectorLength, byte type)
        {
            var genericVolumeDescriptor = new GenericVolumeDescriptor();
            genericVolumeDescriptor.Type = type;
            genericVolumeDescriptor.Identifier = data.ReadBytes(5);

            // Validate Identifier, return null and rewind if invalid
            if (false)
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
            return null;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DirectoryRecord on success, null on error</returns>
        public static DirectoryRecord? ParseDirectoryRecord(Stream data)
        {
            return null;
        }

        /// <summary>
        /// Parse a Stream into a DirectoryExtent
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="volumeDescriptorSet"></param>
        /// <returns>Filled DirectoryExtent on success, null on error</returns>
        public static DirectoryExtent? ParseDirectoryExtent(Stream data, uint sectorLength)
        {
            return null;
        }
    }
}
