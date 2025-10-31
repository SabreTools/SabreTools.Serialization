using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.XZ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class XZ : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
        {
            builder.AppendLine("xz Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, Model.Header);
            Print(builder, Model.Blocks);
            Print(builder, Model.Index);
            Print(builder, Model.Footer);
        }

        private static void Print(StringBuilder builder, Header header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine($" Flags: {header.Flags} (0x{(ushort)header.Flags:X4})");
            builder.AppendLine(header.Crc32, "  CRC-32");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Block[] blocks)
        {
            builder.AppendLine("  Blocks Information:");
            builder.AppendLine("  -------------------------");
            if (blocks.Length == 0)
            {
                builder.AppendLine("  No blocks");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < blocks.Length; i++)
            {
                var block = blocks[i];

                builder.AppendLine($"  Block {i}:");
                builder.AppendLine(block.HeaderSize, "    Header size");
                builder.AppendLine($" Flags: {block.Flags} (0x{(byte)block.Flags:X2})");
                builder.AppendLine(block.CompressedSize, "    Compressed size");
                builder.AppendLine(block.UncompressedSize, "    Uncompressed size");
                // TODO: Print filter flags
                builder.AppendLine(block.HeaderPadding, "    Header padding");
                builder.AppendLine(block.Crc32, "    CRC-32");
                if (block.CompressedData == null)
                    builder.AppendLine("    Compressed data length: [NULL]");
                else
                    builder.AppendLine(block.CompressedData.Length, "    Compressed data length");
                builder.AppendLine(block.BlockPadding, "    Block padding");
                builder.AppendLine(block.Check, "    Check");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Index index)
        {
            builder.AppendLine("  Index Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(index.IndexIndicator, "  Index indicator");
            builder.AppendLine(index.NumberOfRecords, "  Number of records");
            Print(builder, index.Records);
            builder.AppendLine(index.Padding, "  Padding");
            builder.AppendLine(index.Crc32, "  CRC-32");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Record[] records)
        {
            builder.AppendLine("  Records Information:");
            builder.AppendLine("  -------------------------");
            if (records.Length == 0)
            {
                builder.AppendLine("  No records");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < records.Length; i++)
            {
                var record = records[i];

                builder.AppendLine($"  Block {i}:");
                builder.AppendLine(record.UnpaddedSize, "    Unpadded size");
                builder.AppendLine(record.UncompressedSize, "    Uncompressed size");
            }
        }

        private static void Print(StringBuilder builder, Footer footer)
        {
            builder.AppendLine("  Footer Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(footer.Crc32, "  CRC-32");
            builder.AppendLine(footer.BackwardSize, "  Backward size");
            builder.AppendLine($" Flags: {footer.Flags} (0x{(ushort)footer.Flags:X4})");
            builder.AppendLine(footer.Signature, "  Signature");
            builder.AppendLine();
        }
    }
}
