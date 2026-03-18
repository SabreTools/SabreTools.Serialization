using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.StarForce;

namespace SabreTools.Serialization.Wrappers
{
    public partial class SFFS : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("StarForce File System Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
            Print(builder, Model.Files);
            Print(builder, Model.FileHeaders);
        }

        private static void Print(StringBuilder builder, Header header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Magic, "  Magic");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.FileCount, "  FileCount");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileEntry[] entries)
        {
            builder.AppendLine("  File Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
            {
                builder.AppendLine("  No file entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var file = entries[i];

                builder.AppendLine($"  File {i}");
                builder.AppendLine(file.FilenameMD5Hash, "    Filename MD5 hash");
                builder.AppendLine(file.FileHeaderIndex, "    File header index");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileHeader[] entries)
        {
            builder.AppendLine("  File Headers Information:");
            builder.AppendLine("  -------------------------");
            if (entries.Length == 0)
            {
                builder.AppendLine("  No file headers");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var file = entries[i];

                builder.AppendLine($"  File {i}");
                builder.AppendLine(file.FileContentStart, "    File content start");
                builder.AppendLine(file.FileInfo, "    File info");
            }

            builder.AppendLine();
        }
    }
}
