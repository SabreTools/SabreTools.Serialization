using System.Text;
using SabreTools.Data.Models.PIC;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Data.Printers
{
    public class PIC : IPrinter<DiscInformation>
    {
        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder, DiscInformation model)
            => Print(builder, model);

        public static void Print(StringBuilder builder, DiscInformation di)
        {
            builder.AppendLine("PIC Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine(di.DataStructureLength, "Data structure length");
            builder.AppendLine(di.Reserved0, "Reserved");
            builder.AppendLine(di.Reserved1, "Reserved");
            builder.AppendLine();

            Print(builder, di.Units);
        }

        private static void Print(StringBuilder builder, DiscInformationUnit[]? entries)
        {
            builder.AppendLine("  Disc Information Units:");
            builder.AppendLine("  -------------------------");
            if (entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No disc information units");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Disc Information Unit {i}");
                if (entry.Header == null)
                {
                    builder.AppendLine("    No header");
                }
                else
                {
                    var header = entry.Header;

                    builder.AppendLine(header.DiscInformationIdentifier, "    Disc information identifier");
                    builder.AppendLine(header.DiscInformationFormat, "    Disc information format");
                    builder.AppendLine(header.Reserved0, "    Reserved");
                    builder.AppendLine(header.SequenceNumber, "    Sequence number");
                    builder.AppendLine(header.BytesInUse, "    Bytes in use");
                    builder.AppendLine(header.Reserved1, "    Reserved");
                }

                if (entry.Body == null)
                {
                    builder.AppendLine("    No body");
                }
                else
                {
                    var body = entry.Body;

                    builder.AppendLine(body.DiscTypeIdentifier, "    Disc type identifer");
                    builder.AppendLine(body.DiscSizeClassVersion, "    Disc size class version");
                    builder.AppendLine(body.FormatDependentContents, "    Format-dependent contents");
                }

                if (entry.Trailer == null)
                {
                    builder.AppendLine("    No trailer");
                }
                else
                {
                    var trailer = entry.Trailer;

                    builder.AppendLine(trailer.DiscManufacturerID, "    Disc manufacturer ID");
                    builder.AppendLine(trailer.MediaTypeID, "    Media type ID");
                    builder.AppendLine(trailer.TimeStamp, "    Timestamp");
                    builder.AppendLine(trailer.ProductRevisionNumber, "    Product revision number");
                }
            }

            builder.AppendLine();
        }
    }
}
