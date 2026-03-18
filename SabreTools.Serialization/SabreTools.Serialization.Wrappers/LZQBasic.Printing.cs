using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.LZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class LZQBasic : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("LZ-compressed File, QBasic Variant Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
        }

        private static void Print(StringBuilder builder, QBasicHeader header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Magic, "  Magic number");
            builder.AppendLine(header.RealLength, "  Real length");
            builder.AppendLine();
        }
    }
}
