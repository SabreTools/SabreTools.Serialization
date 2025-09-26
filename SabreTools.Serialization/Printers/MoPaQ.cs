using System.Text;
using SabreTools.Data.Models.MoPaQ;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Data.Printers
{
    public class MoPaQ : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("MoPaQ Archive Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.UserData);
            Print(builder, archive.ArchiveHeader);
            Print(builder, archive.HetTable);
            Print(builder, archive.BetTable);
            Print(builder, archive.HashTable);
            Print(builder, archive.BlockTable);
            Print(builder, archive.HiBlockTable);
        }

        private static void Print(StringBuilder builder, UserData? userData)
        {
            builder.AppendLine("  User Data Information:");
            builder.AppendLine("  -------------------------");
            if (userData == null)
            {
                builder.AppendLine("  No user data");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(userData.Signature, "  Signature");
            builder.AppendLine(userData.UserDataSize, "  User data size");
            builder.AppendLine(userData.HeaderOffset, "  Header offset");
            builder.AppendLine(userData.UserDataHeaderSize, "  User data header size");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, ArchiveHeader? header)
        {
            builder.AppendLine("  Archive Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No archive header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.HeaderSize, "  Header size");
            builder.AppendLine(header.ArchiveSize, "  Archive size");
            builder.AppendLine($"  Format version: {header.FormatVersion} (0x{header.FormatVersion:X})");
            builder.AppendLine(header.BlockSize, "  Block size");
            builder.AppendLine(header.HashTablePosition, "  Hash table position");
            builder.AppendLine(header.BlockTablePosition, "  Block table position");
            builder.AppendLine(header.HashTableSize, "  Hash table size");
            builder.AppendLine(header.BlockTableSize, "  Block table size");
            builder.AppendLine(header.HiBlockTablePosition, "  Hi-block table position");
            builder.AppendLine(header.HashTablePositionHi, "  Hash table position hi");
            builder.AppendLine(header.BlockTablePositionHi, "  Block table position hi");
            builder.AppendLine(header.ArchiveSizeLong, "  Archive size long");
            builder.AppendLine(header.BetTablePosition, "  BET table position");
            builder.AppendLine(header.HetTablePosition, "  HET table position");
            builder.AppendLine(header.HashTableSizeLong, "  Hash table size long");
            builder.AppendLine(header.BlockTableSizeLong, "  Block table size long");
            builder.AppendLine(header.HiBlockTableSize, "  Hi-block table size");
            builder.AppendLine(header.HetTableSize, "  HET table size");
            builder.AppendLine(header.BetTableSize, "  BET table size");
            builder.AppendLine(header.RawChunkSize, "  Raw chunk size");
            builder.AppendLine(header.BlockTableMD5, "  Block table MD5");
            builder.AppendLine(header.HashTableMD5, "  Hash table MD5");
            builder.AppendLine(header.HiBlockTableMD5, "  Hi-block table MD5");
            builder.AppendLine(header.BetTableMD5, "  BET table MD5");
            builder.AppendLine(header.HetTableMD5, "  HET table MD5");
            builder.AppendLine(header.MpqHeaderMD5, "  MPQ header MD5");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, HetTable? table)
        {
            builder.AppendLine("  HET Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No HET table");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(table.Signature, "  Signature");
            builder.AppendLine(table.Version, "  Version");
            builder.AppendLine(table.DataSize, "  Data size");
            builder.AppendLine(table.TableSize, "  Table size");
            builder.AppendLine(table.MaxFileCount, "  Max file count");
            builder.AppendLine(table.HashTableSize, "  Hash table size");
            builder.AppendLine(table.HashEntrySize, "  Hash entry size");
            builder.AppendLine(table.TotalIndexSize, "  Total index size");
            builder.AppendLine(table.IndexSizeExtra, "  Index size extra");
            builder.AppendLine(table.IndexSize, "  Index size");
            builder.AppendLine(table.BlockTableSize, "  Block table size");
            builder.AppendLine(table.HashTable, "  Hash table");

            builder.AppendLine("  File indexes:");
            builder.AppendLine("  -------------------------");
            if (table.FileIndexes == null)
            {
                builder.AppendLine("  No file indexes ");
            }
            else
            {
                for (int i = 0; i < table.FileIndexes.Length; i++)
                {
                    builder.AppendLine(table.FileIndexes[i], $"    File index {i}");
                }
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, BetTable? table)
        {
            builder.AppendLine("  BET Table Information:");
            builder.AppendLine("  -------------------------");
            if (table == null)
            {
                builder.AppendLine("  No BET table");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(table.Signature, "  Signature");
            builder.AppendLine(table.Version, "  Version");
            builder.AppendLine(table.DataSize, "  Data size");
            builder.AppendLine(table.TableSize, "  Table size");
            builder.AppendLine(table.FileCount, "  File count");
            builder.AppendLine(table.Unknown, "  Unknown");
            builder.AppendLine(table.TableEntrySize, "  Table entry size");
            builder.AppendLine(table.FilePositionBitIndex, "  File position bit index");
            builder.AppendLine(table.FileSizeBitIndex, "  File size bit index");
            builder.AppendLine(table.CompressedSizeBitIndex, "  Compressed size bit index");
            builder.AppendLine(table.FlagIndexBitIndex, "  Flag index bit index");
            builder.AppendLine(table.UnknownBitIndex, "  Unknown bit index");
            builder.AppendLine(table.FilePositionBitCount, "  File position bit count");
            builder.AppendLine(table.FileSizeBitCount, "  File size bit count");
            builder.AppendLine(table.CompressedSizeBitCount, "  Compressed size bit count");
            builder.AppendLine(table.FlagIndexBitCount, "  Flag index bit count");
            builder.AppendLine(table.UnknownBitCount, "  Unknown bit count");
            builder.AppendLine(table.TotalBetHashSize, "  Total BET hash size");
            builder.AppendLine(table.BetHashSizeExtra, "  BET hash size extra");
            builder.AppendLine(table.BetHashSize, "  BET hash size");
            builder.AppendLine(table.BetHashArraySize, "  BET hash array size");
            builder.AppendLine(table.FlagCount, "  Flag count");
            builder.AppendLine(table.FlagsArray, "  Flags array");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, HashEntry[]? entries)
        {
            builder.AppendLine("  Hash Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No hash table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Hash Table Entry {i}");
                builder.AppendLine(entry.NameHashPartA, "    Name hash, part A");
                builder.AppendLine(entry.NameHashPartB, "    Name hash, part B");
                builder.AppendLine($"    Locale: {entry.Locale} (0x{entry.Locale:X})");
                builder.AppendLine(entry.Platform, "    Platform");
                builder.AppendLine(entry.BlockIndex, "    BlockIndex");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, BlockEntry[]? entries)
        {
            builder.AppendLine("  Block Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No block table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Block Table Entry {i}");
                builder.AppendLine(entry.FilePosition, "    File position");
                builder.AppendLine(entry.CompressedSize, "    Compressed size");
                builder.AppendLine(entry.UncompressedSize, "    Uncompressed size");
                builder.AppendLine($"    Flags: {entry.Flags} (0x{entry.Flags:X})");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, short[]? entries)
        {
            builder.AppendLine("  Hi-block Table Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No hi-block table items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                builder.AppendLine($"  Hi-block Table Entry {i}: {entry}");
            }

            builder.AppendLine();
        }
    }
}
