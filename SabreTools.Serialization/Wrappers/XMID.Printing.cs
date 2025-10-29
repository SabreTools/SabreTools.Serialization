using System.Text;
using SabreTools.Data.Extensions;
using static SabreTools.Data.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XMID : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
            => Print(builder, Model);

        private static void Print(StringBuilder builder, Data.Models.Xbox.XMID xmid)
        {
            builder.AppendLine("Xbox Media Identifier Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(xmid.PublisherIdentifier, "Publisher identifier");
            if (!string.IsNullOrEmpty(xmid.PublisherIdentifier) && Publishers.ContainsKey(xmid.PublisherIdentifier ?? string.Empty))
                builder.AppendLine(Publishers[xmid.PublisherIdentifier ?? string.Empty], "Publisher");
            builder.AppendLine(xmid.GameID, "Game ID");
            builder.AppendLine(xmid.VersionNumber, "Version number");
            builder.AppendLine(xmid.RegionIdentifier, "Region identifier");
            if (Regions.ContainsKey(xmid.RegionIdentifier))
                builder.AppendLine(Regions[xmid.RegionIdentifier], "Region");
            builder.AppendLine();
        }
    }
}
