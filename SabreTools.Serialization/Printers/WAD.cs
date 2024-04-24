using System.Text;
using SabreTools.Models.WAD;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class WAD : IPrinter<File>
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
            Print(builder, file.Lumps);
            Print(builder, file.LumpInfos);
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
            builder.AppendLine(header.LumpCount, "  Lump count");
            builder.AppendLine(header.LumpOffset, "  Lump offset");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Lump?[]? entries)
        {
            builder.AppendLine("  Lumps Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No lumps");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Lump {i}");
                if (entry == null)
                {
                    builder.AppendLine("    [NULL]");
                    continue;
                }

                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.DiskLength, "    Disk length");
                builder.AppendLine(entry.Length, "    Length");
                builder.AppendLine(entry.Type, "    Type");
                builder.AppendLine(entry.Compression, "    Compression");
                builder.AppendLine(entry.Padding0, "    Padding 0");
                builder.AppendLine(entry.Padding1, "    Padding 1");
                builder.AppendLine(entry.Name, "    Name");
            }
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, LumpInfo?[]? entries)
        {
            builder.AppendLine("  Lump Infos Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No lump infos");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Lump Info {i}");
                if (entry == null)
                {
                    builder.AppendLine("    Lump is compressed");
                    continue;
                }

                builder.AppendLine(entry.Name, "    Name");
                builder.AppendLine(entry.Width, "    Width");
                builder.AppendLine(entry.Height, "    Height");
                builder.AppendLine(entry.PixelOffset, "    Pixel offset");
                // TODO: Print unknown data?
                // TODO: Print pixel data?
                builder.AppendLine(entry.PaletteSize, "    Palette size");
                // TODO: Print palette data?
            }
            builder.AppendLine();
        }
    }
}