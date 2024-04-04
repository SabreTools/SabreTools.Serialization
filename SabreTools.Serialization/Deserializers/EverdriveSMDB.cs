using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class EverdriveSMDB :
        IByteDeserializer<MetadataFile>,
        IFileDeserializer<MetadataFile>,
        IStreamDeserializer<MetadataFile>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static MetadataFile? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new EverdriveSMDB();
            return deserializer.Deserialize(data, offset);
        }

        /// <inheritdoc/>
        public MetadataFile? Deserialize(byte[]? data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static MetadataFile? DeserializeFile(string? path)
        {
            var deserializer = new EverdriveSMDB();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="IStreamDeserializer.Deserialize(Stream?)"/>
        public static MetadataFile? DeserializeStream(Stream? data)
        {
            var deserializer = new EverdriveSMDB();
            return deserializer.Deserialize(data);
        }
        
        /// <inheritdoc/>
        public MetadataFile? Deserialize(Stream? data)
        {
            // If the stream is null
            if (data == null)
                return default;

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

                // If we have additional fields
                if (reader.Line.Count > 6)
                    row.ADDITIONAL_ELEMENTS = reader.Line.Skip(5).ToArray();

                rows.Add(row);
            }

            // Assign the rows to the Dat and return
            dat.Row = rows.ToArray();
            return dat;
        }

        #endregion
    }
}