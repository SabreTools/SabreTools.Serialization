using System.Text;
using SabreTools.Data.Models.PFF;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Printers
{
    public class PFF : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("PFF Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
            Print(builder, archive.Segments);
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

            builder.AppendLine(header.HeaderSize, "  Header size");
            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.NumberOfFiles, "  Number of files");
            builder.AppendLine(header.FileSegmentSize, "  File segment size");
            builder.AppendLine(header.FileListOffset, "  File list offset");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, Segment[]? entries)
        {
            builder.AppendLine("  Segments Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No segments");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Segment {i}");
                builder.AppendLine(entry.Deleted, "    Deleted");
                builder.AppendLine(entry.FileLocation, "    File location");
                builder.AppendLine(entry.FileSize, "    File size");
                builder.AppendLine(entry.PackedDate, "    Packed date");
                builder.AppendLine(entry.FileName, "    File name");
                builder.AppendLine(entry.ModifiedDate, "    Modified date");
                builder.AppendLine(entry.CompressionLevel, "    Compression level");
            }

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

            builder.AppendLine(footer.SystemIP, "  System IP");
            builder.AppendLine(footer.Reserved, "  Reserved");
            builder.AppendLine(footer.KingTag, "  King tag");
            builder.AppendLine();
        }
    }
}
