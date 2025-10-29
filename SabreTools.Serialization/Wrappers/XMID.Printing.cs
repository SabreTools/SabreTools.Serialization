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
        {
            builder.AppendLine("Xbox Media Identifier Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.PublisherIdentifier, "Publisher identifier");
            if (!string.IsNullOrEmpty(Model.PublisherIdentifier) && Publishers.ContainsKey(Model.PublisherIdentifier ?? string.Empty))
                builder.AppendLine(Publishers[Model.PublisherIdentifier ?? string.Empty], "Publisher");
            builder.AppendLine(Model.GameID, "Game ID");
            builder.AppendLine(Model.VersionNumber, "Version number");
            builder.AppendLine(Model.RegionIdentifier, "Region identifier");
            if (Regions.ContainsKey(Model.RegionIdentifier))
                builder.AppendLine(Regions[Model.RegionIdentifier], "Region");
            builder.AppendLine();
        }
    }
}
