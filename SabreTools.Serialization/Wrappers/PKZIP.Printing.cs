using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.PKZIP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PKZIP : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("PKZIP Archive (or Derived Format) Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.LocalFiles);
            Print(builder, Model.EndOfCentralDirectoryRecord);
            Print(builder, Model.ZIP64EndOfCentralDirectoryLocator);
            Print(builder, Model.ZIP64EndOfCentralDirectoryRecord);
            Print(builder, Model.CentralDirectoryHeaders);
            Print(builder, Model.ArchiveExtraDataRecord);
        }

        private static void Print(StringBuilder builder, LocalFile[]? localFiles)
        {
            builder.AppendLine("  Local File Information:");
            builder.AppendLine("  -------------------------");
            if (localFiles == null || localFiles.Length == 0)
            {
                builder.AppendLine("  No local files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < localFiles.Length; i++)
            {
                var localFile = localFiles[i];
                Print(builder, localFile, i);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, LocalFile localFile, int index)
        {
            builder.AppendLine($"  Local File Entry {index}");

            #region Local File Header

            var localFileHeader = localFile.LocalFileHeader;
            builder.AppendLine(localFileHeader.Signature, "    [Local File Header] Signature");
            builder.AppendLine(localFileHeader.Version, "    [Local File Header] Version");
            builder.AppendLine($"    [Local File Header] Flags: {localFileHeader.Flags} (0x{localFileHeader.Flags:X})");
            builder.AppendLine($"    [Local File Header] Compression method: {localFileHeader.CompressionMethod} (0x{localFileHeader.CompressionMethod:X})");
            builder.AppendLine(localFileHeader.LastModifedFileTime, "    [Local File Header] Last modified file time"); // TODO: Parse from MS-DOS
            builder.AppendLine(localFileHeader.LastModifiedFileDate, "    [Local File Header] Last modified file date"); // TODO: Parse from MS-DOS
            builder.AppendLine(localFileHeader.CRC32, "    [Local File Header] CRC-32");
            builder.AppendLine(localFileHeader.CompressedSize, "    [Local File Header] Compressed size");
            builder.AppendLine(localFileHeader.UncompressedSize, "    [Local File Header] Uncompressed size");
            builder.AppendLine(localFileHeader.FileNameLength, "    [Local File Header] File name length");
            builder.AppendLine(localFileHeader.ExtraFieldLength, "    [Local File Header] Extra field length");
            builder.AppendLine(localFileHeader.FileName, "    [Local File Header] File name");
            Print(builder, "    [Local File Header] Extra Fields", localFileHeader.ExtraFields);

            #endregion

            #region Encryption Headers

            var encryptionHeaders = localFile.EncryptionHeaders;
            if (encryptionHeaders == null)
            {
                builder.AppendLine("    [Encryption Headers]: [NULL]");
            }
            else
            {
                builder.AppendLine(encryptionHeaders.Length, "    [Encryption Headers] Length");
                builder.AppendLine(encryptionHeaders, "    [Encryption Headers] Data");
            }

            #endregion

            #region File Data

            var fileData = localFile.FileData;
            if (fileData == null)
            {
                builder.AppendLine("    [File Data]: [NULL]");
            }
            else
            {
                builder.AppendLine(fileData.Length, "    [File Data] Length");
                //builder.AppendLine(fileData, "    [File Data] Data");
            }

            #endregion

            #region Data Descriptor

            var dataDescriptor = localFile.DataDescriptor;
            if (dataDescriptor == null)
            {
                builder.AppendLine("    [Data Descriptor]: [NULL]");
            }
            else
            {
                builder.AppendLine(dataDescriptor.Signature, "    [Data Descriptor] Signature");
                builder.AppendLine(dataDescriptor.CRC32, $"    [Data Descriptor] CRC-32");
                builder.AppendLine(dataDescriptor.CompressedSize, $"    [Data Descriptor] Compressed size");
                builder.AppendLine(dataDescriptor.UncompressedSize, $"    [Data Descriptor] Uncompressed size");
            }

            var zip64DataDescriptor = localFile.ZIP64DataDescriptor;
            if (zip64DataDescriptor == null)
            {
                builder.AppendLine("    [ZIP64 Data Descriptor]: [NULL]");
            }
            else
            {
                builder.AppendLine(zip64DataDescriptor.Signature, "    [ZIP64 Data Descriptor] Signature");
                builder.AppendLine(zip64DataDescriptor.CRC32, $"    [ZIP64 Data Descriptor] CRC-32");
                builder.AppendLine(zip64DataDescriptor.CompressedSize, $"    [ZIP64 Data Descriptor] Compressed size");
                builder.AppendLine(zip64DataDescriptor.UncompressedSize, $"    [ZIP64 Data Descriptor] Uncompressed size");
            }

            #endregion
        }

        private static void Print(StringBuilder builder, EndOfCentralDirectoryRecord? record)
        {
            builder.AppendLine("  End of Central Directory Record Information:");
            builder.AppendLine("  -------------------------");
            if (record == null)
            {
                builder.AppendLine("  No end of central directory record");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(record.Signature, "  Signature");
            builder.AppendLine(record.DiskNumber, "  Disk number");
            builder.AppendLine(record.StartDiskNumber, "  Start disk number");
            builder.AppendLine(record.TotalEntriesOnDisk, "  Total entries on disk");
            builder.AppendLine(record.TotalEntries, "  Total entries");
            builder.AppendLine(record.CentralDirectorySize, "  Central directory size");
            builder.AppendLine(record.CentralDirectoryOffset, "  Central directory offset");
            builder.AppendLine(record.FileCommentLength, "  File comment length");
            builder.AppendLine(record.FileComment, "  File comment");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, EndOfCentralDirectoryLocator64? locator)
        {
            builder.AppendLine("  ZIP64 End of Central Directory Locator Information:");
            builder.AppendLine("  -------------------------");
            if (locator == null)
            {
                builder.AppendLine("  No ZIP64 end of central directory locator");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(locator.Signature, "  Signature");
            builder.AppendLine(locator.StartDiskNumber, "  Start disk number");
            builder.AppendLine(locator.CentralDirectoryOffset, "  Central directory offset");
            builder.AppendLine(locator.TotalDisks, "  Total disks");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, EndOfCentralDirectoryRecord64? record)
        {
            builder.AppendLine("  ZIP64 End of Central Directory Record Information:");
            builder.AppendLine("  -------------------------");
            if (record == null)
            {
                builder.AppendLine("  No ZIP64 end of central directory record");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(record.Signature, "  Signature");
            builder.AppendLine(record.DirectoryRecordSize, "  Directory record size");
            builder.AppendLine($"  Host system: {record.HostSystem} (0x{record.HostSystem:X})");
            builder.AppendLine(record.VersionMadeBy, "  Version made by");
            builder.AppendLine(record.VersionNeededToExtract, "  Version needed to extract");
            builder.AppendLine(record.DiskNumber, "  Disk number");
            builder.AppendLine(record.StartDiskNumber, "  Start disk number");
            builder.AppendLine(record.TotalEntriesOnDisk, "  Total entries on disk");
            builder.AppendLine(record.TotalEntries, "  Total entries");
            builder.AppendLine(record.CentralDirectorySize, "  Central directory size");
            builder.AppendLine(record.CentralDirectoryOffset, "  Central directory offset");
            //builder.AppendLine(record.ExtensibleDataSector, "  Extensible data sector");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, CentralDirectoryFileHeader[]? entries)
        {
            builder.AppendLine("  Central Directory File Headers Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No central directory file headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Central Directory File Header Entry {i}");
                builder.AppendLine(entry.Signature, "    Signature");
                builder.AppendLine($"    Host system: {entry.HostSystem} (0x{entry.HostSystem:X})");
                builder.AppendLine(entry.VersionMadeBy, "    Version made by");
                builder.AppendLine(entry.VersionNeededToExtract, "    Version needed to extract");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
                builder.AppendLine($"    Compression method: {entry.CompressionMethod} (0x{entry.CompressionMethod:X})");
                builder.AppendLine(entry.LastModifedFileTime, "    Last modified file time"); // TODO: Parse from MS-DOS
                builder.AppendLine(entry.LastModifiedFileDate, "    Last modified file date"); // TODO: Parse from MS-DOS
                builder.AppendLine(entry.CRC32, "    CRC-32");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
                builder.AppendLine(entry.UncompressedSize, "    Uncompressed size");
                builder.AppendLine(entry.FileNameLength, "    File name length");
                builder.AppendLine(entry.ExtraFieldLength, "    Extra field length");
                builder.AppendLine(entry.FileCommentLength, "    File comment length");
                builder.AppendLine(entry.DiskNumberStart, "    Disk number start");
                builder.AppendLine($"    Internal file attributes: {entry.InternalFileAttributes} (0x{entry.InternalFileAttributes:X})");
                builder.AppendLine(entry.ExternalFileAttributes, "    External file attributes");
                builder.AppendLine(entry.RelativeOffsetOfLocalHeader, "    Relative offset of local header");
                builder.AppendLine(entry.FileName, "    File name");
                builder.AppendLine(entry.FileComment, "    File comment");
                Print(builder, "    Extra Fields", entry.ExtraFields);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ArchiveExtraDataRecord? record)
        {
            builder.AppendLine("  Archive Extra Data Record Information:");
            builder.AppendLine("  -------------------------");
            if (record == null)
            {
                builder.AppendLine("  No archive extra data record");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(record.Signature, "  Signature");
            builder.AppendLine(record.ExtraFieldLength, "  Extra field length");
            builder.AppendLine(record.ExtraFieldData, "  Extra field data");
            builder.AppendLine();
        }

        #region Extras Fields

        private static void Print(StringBuilder builder, string title, ExtensibleDataField[]? entries)
        {
            builder.AppendLine(title);
            builder.AppendLine("    -------------------------");

            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("    No extra fields");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"    Extra Field {i}:");
                builder.AppendLine($"      Header ID: {entry.HeaderID} (0x{(ushort)entry.HeaderID:X4})");
                builder.AppendLine(entry.DataSize, "      Data size");
                switch (entry)
                {
                    case Zip64ExtendedInformationExtraField field: Print(builder, field); break;
                    case OS2ExtraField field: Print(builder, field); break;
                    case NTFSExtraField field: Print(builder, field); break;
                    case OpenVMSExtraField field: Print(builder, field); break;
                    case UnixExtraField field: Print(builder, field); break;
                    case PatchDescriptorExtraField field: Print(builder, field); break;
                    case PKCS7Store field: Print(builder, field); break;
                    case X509IndividualFile field: Print(builder, field); break;
                    case X509CentralDirectory field: Print(builder, field); break;
                    case StrongEncryptionHeader field: Print(builder, field); break;
                    case RecordManagementControls field: Print(builder, field); break;
                    case PKCS7EncryptionRecipientCertificateList field: Print(builder, field); break;
                    case PolicyDecryptionKeyRecordExtraField field: Print(builder, field); break;
                    case KeyProviderRecordExtraField field: Print(builder, field); break;
                    case PolicyKeyDataRecordRecordExtraField field: Print(builder, field); break;
                    case AS400ExtraFieldAttribute field: Print(builder, field); break;
                    case ZipItMacintoshExtraField field: Print(builder, field); break;
                    case ZipItMacintoshShortFileExtraField field: Print(builder, field); break;
                    case ZipItMacintoshShortDirectoryExtraField field: Print(builder, field); break;
                    case FWKCSMD5ExtraField field: Print(builder, field); break;
                    case InfoZIPUnicodeCommentExtraField field: Print(builder, field); break;
                    case InfoZIPUnicodePathExtraField field: Print(builder, field); break;
                    case DataStreamAlignment field: Print(builder, field); break;
                    case MicrosoftOpenPackagingGrowthHint field: Print(builder, field); break;

                    case UnknownExtraField field: Print(builder, field); break;
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Zip64ExtendedInformationExtraField field)
        {
            builder.AppendLine(field.OriginalSize, "      Original size");
            builder.AppendLine(field.CompressedSize, "      Compressed size");
            builder.AppendLine(field.RelativeHeaderOffset, "      Relative header offset");
            builder.AppendLine(field.DiskStartNumber, "      Disk start number");
        }

        private static void Print(StringBuilder builder, OS2ExtraField field)
        {
            builder.AppendLine(field.UncompressedBlockSize, "      Uncompressed block size");
            builder.AppendLine(field.CompressionType, "      Compression type");
            builder.AppendLine(field.CRC32, "      CRC-32");
            builder.AppendLine(field.Data, "      Data");
        }

        private static void Print(StringBuilder builder, NTFSExtraField field)
        {
            builder.AppendLine(field.Reserved, "      Reserved");
            Print(builder, field.TagSizeVars);
        }

        private static void Print(StringBuilder builder, OpenVMSExtraField field)
        {
            builder.AppendLine(field.CRC, "      CRC-32");
            Print(builder, field.TagSizeVars);
        }

        private static void Print(StringBuilder builder, UnixExtraField field)
        {
            builder.AppendLine(field.FileLastAccessTime, "      File last access time");
            builder.AppendLine(field.FileLastModificationTime, "      File last modification time");
            builder.AppendLine(field.FileUserID, "      File user ID");
            builder.AppendLine(field.FileGroupID, "      File group ID");
            builder.AppendLine(field.Data, "      Data");
        }

        private static void Print(StringBuilder builder, PatchDescriptorExtraField field)
        {
            builder.AppendLine(field.Version, "      Version");
            builder.AppendLine($"      Flags: {field.Flags} (0x{field.Flags:X8})");
            builder.AppendLine(field.OldSize, "      Old size");
            builder.AppendLine(field.OldCRC, "      Old CRC-32");
            builder.AppendLine(field.NewSize, "      New size");
            builder.AppendLine(field.NewCRC, "      New CRC-32");
        }

        private static void Print(StringBuilder builder, PKCS7Store field)
        {
            builder.AppendLine(field.TData, "      Data");
        }

        private static void Print(StringBuilder builder, X509IndividualFile field)
        {
            builder.AppendLine(field.TData, "      Data");
        }

        private static void Print(StringBuilder builder, X509CentralDirectory field)
        {
            builder.AppendLine(field.TData, "      Data");
        }

        private static void Print(StringBuilder builder, StrongEncryptionHeader field)
        {
            builder.AppendLine(field.Format, "      Format");
            builder.AppendLine(field.AlgID, "      Algorithm ID");
            builder.AppendLine(field.Bitlen, "      Bit length");
            builder.AppendLine(field.Flags, "      Flags");
            builder.AppendLine(field.CertData, "      Data");
        }

        private static void Print(StringBuilder builder, RecordManagementControls field)
        {
            Print(builder, field.TagSizeVars);
        }

        private static void Print(StringBuilder builder, PKCS7EncryptionRecipientCertificateList field)
        {
            builder.AppendLine(field.Version, "      Version");
            builder.AppendLine(field.CStore, "      Data");
        }

        private static void Print(StringBuilder builder, PolicyDecryptionKeyRecordExtraField field)
        {
            builder.AppendLine(field.TData, "      Data");
        }

        private static void Print(StringBuilder builder, KeyProviderRecordExtraField field)
        {
            builder.AppendLine(field.TData, "      Data");
        }

        private static void Print(StringBuilder builder, PolicyKeyDataRecordRecordExtraField field)
        {
            builder.AppendLine(field.TData, "      Data");
        }

        private static void Print(StringBuilder builder, AS400ExtraFieldAttribute field)
        {
            builder.AppendLine(field.FieldLength, "      Field length");
            builder.AppendLine($"      Field code: {field.FieldCode} (0x{field.FieldCode:X8})");
            builder.AppendLine(field.Data, "      Data");
        }

        private static void Print(StringBuilder builder, ZipItMacintoshExtraField field)
        {
            builder.AppendLine(field.ExtraFieldSignature, "      Extra field signature");
            builder.AppendLine(field.FnLen, "      Filename length");
            builder.AppendLine(field.FileName, "      Filename");
            builder.AppendLine(field.FileType, "      File type");
            builder.AppendLine(field.Creator, "      Creator");
        }

        private static void Print(StringBuilder builder, ZipItMacintoshShortFileExtraField field)
        {
            builder.AppendLine(field.ExtraFieldSignature, "      Extra field signature");
            builder.AppendLine(field.FileType, "      File type");
            builder.AppendLine(field.Creator, "      Creator");
            builder.AppendLine(field.FdFlags, "      Flags");
            builder.AppendLine(field.Reserved, "      Reserved");
        }

        private static void Print(StringBuilder builder, ZipItMacintoshShortDirectoryExtraField field)
        {
            builder.AppendLine(field.ExtraFieldSignature, "      Extra field signature");
            builder.AppendLine(field.FrFlags, "      Flags");
            builder.AppendLine($"      Field code: {field.View} (0x{field.View:X8})");
        }

        private static void Print(StringBuilder builder, FWKCSMD5ExtraField field)
        {
            builder.AppendLine(field.Preface, "      Preface");
            builder.AppendLine(field.MD5, "      MD5");
        }

        private static void Print(StringBuilder builder, InfoZIPUnicodeCommentExtraField field)
        {
            builder.AppendLine(field.Version, "      Version");
            builder.AppendLine(field.ComCRC32, "      Comment CRC-32");
            builder.AppendLine(field.UnicodeCom, "      Unicode comment");
        }

        private static void Print(StringBuilder builder, InfoZIPUnicodePathExtraField field)
        {
            builder.AppendLine(field.Version, "      Version");
            builder.AppendLine(field.NameCRC32, "      Name CRC-32");
            builder.AppendLine(field.UnicodeName, "      Unicode name");
        }

        private static void Print(StringBuilder builder, DataStreamAlignment field)
        {
            builder.AppendLine(field.Alignment, "      Alignment");
            builder.AppendLine(field.Padding, "      Padding");
        }

        private static void Print(StringBuilder builder, MicrosoftOpenPackagingGrowthHint field)
        {
            builder.AppendLine(field.Sig, "      Signature");
            builder.AppendLine(field.PadVal, "      Initial padding value");
            builder.AppendLine(field.Padding, "      Padding");
        }

        private static void Print(StringBuilder builder, UnknownExtraField field)
        {
            builder.AppendLine(field.Data, "      Data");
        }

        private static void Print(StringBuilder builder, TagSizeVar[] tuples)
        {
            builder.AppendLine("      Tag/Size/Var Tuples:");
            builder.AppendLine("      -------------------------");
            if (tuples.Length == 0)
            {
                builder.AppendLine("      No tuples");
                return;
            }

            for (int i = 0; i < tuples.Length; i++)
            {
                var tuple = tuples[i];

                builder.AppendLine(tuple.Tag, "        Tags");
                builder.AppendLine(tuple.Size, "        Size");
                builder.AppendLine(tuple.Var, "        Vars");
            }
        }

        #endregion
    }
}
