using System.Text;
using SabreTools.Models.PKZIP;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class PKZIP : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("PKZIP Archive (or Derived Format) Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.EndOfCentralDirectoryRecord);
            Print(builder, archive.ZIP64EndOfCentralDirectoryLocator);
            Print(builder, archive.ZIP64EndOfCentralDirectoryRecord);
            Print(builder, archive.CentralDirectoryHeaders);
            Print(builder, archive.ArchiveExtraDataRecord);
            Print(builder,
                archive.LocalFileHeaders,
                archive.EncryptionHeaders,
                archive.FileData,
                archive.DataDescriptors,
                archive.ZIP64DataDescriptors);
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
                builder.AppendLine(entry.ExtraField, "    Extra field");
                builder.AppendLine(entry.FileComment, "    File comment");
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

        private static void Print(StringBuilder builder,
            LocalFileHeader[]? localFileHeaders,
            byte[][]? encryptionHeaders,
            byte[][]? fileData,
            DataDescriptor[]? dataDescriptors,
            DataDescriptor64[]? zip64DataDescriptors)
        {
            builder.AppendLine("  Local File Information:");
            builder.AppendLine("  -------------------------");
            if (localFileHeaders == null || localFileHeaders.Length == 0)
            {
                builder.AppendLine("  No local files");
                builder.AppendLine();
                return;
            }

            if (encryptionHeaders == null || localFileHeaders.Length > encryptionHeaders.Length
                || fileData == null || localFileHeaders.Length > fileData.Length
                || dataDescriptors == null || localFileHeaders.Length > dataDescriptors.Length
                || zip64DataDescriptors == null || localFileHeaders.Length > zip64DataDescriptors.Length)
            {
                builder.AppendLine("  Mismatch in local file array values");
                builder.AppendLine();
            }

            for (int i = 0; i < localFileHeaders.Length; i++)
            {
                var localFileHeader = localFileHeaders[i];
                var encryptionHeader = encryptionHeaders != null && i < encryptionHeaders.Length ? encryptionHeaders[i] : null;
                var fileDatum = fileData != null && i < fileData.Length ? fileData[i] : null;
                var dataDescriptor = dataDescriptors != null && i < dataDescriptors.Length ? dataDescriptors[i] : null;
                var zip64DataDescriptor = zip64DataDescriptors != null && i < zip64DataDescriptors.Length ? zip64DataDescriptors[i] : null;

                Print(builder, localFileHeader, encryptionHeader, fileDatum, dataDescriptor, zip64DataDescriptor, i);
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder,
            LocalFileHeader localFileHeader,
            byte[]? encryptionHeader,
            byte[]? fileData,
            DataDescriptor? dataDescriptor,
            DataDescriptor64? zip64DataDescriptor,
            int index)
        {
            builder.AppendLine($"  Local File Entry {index}");
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
            builder.AppendLine(localFileHeader.ExtraField, "    [Local File Header] Extra field");

            if (encryptionHeader == null)
            {
                builder.AppendLine("    [Encryption Header]: [NULL]");
            }
            else
            {
                builder.AppendLine(encryptionHeader.Length, "    [Encryption Header] Length");
                builder.AppendLine(encryptionHeader, "    [Encryption Header] Data");
            }

            if (fileData == null)
            {
                builder.AppendLine("    [File Data]: [NULL]");
            }
            else
            {
                builder.AppendLine(fileData.Length, "    [File Data] Length");
                //builder.AppendLine(fileData, "    [File Data] Data");
            }

            if (dataDescriptor == null)
            {
                builder.AppendLine("    [Data Descriptor]: [NULL]");
            }
            else
            {
                builder.AppendLine(dataDescriptor.Signature, "    [Data Descriptor] Signature");
                builder.AppendLine(dataDescriptor.CRC32, "    [Data Descriptor] CRC-32");
                builder.AppendLine(dataDescriptor.CompressedSize, "    [Data Descriptor] Compressed size");
                builder.AppendLine(dataDescriptor.UncompressedSize, "    [Data Descriptor] Uncompressed size");
            }

            if (zip64DataDescriptor == null)
            {
                builder.AppendLine("    [ZIP64 Data Descriptor]: [NULL]");
            }
            else
            {
                builder.AppendLine(zip64DataDescriptor.Signature, "    [ZIP64 Data Descriptor] Signature");
                builder.AppendLine(zip64DataDescriptor.CRC32, "    [ZIP64 Data Descriptor] CRC-32");
                builder.AppendLine(zip64DataDescriptor.CompressedSize, "    [ZIP64 Data Descriptor] Compressed size");
                builder.AppendLine(zip64DataDescriptor.UncompressedSize, "    [ZIP64 Data Descriptor] Uncompressed size");
            }
        }
    }
}
