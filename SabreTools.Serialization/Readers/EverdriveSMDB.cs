using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.EverdriveSMDB;
using SabreTools.IO.Readers;

namespace SabreTools.Serialization.Readers
{
    public class EverdriveSMDB : BaseBinaryReader<MetadataFile>
    {
        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Setup the reader and output
                var reader = new SeparatedValueReader(data, Encoding.UTF8)
                {
                    Header = false,
                    Separator = '\t',
                    VerifyFieldCount = false,
                };
                var dat = new MetadataFile();

                // Loop through the rows and parse out values
                var rows = new List<Row>();
                while (!reader.EndOfStream)
                {
                    // If we have no next line
                    if (!reader.ReadNextLine() || reader.Line == null)
                        break;

                    // If the next line has an invalid count
                    if (reader.Line.Count < 5)
                        break;

                    // Parse the line into a row
                    var row = new Row
                    {
                        SHA256 = reader.Line[0],
                        Name = reader.Line[1],
                        SHA1 = reader.Line[2],
                        MD5 = reader.Line[3],
                        CRC32 = reader.Line[4],
                    };

                    // If we have the size field
                    if (reader.Line.Count > 5)
                        row.Size = reader.Line[5];

                    rows.Add(row);
                }

                // Assign the rows to the Dat and return
                if (rows.Count > 0)
                {
                    dat.Row = [.. rows];
                    return dat;
                }

                return null;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
