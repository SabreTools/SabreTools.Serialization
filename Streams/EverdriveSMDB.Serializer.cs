using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class EverdriveSMDB : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(MetadataFile obj)
#else
        public Stream? Serialize(MetadataFile? obj)
#endif
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8) { Separator = '\t', Quotes = false };

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
#if NET48
        private static void WriteRows(Row[] rows, SeparatedValueWriter writer)
#else
        private static void WriteRows(Row[]? rows, SeparatedValueWriter writer)
#endif
        {
            // If the games information is missing, we can't do anything
            if (rows == null || !rows.Any())
                return;

            // Loop through and write out the rows
            foreach (var row in rows)
            {
                if (row == null)
                    continue;

#if NET48
                var rowArray = new List<string>
#else
                var rowArray = new List<string?>
#endif
                {
                    row.SHA256,
                    row.Name,
                    row.SHA1,
                    row.MD5,
                    row.CRC32,
                };

                if (row.Size != null)
                    rowArray.Add(row.Size);

                writer.WriteValues(rowArray.ToArray());
                writer.Flush();
            }
        }
    }
}