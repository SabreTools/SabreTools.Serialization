using System.Text;
using SabreTools.Data.Models.NintendoDisc;
using SabreTools.Data.Models.WIA;
using SabreTools.Text.Extensions;

namespace SabreTools.Wrappers
{
    public partial class WIA : IPrintable
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
            string formatName = IsRvz ? "RVZ" : "WIA";
            builder.AppendLine($"{formatName} Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Header1);
            Print(builder, Header2);
            Print(builder, DiscHeader);
            Print(builder, PartitionEntries);
            Print(builder, RawDataEntries);
        }

        private static void Print(StringBuilder builder, WiaHeader1 header)
        {
            builder.AppendLine("  Header 1:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Magic, "  Magic");
            builder.AppendLine(header.Version, "  Version");
            builder.AppendLine(header.VersionCompatible, "  Version Compatible");
            builder.AppendLine(header.Header2Size, "  Header 2 Size");
            builder.AppendLine(header.Header2Hash, "  Header 2 Hash");
            builder.AppendLine(header.IsoFileSize, "  ISO File Size");
            builder.AppendLine(header.WiaFileSize, "  WIA File Size");
            builder.AppendLine(header.Header1Hash, "  Header 1 Hash");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, WiaHeader2 header)
        {
            builder.AppendLine("  Header 2:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.DiscType.ToString(), "  Disc Type");
            builder.AppendLine(header.CompressionType.ToString(), "  Compression Type");
            builder.AppendLine(header.CompressionLevel, "  Compression Level");
            builder.AppendLine(header.ChunkSize, "  Chunk Size");
            builder.AppendLine(header.DiscHeader, "  Disc Header");
            builder.AppendLine(header.NumberOfPartitionEntries, "  Partition Entry Count");
            builder.AppendLine(header.PartitionEntrySize, "  Partition Entry Size");
            builder.AppendLine(header.PartitionEntriesOffset, "  Partition Entries Offset");
            builder.AppendLine(header.PartitionEntriesHash, "  Partition Entries Hash");
            builder.AppendLine(header.NumberOfRawDataEntries, "  Raw Data Entry Count");
            builder.AppendLine(header.RawDataEntriesOffset, "  Raw Data Entries Offset");
            builder.AppendLine(header.RawDataEntriesSize, "  Raw Data Entries Size");
            builder.AppendLine(header.NumberOfGroupEntries, "  Group Entry Count");
            builder.AppendLine(header.GroupEntriesOffset, "  Group Entries Offset");
            builder.AppendLine(header.GroupEntriesSize, "  Group Entries Size");
            builder.AppendLine(header.CompressorDataSize, "  Compressor Data Size");
            builder.AppendLine(header.CompressorData, "  Compressor Data");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, DiscHeader? header)
        {
            builder.AppendLine("  Embedded Disc Header:");
            builder.AppendLine("  -------------------------");
            if (header is null)
            {
                builder.AppendLine("  No embedded disc header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.GameId, "  Game ID");
            builder.AppendLine(header.DiscNumber, "  Disc Number");
            builder.AppendLine(header.DiscVersion, "  Disc Version");
            builder.AppendLine(header.GameTitle, "  Game Title");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, PartitionEntry[]? entries)
        {
            builder.AppendLine("  Partition Entry Table:");
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

                builder.AppendLine($"  Partition Table Entry {i}:");
                builder.AppendLine(entry.PartitionKey, "    Partition Key");
                builder.AppendLine(entry.DataEntry0.FirstSector, "    Data Entry 0 First Sector");
                builder.AppendLine(entry.DataEntry0.NumberOfSectors, "    Data Entry 0 Sector Count");
                builder.AppendLine(entry.DataEntry0.GroupIndex, "    Data Entry 0 Group Index");
                builder.AppendLine(entry.DataEntry0.NumberOfGroups, "    Data Entry 0 Group Count");
                builder.AppendLine(entry.DataEntry1.FirstSector, "    Data Entry 1 First Sector");
                builder.AppendLine(entry.DataEntry1.NumberOfSectors, "    Data Entry 1 Sector Count");
                builder.AppendLine(entry.DataEntry1.GroupIndex, "    Data Entry 1 Group Index");
                builder.AppendLine(entry.DataEntry1.NumberOfGroups, "    Data Entry 1 Group Count");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, RawDataEntry[]? entries)
        {
            builder.AppendLine("  Raw Data Entry Table:");
            builder.AppendLine("  -------------------------");
            if (entries is null || entries.Length == 0)
            {
                builder.AppendLine("  No raw data table entries");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Raw Data Table Entry {i}:");
                builder.AppendLine(entry.DataOffset, "    Data Offset");
                builder.AppendLine(entry.DataSize, "    Data Size");
                builder.AppendLine(entry.GroupIndex, "    Group Index");
                builder.AppendLine(entry.NumberOfGroups, "    Group Count");
            }

            builder.AppendLine();
        }
    }
}
