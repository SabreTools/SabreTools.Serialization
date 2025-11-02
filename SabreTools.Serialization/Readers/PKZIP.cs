using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.PKZIP;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.PKZIP.Constants;

namespace SabreTools.Serialization.Readers
{
    public class PKZIP : BaseBinaryReader<Archive>
    {
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
                var localFiles = new List<LocalFile>();
                var cdrs = new List<CentralDirectoryFileHeader>();

                // Read all blocks
                do
                {
                    // Read the signature
                    long beforeSignature = data.Position;
                    uint signature = data.ReadUInt32LittleEndian();
                    data.SeekIfPossible(beforeSignature, SeekOrigin.Begin);

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
                            var lf = ParseLocalFile(data);
                            if (lf == null)
                                return null;

                            // Add the local file
                            validBlock = true;
                            localFiles.Add(lf);
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
                            validBlock = true;
                            archive.ZIP64EndOfCentralDirectoryRecord = eocdr64;
                            break;

                        // ZIP64 End of Central Directory Locator
                        case EndOfCentralDirectoryLocator64Signature:
                            var eocdl64 = ParseEndOfCentralDirectoryLocator64(data);
                            if (eocdl64 == null)
                                return null;

                            // Assign the ZIP64 end of central directory record
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
                    }

                    // If there was an invalid block
                    if (!validBlock)
                        break;

                } while (data.Position < data.Length);

                // If no blocks were read
                if (localFiles.Count == 0 && cdrs.Count == 0)
                    return null;

                // Assign the local files
                archive.LocalFiles = [.. localFiles];

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
            var obj = new CentralDirectoryFileHeader();

            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != CentralDirectoryFileHeaderSignature)
                return null;

            obj.HostSystem = (HostSystem)data.ReadByteValue();
            obj.VersionMadeBy = data.ReadByteValue();
            obj.VersionNeededToExtract = data.ReadUInt16LittleEndian();
            obj.Flags = (GeneralPurposeBitFlags)data.ReadUInt16LittleEndian();
            obj.CompressionMethod = (CompressionMethod)data.ReadUInt16LittleEndian();
            obj.LastModifedFileTime = data.ReadUInt16LittleEndian();
            obj.LastModifiedFileDate = data.ReadUInt16LittleEndian();
            obj.CRC32 = data.ReadUInt32LittleEndian();
            obj.CompressedSize = data.ReadUInt32LittleEndian();
            obj.UncompressedSize = data.ReadUInt32LittleEndian();
            obj.FileNameLength = data.ReadUInt16LittleEndian();
            obj.ExtraFieldLength = data.ReadUInt16LittleEndian();
            obj.FileCommentLength = data.ReadUInt16LittleEndian();
            obj.DiskNumberStart = data.ReadUInt16LittleEndian();
            obj.InternalFileAttributes = (InternalFileAttributes)data.ReadUInt16LittleEndian();
            obj.ExternalFileAttributes = data.ReadUInt32LittleEndian();
            obj.RelativeOffsetOfLocalHeader = data.ReadUInt32LittleEndian();

#if NET20 || NET35
            bool utf8 = (obj.Flags & GeneralPurposeBitFlags.LanguageEncodingFlag) != 0;
#else
            bool utf8 = obj.Flags.HasFlag(GeneralPurposeBitFlags.LanguageEncodingFlag);
#endif

            if (obj.FileNameLength > 0 && data.Position + obj.FileNameLength <= data.Length)
            {
                byte[] filenameBytes = data.ReadBytes(obj.FileNameLength);
                if (filenameBytes.Length != obj.FileNameLength)
                    return null;

                if (utf8)
                    obj.FileName = Encoding.UTF8.GetString(filenameBytes);
                else
                    obj.FileName = Encoding.ASCII.GetString(filenameBytes);
            }
            if (obj.ExtraFieldLength > 0 && data.Position + obj.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes(obj.ExtraFieldLength);
                if (extraBytes.Length != obj.ExtraFieldLength)
                    return null;

                obj.ExtraFields = ParseExtraFields(obj, extraBytes);
            }
            if (obj.FileCommentLength > 0 && data.Position + obj.FileCommentLength <= data.Length)
            {
                byte[] commentBytes = data.ReadBytes(obj.FileCommentLength);
                if (commentBytes.Length != obj.FileCommentLength)
                    return null;

                if (utf8)
                    obj.FileComment = Encoding.UTF8.GetString(commentBytes);
                else
                    obj.FileComment = Encoding.ASCII.GetString(commentBytes);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a data descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled data descriptor on success, null on error</returns>
        public static DataDescriptor? ParseDataDescriptor(Stream data)
        {
            var obj = new DataDescriptor();

            // Cache the current position
            long currentPosition = data.Position;

            // Check if the block is a 12- or 16-byte
            bool isShort = false;
            if (data.Position + 12 == data.Length)
            {
                isShort = true;
            }
            else
            {
                data.SeekIfPossible(12, SeekOrigin.Current);
                byte[] nextBlock = data.ReadBytes(2);
                data.SeekIfPossible(currentPosition, SeekOrigin.Begin);
                if (nextBlock.EqualsExactly([0x50, 0x4B]))
                    isShort = true;
            }

            // If the 16-byte variant
            if (!isShort)
            {
                obj.Signature = data.ReadUInt32LittleEndian();
                if (obj.Signature != DataDescriptorSignature)
                    return null;
            }

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

            // Cache the current position
            long currentPosition = data.Position;

            // Check if the block is a 20- or 24-byte
            bool isShort = false;
            if (data.Position + 20 == data.Length)
            {
                isShort = true;
            }
            else
            {
                data.SeekIfPossible(20, SeekOrigin.Current);
                byte[] nextBlock = data.ReadBytes(2);
                data.SeekIfPossible(currentPosition, SeekOrigin.Begin);
                if (nextBlock.EqualsExactly([0x50, 0x4B]))
                    isShort = true;
            }

            // If the 24-byte variant
            if (!isShort)
            {
                obj.Signature = data.ReadUInt32LittleEndian();
                if (obj.Signature != DataDescriptorSignature)
                    return null;
            }

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
                data.SeekIfPossible(-4, SeekOrigin.Current);

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
            var obj = new EndOfCentralDirectoryRecord();

            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != EndOfCentralDirectoryRecordSignature)
                return null;

            obj.DiskNumber = data.ReadUInt16LittleEndian();
            obj.StartDiskNumber = data.ReadUInt16LittleEndian();
            obj.TotalEntriesOnDisk = data.ReadUInt16LittleEndian();
            obj.TotalEntries = data.ReadUInt16LittleEndian();
            obj.CentralDirectorySize = data.ReadUInt32LittleEndian();
            obj.CentralDirectoryOffset = data.ReadUInt32LittleEndian();
            obj.FileCommentLength = data.ReadUInt16LittleEndian();
            if (obj.FileCommentLength > 0 && data.Position + obj.FileCommentLength <= data.Length)
            {
                byte[] commentBytes = data.ReadBytes(obj.FileCommentLength);
                if (commentBytes.Length != obj.FileCommentLength)
                    return null;

                obj.FileComment = Encoding.ASCII.GetString(commentBytes);
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ZIP64 end of central directory record
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled ZIP64 end of central directory record on success, null on error</returns>
        public static EndOfCentralDirectoryRecord64? ParseEndOfCentralDirectoryRecord64(Stream data)
        {
            var obj = new EndOfCentralDirectoryRecord64();

            obj.Signature = data.ReadUInt32LittleEndian();
            if (obj.Signature != EndOfCentralDirectoryRecord64Signature)
                return null;

            obj.DirectoryRecordSize = data.ReadUInt64LittleEndian();
            obj.HostSystem = (HostSystem)data.ReadByteValue();
            obj.VersionMadeBy = data.ReadByteValue();
            obj.VersionNeededToExtract = data.ReadUInt16LittleEndian();
            obj.DiskNumber = data.ReadUInt32LittleEndian();
            obj.StartDiskNumber = data.ReadUInt32LittleEndian();
            obj.TotalEntriesOnDisk = data.ReadUInt64LittleEndian();
            obj.TotalEntries = data.ReadUInt64LittleEndian();
            obj.CentralDirectorySize = data.ReadUInt64LittleEndian();
            obj.CentralDirectoryOffset = data.ReadUInt64LittleEndian();

            // TODO: Handle the ExtensibleDataSector -- How to detect if exists?

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a local file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled local file on success, null on error</returns>
        public static LocalFile? ParseLocalFile(Stream data)
        {
            var obj = new LocalFile();

            #region Local File Header

            // Try to read the header
            var localFileHeader = ParseLocalFileHeader(data);
            if (localFileHeader == null)
                return null;

            // Assign the header
            obj.LocalFileHeader = localFileHeader;

            ulong compressedSize = localFileHeader.CompressedSize;
            if (localFileHeader.ExtraFields != null)
            {
                foreach (var field in localFileHeader.ExtraFields)
                {
                    if (field is not Zip64ExtendedInformationExtraField infoField)
                        continue;
                    if (infoField.CompressedSize == null)
                        continue;

                    compressedSize = infoField.CompressedSize.Value;
                }
            }

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
                byte[] encryptionHeaders = data.ReadBytes(12);
                if (encryptionHeaders.Length != 12)
                    return null;

                // Set the encryption headers
                obj.EncryptionHeaders = encryptionHeaders;
            }
            else
            {
                // Add the empty encryption header
                obj.EncryptionHeaders = [];
            }

            #endregion

            #region File Data

            // Try to read the file data
            var fileData = data.ReadBytes((int)compressedSize);
            if (fileData.Length < (long)compressedSize)
                return null;

            // Set the file data
            obj.FileData = fileData;

            #endregion

            #region Data Descriptor

            // Only attempt to read the descriptor if necessary
#if NET20 || NET35
            if ((localFileHeader.Flags & GeneralPurposeBitFlags.NoCRC) == 0)
#else
            if (!localFileHeader.Flags.HasFlag(GeneralPurposeBitFlags.NoCRC))
#endif
            {
                obj.DataDescriptor = new DataDescriptor();
                obj.ZIP64DataDescriptor = new DataDescriptor64();
                return obj;
            }

            // Read the signature
            long beforeSignature = data.Position;
            uint signature = data.ReadUInt32LittleEndian();
            data.SeekIfPossible(beforeSignature, SeekOrigin.Begin);

            // Don't fail if descriptor is missing
            if (signature != DataDescriptorSignature)
            {
                obj.DataDescriptor = new DataDescriptor();
                obj.ZIP64DataDescriptor = new DataDescriptor64();
                return obj;
            }

            // Determine if the entry is ZIP64
            bool zip64 = IsZip64Descriptor(data);

            // Try to parse the correct data descriptor
            if (zip64)
            {
                obj.DataDescriptor = new DataDescriptor();
                obj.ZIP64DataDescriptor = ParseDataDescriptor64(data);
                if (obj.ZIP64DataDescriptor == null)
                    return null;
            }
            else
            {
                obj.DataDescriptor = ParseDataDescriptor(data);
                obj.ZIP64DataDescriptor = new DataDescriptor64();
                if (obj.DataDescriptor == null)
                    return null;
            }

            #endregion

            return obj;
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

#if NET20 || NET35
            bool utf8 = (obj.Flags & GeneralPurposeBitFlags.LanguageEncodingFlag) != 0;
#else
            bool utf8 = obj.Flags.HasFlag(GeneralPurposeBitFlags.LanguageEncodingFlag);
#endif

            if (obj.FileNameLength > 0 && data.Position + obj.FileNameLength <= data.Length)
            {
                byte[] filenameBytes = data.ReadBytes(obj.FileNameLength);
                if (filenameBytes.Length != obj.FileNameLength)
                    return null;

                if (utf8)
                    obj.FileName = Encoding.UTF8.GetString(filenameBytes);
                else
                    obj.FileName = Encoding.ASCII.GetString(filenameBytes);
            }
            if (obj.ExtraFieldLength > 0 && data.Position + obj.ExtraFieldLength <= data.Length)
            {
                byte[] extraBytes = data.ReadBytes(obj.ExtraFieldLength);
                if (extraBytes.Length != obj.ExtraFieldLength)
                    return null;

                obj.ExtraFields = ParseExtraFields(obj, extraBytes);
            }

            return obj;
        }

        #region Extras Fields

        /// <summary>
        /// Process all extensible data fields in a central directory file extras block
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Array of data fields on success, null otherwise</returns>
        public static ExtensibleDataField[]? ParseExtraFields(CentralDirectoryFileHeader header, byte[]? data)
        {
            if (data == null)
                return null;

            List<ExtensibleDataField> fields = [];

            int offset = 0;
            while (offset < data.Length)
            {
                // Peek at the next header ID
                HeaderID id = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
                offset -= 2;

                // Read based on the ID
                ExtensibleDataField? field = id switch
                {
                    HeaderID.Zip64ExtendedInformation => ParseZip64ExtendedInformationExtraField(data, ref offset, header),
                    HeaderID.AVInfo => ParseUnknownExtraField(data, ref offset), // TODO: Implement model
                    HeaderID.ExtendedLanguageEncodingData => ParseUnknownExtraField(data, ref offset), // TODO: Implement model
                    HeaderID.OS2 => ParseOS2ExtraField(data, ref offset),
                    HeaderID.NTFS => ParseNTFSExtraField(data, ref offset),
                    HeaderID.OpenVMS => ParseOpenVMSExtraField(data, ref offset),
                    HeaderID.UNIX => ParseUnixExtraField(data, ref offset),
                    HeaderID.FileStreamFork => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.PatchDescriptor => ParsePatchDescriptorExtraField(data, ref offset),
                    HeaderID.PKCSStore => ParsePKCS7Store(data, ref offset),
                    HeaderID.X509IndividualFile => ParseX509IndividualFile(data, ref offset),
                    HeaderID.X509CentralDirectory => ParseX509CentralDirectory(data, ref offset),
                    HeaderID.StrongEncryptionHeader => ParseStrongEncryptionHeader(data, ref offset),
                    HeaderID.RecordManagementControls => ParseRecordManagementControls(data, ref offset),
                    HeaderID.PKCSCertificateList => ParsePKCS7EncryptionRecipientCertificateList(data, ref offset),
                    HeaderID.Timestamp => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.PolicyDecryptionKey => ParsePolicyDecryptionKeyRecordExtraField(data, ref offset),
                    HeaderID.SmartcryptKeyProvider => ParseKeyProviderRecordExtraField(data, ref offset),
                    HeaderID.SmartcryptPolicyKeyData => ParsePolicyKeyDataRecordRecordExtraField(data, ref offset),
                    HeaderID.IBMS390AttributesUncompressed => ParseAS400ExtraFieldAttribute(data, ref offset),
                    HeaderID.IBMS390AttributesCompressed => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.POSZIP4690 => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.Macintosh => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.PixarUSD => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ZipItMacintosh => ParseZipItMacintoshExtraField(data, ref offset),
                    HeaderID.ZipItMacintosh135Plus => ParseZipItMacintoshShortFileExtraField(data, ref offset),
                    HeaderID.ZipItMacintosh135PlusAlt => ParseZipItMacintoshShortDirectoryExtraField(data, ref offset),
                    HeaderID.InfoZIPMacintosh => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.AcornSparkFS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.WindowsNTSecurityDescriptor => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.VMCMS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.MVS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.THEOSold => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.FWKCSMD5 => ParseFWKCSMD5ExtraField(data, ref offset),
                    HeaderID.OS2AccessControlList => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPOpenVMS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.MacintoshSmartzip => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.XceedOriginalLocation => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ADSVS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ExtendedTimestamp => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.XceedUnicode => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUNIX => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUnicodeComment => ParseInfoZIPUnicodeCommentExtraField(data, ref offset),
                    HeaderID.BeOSBeBox => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.THEOS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUnicodePath => ParseInfoZIPUnicodePathExtraField(data, ref offset),
                    HeaderID.AtheOSSyllable => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ASiUNIX => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUNIXNew => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUNIXNewer => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.DataStreamAlignment => ParseDataStreamAlignment(data, ref offset),
                    HeaderID.MicrosoftOpenPackagingGrowthHint => ParseMicrosoftOpenPackagingGrowthHint(data, ref offset),
                    HeaderID.JavaJAR => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.AndroidZIPAlignment => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.KoreanZIPCodePage => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.SMSQDOS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.AExEncryptionStructure => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.Unknown => ParseUnknownExtraField(data, ref offset), // TODO: Implement

                    _ => ParseUnknownExtraField(data, ref offset),
                };

                if (field != null)
                    fields.Add(field);
            }

            return [.. fields];
        }

        /// <summary>
        /// Process all extensible data fields in a local file extras block
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <returns>Array of data fields on success, null otherwise</returns>
        public static ExtensibleDataField[]? ParseExtraFields(LocalFileHeader header, byte[]? data)
        {
            if (data == null)
                return null;

            List<ExtensibleDataField> fields = [];

            int offset = 0;
            while (offset < data.Length)
            {
                // Peek at the next header ID
                HeaderID id = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
                offset -= 2;

                // Read based on the ID
                ExtensibleDataField? field = id switch
                {
                    HeaderID.Zip64ExtendedInformation => ParseZip64ExtendedInformationExtraField(data, ref offset, header),
                    HeaderID.AVInfo => ParseUnknownExtraField(data, ref offset), // TODO: Implement model
                    HeaderID.ExtendedLanguageEncodingData => ParseUnknownExtraField(data, ref offset), // TODO: Implement model
                    HeaderID.OS2 => ParseOS2ExtraField(data, ref offset),
                    HeaderID.NTFS => ParseNTFSExtraField(data, ref offset),
                    HeaderID.OpenVMS => ParseOpenVMSExtraField(data, ref offset),
                    HeaderID.UNIX => ParseUnixExtraField(data, ref offset),
                    HeaderID.FileStreamFork => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.PatchDescriptor => ParsePatchDescriptorExtraField(data, ref offset),
                    HeaderID.PKCSStore => ParsePKCS7Store(data, ref offset),
                    HeaderID.X509IndividualFile => ParseX509IndividualFile(data, ref offset),
                    HeaderID.X509CentralDirectory => ParseX509CentralDirectory(data, ref offset),
                    HeaderID.StrongEncryptionHeader => ParseStrongEncryptionHeader(data, ref offset),
                    HeaderID.RecordManagementControls => ParseRecordManagementControls(data, ref offset),
                    HeaderID.PKCSCertificateList => ParsePKCS7EncryptionRecipientCertificateList(data, ref offset),
                    HeaderID.Timestamp => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.PolicyDecryptionKey => ParsePolicyDecryptionKeyRecordExtraField(data, ref offset),
                    HeaderID.SmartcryptKeyProvider => ParseKeyProviderRecordExtraField(data, ref offset),
                    HeaderID.SmartcryptPolicyKeyData => ParsePolicyKeyDataRecordRecordExtraField(data, ref offset),
                    HeaderID.IBMS390AttributesUncompressed => ParseAS400ExtraFieldAttribute(data, ref offset),
                    HeaderID.IBMS390AttributesCompressed => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.POSZIP4690 => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.Macintosh => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.PixarUSD => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ZipItMacintosh => ParseZipItMacintoshExtraField(data, ref offset),
                    HeaderID.ZipItMacintosh135Plus => ParseZipItMacintoshShortFileExtraField(data, ref offset),
                    HeaderID.ZipItMacintosh135PlusAlt => ParseZipItMacintoshShortDirectoryExtraField(data, ref offset),
                    HeaderID.InfoZIPMacintosh => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.AcornSparkFS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.WindowsNTSecurityDescriptor => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.VMCMS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.MVS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.THEOSold => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.FWKCSMD5 => ParseFWKCSMD5ExtraField(data, ref offset),
                    HeaderID.OS2AccessControlList => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPOpenVMS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.MacintoshSmartzip => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.XceedOriginalLocation => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ADSVS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ExtendedTimestamp => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.XceedUnicode => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUNIX => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUnicodeComment => ParseInfoZIPUnicodeCommentExtraField(data, ref offset),
                    HeaderID.BeOSBeBox => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.THEOS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUnicodePath => ParseInfoZIPUnicodePathExtraField(data, ref offset),
                    HeaderID.AtheOSSyllable => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.ASiUNIX => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUNIXNew => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.InfoZIPUNIXNewer => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.DataStreamAlignment => ParseDataStreamAlignment(data, ref offset),
                    HeaderID.MicrosoftOpenPackagingGrowthHint => ParseMicrosoftOpenPackagingGrowthHint(data, ref offset),
                    HeaderID.JavaJAR => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.AndroidZIPAlignment => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.KoreanZIPCodePage => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.SMSQDOS => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.AExEncryptionStructure => ParseUnknownExtraField(data, ref offset), // TODO: Implement
                    HeaderID.Unknown => ParseUnknownExtraField(data, ref offset), // TODO: Implement

                    _ => ParseUnknownExtraField(data, ref offset),
                };

                if (field != null)
                    fields.Add(field);
            }

            return [.. fields];
        }

        /// <summary>
        /// Parse a Stream into an unknown extras field
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled unknown extras field on success, null on error</returns>
        private static UnknownExtraField? ParseUnknownExtraField(byte[] data, ref int offset)
        {
            var obj = new UnknownExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            if (obj.DataSize <= 0)            
                obj.Data = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Zip64ExtendedInformationExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Zip64ExtendedInformationExtraField on success, null on error</returns>
        private static Zip64ExtendedInformationExtraField? ParseZip64ExtendedInformationExtraField(byte[] data, ref int offset, CentralDirectoryFileHeader header)
        {
            var obj = new Zip64ExtendedInformationExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.Zip64ExtendedInformation)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);

            int bytesRemaining = obj.DataSize;
            if (header.UncompressedSize == uint.MaxValue)
            {
                obj.OriginalSize = data.ReadUInt64LittleEndian(ref offset);
                bytesRemaining -= 8;
            }

            if (header.CompressedSize == uint.MaxValue)
            {
                obj.CompressedSize = data.ReadUInt64LittleEndian(ref offset);
                bytesRemaining -= 8;
            }

            if (header.RelativeOffsetOfLocalHeader == uint.MaxValue)
            {
                obj.RelativeHeaderOffset = data.ReadUInt64LittleEndian(ref offset);
                bytesRemaining -= 8;
            }

            if (header.DiskNumberStart == ushort.MaxValue)
            {
                obj.DiskStartNumber = data.ReadUInt32LittleEndian(ref offset);
                bytesRemaining -= 4;
            }

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a Zip64ExtendedInformationExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled Zip64ExtendedInformationExtraField on success, null on error</returns>
        private static Zip64ExtendedInformationExtraField? ParseZip64ExtendedInformationExtraField(byte[] data, ref int offset, LocalFileHeader header)
        {
            var obj = new Zip64ExtendedInformationExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.Zip64ExtendedInformation)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);

            int bytesRemaining = obj.DataSize;
            if (header.UncompressedSize == uint.MaxValue)
            {
                obj.OriginalSize = data.ReadUInt64LittleEndian(ref offset);
                bytesRemaining -= 8;
            }

            if (header.CompressedSize == uint.MaxValue)
            {
                obj.CompressedSize = data.ReadUInt64LittleEndian(ref offset);
                bytesRemaining -= 8;
            }

            // TODO: These rely on values from the central directory
            if (bytesRemaining >= 8)
            {
                obj.RelativeHeaderOffset = data.ReadUInt64LittleEndian(ref offset);
                bytesRemaining -= 8;
            }

            if (bytesRemaining >= 4)
                obj.DiskStartNumber = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an OS2ExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled OS2ExtraField on success, null on error</returns>
        private static OS2ExtraField? ParseOS2ExtraField(byte[] data, ref int offset)
        {
            var obj = new OS2ExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.OS2)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.UncompressedBlockSize = data.ReadUInt32LittleEndian(ref offset);
            obj.CompressionType = data.ReadUInt16LittleEndian(ref offset);
            obj.CRC32 = data.ReadUInt32LittleEndian(ref offset);
            obj.Data = data.ReadBytes(ref offset, obj.DataSize - 10);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a NTFSExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled NTFSExtraField on success, null on error</returns>
        private static NTFSExtraField? ParseNTFSExtraField(byte[] data, ref int offset)
        {
            var obj = new NTFSExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.NTFS)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Reserved = data.ReadUInt32LittleEndian(ref offset);

            List<TagSizeVar> entries = [];

            int bytesRemaining = obj.DataSize - 4;
            while (bytesRemaining > 0)
            {
                var entry = new TagSizeVar();

                entry.Tag = data.ReadUInt16LittleEndian(ref offset);
                entry.Size = data.ReadUInt16LittleEndian(ref offset);
                entry.Var = data.ReadBytes(ref offset, entry.Size);

                entries.Add(entry);

                bytesRemaining -= 4 + entry.Size;
            }

            obj.TagSizeVars = [.. entries];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an OpenVMSExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled OpenVMSExtraField on success, null on error</returns>
        private static OpenVMSExtraField? ParseOpenVMSExtraField(byte[] data, ref int offset)
        {
            var obj = new OpenVMSExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.OpenVMS)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.CRC = data.ReadUInt32LittleEndian(ref offset);

            List<TagSizeVar> entries = [];

            int bytesRemaining = obj.DataSize - 4;
            while (bytesRemaining > 0)
            {
                var entry = new TagSizeVar();

                entry.Tag = data.ReadUInt16LittleEndian(ref offset);
                entry.Size = data.ReadUInt16LittleEndian(ref offset);
                entry.Var = data.ReadBytes(ref offset, entry.Size);

                entries.Add(entry);

                bytesRemaining -= 4 + entry.Size;
            }

            obj.TagSizeVars = [.. entries];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a UnixExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled UnixExtraField on success, null on error</returns>
        private static UnixExtraField? ParseUnixExtraField(byte[] data, ref int offset)
        {
            var obj = new UnixExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.UNIX)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.FileLastAccessTime = data.ReadUInt32LittleEndian(ref offset);
            obj.FileLastModificationTime = data.ReadUInt32LittleEndian(ref offset);
            obj.FileUserID = data.ReadUInt16LittleEndian(ref offset);
            obj.FileGroupID = data.ReadUInt16LittleEndian(ref offset);
            obj.Data = data.ReadBytes(ref offset, obj.DataSize - 12);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PatchDescriptorExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled PatchDescriptorExtraField on success, null on error</returns>
        private static PatchDescriptorExtraField? ParsePatchDescriptorExtraField(byte[] data, ref int offset)
        {
            var obj = new PatchDescriptorExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.PatchDescriptor)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Version = data.ReadUInt16LittleEndian(ref offset);
            obj.Flags = (ActionsReactions)data.ReadUInt32LittleEndian(ref offset);
            obj.OldSize = data.ReadUInt32LittleEndian(ref offset);
            obj.OldCRC = data.ReadUInt32LittleEndian(ref offset);
            obj.NewSize = data.ReadUInt32LittleEndian(ref offset);
            obj.NewCRC = data.ReadUInt32LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PKCS7Store
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled PKCS7Store on success, null on error</returns>
        private static PKCS7Store? ParsePKCS7Store(byte[] data, ref int offset)
        {
            var obj = new PKCS7Store();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.PKCSStore)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.TData = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a X509IndividualFile
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled X509IndividualFile on success, null on error</returns>
        private static X509IndividualFile? ParseX509IndividualFile(byte[] data, ref int offset)
        {
            var obj = new X509IndividualFile();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.X509IndividualFile)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.TData = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a X509CentralDirectory
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled X509CentralDirectory on success, null on error</returns>
        private static X509CentralDirectory? ParseX509CentralDirectory(byte[] data, ref int offset)
        {
            var obj = new X509CentralDirectory();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.X509CentralDirectory)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.TData = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a StrongEncryptionHeader
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled StrongEncryptionHeader on success, null on error</returns>
        private static StrongEncryptionHeader? ParseStrongEncryptionHeader(byte[] data, ref int offset)
        {
            var obj = new StrongEncryptionHeader();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.StrongEncryptionHeader)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Format = data.ReadUInt16LittleEndian(ref offset);
            obj.AlgID = data.ReadUInt16LittleEndian(ref offset);
            obj.Bitlen = data.ReadUInt16LittleEndian(ref offset);
            obj.Flags = data.ReadUInt16LittleEndian(ref offset);
            obj.CertData = data.ReadBytes(ref offset, obj.DataSize - 8);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an RecordManagementControls
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled RecordManagementControls on success, null on error</returns>
        private static RecordManagementControls? ParseRecordManagementControls(byte[] data, ref int offset)
        {
            var obj = new RecordManagementControls();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.RecordManagementControls)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);

            List<TagSizeVar> entries = [];

            int bytesRemaining = obj.DataSize - 4;
            while (bytesRemaining > 0)
            {
                var entry = new TagSizeVar();

                entry.Tag = data.ReadUInt16LittleEndian(ref offset);
                entry.Size = data.ReadUInt16LittleEndian(ref offset);
                entry.Var = data.ReadBytes(ref offset, entry.Size);

                entries.Add(entry);

                bytesRemaining -= 4 + entry.Size;
            }

            obj.TagSizeVars = [.. entries];

            return obj;
        }

        /// <summary>
        /// Parse a Stream into an PKCS7EncryptionRecipientCertificateList
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled PKCS7EncryptionRecipientCertificateList on success, null on error</returns>
        private static PKCS7EncryptionRecipientCertificateList? ParsePKCS7EncryptionRecipientCertificateList(byte[] data, ref int offset)
        {
            var obj = new PKCS7EncryptionRecipientCertificateList();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.PKCSCertificateList)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Version = data.ReadUInt16LittleEndian(ref offset);
            obj.CStore = data.ReadBytes(ref offset, obj.DataSize - 2);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PolicyDecryptionKeyRecordExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled PolicyDecryptionKeyRecordExtraField on success, null on error</returns>
        private static PolicyDecryptionKeyRecordExtraField? ParsePolicyDecryptionKeyRecordExtraField(byte[] data, ref int offset)
        {
            var obj = new PolicyDecryptionKeyRecordExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.PolicyDecryptionKey)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.TData = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a KeyProviderRecordExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled KeyProviderRecordExtraField on success, null on error</returns>
        private static KeyProviderRecordExtraField? ParseKeyProviderRecordExtraField(byte[] data, ref int offset)
        {
            var obj = new KeyProviderRecordExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.SmartcryptKeyProvider)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.TData = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a PolicyKeyDataRecordRecordExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled PolicyKeyDataRecordRecordExtraField on success, null on error</returns>
        private static PolicyKeyDataRecordRecordExtraField? ParsePolicyKeyDataRecordRecordExtraField(byte[] data, ref int offset)
        {
            var obj = new PolicyKeyDataRecordRecordExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.SmartcryptPolicyKeyData)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.TData = data.ReadBytes(ref offset, obj.DataSize);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a AS400ExtraFieldAttribute
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled AS400ExtraFieldAttribute on success, null on error</returns>
        /// <remarks>
        /// This header ID is shared with MVSExtraField, OS400ExtraField, and ZOSExtraFieldAttribute.
        /// This code makes an assumption that it's always AS400ExtraFieldAttribute.
        /// </remarks>
        private static AS400ExtraFieldAttribute? ParseAS400ExtraFieldAttribute(byte[] data, ref int offset)
        {
            var obj = new AS400ExtraFieldAttribute();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.IBMS390AttributesUncompressed)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.FieldLength = data.ReadUInt16BigEndian(ref offset);
            obj.FieldCode = (AS400ExtraFieldAttributeFieldCode)data.ReadUInt16LittleEndian(ref offset);
            obj.Data = data.ReadBytes(ref offset, obj.DataSize - 4);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ZipItMacintoshExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ZipItMacintoshExtraField on success, null on error</returns>
        private static ZipItMacintoshExtraField? ParseZipItMacintoshExtraField(byte[] data, ref int offset)
        {
            var obj = new ZipItMacintoshExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.ZipItMacintosh)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.ExtraFieldSignature = data.ReadUInt32LittleEndian(ref offset);
            obj.FnLen = data.ReadByteValue(ref offset);
            byte[] filenameBytes = data.ReadBytes(ref offset, obj.FnLen);
            obj.FileName = Encoding.ASCII.GetString(filenameBytes);
            obj.FileType = data.ReadBytes(ref offset, 4);
            obj.Creator = data.ReadBytes(ref offset, 4);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ZipItMacintoshShortFileExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ZipItMacintoshShortFileExtraField on success, null on error</returns>
        private static ZipItMacintoshShortFileExtraField? ParseZipItMacintoshShortFileExtraField(byte[] data, ref int offset)
        {
            var obj = new ZipItMacintoshShortFileExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.ZipItMacintosh135Plus)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.ExtraFieldSignature = data.ReadUInt32LittleEndian(ref offset);
            obj.FileType = data.ReadBytes(ref offset, 4);
            obj.Creator = data.ReadBytes(ref offset, 4);

            if (obj.DataSize > 12)
                obj.FdFlags = data.ReadUInt16LittleEndian(ref offset);

            if (obj.DataSize > 14)
                obj.Reserved = data.ReadUInt16LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a ZipItMacintoshShortDirectoryExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled ZipItMacintoshShortDirectoryExtraField on success, null on error</returns>
        private static ZipItMacintoshShortDirectoryExtraField? ParseZipItMacintoshShortDirectoryExtraField(byte[] data, ref int offset)
        {
            var obj = new ZipItMacintoshShortDirectoryExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.ZipItMacintosh135PlusAlt)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.ExtraFieldSignature = data.ReadUInt32LittleEndian(ref offset);

            if (obj.DataSize > 4)
                obj.FrFlags = data.ReadUInt16LittleEndian(ref offset);

            if (obj.DataSize > 6)
                obj.View = (ZipItInternalSettings)data.ReadUInt16LittleEndian(ref offset);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a FWKCSMD5ExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled FWKCSMD5ExtraField on success, null on error</returns>
        private static FWKCSMD5ExtraField? ParseFWKCSMD5ExtraField(byte[] data, ref int offset)
        {
            var obj = new FWKCSMD5ExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.FWKCSMD5)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Preface = data.ReadBytes(ref offset, 3);
            obj.MD5 = data.ReadBytes(ref offset, 16);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a InfoZIPUnicodeCommentExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled InfoZIPUnicodeCommentExtraField on success, null on error</returns>
        private static InfoZIPUnicodeCommentExtraField? ParseInfoZIPUnicodeCommentExtraField(byte[] data, ref int offset)
        {
            var obj = new InfoZIPUnicodeCommentExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.InfoZIPUnicodeComment)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Version = data.ReadByteValue(ref offset);
            obj.ComCRC32 = data.ReadUInt32LittleEndian(ref offset);
            byte[] unicodeBytes = data.ReadBytes(ref offset, obj.DataSize - 5);
            obj.UnicodeCom = Encoding.UTF8.GetString(unicodeBytes);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a InfoZIPUnicodePathExtraField
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled InfoZIPUnicodePathExtraField on success, null on error</returns>
        private static InfoZIPUnicodePathExtraField? ParseInfoZIPUnicodePathExtraField(byte[] data, ref int offset)
        {
            var obj = new InfoZIPUnicodePathExtraField();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.InfoZIPUnicodePath)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Version = data.ReadByteValue(ref offset);
            obj.NameCRC32 = data.ReadUInt32LittleEndian(ref offset);
            byte[] unicodeBytes = data.ReadBytes(ref offset, obj.DataSize - 5);
            obj.UnicodeName = Encoding.UTF8.GetString(unicodeBytes);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a DataStreamAlignment
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled DataStreamAlignment on success, null on error</returns>
        private static DataStreamAlignment? ParseDataStreamAlignment(byte[] data, ref int offset)
        {
            var obj = new DataStreamAlignment();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.DataStreamAlignment)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Alignment = data.ReadUInt16LittleEndian(ref offset);
            obj.Padding = data.ReadBytes(ref offset, obj.DataSize - 2);

            return obj;
        }

        /// <summary>
        /// Parse a Stream into a MicrosoftOpenPackagingGrowthHint
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled MicrosoftOpenPackagingGrowthHint on success, null on error</returns>
        private static MicrosoftOpenPackagingGrowthHint? ParseMicrosoftOpenPackagingGrowthHint(byte[] data, ref int offset)
        {
            var obj = new MicrosoftOpenPackagingGrowthHint();

            obj.HeaderID = (HeaderID)data.ReadUInt16LittleEndian(ref offset);
            if (obj.HeaderID != HeaderID.MicrosoftOpenPackagingGrowthHint)
                return null;

            obj.DataSize = data.ReadUInt16LittleEndian(ref offset);
            obj.Sig = data.ReadUInt16LittleEndian(ref offset);
            obj.PadVal = data.ReadUInt16LittleEndian(ref offset);
            obj.Padding = data.ReadBytes(ref offset, obj.DataSize - 2);

            return obj;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Determine if a data descriptor is 64-bit
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>True if the descriptor is 64-bit, false otherwise</returns>
        private static bool IsZip64Descriptor(Stream data)
        {
            // Last item in the stream
            if (data.Position + 12 == data.Length) // Short 32-bit
                return false;
            else if (data.Position + 16 == data.Length) // Long 32-bit
                return false;
            else if (data.Position + 20 == data.Length) // Short 64-bit
                return true;
            else if (data.Position + 24 == data.Length) // Long 64-bit
                return true;

            // Cache the current position
            long currentPosition = data.Position;

            // Short 32-bit
            data.SeekIfPossible(12, SeekOrigin.Current);
            byte[] nextBlock = data.ReadBytes(2);
            data.SeekIfPossible(currentPosition, SeekOrigin.Begin);
            if (nextBlock.EqualsExactly([0x50, 0x4B]))
                return false;

            // Long 32-bit
            data.SeekIfPossible(16, SeekOrigin.Current);
            nextBlock = data.ReadBytes(2);
            data.SeekIfPossible(currentPosition, SeekOrigin.Begin);
            if (nextBlock.EqualsExactly([0x50, 0x4B]))
                return false;

            // Assume 64-bit otherwise
            return true;
        }

        #endregion
    }
}
