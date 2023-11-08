using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class SeparatedValue : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public Stream? Serialize(MetadataFile? obj) => Serialize(obj, ',');

        /// <inheritdoc cref="Serialize(MetadataFile)"/>
        public Stream? Serialize(MetadataFile? obj, char delim)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8) { Separator = delim, Quotes = true };

            // TODO: Include flag to write out long or short header
            // Write the short header
            WriteHeader(writer);

            // Write out the rows, if they exist
            WriteRows(obj.Row, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteHeader(SeparatedValueWriter writer)
        {
            var headerArray = new string?[]
            {
                "File Name",
                "Internal Name",
                "Description",
                "Game Name",
                "Game Description",
                "Type",
                "Rom Name",
                "Disk Name",
                "Size",
                "CRC",
                "MD5",
                "SHA1",
                "SHA256",
                //"SHA384",
                //"SHA512",
                //"SpamSum",
                "Status",
            };

            writer.WriteHeader(headerArray);
            writer.Flush();
        }

        /// <summary>
        /// Write rows information to the current writer
        /// </summary>
        /// <param name="rows">Array of Row objects representing the rows information</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        private static void WriteRows(Row[]? rows, SeparatedValueWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (rows == null || !rows.Any())
                return;

            // Loop through and write out the rows
            foreach (var row in rows)
            {
                var rowArray = new string?[]
                {
                    row.FileName,
                    row.InternalName,
                    row.Description,
                    row.GameName,
                    row.GameDescription,
                    row.Type,
                    row.RomName,
                    row.DiskName,
                    row.Size,
                    row.CRC,
                    row.MD5,
                    row.SHA1,
                    row.SHA256,
                    //row.SHA384,
                    //row.SHA512,
                    //row.SpamSum,
                    row.Status,
                };

                writer.WriteValues(rowArray);
                writer.Flush();
            }
        }
    }
}