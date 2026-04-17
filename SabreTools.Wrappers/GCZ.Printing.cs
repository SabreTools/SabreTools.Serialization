using System.Text;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class GCZ : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("GCZ Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Header.MagicCookie, "Magic Cookie");
            builder.AppendLine(Header.SubType, "Sub-Type");
            builder.AppendLine(Header.CompressedDataSize, "Compressed Data Size");
            builder.AppendLine(Header.DataSize, "Uncompressed Data Size");
            builder.AppendLine(Header.BlockSize, "Block Size");
            builder.AppendLine(Header.NumBlocks, "Block Count");
            builder.AppendLine();

            var discHeader = DiscHeader;
            if (discHeader is not null)
            {
                builder.AppendLine("Embedded Disc Header:");
                builder.AppendLine(discHeader.GameId, "  Game ID");
                builder.AppendLine(discHeader.MakerCode, "  Maker Code");
                builder.AppendLine(discHeader.DiscNumber, "  Disc Number");
                builder.AppendLine(discHeader.DiscVersion, "  Disc Version");
                builder.AppendLine(discHeader.GameTitle, "  Game Title");
                builder.AppendLine();
            }
        }
    }
}
