using System.Text;
using static SabreTools.Data.Models.Xbox.Constants;

namespace SabreTools.Data.Printers
{
    public class XeMID : IPrinter<Models.Xbox.XeMID>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Models.Xbox.XeMID model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Models.Xbox.XeMID xemid)
        {
            builder.AppendLine("Xbox Media Identifier Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(xemid.PublisherIdentifier, "Publisher identifier");
            if (!string.IsNullOrEmpty(xemid.PublisherIdentifier) && Publishers.ContainsKey(xemid.PublisherIdentifier ?? string.Empty))
                builder.AppendLine(Publishers[xemid.PublisherIdentifier ?? string.Empty], "Publisher");
            builder.AppendLine(xemid.PlatformIdentifier, "Platform identifier");
            builder.AppendLine(xemid.GameID, "Game ID");
            builder.AppendLine(xemid.SKU, "SKU");
            builder.AppendLine(xemid.RegionIdentifier, "Region identifier");
            if (Regions.ContainsKey(xemid.RegionIdentifier))
                builder.AppendLine(Regions[xemid.RegionIdentifier], "Region");
            builder.AppendLine(xemid.BaseVersion, "Base version");
            builder.AppendLine(xemid.MediaSubtypeIdentifier, "Media subtype identifier");
            if (MediaSubtypes.ContainsKey(xemid.MediaSubtypeIdentifier))
                builder.AppendLine(MediaSubtypes[xemid.MediaSubtypeIdentifier], "Media subtype");
            builder.AppendLine(xemid.DiscNumberIdentifier, "Disc number identifier");
            builder.AppendLine(xemid.CertificationSubmissionIdentifier, "Certification submission identifier");
            builder.AppendLine();
        }
    }
}
