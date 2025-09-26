using System.Text;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.XZP;

namespace SabreTools.Serialization.Printers
{
    public class XZP : IPrinter<File>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, File model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, File file)
        {
            builder.AppendLine("XZP Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            Print(builder, file.DirectoryEntries, "Directory");
            Print(builder, file.PreloadDirectoryEntries, "Preload Directory");
            Print(builder, file.PreloadDirectoryMappings);
            Print(builder, file.DirectoryItems);
            Print(builder, file.Footer);
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

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.PreloadDirectoryEntryCount, "  Preload directory entry count");
            builder.AppendLine(header.DirectoryEntryCount, "  Directory entry count");
            builder.AppendLine(header.PreloadBytes, "  Preload bytes");
            builder.AppendLine(header.HeaderLength, "  Header length");
            builder.AppendLine(header.DirectoryItemCount, "  Directory item count");
            builder.AppendLine(header.DirectoryItemOffset, "  Directory item offset");
            builder.AppendLine(header.DirectoryItemLength, "  Directory item length");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryEntry[]? entries, string prefix)
        {
            builder.AppendLine($"  {prefix} Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Directory Entry {i}");
                builder.AppendLine(entry.FileNameCRC, "    File name CRC");
                builder.AppendLine(entry.EntryLength, "    Entry length");
                builder.AppendLine(entry.EntryOffset, "    Entry offset");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryMapping[]? entries)
        {
            builder.AppendLine("  Preload Directory Mappings Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No preload directory mappings");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Directory Mapping {i}");
                builder.AppendLine(entry.PreloadDirectoryEntryIndex, "    Preload directory entry index");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryItem[]? entries)
        {
            builder.AppendLine("  Directory Items Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No directory items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Directory Item {i}");
                builder.AppendLine(entry.FileNameCRC, "    File name CRC");
                builder.AppendLine(entry.NameOffset, "    Name offset");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.TimeCreated, "    Time created");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Footer? footer)
        {
            builder.AppendLine("  Footer Information:");
            builder.AppendLine("  -------------------------");
            if (footer == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(footer.FileLength, "  File length");
            builder.AppendLine(footer.Signature, "  Signature");
            builder.AppendLine();
        }
    }
}
