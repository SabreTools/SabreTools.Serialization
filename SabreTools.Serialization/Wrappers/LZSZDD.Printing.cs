using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZSZDD : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
            => Print(builder, Model);

        private static void Print(StringBuilder builder, SZDDFile file)
        {
            builder.AppendLine("LZ-compressed File, SZDD Variant Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
        }

        private static void Print(StringBuilder builder, SZDDHeader? header)
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
            builder.AppendLine(header.LastChar, "  Last char");
            builder.AppendLine(header.RealLength, "  Real length");
            builder.AppendLine();
        }
    }
}
