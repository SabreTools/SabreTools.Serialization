using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.AttractMode;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class AttractMode : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc cref="IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static MetadataFile? Deserialize(Stream? data)
        {
            var deserializer = new AttractMode();
            return deserializer.DeserializeImpl(data);
        }
        
        /// <inheritdoc/>
        public MetadataFile? DeserializeImpl(Stream? data)
        {
            // If the stream is null
            if (data == null)
                return default;

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

            dat.Header = reader.HeaderValues.ToArray();

            // Loop through the rows and parse out values
            var rows = new List<Row>();
            while (!reader.EndOfStream)
            {
                // If we have no next line
                if (!reader.ReadNextLine() || reader.Line == null)
                    break;

                // Parse the line into a row
                Row row;
                if (reader.Line.Count < Serialization.AttractMode.HeaderWithRomnameCount)
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

                    // If we have additional fields
                    if (reader.Line.Count > Serialization.AttractMode.HeaderWithoutRomnameCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(Serialization.AttractMode.HeaderWithoutRomnameCount).ToArray();
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

                    // If we have additional fields
                    if (reader.Line.Count > Serialization.AttractMode.HeaderWithRomnameCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(Serialization.AttractMode.HeaderWithRomnameCount).ToArray();
                }

                rows.Add(row);
            }

            // Assign the rows to the Dat and return
            dat.Row = rows.ToArray();
            return dat;
        }
    }
}