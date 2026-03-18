using System.IO;
using System.Text;
using SabreTools.Data.Models.Listrom;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Writers
{
    public class Listrom : BaseBinaryWriter<MetadataFile>
    {
        /// <inheritdoc/>
        public override Stream? SerializeStream(MetadataFile? obj)
        {
            // If the metadata file is null
            if (obj is null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);

            // Write out the sets, if they exist
            WriteSets(obj.Set, writer);

            // Return the stream
            stream.SeekIfPossible(0, SeekOrigin.Begin);
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
            if (sets is null || sets.Length == 0)
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
            if (set is null)
                return;

            if (!string.IsNullOrEmpty(set.Driver))
            {
                if (set.Row is not null && set.Row.Length > 0)
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
            else if (!string.IsNullOrEmpty(set.Device))
            {
                if (set.Row is not null && set.Row.Length > 0)
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
            if (rows is null)
                return;

            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row.Name))
                    continue;

                var rowBuilder = new StringBuilder();

                int padding = 40 - (row.Size?.Length ?? 0);
                if (padding < row.Name!.Length)
                    padding = row.Name.Length + 2;

                rowBuilder.Append($"{row.Name.PadRight(padding, ' ')}");
                if (row.Size is not null)
                    rowBuilder.Append($"{row.Size} ");

                if (row.NoGoodDumpKnown)
                {
                    rowBuilder.Append("NO GOOD DUMP KNOWN");
                }
                else
                {
                    if (row.Bad)
                        rowBuilder.Append("BAD ");

                    if (row.Size is not null)
                    {
                        rowBuilder.Append($"CRC({row.CRC}) ");
                        rowBuilder.Append($"SHA1({row.SHA1}) ");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(row.MD5))
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
