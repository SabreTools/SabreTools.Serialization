using System.Text;
using SabreTools.Serialization.Interfaces;
using static SabreTools.Serialization.Models.Xbox.Constants;

namespace SabreTools.Serialization.Printers
{
    public class XMID : IPrinter<Models.Xbox.XMID>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Models.Xbox.XMID model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Models.Xbox.XMID xmid)
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
