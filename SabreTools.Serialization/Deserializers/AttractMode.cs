using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.AttractMode;

namespace SabreTools.Serialization.Deserializers
{
    public class AttractMode : BaseBinaryDeserializer<MetadataFile>
    {
        #region Constants

        public const int HeaderWithoutRomnameCount = 17;

        public const int HeaderWithRomnameCount = 22;

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Stream? data)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return default;

            try
            {
                // Setup the reader and output
                var reader = new SeparatedValueReader(data, Encoding.UTF8)
                {
                    Separator = ';',
                    VerifyFieldCount = false,
                };
                var dat = new MetadataFile();

                // Read the header values first
                if (!reader.ReadHeader() || reader.HeaderValues == null)
                    return null;

                dat.Header = [.. reader.HeaderValues];

                // Loop through the rows and parse out values
                var rows = new List<Row>();
                while (!reader.EndOfStream)
                {
                    // If we have no next line
                    if (!reader.ReadNextLine() || reader.Line == null)
                        break;

                    // Parse the line into a row
                    Row row;
                    if (reader.Line.Count < HeaderWithRomnameCount)
                    {
                        row = new Row
                        {
                            Name = reader.Line[0],
                            Title = reader.Line[1],
                            Emulator = reader.Line[2],
                            CloneOf = reader.Line[3],
                            Year = reader.Line[4],
                            Manufacturer = reader.Line[5],
                            Category = reader.Line[6],
                            Players = reader.Line[7],
                            Rotation = reader.Line[8],
                            Control = reader.Line[9],
                            Status = reader.Line[10],
                            DisplayCount = reader.Line[11],
                            DisplayType = reader.Line[12],
                            AltRomname = reader.Line[13],
                            AltTitle = reader.Line[14],
                            Extra = reader.Line[15],
                            Buttons = reader.Line[16],
                        };
                    }
                    else
                    {
                        row = new Row
                        {
                            Name = reader.Line[0],
                            Title = reader.Line[1],
                            Emulator = reader.Line[2],
                            CloneOf = reader.Line[3],
                            Year = reader.Line[4],
                            Manufacturer = reader.Line[5],
                            Category = reader.Line[6],
                            Players = reader.Line[7],
                            Rotation = reader.Line[8],
                            Control = reader.Line[9],
                            Status = reader.Line[10],
                            DisplayCount = reader.Line[11],
                            DisplayType = reader.Line[12],
                            AltRomname = reader.Line[13],
                            AltTitle = reader.Line[14],
                            Extra = reader.Line[15],
                            Buttons = reader.Line[16],
                        };
                    }

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

        #endregion
    }
}