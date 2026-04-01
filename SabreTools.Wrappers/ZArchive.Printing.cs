using System.Text;
using SabreTools.Data.Models.ZArchive;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class ZArchive : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("ZArchive Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, OffsetRecords);
            Print(builder, NameTable);
            Print(builder, FileTree);
            Print(builder, Footer);
        }

        public void Print(StringBuilder builder, OffsetRecord[] records)
        {
            builder.AppendLine("  Compression Offset Records:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine();
            if (records.Length == 0)
            {
                builder.AppendLine("  No compression offset records");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < records.Length; i++)
            {
                var record = records[i];

                builder.AppendLine(record.Offset, "    Base Offset");
                builder.AppendLine(record.Size, "    Block Sizes");
                builder.AppendLine();
            }
        }

        public void Print(StringBuilder builder, NameTable nameTable)
        {
            builder.AppendLine("  Name Table:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine();
            if (nameTable.NameEntries.Length != nameTable.NameTableOffsets.Length)
            {
                builder.AppendLine("  Mismatched Name Table entry count");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < nameTable.NameEntries.Length; i++)
            {
                var entry = nameTable.NameEntries[i];

                builder.AppendLine(nameTable.NameTableOffsets[i], "    Name Table Offset");
                if (entry.NodeLengthShort is not null)
                    builder.AppendLine(entry.NodeLengthShort, "    Name Length");
                else if (entry.NodeLengthLong is not null)
                    builder.AppendLine(entry.NodeLengthLong, "    Name Length");
                builder.AppendLine(Encoding.UTF8.GetString(entry.NodeName), "    Name");
                builder.AppendLine();
            }
        }

        public void Print(StringBuilder builder, FileDirectoryEntry[] fileTree)
        {
            builder.AppendLine("  File Tree:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine();
            if (fileTree.Length == 0)
            {
                builder.AppendLine("  No nodes in file tree");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < fileTree.Length; i++)
            {
                var node = fileTree[i];

                builder.AppendLine(node.NameOffsetAndTypeFlag, "    Base Offset");
                bool fileFlag = (node.NameOffsetAndTypeFlag & Constants.FileFlag) == Constants.FileFlag;
                builder.AppendLine(fileFlag, "    File Flag");
                builder.AppendLine(node.NameOffsetAndTypeFlag & Constants.RootNode, "    Name Table Offset");

                if (node is FileEntry fe)
                {
                    var fileOffset = ((ulong)fe.FileOffsetHigh << 32) | (ulong)fe.FileOffsetLow;
                    builder.AppendLine(fileOffset, "    File Offset");
                    var fileSize = ((ulong)fe.FileSizeHigh << 32) | (ulong)fe.FileSizeLow;
                    builder.AppendLine(fileSize, "    File Size");
                }
                else if (node is DirectoryEntry de)
                {
                    builder.AppendLine(de.NodeStartIndex, "    Node Start Index");
                    builder.AppendLine(de.Count, "    Count");
                    builder.AppendLine(de.Reserved, "    Reserved");
                }
                else
                {
                    builder.AppendLine("    Unknown Node");
                }

                builder.AppendLine();
            }
        }

        public void Print(StringBuilder builder, Footer footer)
        {
            builder.AppendLine("  Footer:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine();

            builder.AppendLine(footer.SectionCompressedData.Offset, "    Compressed Data Base Offset");
            builder.AppendLine(footer.SectionCompressedData.Size, "    Compressed Data Length");
            builder.AppendLine(footer.SectionOffsetRecords.Offset, "    Compression Offset Records Base Offset");
            builder.AppendLine(footer.SectionOffsetRecords.Size, "    Compression Offset Records Length");
            builder.AppendLine(footer.SectionNameTable.Offset, "    Name Table Base Offset");
            builder.AppendLine(footer.SectionNameTable.Size, "    Name Table Length");
            builder.AppendLine(footer.SectionFileTree.Offset, "    File Tree Base Offset");
            builder.AppendLine(footer.SectionFileTree.Size, "    File Tree Length");
            builder.AppendLine(footer.SectionMetaDirectory.Offset, "    Meta Directory Base Offset");
            builder.AppendLine(footer.SectionMetaDirectory.Size, "    Meta Directory Length");
            builder.AppendLine(footer.SectionMetaData.Offset, "    Meta Data Base Offset");
            builder.AppendLine(footer.SectionMetaData.Size, "    Meta Data Length");
            builder.AppendLine(footer.IntegrityHash, "    Integrity Hash");
            builder.AppendLine(footer.Size, "    Size");
            builder.AppendLine(footer.Version, "    Version");
            builder.AppendLine(footer.Magic, "    Magic");

            builder.AppendLine();
        }
    }
}
