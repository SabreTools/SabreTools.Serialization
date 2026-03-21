using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.NES;

namespace SabreTools.Wrappers
{
    public partial class FDS : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("fwNES FDS File Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);

            //builder.AppendLine(Model.Data, "Disk Data");
            builder.AppendLine(Model.Data.Length, "Disk Data Length");
        }

        private static void Print(StringBuilder builder, FDSHeader? header)
        {
            builder.AppendLine("  FDS Header Information:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No header present");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.IdentificationString, "  Identification string");
            builder.AppendLine(header.DiskSides, "  Disk sides");
            builder.AppendLine(header.Padding, "  Padding");
            builder.AppendLine();
        }
    }
}
