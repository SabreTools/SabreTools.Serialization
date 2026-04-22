using System.Text;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class NintendoDisc : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine($"{Platform} Disc Image Information:");
            builder.AppendLine("-------------------------");

            builder.AppendLine("Disc Header:");
            builder.AppendLine(Header.GameId, "  Game ID");
            builder.AppendLine(Header.MakerCode, "  Maker Code");
            builder.AppendLine(Header.DiscNumber, "  Disc Number");
            builder.AppendLine(Header.DiscVersion, "  Disc Version");
            builder.AppendLine(Header.AudioStreaming, "  Audio Streaming");
            builder.AppendLine(Header.StreamingBufferSize, "  Streaming Buffer Size");
            builder.AppendLine(Header.WiiMagic, "  Wii Magic");
            builder.AppendLine(Header.GCMagic, "  GC Magic");
            builder.AppendLine(Header.GameTitle, "  Game Title");
            builder.AppendLine(Header.DisableHashVerification, "  Disable Hash Verification");
            builder.AppendLine(Header.DisableDiscEncryption, "  Disable Disc Encryption");
            builder.AppendLine(Header.DolOffset, "  DOL Offset");
            builder.AppendLine(Header.FstOffset, "  FST Offset");
            builder.AppendLine(Header.FstSize, "  FST Size");
            builder.AppendLine();

            if (PartitionTableEntries is { Length: > 0 })
            {
                builder.AppendLine($"Partition Table ({PartitionTableEntries.Length} entries):");
                for (int i = 0; i < PartitionTableEntries.Length; i++)
                {
                    var pt = PartitionTableEntries[i];
                    builder.AppendLine($"  Partition {i}:");
                    builder.AppendLine(pt.Offset, "    Offset");
                    builder.AppendLine(pt.Type, "    Type");
                }

                builder.AppendLine();
            }

            if (RegionData is not null)
            {
                builder.AppendLine("Region Data:");
                builder.AppendLine(RegionData.RegionSetting, "  Region Setting");
                builder.AppendLine(RegionData.AgeRatings, "  Age Ratings");
                builder.AppendLine();
            }
        }
    }
}
