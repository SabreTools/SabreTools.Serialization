using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZKWAJ : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("LZ-compressed File, KWAJ Variant Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
            Print(builder, Model.HeaderExtensions);
        }

        private static void Print(StringBuilder builder, KWAJHeader? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Magic, "  Magic number");
            builder.AppendLine($"  Compression type: {header.CompressionType} (0x{header.CompressionType:X})");
            builder.AppendLine(header.DataOffset, "  Data offset");
            builder.AppendLine($"  Header flags: {header.HeaderFlags} (0x{header.HeaderFlags:X})");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, KWAJHeaderExtensions? header)
        {
            builder.AppendLine("  Header Extensions Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header extensions");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.DecompressedLength, "  Decompressed length");
            builder.AppendLine(header.UnknownPurpose, "  Unknown purpose");
            builder.AppendLine(header.UnknownDataLength, "  Unknown data length");
            builder.AppendLine(header.UnknownData, "  Unknown data");
            builder.AppendLine(header.FileName, "  File name");
            builder.AppendLine(header.FileExtension, "  File extension");
            builder.AppendLine(header.ArbitraryTextLength, "  Arbitrary text length");
            builder.AppendLine(header.ArbitraryText, "  Arbitrary text");
            builder.AppendLine();
        }
    }
}
