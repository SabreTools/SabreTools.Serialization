using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.IRD;

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
            => Print(builder, Model);

        private static void Print(StringBuilder builder, File ird)
        {
            builder.AppendLine("IRD Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(ird.Magic, "Magic", Encoding.ASCII);
            builder.AppendLine(ird.Version, "Version");
            builder.AppendLine(ird.TitleID, "Title ID");
            builder.AppendLine(ird.TitleLength, "Title length");
            builder.AppendLine(ird.Title, "Title");
            builder.AppendLine(ird.SystemVersion, "System version");
            builder.AppendLine(ird.GameVersion, "Game version");
            builder.AppendLine(ird.AppVersion, "App version");
            builder.AppendLine(ird.HeaderLength, "Header length");
            builder.AppendLine(ird.Header, "Header");
            builder.AppendLine(ird.FooterLength, "Footer length");
            builder.AppendLine(ird.Footer, "Footer");
            builder.AppendLine(ird.RegionCount, "Region count");
            if (ird.RegionCount != 0 && ird.RegionHashes != null && ird.RegionHashes.Length != 0)
            {
                for (int i = 0; i < ird.RegionCount; i++)
                {
                    builder.AppendLine(ird.RegionHashes[i], $"Region {i} hash");
                }
            }
            builder.AppendLine(ird.FileCount, "File count");
            for (int i = 0; i < ird.FileCount; i++)
            {
                if (ird.FileKeys != null)
                    builder.AppendLine(ird.FileKeys[i], $"File {i} key");
                if (ird.FileHashes != null)
                    builder.AppendLine(ird.FileHashes[i], $"File {i} hash");
            }
            builder.AppendLine(ird.ExtraConfig, "Extra config");
            builder.AppendLine(ird.Attachments, "Attachments");
            builder.AppendLine(ird.Data1Key, "Data 1 key");
            builder.AppendLine(ird.Data2Key, "Data 2 key");
            builder.AppendLine(ird.PIC, "PIC");
            builder.AppendLine(ird.UID, "UID");
            builder.AppendLine(ird.CRC, "CRC");
            builder.AppendLine();
        }
    }
}
