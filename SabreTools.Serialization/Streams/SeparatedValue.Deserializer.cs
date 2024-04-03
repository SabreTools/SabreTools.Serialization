using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class SeparatedValue : IStreamSerializer<MetadataFile>
    {
        /// <inheritdoc cref="IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static MetadataFile? Deserialize(Stream? data)
        {
            var deserializer = new SeparatedValue();
            return deserializer.DeserializeImpl(data);
        }

        /// <inheritdoc cref="IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static MetadataFile? Deserialize(Stream? data, char delim)
        {
            var deserializer = new SeparatedValue();
            return deserializer.DeserializeImpl(data, delim);
        }
        
        /// <inheritdoc/>
        public MetadataFile? DeserializeImpl(Stream? data)
            => Deserialize(data, ',');

        /// <inheritdoc cref="DeserializeImpl(Stream)"/>
        public MetadataFile? DeserializeImpl(Stream? data, char delim)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the reader and output
            var reader = new SeparatedValueReader(data, Encoding.UTF8)
            {
                Header = true,
                Separator = delim,
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
                Row? row = null;
                if (reader.Line.Count < Serialization.SeparatedValue.HeaderWithExtendedHashesCount)
                {
                    row = new Row
                    {
                        FileName = reader.Line[0],
                        InternalName = reader.Line[1],
                        Description = reader.Line[2],
                        GameName = reader.Line[3],
                        GameDescription = reader.Line[4],
                        Type = reader.Line[5],
                        RomName = reader.Line[6],
                        DiskName = reader.Line[7],
                        Size = reader.Line[8],
                        CRC = reader.Line[9],
                        MD5 = reader.Line[10],
                        SHA1 = reader.Line[11],
                        SHA256 = reader.Line[12],
                        Status = reader.Line[13],
                    };

                    // If we have additional fields
                    if (reader.Line.Count > Serialization.SeparatedValue.HeaderWithoutExtendedHashesCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(Serialization.SeparatedValue.HeaderWithoutExtendedHashesCount).ToArray();
                }
                else
                {
                    row = new Row
                    {
                        FileName = reader.Line[0],
                        InternalName = reader.Line[1],
                        Description = reader.Line[2],
                        GameName = reader.Line[3],
                        GameDescription = reader.Line[4],
                        Type = reader.Line[5],
                        RomName = reader.Line[6],
                        DiskName = reader.Line[7],
                        Size = reader.Line[8],
                        CRC = reader.Line[9],
                        MD5 = reader.Line[10],
                        SHA1 = reader.Line[11],
                        SHA256 = reader.Line[12],
                        SHA384 = reader.Line[13],
                        SHA512 = reader.Line[14],
                        SpamSum = reader.Line[15],
                        Status = reader.Line[16],
                    };

                    // If we have additional fields
                    if (reader.Line.Count > Serialization.SeparatedValue.HeaderWithExtendedHashesCount)
                        row.ADDITIONAL_ELEMENTS = reader.Line.Skip(Serialization.SeparatedValue.HeaderWithExtendedHashesCount).ToArray();
                }
                rows.Add(row);
            }

            // Assign the rows to the Dat and return
            dat.Row = rows.ToArray();
            return dat;
        }
    }
}