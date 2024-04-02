using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.AttractMode;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class AttractMode : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public Stream? Serialize(MetadataFile? obj)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8)
            {
                Separator = ';',
                Quotes = false,
                VerifyFieldCount = false,
            };

            // TODO: Include flag to write out long or short header
            // Write the short header
            writer.WriteString(Serialization.AttractMode.HeaderWithoutRomname); // TODO: Convert to array of values
            writer.WriteLine();

            // Write out the rows, if they exist
            WriteRows(obj.Row, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write rows information to the current writer
        /// </summary>
        /// <param name="rows">Array of Row objects representing the rows information</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteRows(Row?[]? rows, SeparatedValueWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (rows == null || !rows.Any())
                return;

            // Loop through and write out the rows
            foreach (var row in rows)
            {
                if (row == null)
                    continue;

                var rowArray = new string?[]
                {
                    row.Name,
                    row.Title,
                    row.Emulator,
                    row.CloneOf,
                    row.Year,
                    row.Manufacturer,
                    row.Category,
                    row.Players,
                    row.Rotation,
                    row.Control,
                    row.Status,
                    row.DisplayCount,
                    row.DisplayType,
                    row.AltRomname,
                    row.AltTitle,
                    row.Extra,
                    row.Buttons,
                };

                writer.WriteValues(rowArray);
                writer.Flush();
            }
        }
    }
}