using System.Text;
using SabreTools.Data.Models.Quantum;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Data.Printers
{
    public class Quantum : IPrinter<Archive>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, Archive model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, Archive archive)
        {
            builder.AppendLine("Quantum Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, archive.Header);
            Print(builder, archive.FileList);
            builder.AppendLine(archive.CompressedDataOffset, "  Compressed data offset");
            builder.AppendLine();
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
            builder.AppendLine(header.MajorVersion, "  Major version");
            builder.AppendLine(header.MinorVersion, "  Minor version");
            builder.AppendLine(header.FileCount, "  File count");
            builder.AppendLine(header.TableSize, "  Table size");
            builder.AppendLine(header.CompressionFlags, "  Compression flags");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, FileDescriptor[]? entries)
        {
            builder.AppendLine("  File List Information:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No file list items");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var fileDescriptor = entries[i];

                builder.AppendLine($"  File Descriptor {i}");
                builder.AppendLine(fileDescriptor.FileNameSize, "    File name size");
                builder.AppendLine(fileDescriptor.FileName, "    File name");
                builder.AppendLine(fileDescriptor.CommentFieldSize, "    Comment field size");
                builder.AppendLine(fileDescriptor.CommentField, "    Comment field");
                builder.AppendLine(fileDescriptor.ExpandedFileSize, "    Expanded file size");
                builder.AppendLine(fileDescriptor.FileTime, "    File time");
                builder.AppendLine(fileDescriptor.FileDate, "    File date");
                if (fileDescriptor.Unknown != null)
                    builder.AppendLine(fileDescriptor.Unknown.Value, "    Unknown (Checksum?)");
            }

            builder.AppendLine();
        }
    }
}
