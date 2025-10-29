using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.BFPK;

namespace SabreTools.Serialization.Wrappers
{
    public partial class BFPK : IPrintable
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
            builder.AppendLine("BFPK Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
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

            builder.AppendLine(header.Magic, "  Magic");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.Files, "  Files");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileEntry[]? entries)
        {
            builder.AppendLine("  File Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file table items");
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  File Table Entry {i}");
                builder.AppendLine(entry.NameSize, "    Name size");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.UncompressedSize, "    Uncompressed size");
                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
            }
        }
    }
}
