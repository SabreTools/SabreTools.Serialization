using System.Text;
using SabreTools.Serialization.Interfaces;
using SabreTools.Serialization.Models.LZ;

namespace SabreTools.Serialization.Printers
{
    public class LZSZDD : IPrinter<SZDDFile>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, SZDDFile model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, SZDDFile file)
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
