using System.Text;
using SabreTools.Data.Extensions;

namespace SabreTools.Serialization.Wrappers
{
    public partial class IRD : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("IRD Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(Model.Magic, "Magic", Encoding.ASCII);
            builder.AppendLine(Model.Version, "Version");
            builder.AppendLine(Model.TitleID, "Title ID");
            builder.AppendLine(Model.TitleLength, "Title length");
            builder.AppendLine(Model.Title, "Title");
            builder.AppendLine(Model.SystemVersion, "System version");
            builder.AppendLine(Model.GameVersion, "Game version");
            builder.AppendLine(Model.AppVersion, "App version");
            builder.AppendLine(Model.HeaderLength, "Header length");
            builder.AppendLine(Model.Header, "Header");
            builder.AppendLine(Model.FooterLength, "Footer length");
            builder.AppendLine(Model.Footer, "Footer");
            builder.AppendLine(Model.RegionCount, "Region count");
            if (Model.RegionCount != 0 && Model.RegionHashes != null && Model.RegionHashes.Length != 0)
            {
                for (int i = 0; i < Model.RegionCount; i++)
                {
                    builder.AppendLine(Model.RegionHashes[i], $"Region {i} hash");
                }
            }
            builder.AppendLine(Model.FileCount, "File count");
            for (int i = 0; i < Model.FileCount; i++)
            {
                if (Model.FileKeys != null)
                    builder.AppendLine(Model.FileKeys[i], $"File {i} key");
                if (Model.FileHashes != null)
                    builder.AppendLine(Model.FileHashes[i], $"File {i} hash");
            }
            builder.AppendLine(Model.ExtraConfig, "Extra config");
            builder.AppendLine(Model.Attachments, "Attachments");
            builder.AppendLine(Model.Data1Key, "Data 1 key");
            builder.AppendLine(Model.Data2Key, "Data 2 key");
            builder.AppendLine(Model.PIC, "PIC");
            builder.AppendLine(Model.UID, "UID");
            builder.AppendLine(Model.CRC, "CRC");
            builder.AppendLine();
        }
    }
}
