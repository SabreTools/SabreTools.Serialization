using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.TAR;

namespace SabreTools.Serialization.Wrappers
{
    public partial class TapeArchive : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
            => Print(builder, Model);

        private static void Print(StringBuilder builder, Archive file)
        {
            builder.AppendLine("Tape Archive Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Entries);
        }

        private static void Print(StringBuilder builder, Entry[]? entries)
        {
            builder.AppendLine("  Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Entry {i}:");

                Print(builder, entry.Header);
                Print(builder, entry.Blocks);
            }
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("    Header:");
            builder.AppendLine("    -------------------------");

            if (header == null)
            {
                builder.AppendLine("    No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.FileName?.TrimEnd('\0'), "    File name");
            builder.AppendLine(header.Mode, "    Mode");
            builder.AppendLine(header.UID, "    UID");
            builder.AppendLine(header.GID, "    GID");
            builder.AppendLine(header.Size, "    Size");
            builder.AppendLine(header.ModifiedTime, "    Modified time");
            builder.AppendLine(header.Checksum, "    Checksum");
            builder.AppendLine($"    Type flag: {header.TypeFlag} (0x{(byte)header.TypeFlag:X2})");
            builder.AppendLine(header.LinkName?.TrimEnd('\0'), "    Link name");
            builder.AppendLine(header.Magic, "    Magic");
            builder.AppendLine(header.Version?.TrimEnd('\0'), "    Version");
            builder.AppendLine(header.UserName?.TrimEnd('\0'), "    User name");
            builder.AppendLine(header.GroupName?.TrimEnd('\0'), "    Group name");
            builder.AppendLine(header.DevMajor?.TrimEnd('\0'), "    Device major");
            builder.AppendLine(header.DevMinor?.TrimEnd('\0'), "    Device minor");
            builder.AppendLine(header.Prefix?.TrimEnd('\0'), "    Prefix");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Block[]? entries)
        {
            builder.AppendLine("    Blocks:");
            builder.AppendLine("    -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("    No blocks");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var block = entries[i];
                if (block.Data == null)
                    builder.AppendLine($"    Block {i} Length: 0");
                else
                    builder.AppendLine(block.Data.Length, $"    Block {i} Length");
            }

            builder.AppendLine();
        }
    }
}
