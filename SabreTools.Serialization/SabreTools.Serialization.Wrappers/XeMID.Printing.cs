using System.Text;
using SabreTools.Data.Extensions;
using static SabreTools.Data.Models.Xbox.Constants;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XeMID : IPrintable
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
            if (Publishers.TryGetValue(Model.PublisherIdentifier, out var publisher))
                builder.AppendLine(publisher, "Publisher");
            builder.AppendLine(Model.PlatformIdentifier, "Platform identifier");
            builder.AppendLine(Model.GameID, "Game ID");
            builder.AppendLine(Model.SKU, "SKU");
            builder.AppendLine(Model.RegionIdentifier, "Region identifier");
            if (Regions.TryGetValue(Model.RegionIdentifier, out var region))
                builder.AppendLine(region, "Region");
            builder.AppendLine(Model.BaseVersion, "Base version");
            builder.AppendLine(Model.MediaSubtypeIdentifier, "Media subtype identifier");
            if (MediaSubtypes.TryGetValue(Model.MediaSubtypeIdentifier, out var mediaSubtype))
                builder.AppendLine(mediaSubtype, "Media subtype");
            builder.AppendLine(Model.DiscNumberIdentifier, "Disc number identifier");
            builder.AppendLine(Model.CertificationSubmissionIdentifier, "Certification submission identifier");
            builder.AppendLine();
        }
    }
}
