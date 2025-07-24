using System.Text;
using SabreTools.Models.VPK;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class VPK : IPrinter<File>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, File model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, File file)
        {
            builder.AppendLine("VPK Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            Print(builder, file.ExtendedHeader);
            Print(builder, file.ArchiveHashes);
            Print(builder, file.DirectoryItems);
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
            builder.AppendLine(header.TreeSize, "  Tree size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ExtendedHeader? header)
        {
            builder.AppendLine("  Extended Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No extended header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.FileDataSectionSize, "  File data section size");
            builder.AppendLine(header.ArchiveMD5SectionSize, "  Archive MD5 section size");
            builder.AppendLine(header.OtherMD5SectionSize, "  Other MD5 section size");
            builder.AppendLine(header.SignatureSectionSize, "  Signature section size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ArchiveHash[]? entries)
        {
            builder.AppendLine("  Archive Hashes Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No archive hashes");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Archive Hash {i}");
                builder.AppendLine(entry.ArchiveIndex, "    Archive index");
                builder.AppendLine(entry.ArchiveOffset, "    Archive offset");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Hash, "    Hash");
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
                builder.AppendLine(entry.Extension, "    Extension");
                builder.AppendLine(entry.Path, "    Path");
                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine();

                Print(builder, entry.DirectoryEntry);
                // TODO: Print out preload data?
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirectoryEntry? entry)
        {
            builder.AppendLine("    Directory Entry:");
            builder.AppendLine("    -------------------------");
            if (entry == null)
            {
                builder.AppendLine("    [NULL]");
                return;
            }

            builder.AppendLine(entry.CRC, "    CRC");
            builder.AppendLine(entry.PreloadBytes, "    Preload bytes");
            builder.AppendLine(entry.ArchiveIndex, "    Archive index");
            builder.AppendLine(entry.EntryOffset, "    Entry offset");
            builder.AppendLine(entry.EntryLength, "    Entry length");
            builder.AppendLine(entry.Dummy0, "    Dummy 0");
        }
    }
}
