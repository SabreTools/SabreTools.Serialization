using System.Text;
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

            builder.AppendLine("Header 1:");
            builder.AppendLine(Header1.Magic, "  Magic");
            builder.AppendLine(Header1.Version, "  Version");
            builder.AppendLine(Header1.VersionCompatible, "  Version Compatible");
            builder.AppendLine(Header1.Header2Size, "  Header 2 Size");
            builder.AppendLine(Header1.Header2Hash, "  Header 2 Hash");
            builder.AppendLine(Header1.IsoFileSize, "  ISO File Size");
            builder.AppendLine(Header1.WiaFileSize, "  WIA File Size");
            builder.AppendLine(Header1.Header1Hash, "  Header 1 Hash");
            builder.AppendLine();

            builder.AppendLine("Header 2:");
            builder.AppendLine(Header2.DiscType.ToString(), "  Disc Type");
            builder.AppendLine(Header2.CompressionType.ToString(), "  Compression Type");
            builder.AppendLine(Header2.CompressionLevel, "  Compression Level");
            builder.AppendLine(Header2.ChunkSize, "  Chunk Size");
            builder.AppendLine(Header2.DiscHeader, "  Disc Header");
            builder.AppendLine(Header2.NumberOfPartitionEntries, "  Partition Entry Count");
            builder.AppendLine(Header2.PartitionEntrySize, "  Partition Entry Size");
            builder.AppendLine(Header2.PartitionEntriesOffset, "  Partition Entries Offset");
            builder.AppendLine(Header2.PartitionEntriesHash, "  Partition Entries Hash");
            builder.AppendLine(Header2.NumberOfRawDataEntries, "  Raw Data Entry Count");
            builder.AppendLine(Header2.RawDataEntriesOffset, "  Raw Data Entries Offset");
            builder.AppendLine(Header2.RawDataEntriesSize, "  Raw Data Entries Size");
            builder.AppendLine(Header2.NumberOfGroupEntries, "  Group Entry Count");
            builder.AppendLine(Header2.GroupEntriesOffset, "  Group Entries Offset");
            builder.AppendLine(Header2.GroupEntriesSize, "  Group Entries Size");
            builder.AppendLine(Header2.CompressorDataSize, "  Compressor Data Size");
            builder.AppendLine(Header2.CompressorData, "  Compressor Data");
            builder.AppendLine();

            var discHeader = DiscHeader;
            if (discHeader is not null)
            {
                builder.AppendLine("Embedded Disc Header:");
                builder.AppendLine(discHeader.GameId, "  Game ID");
                builder.AppendLine(discHeader.MakerCode, "  Maker Code");
                builder.AppendLine(discHeader.DiscNumber, "  Disc Number");
                builder.AppendLine(discHeader.DiscVersion, "  Disc Version");
                builder.AppendLine(discHeader.GameTitle, "  Game Title");
                builder.AppendLine();
            }

            if (PartitionEntries is { Length: > 0 })
            {
                builder.AppendLine($"Partition Entries ({PartitionEntries.Length}):");
                for (int i = 0; i < PartitionEntries.Length; i++)
                {
                    var pe = PartitionEntries[i];
                    builder.AppendLine($"  Partition {i}:");
                    builder.AppendLine(pe.PartitionKey, "    Partition Key");
                    builder.AppendLine(pe.DataEntry0.FirstSector, "    Data Entry 0 First Sector");
                    builder.AppendLine(pe.DataEntry0.NumberOfSectors, "    Data Entry 0 Sector Count");
                    builder.AppendLine(pe.DataEntry0.GroupIndex, "    Data Entry 0 Group Index");
                    builder.AppendLine(pe.DataEntry0.NumberOfGroups, "    Data Entry 0 Group Count");
                    builder.AppendLine(pe.DataEntry1.FirstSector, "    Data Entry 1 First Sector");
                    builder.AppendLine(pe.DataEntry1.NumberOfSectors, "    Data Entry 1 Sector Count");
                    builder.AppendLine(pe.DataEntry1.GroupIndex, "    Data Entry 1 Group Index");
                    builder.AppendLine(pe.DataEntry1.NumberOfGroups, "    Data Entry 1 Group Count");
                }

                builder.AppendLine();
            }

            if (RawDataEntries is { Length: > 0 })
            {
                builder.AppendLine($"Raw Data Entries ({RawDataEntries.Length}):");
                for (int i = 0; i < RawDataEntries.Length; i++)
                {
                    var rde = RawDataEntries[i];
                    builder.AppendLine($"  Raw Data Entry {i}:");
                    builder.AppendLine(rde.DataOffset, "    Data Offset");
                    builder.AppendLine(rde.DataSize, "    Data Size");
                    builder.AppendLine(rde.GroupIndex, "    Group Index");
                    builder.AppendLine(rde.NumberOfGroups, "    Group Count");
                }

                builder.AppendLine();
            }
        }
    }
}
