using System.Text;
using SabreTools.Data.Models.LZ;

namespace SabreTools.Data.Printers
{
    public class LZKWAJ : IPrinter<KWAJFile>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, KWAJFile model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, KWAJFile file)
        {
            builder.AppendLine("LZ-compressed File, KWAJ Variant Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
            Print(builder, file.HeaderExtensions);
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
