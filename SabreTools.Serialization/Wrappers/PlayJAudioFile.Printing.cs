using System.Text;
using SabreTools.Data.Extensions;
using SabreTools.Data.Models.PlayJ;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PlayJAudioFile : IPrintable
    {
#if NETCOREAPP
        /// <inheritdoc/>
        public string ExportJSON() => System.Text.Json.JsonSerializer.Serialize(Model, _jsonSerializerOptions);
#endif

        /// <inheritdoc/>
        public void PrintInformation(StringBuilder builder)
            => Print(builder, Model);

        private static void Print(StringBuilder builder, AudioFile audio)
        {
            builder.AppendLine("PlayJ Audio File Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            Print(builder, audio.Header);
            Print(builder, audio.UnknownBlock1);

            if (audio.Header?.Version == 0x00000000)
            {
                Print(builder, audio.UnknownValue2);
                Print(builder, audio.UnknownBlock3);
            }
            else if (audio.Header?.Version == 0x0000000A)
            {
                Print(builder, audio.DataFilesCount, audio.DataFiles);
            }
        }

        private static void Print(StringBuilder builder, AudioHeader? header)
        {
            builder.AppendLine("  Audio Header Information:");
            builder.AppendLine("  -------------------------");
            if (header == null)
            {
                builder.AppendLine("  No audio header");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(header.Signature, "  Signature");
            builder.AppendLine(header.Version, "  Version");
            if (header.Version == 0x00000000 && header is AudioHeaderV1 headerV1)
            {
                builder.AppendLine(headerV1.TrackID, "  Track ID");
                builder.AppendLine(headerV1.UnknownOffset1, "  Unknown offset 1");
                builder.AppendLine(headerV1.UnknownOffset2, "  Unknown offset 2");
                builder.AppendLine(headerV1.UnknownOffset3, "  Unknown offset 3");
                builder.AppendLine(headerV1.Unknown1, "  Unknown 1");
                builder.AppendLine(headerV1.Unknown2, "  Unknown 2");
                builder.AppendLine(headerV1.Year, "  Year");
                builder.AppendLine(headerV1.TrackNumber, "  Track number");
                builder.AppendLine($"  Subgenre: {headerV1.Subgenre} (0x{headerV1.Subgenre:X})");
                builder.AppendLine(headerV1.Duration, "  Duration in seconds");
            }
            else if (header.Version == 0x0000000A && header is AudioHeaderV2 headerV2)
            {
                builder.AppendLine(headerV2.Unknown1, "  Unknown 1");
                builder.AppendLine(headerV2.Unknown2, "  Unknown 2");
                builder.AppendLine(headerV2.Unknown3, "  Unknown 3");
                builder.AppendLine(headerV2.Unknown4, "  Unknown 4");
                builder.AppendLine(headerV2.Unknown5, "  Unknown 5");
                builder.AppendLine(headerV2.Unknown6, "  Unknown 6");
                builder.AppendLine(headerV2.UnknownOffset1, "  Unknown Offset 1");
                builder.AppendLine(headerV2.Unknown7, "  Unknown 7");
                builder.AppendLine(headerV2.Unknown8, "  Unknown 8");
                builder.AppendLine(headerV2.Unknown9, "  Unknown 9");
                builder.AppendLine(headerV2.UnknownOffset2, "  Unknown Offset 2");
                builder.AppendLine(headerV2.Unknown10, "  Unknown 10");
                builder.AppendLine(headerV2.Unknown11, "  Unknown 11");
                builder.AppendLine(headerV2.Unknown12, "  Unknown 12");
                builder.AppendLine(headerV2.Unknown13, "  Unknown 13");
                builder.AppendLine(headerV2.Unknown14, "  Unknown 14");
                builder.AppendLine(headerV2.Unknown15, "  Unknown 15");
                builder.AppendLine(headerV2.Unknown16, "  Unknown 16");
                builder.AppendLine(headerV2.Unknown17, "  Unknown 17");
                builder.AppendLine(headerV2.TrackID, "  Track ID");
                builder.AppendLine(headerV2.Year, "  Year");
                builder.AppendLine(headerV2.TrackNumber, "  Track number");
                builder.AppendLine(headerV2.Unknown18, "  Unknown 18");
            }
            else
            {
                builder.AppendLine("  Unrecognized version, not parsed...");
            }

            builder.AppendLine(header.TrackLength, "  Track length");
            builder.AppendLine(header.Track, "  Track");
            builder.AppendLine(header.ArtistLength, "  Artist length");
            builder.AppendLine(header.Artist, "  Artist");
            builder.AppendLine(header.AlbumLength, "  Album length");
            builder.AppendLine(header.Album, "  Album");
            builder.AppendLine(header.WriterLength, "  Writer length");
            builder.AppendLine(header.Writer, "  Writer");
            builder.AppendLine(header.PublisherLength, "  Publisher length");
            builder.AppendLine(header.Publisher, "  Publisher");
            builder.AppendLine(header.LabelLength, "  Label length");
            builder.AppendLine(header.Label, "  Label");
            builder.AppendLine(header.CommentsLength, "  Comments length");
            builder.AppendLine(header.Comments, "  Comments");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, UnknownBlock1? block)
        {
            builder.AppendLine("  Unknown Block 1 Information:");
            builder.AppendLine("  -------------------------");
            if (block == null)
            {
                builder.AppendLine("  No unknown block 1r");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(block.Length, "  Length");
            builder.AppendLine(block.Data, "  Data");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, uint? value)
        {
            builder.AppendLine("  Unknown Value 2 Information:");
            builder.AppendLine("  -------------------------");
            if (value == null)
            {
                builder.AppendLine("  No unknown block 1r");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(value, "  Value");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, UnknownBlock3? block)
        {
            builder.AppendLine("  Unknown Block 3 Information:");
            builder.AppendLine("  -------------------------");
            if (block == null)
            {
                builder.AppendLine("  No unknown block 1r");
                builder.AppendLine();
                return;
            }

            builder.AppendLine(block.Data, "  Data");
            builder.AppendLine();
        }

        private static void Print(StringBuilder builder, uint count, DataFile[]? entries)
        {
            builder.AppendLine("  Data Files Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine(count, "  Data files count");
            if (count == 0 || entries == null || entries.Length == 0)
            {
                builder.AppendLine("  No data files");
                builder.AppendLine();
                return;
            }

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                builder.AppendLine($"  Data File {i}:");
                builder.AppendLine(entry.FileNameLength, "    File name length");
                builder.AppendLine(entry.FileName, "    File name");
                builder.AppendLine(entry.DataLength, "    Data length");
                builder.AppendLine(entry.Data, "    Data");
            }

            builder.AppendLine();
        }
    }
}
