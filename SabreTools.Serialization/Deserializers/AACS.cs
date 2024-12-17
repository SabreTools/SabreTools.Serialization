using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.AACS;

namespace SabreTools.Serialization.Deserializers
{
    public class AACS : BaseBinaryDeserializer<MediaKeyBlock>
    {
        /// <inheritdoc/>
        public override MediaKeyBlock? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                #region Records

                // Create the records list
                var records = new List<Record>();

                // Try to parse the records
                while (data.Position < data.Length)
                {
                    // Try to parse the record
                    var record = ParseRecord(data);
                    if (record == null)
                        return null;

                    // Add the record
                    records.Add(record);

                    // If we have an end of media key block record
                    if (record.RecordType == RecordType.EndOfMediaKeyBlock)
                        break;

                    // Align to the 4-byte boundary if we're not at the end
                    if (!data.AlignToBoundary(4))
                        break;
                }

                #endregion

                // Set the records
                if (records.Count > 0)
                    return new MediaKeyBlock { Records = [.. records] };

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a Record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled Record on success, null on error</returns>
        private static Record? ParseRecord(Stream data)
        {
            // The first byte is the type
            RecordType type = (RecordType)data.ReadByteValue();
            data.Seek(-1, SeekOrigin.Current);

            // Create a record based on the type
            return type switch
            {
                // Known record types
                RecordType.EndOfMediaKeyBlock => ParseEndOfMediaKeyBlockRecord(data),
                RecordType.ExplicitSubsetDifference => ParseExplicitSubsetDifferenceRecord(data),
                RecordType.MediaKeyData => ParseMediaKeyDataRecord(data),
                RecordType.SubsetDifferenceIndex => ParseSubsetDifferenceIndexRecord(data),
                RecordType.TypeAndVersion => ParseTypeAndVersionRecord(data),
                RecordType.DriveRevocationList => ParseDriveRevocationListRecord(data),
                RecordType.HostRevocationList => ParseHostRevocationListRecord(data),
                RecordType.VerifyMediaKey => ParseVerifyMediaKeyRecord(data),
                RecordType.Copyright => ParseCopyrightRecord(data),

                // Unknown record type
                _ => null,
            };
        }

        /// <summary>
        /// Parse a Stream into a CopyrightRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled CopyrightRecord on success, null on error</returns>
        public static CopyrightRecord ParseCopyrightRecord(Stream data)
        {
            var obj = new CopyrightRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            if (obj.RecordLength > 4)
            {
                byte[] copyright = data.ReadBytes((int)(obj.RecordLength - 4));
                obj.Copyright = Encoding.ASCII.GetString(copyright).TrimEnd('\0');
            }

            return obj;
        }
        
        /// <summary>
        /// Parse a Stream into a DriveRevocationListEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DriveRevocationListEntry on success, null on error</returns>
        public static DriveRevocationListEntry ParseDriveRevocationListEntry(Stream data)
        {
            var obj = new DriveRevocationListEntry();

            obj.Range = data.ReadUInt16BigEndian();
            obj.DriveID = data.ReadBytes(6);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DriveRevocationListRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DriveRevocationListRecord on success, null on error</returns>
        public static DriveRevocationListRecord ParseDriveRevocationListRecord(Stream data)
        {
            // Cache the current offset
            long initialOffset = data.Position;

            var obj = new DriveRevocationListRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            obj.TotalNumberOfEntries = data.ReadUInt32BigEndian();

            // Try to parse the signature blocks
            var blocks = new List<DriveRevocationSignatureBlock>();
            uint entryCount = 0;
            while (entryCount < obj.TotalNumberOfEntries && data.Position < initialOffset + obj.RecordLength)
            {
                var block = ParseDriveRevocationSignatureBlock(data);
                entryCount += block.NumberOfEntries;
                blocks.Add(block);

                // If we have an empty block
                if (block.NumberOfEntries == 0)
                    break;
            }

            // Set the signature blocks
            obj.SignatureBlocks = [.. blocks];

            // If there's any data left, discard it
            if (data.Position < initialOffset + obj.RecordLength)
                _ = data.ReadBytes((int)(initialOffset + obj.RecordLength - data.Position));

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DriveRevocationSignatureBlock
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled DriveRevocationSignatureBlock on success, null on error</returns>
        public static DriveRevocationSignatureBlock ParseDriveRevocationSignatureBlock(Stream data)
        {
            var obj = new DriveRevocationSignatureBlock();

            obj.NumberOfEntries = data.ReadUInt32BigEndian();
            obj.EntryFields = new DriveRevocationListEntry[obj.NumberOfEntries];
            for (int i = 0; i < obj.EntryFields.Length; i++)
            {
                obj.EntryFields[i] = ParseDriveRevocationListEntry(data);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a EndOfMediaKeyBlockRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled EndOfMediaKeyBlockRecord on success, null on error</returns>
        public static EndOfMediaKeyBlockRecord ParseEndOfMediaKeyBlockRecord(Stream data)
        {
            var obj = new EndOfMediaKeyBlockRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            if (obj.RecordLength > 4)
                obj.SignatureData = data.ReadBytes((int)(obj.RecordLength - 4));

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ExplicitSubsetDifferenceRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ExplicitSubsetDifferenceRecord on success, null on error</returns>
        public static ExplicitSubsetDifferenceRecord ParseExplicitSubsetDifferenceRecord(Stream data)
        {
            // Cache the current offset
            long initialOffset = data.Position;

            var obj = new ExplicitSubsetDifferenceRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();

            // Try to parse the subset differences
            var subsetDifferences = new List<SubsetDifference>();
            while (data.Position < initialOffset + obj.RecordLength - 5)
            {
                var subsetDifference = ParseSubsetDifference(data);
                subsetDifferences.Add(subsetDifference);
            }

            // Set the subset differences
            obj.SubsetDifferences = [.. subsetDifferences];

            // If there's any data left, discard it
            if (data.Position < initialOffset + obj.RecordLength)
                _ = data.ReadBytes((int)(initialOffset + obj.RecordLength - data.Position));

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HostRevocationListEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HostRevocationListEntry on success, null on error</returns>
        public static HostRevocationListEntry ParseHostRevocationListEntry(Stream data)
        {
            var obj = new HostRevocationListEntry();

            obj.Range = data.ReadUInt16BigEndian();
            obj.HostID = data.ReadBytes(6);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HostRevocationListRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HostRevocationListRecord on success, null on error</returns>
        public static HostRevocationListRecord ParseHostRevocationListRecord(Stream data)
        {
            // Cache the current offset
            long initialOffset = data.Position;

            var obj = new HostRevocationListRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            obj.TotalNumberOfEntries = data.ReadUInt32BigEndian();

            // Try to parse the signature blocks
            var blocks = new List<HostRevocationSignatureBlock>();
            for (uint entryCount = 0; entryCount < obj.TotalNumberOfEntries && data.Position < initialOffset + obj.RecordLength;)
            {
                var block = ParseHostRevocationSignatureBlock(data);
                entryCount += block.NumberOfEntries;
                blocks.Add(block);

                // If we have an empty block
                if (block.NumberOfEntries == 0)
                    break;
            }

            // Set the signature blocks
            obj.SignatureBlocks = [.. blocks];

            // If there's any data left, discard it
            if (data.Position < initialOffset + obj.RecordLength)
                _ = data.ReadBytes((int)(initialOffset + obj.RecordLength - data.Position));

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a HostRevocationSignatureBlock
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled HostRevocationSignatureBlock on success, null on error</returns>
        public static HostRevocationSignatureBlock ParseHostRevocationSignatureBlock(Stream data)
        {
            var obj = new HostRevocationSignatureBlock();

            obj.NumberOfEntries = data.ReadUInt32BigEndian();
            obj.EntryFields = new HostRevocationListEntry[obj.NumberOfEntries];
            for (int i = 0; i < obj.EntryFields.Length; i++)
            {
                obj.EntryFields[i] = ParseHostRevocationListEntry(data);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a MediaKeyDataRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled MediaKeyDataRecord on success, null on error</returns>
        public static MediaKeyDataRecord ParseMediaKeyDataRecord(Stream data)
        {
            // Cache the current offset
            long initialOffset = data.Position;

            var obj = new MediaKeyDataRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();

            // Try to parse the media keys
            var mediaKeys = new List<byte[]>();
            while (data.Position < initialOffset + obj.RecordLength)
            {
                byte[] mediaKey = data.ReadBytes(0x10);
                mediaKeys.Add(mediaKey);
            }

            // Set the media keys
            obj.MediaKeyData = [.. mediaKeys];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SubsetDifference
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SubsetDifference on success, null on error</returns>
        public static SubsetDifference ParseSubsetDifference(Stream data)
        {
            var obj = new SubsetDifference();

            obj.Mask = data.ReadByteValue();
            obj.Number = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a SubsetDifferenceIndexRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled SubsetDifferenceIndexRecord on success, null on error</returns>
        public static SubsetDifferenceIndexRecord ParseSubsetDifferenceIndexRecord(Stream data)
        {
            // Cache the current offset
            long initialOffset = data.Position;

            var obj = new SubsetDifferenceIndexRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            obj.Span = data.ReadUInt32BigEndian();

            // Try to parse the offsets
            var offsets = new List<uint>();
            while (data.Position < initialOffset + obj.RecordLength)
            {
                uint offset = data.ReadUInt32BigEndian();
                offsets.Add(offset);
            }

            // Set the offsets
            obj.Offsets = [.. offsets];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a TypeAndVersionRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled TypeAndVersionRecord on success, null on error</returns>
        public static TypeAndVersionRecord ParseTypeAndVersionRecord(Stream data)
        {
            var obj = new TypeAndVersionRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            obj.MediaKeyBlockType = (MediaKeyBlockType)data.ReadUInt32BigEndian();
            obj.VersionNumber = data.ReadUInt32BigEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a VerifyMediaKeyRecord
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled VerifyMediaKeyRecord on success, null on error</returns>
        public static VerifyMediaKeyRecord ParseVerifyMediaKeyRecord(Stream data)
        {
            var obj = new VerifyMediaKeyRecord();

            obj.RecordType = (RecordType)data.ReadByteValue();
            obj.RecordLength = data.ReadUInt24();
            obj.CiphertextValue = data.ReadBytes(0x10);

            return obj;
        }
    }
}