using System.Text;
using SabreTools.Data.Models.XZ;

namespace SabreTools.Data.Printers
{
    public class XZ : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("xz Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
            Print(builder, archive.Blocks);
            Print(builder, archive.Index);
            Print(builder, archive.Footer);
        }

        private static void Print(StringBuilder builder, Header? header)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine($" Flags: {header.Flags} (0x{(ushort)header.Flags:X4})");
            builder.AppendLine(header.Crc32, "  CRC-32");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Block[]? blocks)
        {
            builder.AppendLine("  Blocks Information:");
            builder.AppendLine("  -------------------------");
            if (blocks == null || blocks.Length == 0)
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
                // TODO: Print the actual compressed data length
                builder.AppendLine(block.BlockPadding, "    Block padding");
                builder.AppendLine(block.Check, "    Check");
            }

            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Index? index)
        {
            builder.AppendLine("  Index Information:");
            builder.AppendLine("  -------------------------");
            if (index == null)
            {
                builder.AppendLine("  No index");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(index.IndexIndicator, "  Index indicator");
            builder.AppendLine(index.NumberOfRecords, "  Number of records");
            // TODO: Print records
            builder.AppendLine(index.Padding, "  Padding");
            builder.AppendLine(index.Crc32, "  CRC-32");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Footer? footer)
        {
            builder.AppendLine("  Footer Information:");
            builder.AppendLine("  -------------------------");
            if (footer == null)
            {
                builder.AppendLine("  No footer");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(footer.Crc32, "  CRC-32");
            builder.AppendLine(footer.BackwardSize, "  Backward size");
            builder.AppendLine($" Flags: {footer.Flags} (0x{(ushort)footer.Flags:X4})");
            builder.AppendLine(footer.Signature, "  Signature");
            builder.AppendLine();
        }
    }
}
