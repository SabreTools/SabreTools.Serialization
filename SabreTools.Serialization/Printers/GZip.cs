using System.Text;
using SabreTools.Models.GZIP;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class GZip : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive file)
        {
            builder.AppendLine("gzip Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            // TODO: Capture or print the compressed data
            Print(builder, file.Trailer);
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.ID1, "  ID1");
            builder.AppendLine(header.ID2, "  ID1");
            builder.AppendLine($"  Compression method: {header.CM} (0x{(byte)header.CM:X2})");
            builder.AppendLine($"  Flags: {header.FLG} (0x{(byte)header.FLG:X2})");
            builder.AppendLine(header.MTIME, "  Last modified time");
            builder.AppendLine($"  Extra flags: {header.XFL} (0x{(byte)header.XFL:X2})");
            builder.AppendLine($"  Operating system: {header.OS} (0x{(byte)header.OS:X2})");
            builder.AppendLine(header.XLEN, "  Extra length");
            Print(builder, header.ExtraField);
            builder.AppendLine(header.OriginalFileName, "  Original file name");
            builder.AppendLine(header.FileComment, "  File comment");
            builder.AppendLine(header.CRC16, "  CRC-16");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ExtraFieldData[]? entries)
        {
            builder.AppendLine("  Extra Fields:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No extra fields");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Extra Field {i}:");
                builder.AppendLine(entry.SI1, "    Subfield ID1");
                builder.AppendLine(entry.SI2, "    Subfield ID2");
                builder.AppendLine(entry.LEN, "    Length");
                builder.AppendLine(entry.Data, "    Data");
            }
        }

        private static void Print(StringBuilder builder, Trailer? trailer)
        {
            builder.AppendLine("  Trailer:");
            builder.AppendLine("  -------------------------");
            if (trailer == null)
            {
                builder.AppendLine("  No trailer");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(trailer.CRC32, "  CRC-32");
            builder.AppendLine(trailer.ISIZE, "  Input size");
            builder.AppendLine();
        }
    }
}
