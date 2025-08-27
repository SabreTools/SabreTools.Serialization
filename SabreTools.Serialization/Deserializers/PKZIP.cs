using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
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
                // Cache the current offset
                long initialOffset = data.Position;

                var archive = new Archive();

                // Setup all of the collections
                var localFileHeaders = new List<LocalFileHeader>();
                var encryptionHeaders = new List<byte[]>();
                var fileData = new List<byte[]>(); // TODO: Should this data be read here?
                var dataDescriptors = new List<DataDescriptor>();
                var zip64DataDescriptors = new List<DataDescriptor64>();
                var cdrs = new List<CentralDirectoryFileHeader>();

                // Flag if we have a ZIP64 archive
                bool? zip64 = null;

                // Read all blocks
                do
                {
                    // Read the signature
                    long beforeSignature = data.Position;
                    uint signature = data.ReadUInt32LittleEndian();
                    data.Seek(beforeSignature, SeekOrigin.Begin);

                    // Switch based on the signature found
                    bool validBlock = false;
                    switch (signature)
                    {
                        // Central Directory File Header
                        case CentralDirectoryFileHeaderSignature:
                            var cdr = ParseCentralDirectoryFileHeader(data);
                            if (cdr == null)
                                return null;

                            // Add the central directory record
                            validBlock = true;
                            cdrs.Add(cdr);
                            break;

                        // Local File
                        case LocalFileHeaderSignature:
                            #region Local File Header

                            var localFileHeader = ParseLocalFileHeader(data);
                            if (localFileHeader == null)
                                break;

                            // Add the local file header
                            localFileHeaders.Add(localFileHeader);

                            #endregion

                            #region Encryption Header

                            // Only read the encryption header if necessary
#if NET20 || NET35
                            if ((localFileHeader.Flags & GeneralPurposeBitFlags.FileEncrypted) != 0)
#else
                            if (localFileHeader.Flags.HasFlag(GeneralPurposeBitFlags.FileEncrypted))
#endif
                            {
                                // Try to read the encryption header data -- TODO: Verify amount to read
                                byte[] encryptionHeader = data.ReadBytes(12);
                                if (encryptionHeader.Length != 12)
                                    break;

                                // Add the encryption header
                                encryptionHeaders.Add(encryptionHeader);
                            }
                            else
                            {
                                // Add the empty encryption header
                                encryptionHeaders.Add([]);
                            }

                            #endregion

                            #region File Data

                            // Try to read the file data
                            byte[] fileDatum = data.ReadBytes((int)localFileHeader.CompressedSize);
                            if (fileDatum.Length < localFileHeader.CompressedSize)
                                break;

                            // Add the file data
                            validBlock = true;
                            fileData.Add(fileDatum);

                            #endregion

                            break;

                        // TODO: Implement
                        case DigitalSignatureSignature:
                            break;

                        // End of Central Directory Record
                        case EndOfCentralDirectoryRecordSignature:
                            var eocdr = ParseEndOfCentralDirectoryRecord(data);
                            if (eocdr == null)
                                return null;

                            // Assign the end of central directory record
                            validBlock = true;
                            archive.EndOfCentralDirectoryRecord = eocdr;
                            break;

                        // ZIP64 End of Central Directory
                        case EndOfCentralDirectoryRecord64Signature:
                            var eocdr64 = ParseEndOfCentralDirectoryRecord64(data);
                            if (eocdr64 == null)
                                return null;

                            // Assign the ZIP64 end of central directory record
                            zip64 = true;
                            validBlock = true;
                            archive.ZIP64EndOfCentralDirectoryRecord = eocdr64;
                            break;

                        // ZIP64 End of Central Directory Locator
                        case EndOfCentralDirectoryLocator64Signature:
                            var eocdl64 = ParseEndOfCentralDirectoryLocator64(data);
                            if (eocdl64 == null)
                                return null;

                            // Assign the ZIP64 end of central directory record
                            zip64 = true;
                            validBlock = true;
                            archive.ZIP64EndOfCentralDirectoryLocator = eocdl64;
                            break;

                        // Archive Extra Data Record
                        case ArchiveExtraDataRecordSignature:
                            var aedr = ParseArchiveExtraDataRecord(data);
                            if (aedr == null)
                                return null;

                            // Assign the archive extra data record
                            validBlock = true;
                            archive.ArchiveExtraDataRecord = aedr;
                            break;

                        // Data Descriptor -- Usually follows a local file header
                        case DataDescriptorSignature:
                            if (zip64 == null)
                            {
                                if (data.Position + 16 == data.Length)
                                {
                                    zip64 = false;
                                }
                                else if (data.Position + 24 == data.Length)
                                {
                                    zip64 = true;
                                }
                                else
                                {
                                    long beforeCheck = data.Position;
                                    data.Seek(16, SeekOrigin.Current);
                                    byte[] nextBlock = data.ReadBytes(2);
                                    data.Seek(beforeCheck, SeekOrigin.Begin);

                                    zip64 = !nextBlock.EqualsExactly([0x50, 0x4B]);
                                }
                            }

                            if (zip64 == true)
                            {
                                // Try to parse the data descriptor
                                var dataDescriptor64 = ParseDataDescriptor64(data);
                                if (dataDescriptor64 == null)
                                    break;

                                // Add the data descriptor
                                validBlock = true;
                                dataDescriptors.Add(new DataDescriptor());
                                zip64DataDescriptors.Add(dataDescriptor64);
                            }
                            else
                            {
                                // Try to parse the data descriptor
                                var dataDescriptor = ParseDataDescriptor(data);
                                if (dataDescriptor == null)
                                    break;

                                // Add the data descriptor
                                validBlock = true;
                                dataDescriptors.Add(dataDescriptor);
                                zip64DataDescriptors.Add(new DataDescriptor64());
                            }

                            break;
                    }

                    // If there was an invalid block
                    if (!validBlock)
                        break;

                } while (data.Position < data.Length);

                // If no blocks were read
                if (localFileHeaders.Count == 0
                    && encryptionHeaders.Count == 0
                    && fileData.Count == 0
                    && dataDescriptors.Count == 0
                    && zip64DataDescriptors.Count == 0
                    && cdrs.Count == 0)
                {
                    return null;
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

                // Assign the central directory records
                archive.CentralDirectoryHeaders = [.. cdrs];

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into an archive extra data record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled archive extra data record on success, null on error</returns>
        public static ArchiveExtraDataRecord? ParseArchiveExtraDataRecord(Stream data)
        {
            var obj = new ArchiveExtraDataRecord();

            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != ArchiveExtraDataRecordSignature)
                return null;

            obj.ExtraFieldLength = data.ReadUInt32LittleEndian();
            if (obj.ExtraFieldLength > 0 && data.Position + obj.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes((int)obj.ExtraFieldLength);
                if (extraBytes.Length != obj.ExtraFieldLength)
                    return null;

                obj.ExtraFieldData = extraBytes;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a central directory file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled central directory file header on success, null on error</returns>
        public static CentralDirectoryFileHeader? ParseCentralDirectoryFileHeader(Stream data)
        {
            var header = new CentralDirectoryFileHeader();

            header.Signature = data.ReadUInt32LittleEndian();
            if (header.Signature != CentralDirectoryFileHeaderSignature)
                return null;

            header.HostSystem = (HostSystem)data.ReadByteValue();
            header.VersionMadeBy = data.ReadByteValue();
            header.VersionNeededToExtract = data.ReadUInt16LittleEndian();
            header.Flags = (GeneralPurposeBitFlags)data.ReadUInt16LittleEndian();
            header.CompressionMethod = (CompressionMethod)data.ReadUInt16LittleEndian();
            header.LastModifedFileTime = data.ReadUInt16LittleEndian();
            header.LastModifiedFileDate = data.ReadUInt16LittleEndian();
            header.CRC32 = data.ReadUInt32LittleEndian();
            header.CompressedSize = data.ReadUInt32LittleEndian();
            header.UncompressedSize = data.ReadUInt32LittleEndian();
            header.FileNameLength = data.ReadUInt16LittleEndian();
            header.ExtraFieldLength = data.ReadUInt16LittleEndian();
            header.FileCommentLength = data.ReadUInt16LittleEndian();
            header.DiskNumberStart = data.ReadUInt16LittleEndian();
            header.InternalFileAttributes = (InternalFileAttributes)data.ReadUInt16LittleEndian();
            header.ExternalFileAttributes = data.ReadUInt32LittleEndian();
            header.RelativeOffsetOfLocalHeader = data.ReadUInt32LittleEndian();

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
        /// Parse a Stream into a data descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled data descriptor on success, null on error</returns>
        public static DataDescriptor? ParseDataDescriptor(Stream data)
        {
            var obj = new DataDescriptor();

            // Signatures are expected but not required
            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != DataDescriptorSignature)
                data.Seek(-4, SeekOrigin.Current);

            obj.CRC32 = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ZIP64 data descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZIP64 data descriptor on success, null on error</returns>
        public static DataDescriptor64? ParseDataDescriptor64(Stream data)
        {
            var obj = new DataDescriptor64();

            // Signatures are expected but not required
            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != DataDescriptorSignature)
                data.Seek(-4, SeekOrigin.Current);

            obj.CRC32 = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt64LittleEndian();
            obj.UncompressedSize = data.ReadUInt64LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ZIP64 end of central directory locator
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZIP64 end of central directory locator on success, null on error</returns>
        public static EndOfCentralDirectoryLocator64? ParseEndOfCentralDirectoryLocator64(Stream data)
        {
            var obj = new EndOfCentralDirectoryLocator64();

            // Signatures are expected but not required
            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != EndOfCentralDirectoryLocator64Signature)
                data.Seek(-4, SeekOrigin.Current);

            obj.StartDiskNumber = data.ReadUInt32LittleEndian();
            obj.CentralDirectoryOffset = data.ReadUInt64LittleEndian();
            obj.TotalDisks = data.ReadUInt32LittleEndian();

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an end of central directory record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled end of central directory record on success, null on error</returns>
        public static EndOfCentralDirectoryRecord? ParseEndOfCentralDirectoryRecord(Stream data)
        {
            var record = new EndOfCentralDirectoryRecord();

            record.Signature = data.ReadUInt32LittleEndian();
            if (record.Signature != EndOfCentralDirectoryRecordSignature)
                return null;

            record.DiskNumber = data.ReadUInt16LittleEndian();
            record.StartDiskNumber = data.ReadUInt16LittleEndian();
            record.TotalEntriesOnDisk = data.ReadUInt16LittleEndian();
            record.TotalEntries = data.ReadUInt16LittleEndian();
            record.CentralDirectorySize = data.ReadUInt32LittleEndian();
            record.CentralDirectoryOffset = data.ReadUInt32LittleEndian();
            record.FileCommentLength = data.ReadUInt16LittleEndian();
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
        /// Parse a Stream into a ZIP64 end of central directory record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZIP64 end of central directory record on success, null on error</returns>
        public static EndOfCentralDirectoryRecord64? ParseEndOfCentralDirectoryRecord64(Stream data)
        {
            var record = new EndOfCentralDirectoryRecord64();

            record.Signature = data.ReadUInt32LittleEndian();
            if (record.Signature != EndOfCentralDirectoryRecord64Signature)
                return null;

            record.DirectoryRecordSize = data.ReadUInt64LittleEndian();
            record.HostSystem = (HostSystem)data.ReadByteValue();
            record.VersionMadeBy = data.ReadByteValue();
            record.VersionNeededToExtract = data.ReadUInt16LittleEndian();
            record.DiskNumber = data.ReadUInt32LittleEndian();
            record.StartDiskNumber = data.ReadUInt32LittleEndian();
            record.TotalEntriesOnDisk = data.ReadUInt64LittleEndian();
            record.TotalEntries = data.ReadUInt64LittleEndian();
            record.CentralDirectorySize = data.ReadUInt64LittleEndian();
            record.CentralDirectoryOffset = data.ReadUInt64LittleEndian();

            // TODO: Handle the ExtensibleDataSector -- How to detect if exists?

            return record;
        }

        /// <summary>
        /// Parse a Stream into a local file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled local file header on success, null on error</returns>
        public static LocalFileHeader? ParseLocalFileHeader(Stream data)
        {
            var obj = new LocalFileHeader();

            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != LocalFileHeaderSignature)
                return null;

            obj.Version = data.ReadUInt16LittleEndian();
            obj.Flags = (GeneralPurposeBitFlags)data.ReadUInt16LittleEndian();
            obj.CompressionMethod = (CompressionMethod)data.ReadUInt16LittleEndian();
            obj.LastModifedFileTime = data.ReadUInt16LittleEndian();
            obj.LastModifiedFileDate = data.ReadUInt16LittleEndian();
            obj.CRC32 = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();
            obj.FileNameLength = data.ReadUInt16LittleEndian();
            obj.ExtraFieldLength = data.ReadUInt16LittleEndian();

            if (obj.FileNameLength > 0 && data.Position + obj.FileNameLength <= data.Length)
            {
                byte[] filenameBytes = data.ReadBytes(obj.FileNameLength);
                if (filenameBytes.Length != obj.FileNameLength)
                    return null;

                obj.FileName = Encoding.ASCII.GetString(filenameBytes);
            }
            if (obj.ExtraFieldLength > 0 && data.Position + obj.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes(obj.ExtraFieldLength);
                if (extraBytes.Length != obj.ExtraFieldLength)
                    return null;

                obj.ExtraField = extraBytes;
            }

            return obj;
        }
    }
}
