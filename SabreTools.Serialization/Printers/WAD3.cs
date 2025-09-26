using System.Text;
using SabreTools.Data.Models.WAD3;

namespace SabreTools.Data.Printers
{
    public class WAD3 : IPrinter<File>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, File model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, File file)
        {
            builder.AppendLine("WAD Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            Print(builder, file.DirEntries);
            Print(builder, file.FileEntries);
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
            builder.AppendLine(header.NumDirs, "  Number of directory entries");
            builder.AppendLine(header.DirOffset, "  Offset to first directory entry");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DirEntry[]? entries)
        {
            builder.AppendLine("  Directory Entries Information:");
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
                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.DiskLength, "    Disk length");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine($"    File type: {entry.Type} (0x{entry.Type:X})");
                builder.AppendLine(entry.Compression, "    Compression");
                builder.AppendLine(entry.Padding, "    Padding");
                builder.AppendLine(entry.Name, "    Name");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileEntry[]? entries)
        {
            builder.AppendLine("  File Entries Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  File Entry {i}");
                if (entry is MipTex mipTex)
                {
                    builder.AppendLine(mipTex.Name, "    Name");
                    builder.AppendLine(mipTex.Width, "    Width");
                    builder.AppendLine(mipTex.Height, "    Height");
                    builder.AppendLine(mipTex.MipOffsets, "    Mipmap Offsets");
                    builder.AppendLine("    Mipmap Images skipped...");
                    builder.AppendLine(mipTex.ColorsUsed, "    Colors used");
                    builder.AppendLine("    Palette skipped...");
                }
                else if (entry is QpicImage qpic)
                {
                    builder.AppendLine(qpic.Width, "    Width");
                    builder.AppendLine(qpic.Height, "    Height");
                    builder.AppendLine("    Image data skipped...");
                    builder.AppendLine(qpic.ColorsUsed, "    Colors used");
                    builder.AppendLine("    Palette skipped...");
                }
                else if (entry is Font font)
                {
                    builder.AppendLine(font.Width, "    Width");
                    builder.AppendLine(font.Height, "    Height");
                    builder.AppendLine(font.RowCount, "    Row count");
                    builder.AppendLine(font.RowHeight, "    Row height");
                    builder.AppendLine("    Font info skipped...");
                    builder.AppendLine("    Image data skipped...");
                    builder.AppendLine(font.ColorsUsed, "    Colors used");
                    builder.AppendLine("    Palette skipped...");
                }
                else
                {
                    builder.AppendLine("    Unrecognized entry type");
                }
            }

            builder.AppendLine();
        }
    }
}
