using System.Text;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class NintendoDisc : IPrintable
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
            builder.AppendLine($"{Platform} Disc Image Information:");
            builder.AppendLine("-------------------------");

            Print(builder, Header);
            Print(builder, PartitionTableEntries);
            Print(builder, RegionData);
        }

        private static void Print(StringBuilder builder, DiscHeader header)
        {
            builder.AppendLine("  Disc Header:");
            builder.AppendLine("  -------------------------");

            builder.AppendLine(header.GameId, "  Game ID");
            builder.AppendLine(header.DiscNumber, "  Disc Number");
            builder.AppendLine(header.DiscVersion, "  Disc Version");
            builder.AppendLine(header.AudioStreaming, "  Audio Streaming");
            builder.AppendLine(header.StreamingBufferSize, "  Streaming Buffer Size");
            builder.AppendLine(header.WiiMagic, "  Wii Magic");
            builder.AppendLine(header.GCMagic, "  GC Magic");
            builder.AppendLine(header.GameTitle, "  Game Title");
            builder.AppendLine(header.DisableHashVerification, "  Disable Hash Verification");
            builder.AppendLine(header.DisableDiscEncryption, "  Disable Disc Encryption");
            builder.AppendLine(header.DolOffset, "  DOL Offset");
            builder.AppendLine(header.FstOffset, "  FST Offset");
            builder.AppendLine(header.FstSize, "  FST Size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, WiiPartitionTableEntry[]? entries)
        {
            builder.AppendLine("  Partition Table:");
            builder.AppendLine("  -------------------------");
            if (entries is null || entries.Length == 0)
            {
                builder.AppendLine("  No partition table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Partition Table Entry {i}");
                builder.AppendLine(entry.Offset, "    Offset");
                builder.AppendLine(entry.Type, "    Type");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, WiiRegionData? regionData)
        {
            builder.AppendLine("  Region Data:");
            builder.AppendLine("  -------------------------");
            if (regionData is null)
            {
                builder.AppendLine("  No region data");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(regionData.RegionSetting, "  Region Setting");
            builder.AppendLine(regionData.AgeRatings, "  Age Ratings");
            builder.AppendLine();
        }
    }
}
