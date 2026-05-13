using System.Text;
using SabreTools.Data.Models.GCZ;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class GCZ : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#else
        /// <inheritdoc/>
        public string ExportJSON() => Newtonsoft.Json.JsonConvert.SerializeObject(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("GCZ Information:");
            builder.AppendLine("-------------------------");

            Print(builder, Header);
            Print(builder, DiscHeader);
        }

        private static void Print(StringBuilder builder, GczHeader header)
        {
            builder.AppendLine("  Header:");
            builder.AppendLine("  -------------------------");

            builder.AppendLine(header.MagicCookie, "Magic Cookie");
            builder.AppendLine(header.SubType, "Sub-Type");
            builder.AppendLine(header.CompressedDataSize, "Compressed Data Size");
            builder.AppendLine(header.DataSize, "Uncompressed Data Size");
            builder.AppendLine(header.BlockSize, "Block Size");
            builder.AppendLine(header.NumBlocks, "Block Count");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DiscHeader? header)
        {
            builder.AppendLine("  Embedded Disc Header:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No embedded disc header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.GameId, "  Game ID");
            builder.AppendLine(header.DiscNumber, "  Disc Number");
            builder.AppendLine(header.DiscVersion, "  Disc Version");
            builder.AppendLine(header.GameTitle, "  Game Title");
            builder.AppendLine();
        }
    }
}
