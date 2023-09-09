using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.SeparatedValue;

namespace SabreTools.Serialization.Streams
{
    public partial class SeparatedValue : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public Stream Serialize(MetadataFile obj) => Serialize(obj, ',');
#else
        public Stream? Serialize(MetadataFile? obj) => Serialize(obj, ',');
#endif

        /// <inheritdoc cref="Serialize(MetadataFile)"/>
#if NET48
        public Stream Serialize(MetadataFile obj, char delim)
#else
        public Stream? Serialize(MetadataFile? obj, char delim)
#endif
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
#if NET48
            var headerArray = new string[]
#else
            var headerArray = new string?[]
#endif
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
#if NET48
                var rowArray = new string[]
#else
                var rowArray = new string?[]
#endif
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