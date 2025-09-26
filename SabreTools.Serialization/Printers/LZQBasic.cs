using System.Text;
using SabreTools.Data.Models.LZ;

namespace SabreTools.Data.Printers
{
    public class LZQBasic : IPrinter<QBasicFile>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, QBasicFile model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, QBasicFile file)
        {
            builder.AppendLine("LZ-compressed File, QBasic Variant Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, file.Header);
        }

        private static void Print(StringBuilder builder, QBasicHeader? header)
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
            builder.AppendLine(header.RealLength, "  Real length");
            builder.AppendLine();
        }
    }
}
