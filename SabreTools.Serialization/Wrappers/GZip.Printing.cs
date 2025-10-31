using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.GZIP;

namespace SabreTools.Serialization.Wrappers
{
    public partial class GZip : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("gzip Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
            Print(builder, Model.Trailer);
        }

        private static void Print(StringBuilder builder, Header header)
        {
            builder.AppendLine("  Header:");
            builder.AppendLine("  -------------------------");
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

        private static void Print(StringBuilder builder, Trailer trailer)
        {
            builder.AppendLine("  Trailer:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(trailer.CRC32, "  CRC-32");
            builder.AppendLine(trailer.InputSize, "  Input size");
            builder.AppendLine();
        }
    }
}
