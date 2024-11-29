using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Models.PKZIP;
using static SabreTools.Models.PKZIP.Constants;

namespace SabreTools.Serialization.Deserializers
{
    public class PKZIP : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        protected override bool SkipCompression => true;

        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                var archive = new Archive();

                #region End of Central Directory Record

                // Find the end of central directory record
                long eocdrOffset = SearchForEndOfCentralDirectoryRecord(data);
                if (eocdrOffset < 0 || eocdrOffset >= data.Length)
                    return null;

                // Seek to the end of central directory record
                data.Seek(eocdrOffset, SeekOrigin.Begin);

                // Read the end of central directory record
                var eocdr = ParseEndOfCentralDirectoryRecord(data);
                if (eocdr == null)
                    return null;

                // Assign the end of central directory record
                archive.EndOfCentralDirectoryRecord = eocdr;

                #endregion

                #region ZIP64 End of Central Directory Locator and Record

                // Set a flag for ZIP64 not found by default
                bool zip64 = false;

                // Process ZIP64 if any fields are set to max value
                if (eocdr.DiskNumber == 0xFFFF
                    || eocdr.StartDiskNumber == 0xFFFF
                    || eocdr.TotalEntriesOnDisk == 0xFFFF
                    || eocdr.TotalEntries == 0xFFFF
                    || eocdr.CentralDirectorySize == 0xFFFFFFFF
                    || eocdr.CentralDirectoryOffset == 0xFFFFFFFF)
                {
                    // Set the ZIP64 flag
                    zip64 = true;

                    // Find the ZIP64 end of central directory locator
                    long eocdlOffset = SearchForZIP64EndOfCentralDirectoryLocator(data);
                    if (eocdlOffset < 0 || eocdlOffset >= data.Length)
                        return null;

                    // Seek to the ZIP64 end of central directory locator
                    data.Seek(eocdlOffset, SeekOrigin.Begin);

                    // Read the ZIP64 end of central directory locator
                    var eocdl64 = data.ReadType<EndOfCentralDirectoryLocator64>();
                    if (eocdl64 == null)
                        return null;

                    // Assign the ZIP64 end of central directory record
                    archive.ZIP64EndOfCentralDirectoryLocator = eocdl64;

                    // Try to get the ZIP64 end of central directory record offset
                    if ((long)eocdl64.CentralDirectoryOffset < 0 || (long)eocdl64.CentralDirectoryOffset >= data.Length)
                        return null;

                    // Seek to the ZIP64 end of central directory record
                    data.Seek((long)eocdl64.CentralDirectoryOffset, SeekOrigin.Begin);

                    // Read the ZIP64 end of central directory record
                    var eocdr64 = ParseEndOfCentralDirectoryRecord64(data);
                    if (eocdr64 == null)
                        return null;

                    // Assign the ZIP64 end of central directory record
                    archive.ZIP64EndOfCentralDirectoryRecord = eocdr64;
                }

                #endregion

                #region Central Directory Records

                // Try to get the central directory record offset
                long cdrOffset, cdrSize;
                if (zip64 && archive.ZIP64EndOfCentralDirectoryRecord != null)
                {
                    cdrOffset = (long)archive.ZIP64EndOfCentralDirectoryRecord.CentralDirectoryOffset;
                    cdrSize = (long)archive.ZIP64EndOfCentralDirectoryRecord.CentralDirectorySize;
                }
                else if (archive.EndOfCentralDirectoryRecord != null)
                {
                    cdrOffset = archive.EndOfCentralDirectoryRecord.CentralDirectoryOffset;
                    cdrSize = archive.EndOfCentralDirectoryRecord.CentralDirectorySize;
                }
                else
                {
                    return null;
                }

                // Try to get the central directory record offset
                if (cdrOffset < 0 || cdrOffset >= data.Length)
                    return null;

                // Seek to the first central directory record
                data.Seek(cdrOffset, SeekOrigin.Begin);

                // Cache the current offset
                long currentOffset = data.Position;

                // Read the central directory records
                var cdrs = new List<CentralDirectoryFileHeader>();
                while (data.Position < currentOffset + cdrSize)
                {
                    // Read the central directory record
                    var cdr = ParseCentralDirectoryFileHeader(data);
                    if (cdr == null)
                        return null;

                    // Add the central directory record
                    cdrs.Add(cdr);
                }

                // Assign the central directory records
                archive.CentralDirectoryHeaders = [.. cdrs];

                #endregion

                // TODO: Handle digital signature -- immediately following central directory records

                #region Archive Extra Data Record

                // Find the archive extra data record
                long aedrOffset = SearchForArchiveExtraDataRecord(data, cdrOffset);
                if (aedrOffset >= 0 && aedrOffset < data.Length)
                {
                    // Seek to the archive extra data record
                    data.Seek(aedrOffset, SeekOrigin.Begin);

                    // Read the archive extra data record
                    var aedr = ParseArchiveExtraDataRecord(data);
                    if (aedr == null)
                        return null;

                    // Assign the archive extra data record
                    archive.ArchiveExtraDataRecord = aedr;
                }

                #endregion

                #region Local File

                // Setup all of the collections
                var localFileHeaders = new List<LocalFileHeader>();
                var encryptionHeaders = new List<byte[]?>();
                var fileData = new List<byte[]>(); // TODO: Should this data be read here?
                var dataDescriptors = new List<DataDescriptor?>();
                var zip64DataDescriptors = new List<DataDescriptor64?>();

                // Read the local file headers
                for (int i = 0; i < archive.CentralDirectoryHeaders.Length; i++)
                {
                    var header = archive.CentralDirectoryHeaders[i];

                    // Get the local file header offset
                    long headerOffset = header.RelativeOffsetOfLocalHeader;
                    if (headerOffset == 0xFFFFFFFF && header.ExtraField != null)
                    {
                        // TODO: Parse into a proper structure instead of this
                        byte[] extraData = header.ExtraField;
                        if (BitConverter.ToUInt16(extraData, 0) == 0x0001)
                            headerOffset = BitConverter.ToInt64(extraData, 4);
                    }

                    if (headerOffset < 0 || headerOffset >= data.Length)
                        return null;

                    // Seek to the local file header
                    data.Seek(headerOffset, SeekOrigin.Begin);

                    // Try to parse the local header
                    var localFileHeader = ParseLocalFileHeader(data);
                    if (localFileHeader == null)
                    {
                        // Add a placeholder null item
                        localFileHeaders.Add(new LocalFileHeader());
                        encryptionHeaders.Add(null);
                        fileData.Add([]);
                        dataDescriptors.Add(null);
                        zip64DataDescriptors.Add(null);
                        continue;
                    }

                    // Add the local file header
                    localFileHeaders.Add(localFileHeader);

                    // Only read the encryption header if necessary
#if NET20 || NET35
                if ((header.Flags & GeneralPurposeBitFlags.FileEncrypted) != 0)
#else
                    if (header.Flags.HasFlag(GeneralPurposeBitFlags.FileEncrypted))
#endif
                    {
                        // Try to read the encryption header data -- TODO: Verify amount to read
                        byte[] encryptionHeader = data.ReadBytes(12);
                        if (encryptionHeader.Length != 12)
                            return null;

                        // Add the encryption header
                        encryptionHeaders.Add(encryptionHeader);
                    }
                    else
                    {
                        // Add the null encryption header
                        encryptionHeaders.Add(null);
                    }

                    // Try to read the file data
                    byte[] fileDatum = data.ReadBytes((int)header.CompressedSize);
                    if (fileDatum.Length < header.CompressedSize)
                        return null;

                    // Add the file data
                    fileData.Add(fileDatum);

                    // Only read the data descriptor if necessary
#if NET20 || NET35
                if ((header.Flags & GeneralPurposeBitFlags.NoCRC) != 0)
#else
                    if (header.Flags.HasFlag(GeneralPurposeBitFlags.NoCRC))
#endif
                    {
                        // Select the data descriptor that is being used
                        if (zip64)
                        {
                            // Try to parse the data descriptor
                            var dataDescriptor64 = ParseDataDescriptor64(data);
                            if (dataDescriptor64 == null)
                                return null;

                            // Add the data descriptor
                            dataDescriptors.Add(null);
                            zip64DataDescriptors.Add(dataDescriptor64);
                        }
                        else
                        {
                            // Try to parse the data descriptor
                            var dataDescriptor = ParseDataDescriptor(data);
                            if (dataDescriptor == null)
                                return null;

                            // Add the data descriptor
                            dataDescriptors.Add(dataDescriptor);
                            zip64DataDescriptors.Add(null);
                        }
                    }
                    else
                    {
                        // Add the null data descriptor
                        dataDescriptors.Add(null);
                        zip64DataDescriptors.Add(null);
                    }
                }

                // Assign the local file headers
                archive.LocalFileHeaders = [.. localFileHeaders];

                // Assign the encryption headers
                archive.EncryptionHeaders = [.. encryptionHeaders];

                // Assign the file data
                archive.FileData = [.. fileData];

                // Assign the data descriptors
                archive.DataDescriptors = [.. dataDescriptors];
                archive.ZIP64DataDescriptors = [.. zip64DataDescriptors];

                #endregion

                // TODO: Handle archive decryption header

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Search for the end of central directory record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Position of the end of central directory record, -1 on error</returns>
        public static long SearchForEndOfCentralDirectoryRecord(Stream data)
        {
            // Cache the current offset
            long current = data.Position;

            // Seek to the minimum size of the record from the end
            data.Seek(-22, SeekOrigin.End);

            // Attempt to find the end of central directory signature
            while (data.Position > 0)
            {
                // Read the potential signature
                uint possibleSignature = data.ReadUInt32();
                if (possibleSignature == EndOfCentralDirectoryRecordSignature)
                {
                    long signaturePosition = data.Position - 4;
                    data.Seek(current, SeekOrigin.Begin);
                    return signaturePosition;
                }

                // If we find any other signature
                switch (possibleSignature)
                {
                    case ArchiveExtraDataRecordSignature:
                    case CentralDirectoryFileHeaderSignature:
                    case DataDescriptorSignature:
                    case DigitalSignatureSignature:
                    case EndOfCentralDirectoryLocator64Signature:
                    case EndOfCentralDirectoryRecord64Signature:
                    case LocalFileHeaderSignature:
                        data.Seek(current, SeekOrigin.Begin);
                        return -1;
                }

                // Seek backward 5 bytes, if possible
                data.Seek(-5, SeekOrigin.Current);
            }

            // No signature was found
            data.Seek(current, SeekOrigin.Begin);
            return -1;
        }

        /// <summary>
        /// Parse a Stream into an end of central directory record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled end of central directory record on success, null on error</returns>
        public static EndOfCentralDirectoryRecord? ParseEndOfCentralDirectoryRecord(Stream data)
        {
            var record = new EndOfCentralDirectoryRecord();

            record.Signature = data.ReadUInt32();
            if (record.Signature != EndOfCentralDirectoryRecordSignature)
                return null;

            record.DiskNumber = data.ReadUInt16();
            record.StartDiskNumber = data.ReadUInt16();
            record.TotalEntriesOnDisk = data.ReadUInt16();
            record.TotalEntries = data.ReadUInt16();
            record.CentralDirectorySize = data.ReadUInt32();
            record.CentralDirectoryOffset = data.ReadUInt32();
            record.FileCommentLength = data.ReadUInt16();
            if (record.FileCommentLength > 0 && data.Position + record.FileCommentLength <= data.Length)
            {
                byte[] commentBytes = data.ReadBytes(record.FileCommentLength);
                if (commentBytes.Length != record.FileCommentLength)
                    return null;

                record.FileComment = Encoding.ASCII.GetString(commentBytes);
            }

            return record;
        }

        /// <summary>
        /// Search for the ZIP64 end of central directory locator
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Position of the ZIP64 end of central directory locator, -1 on error</returns>
        public static long SearchForZIP64EndOfCentralDirectoryLocator(Stream data)
        {
            // Cache the current offset
            long current = data.Position;

            // Seek to the minimum size of the record from the minimum start
            // of theend of central directory record
            data.Seek(-22 + -20, SeekOrigin.Current);

            // Attempt to find the ZIP64 end of central directory locator signature
            while (data.Position > 0)
            {
                // Read the potential signature
                uint possibleSignature = data.ReadUInt32();
                if (possibleSignature == EndOfCentralDirectoryLocator64Signature)
                {
                    long signaturePosition = data.Position - 4;
                    data.Seek(current, SeekOrigin.Begin);
                    return signaturePosition;
                }

                // If we find any other signature
                switch (possibleSignature)
                {
                    case ArchiveExtraDataRecordSignature:
                    case CentralDirectoryFileHeaderSignature:
                    case DataDescriptorSignature:
                    case DigitalSignatureSignature:
                    case EndOfCentralDirectoryRecordSignature:
                    case EndOfCentralDirectoryRecord64Signature:
                    case LocalFileHeaderSignature:
                        data.Seek(current, SeekOrigin.Begin);
                        return -1;
                }

                // Seek backward 5 bytes, if possible
                data.Seek(-5, SeekOrigin.Current);
            }

            // No signature was found
            data.Seek(current, SeekOrigin.Begin);
            return -1;
        }

        /// <summary>
        /// Parse a Stream into a ZIP64 end of central directory record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZIP64 end of central directory record on success, null on error</returns>
        public static EndOfCentralDirectoryRecord64? ParseEndOfCentralDirectoryRecord64(Stream data)
        {
            var record = new EndOfCentralDirectoryRecord64();

            record.Signature = data.ReadUInt32();
            if (record.Signature != EndOfCentralDirectoryRecord64Signature)
                return null;

            record.DirectoryRecordSize = data.ReadUInt64();
            record.HostSystem = (HostSystem)data.ReadByteValue();
            record.VersionMadeBy = data.ReadByteValue();
            record.VersionNeededToExtract = data.ReadUInt16();
            record.DiskNumber = data.ReadUInt32();
            record.StartDiskNumber = data.ReadUInt32();
            record.TotalEntriesOnDisk = data.ReadUInt64();
            record.TotalEntries = data.ReadUInt64();
            record.CentralDirectorySize = data.ReadUInt64();
            record.CentralDirectoryOffset = data.ReadUInt64();

            // TODO: Handle the ExtensibleDataSector -- How to detect if exists?

            return record;
        }

        /// <summary>
        /// Parse a Stream into a central directory file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled central directory file header on success, null on error</returns>
        public static CentralDirectoryFileHeader? ParseCentralDirectoryFileHeader(Stream data)
        {
            var header = new CentralDirectoryFileHeader();

            header.Signature = data.ReadUInt32();
            if (header.Signature != CentralDirectoryFileHeaderSignature)
                return null;

            header.HostSystem = (HostSystem)data.ReadByteValue();
            header.VersionMadeBy = data.ReadByteValue();
            header.VersionNeededToExtract = data.ReadUInt16();
            header.Flags = (GeneralPurposeBitFlags)data.ReadUInt16();
            header.CompressionMethod = (CompressionMethod)data.ReadUInt16();
            header.LastModifedFileTime = data.ReadUInt16();
            header.LastModifiedFileDate = data.ReadUInt16();
            header.CRC32 = data.ReadUInt32();
            header.CompressedSize = data.ReadUInt32();
            header.UncompressedSize = data.ReadUInt32();
            header.FileNameLength = data.ReadUInt16();
            header.ExtraFieldLength = data.ReadUInt16();
            header.FileCommentLength = data.ReadUInt16();
            header.DiskNumberStart = data.ReadUInt16();
            header.InternalFileAttributes = (InternalFileAttributes)data.ReadUInt16();
            header.ExternalFileAttributes = data.ReadUInt32();
            header.RelativeOffsetOfLocalHeader = data.ReadUInt32();

            if (header.FileNameLength > 0 && data.Position + header.FileNameLength <= data.Length)
            {
                byte[] filenameBytes = data.ReadBytes(header.FileNameLength);
                if (filenameBytes.Length != header.FileNameLength)
                    return null;

                header.FileName = Encoding.ASCII.GetString(filenameBytes);
            }
            if (header.ExtraFieldLength > 0 && data.Position + header.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes(header.ExtraFieldLength);
                if (extraBytes.Length != header.ExtraFieldLength)
                    return null;

                header.ExtraField = extraBytes;
            }
            if (header.FileCommentLength > 0 && data.Position + header.FileCommentLength <= data.Length)
            {
                byte[] commentBytes = data.ReadBytes(header.FileCommentLength);
                if (commentBytes.Length != header.FileCommentLength)
                    return null;

                header.FileComment = Encoding.ASCII.GetString(commentBytes);
            }

            return header;
        }

        /// <summary>
        /// Search for the archive extra data record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="centralDirectoryoffset">Offset to the first central directory record</param>
        /// <returns>Position of the archive extra data record, -1 on error</returns>
        public static long SearchForArchiveExtraDataRecord(Stream data, long centralDirectoryoffset)
        {
            // Cache the current offset
            long current = data.Position;

            // Seek to the minimum size of the record from the central directory
            data.Seek(centralDirectoryoffset - 8, SeekOrigin.Begin);

            // Attempt to find the end of central directory signature
            while (data.Position > 0)
            {
                // Read the potential signature
                uint possibleSignature = data.ReadUInt32();
                if (possibleSignature == ArchiveExtraDataRecordSignature)
                {
                    long signaturePosition = data.Position - 4;
                    data.Seek(current, SeekOrigin.Begin);
                    return signaturePosition;
                }

                // If we find any other signature
                switch (possibleSignature)
                {
                    case CentralDirectoryFileHeaderSignature:
                    case DataDescriptorSignature:
                    case DigitalSignatureSignature:
                    case EndOfCentralDirectoryLocator64Signature:
                    case EndOfCentralDirectoryRecordSignature:
                    case EndOfCentralDirectoryRecord64Signature:
                    case LocalFileHeaderSignature:
                        data.Seek(current, SeekOrigin.Begin);
                        return -1;
                }

                // Seek backward 5 bytes, if possible
                data.Seek(-5, SeekOrigin.Current);
            }

            // No signature was found
            data.Seek(current, SeekOrigin.Begin);
            return -1;
        }

        /// <summary>
        /// Parse a Stream into an archive extra data record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled archive extra data record on success, null on error</returns>
        public static ArchiveExtraDataRecord? ParseArchiveExtraDataRecord(Stream data)
        {
            var record = new ArchiveExtraDataRecord();

            record.Signature = data.ReadUInt32();
            if (record.Signature != ArchiveExtraDataRecordSignature)
                return null;

            record.ExtraFieldLength = data.ReadUInt32();
            if (record.ExtraFieldLength > 0 && data.Position + record.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes((int)record.ExtraFieldLength);
                if (extraBytes.Length != record.ExtraFieldLength)
                    return null;

                record.ExtraFieldData = extraBytes;
            }

            return record;
        }

        /// <summary>
        /// Parse a Stream into a local file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled local file header on success, null on error</returns>
        public static LocalFileHeader? ParseLocalFileHeader(Stream data)
        {
            var header = new LocalFileHeader();

            header.Signature = data.ReadUInt32();
            if (header.Signature != LocalFileHeaderSignature)
                return null;

            header.Version = data.ReadUInt16();
            header.Flags = (GeneralPurposeBitFlags)data.ReadUInt16();
            header.CompressionMethod = (CompressionMethod)data.ReadUInt16();
            header.LastModifedFileTime = data.ReadUInt16();
            header.LastModifiedFileDate = data.ReadUInt16();
            header.CRC32 = data.ReadUInt32();
            header.CompressedSize = data.ReadUInt32();
            header.UncompressedSize = data.ReadUInt32();
            header.FileNameLength = data.ReadUInt16();
            header.ExtraFieldLength = data.ReadUInt16();

            if (header.FileNameLength > 0 && data.Position + header.FileNameLength <= data.Length)
            {
                byte[] filenameBytes = data.ReadBytes(header.FileNameLength);
                if (filenameBytes.Length != header.FileNameLength)
                    return null;

                header.FileName = Encoding.ASCII.GetString(filenameBytes);
            }
            if (header.ExtraFieldLength > 0 && data.Position + header.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes(header.ExtraFieldLength);
                if (extraBytes.Length != header.ExtraFieldLength)
                    return null;

                header.ExtraField = extraBytes;
            }

            return header;
        }

        /// <summary>
        /// Parse a Stream into a data descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled data descriptor on success, null on error</returns>
        public static DataDescriptor? ParseDataDescriptor(Stream data)
        {
            var dataDescriptor = new DataDescriptor();

            // Signatures are expected but not required
            dataDescriptor.Signature = data.ReadUInt32();
            if (dataDescriptor.Signature != DataDescriptorSignature)
                data.Seek(-4, SeekOrigin.Current);

            dataDescriptor.CRC32 = data.ReadUInt32();
            dataDescriptor.CompressedSize = data.ReadUInt32();
            dataDescriptor.UncompressedSize = data.ReadUInt32();

            return dataDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a ZIP64 data descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZIP64 data descriptor on success, null on error</returns>
        public static DataDescriptor64? ParseDataDescriptor64(Stream data)
        {
            var zip64DataDescriptor = new DataDescriptor64();

            // Signatures are expected but not required
            zip64DataDescriptor.Signature = data.ReadUInt32();
            if (zip64DataDescriptor.Signature != DataDescriptorSignature)
                data.Seek(-4, SeekOrigin.Current);

            zip64DataDescriptor.CRC32 = data.ReadUInt32();
            zip64DataDescriptor.CompressedSize = data.ReadUInt64();
            zip64DataDescriptor.UncompressedSize = data.ReadUInt64();

            return zip64DataDescriptor;
        }
    }
}