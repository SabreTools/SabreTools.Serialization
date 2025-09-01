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
            builder.AppendLine($"  Compression method: {header.CompressionMethod} (0x{(byte)header.CompressionMethod:X2})");
            builder.AppendLine($"  Flags: {header.Flags} (0x{(byte)header.Flags:X2})");
            builder.AppendLine(header.LastModifiedTime, "  Last modified time");
            builder.AppendLine($"  Extra flags: {header.ExtraFlags} (0x{(byte)header.ExtraFlags:X2})");
            builder.AppendLine($"  Operating system: {header.OperatingSystem} (0x{(byte)header.OperatingSystem:X2})");
            builder.AppendLine(header.ExtraLength, "  Extra length");
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
                builder.AppendLine(entry.SubfieldID1, "    Subfield ID1");
                builder.AppendLine(entry.SubfieldID2, "    Subfield ID2");
                builder.AppendLine(entry.Length, "    Length");
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
            builder.AppendLine(trailer.InputSize, "  Input size");
            builder.AppendLine();
        }
    }
}
