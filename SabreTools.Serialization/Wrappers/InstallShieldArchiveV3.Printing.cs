using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.InstallShieldArchiveV3;

namespace SabreTools.Serialization.Wrappers
{
    public partial class InstallShieldArchiveV3 : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
            => Print(builder, Model);

        private static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("InstallShield Archive V3 Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
            Print(builder, archive.Directories);
            Print(builder, archive.Files);
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature1, "  Signature 1");
            builder.AppendLine(header.Signature2, "  Signature 2");
            builder.AppendLine(header.Reserved0, "  Reserved 0");
            builder.AppendLine(header.IsMultivolume, "  Is multivolume");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine(header.DateTime, "  Datetime");
            builder.AppendLine(header.CompressedSize, "  Compressed size");
            builder.AppendLine(header.UncompressedSize, "  Uncompressed size");
            builder.AppendLine(header.Reserved1, "  Reserved 1");
            builder.AppendLine(header.VolumeTotal, "  Volume total");
            builder.AppendLine(header.VolumeNumber, "  Volume number");
            builder.AppendLine(header.Reserved2, "  Reserved 2");
            builder.AppendLine(header.SplitBeginAddress, "  Split begin address");
            builder.AppendLine(header.SplitEndAddress, "  Split end address");
            builder.AppendLine(header.TocAddress, "  TOC address");
            builder.AppendLine(header.Reserved3, "  Reserved 3");
            builder.AppendLine(header.DirCount, "  Dir count");
            builder.AppendLine(header.Reserved4, "  Reserved 4");
            builder.AppendLine(header.Reserved5, "  Reserved 5");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Directory[]? entries)
        {
            builder.AppendLine("  Directories:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directories");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"    Directory {i}");
                builder.AppendLine(entry.FileCount, "    File count");
                builder.AppendLine(entry.ChunkSize, "    Chunk size");
                builder.AppendLine(entry.NameLength, "    Name length");
                builder.AppendLine(entry.Name, "    Name");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, File[]? entries)
        {
            builder.AppendLine("  Files:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"    File {i}");
                builder.AppendLine(entry.VolumeEnd, "    Volume end");
                builder.AppendLine(entry.Index, "    Index");
                builder.AppendLine(entry.UncompressedSize, "    Uncompressed size");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.DateTime, "    Datetime");
                builder.AppendLine(entry.Reserved0, "    Reserved 0");
                builder.AppendLine(entry.ChunkSize, "    Chunk size");
                builder.AppendLine($"  Attrib: {entry.Attrib} (0x{entry.Attrib:X})");
                builder.AppendLine(entry.IsSplit, "    Is split");
                builder.AppendLine(entry.Reserved1, "    Reserved 1");
                builder.AppendLine(entry.VolumeStart, "    Volume start");
                builder.AppendLine(entry.Name, "    Name");
            }

            builder.AppendLine();
        }
    }
}
