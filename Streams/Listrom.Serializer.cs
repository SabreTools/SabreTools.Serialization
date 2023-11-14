using System.IO;
using System.Linq;
using System.Text;
using SabreTools.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Listrom : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public Stream? Serialize(MetadataFile? obj)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);

            // Write out the sets, if they exist
            WriteSets(obj.Set, writer);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write sets information to the current writer
        /// </summary>
        /// <param name="sets">Array of Set objects representing the sets information</param>
        /// <param name="writer">StreamWriter representing the output</param>
        private static void WriteSets(Set[]? sets, StreamWriter writer)
        {
            // If the games information is missing, we can't do anything
            if (sets == null || !sets.Any())
                return;

            // Loop through and write out the games
            foreach (var set in sets)
            {
                WriteSet(set, writer);
                writer.Flush();
            }
        }

        /// <summary>
        /// Write set information to the current writer
        /// </summary>
        /// <param name="set">Set object representing the set information</param>
        /// <param name="writer">StreamWriter representing the output</param>
        private static void WriteSet(Set set, StreamWriter writer)
        {
            // If the set information is missing, we can't do anything
            if (set == null)
                return;

            if (!string.IsNullOrWhiteSpace(set.Driver))
            {
                if (set.Row != null && set.Row.Any())
                {
                    writer.WriteLine($"ROMs required for driver \"{set.Driver}\".");
                    writer.WriteLine("Name                                   Size Checksum");
                    writer.Flush();

                    WriteRows(set.Row, writer);

                    writer.WriteLine();
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine($"No ROMs required for driver \"{set.Driver}\".");
                    writer.WriteLine();
                    writer.Flush();
                }
            }
            else if (!string.IsNullOrWhiteSpace(set.Device))
            {
                if (set.Row != null && set.Row.Any())
                {
                    writer.WriteLine($"ROMs required for device \"{set.Device}\".");
                    writer.WriteLine("Name                                   Size Checksum");
                    writer.Flush();

                    WriteRows(set.Row, writer);

                    writer.WriteLine();
                    writer.Flush();
                }
                else
                {
                    writer.WriteLine($"No ROMs required for device \"{set.Device}\".");
                    writer.WriteLine();
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Write rows information to the current writer
        /// </summary>
        /// <param name="rows">Array of Row objects to write</param>
        /// <param name="writer">StreamWriter representing the output</param>
        private static void WriteRows(Row[]? rows, StreamWriter writer)
        {
            // If the array is missing, we can't do anything
            if (rows == null)
                return;

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.Name))
                    continue;

                var rowBuilder = new StringBuilder();

                int padding = 40 - (row.Size?.Length ?? 0);
                if (padding < row.Name!.Length)
                    padding = row.Name.Length + 2;

                rowBuilder.Append($"{row.Name.PadRight(padding, ' ')}");
                if (row.Size != null)
                    rowBuilder.Append($"{row.Size} ");

                if (row.NoGoodDumpKnown)
                {
                    rowBuilder.Append("NO GOOD DUMP KNOWN");
                }
                else
                {
                    if (row.Bad)
                        rowBuilder.Append("BAD ");

                    if (row.Size != null)
                    {
                        rowBuilder.Append($"CRC({row.CRC}) ");
                        rowBuilder.Append($"SHA1({row.SHA1}) ");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(row.MD5))
                            rowBuilder.Append($"MD5({row.MD5}) ");
                        else
                            rowBuilder.Append($"SHA1({row.SHA1}) ");
                    }

                    if (row.Bad)
                        rowBuilder.Append("BAD_DUMP");
                }

                writer.WriteLine(rowBuilder.ToString().TrimEnd());
                writer.Flush();
            }
        }
    }
}