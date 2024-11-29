using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.SeparatedValue;

namespace SabreTools.Serialization.Deserializers
{
    public class SeparatedValue : BaseBinaryDeserializer<MetadataFile>
    {
        #region Constants

        public const int HeaderWithoutExtendedHashesCount = 14;

        public const int HeaderWithExtendedHashesCount = 17;

        #endregion

        #region IByteDeserializer

        /// <inheritdoc cref="IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static MetadataFile? DeserializeBytes(byte[]? data, int offset, char delim)
        {
            var deserializer = new SeparatedValue();
            return deserializer.Deserialize(data, offset, delim);
        }

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, ',');

        /// <inheritdoc/>
        public MetadataFile? Deserialize(byte[]? data, int offset, char delim)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return DeserializeStream(dataStream, delim);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static MetadataFile? DeserializeFile(string? path, char delim = ',')
        {
            var deserializer = new SeparatedValue();
            return deserializer.Deserialize(path, delim);
        }

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(string? path)
            => Deserialize(path, ',');

        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path, char delim)
        {
            using var stream = PathProcessor.OpenStream(path);
            return DeserializeStream(stream, delim);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="IStreamDeserializer.Deserialize(Stream?)"/>
        public static MetadataFile? DeserializeStream(Stream? data, char delim = ',')
        {
            var deserializer = new SeparatedValue();
            return deserializer.Deserialize(data, delim);
        }

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Stream? data)
            => Deserialize(data, ',');

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public MetadataFile? Deserialize(Stream? data, char delim)
        {
            // If tthe data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
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
                    if (reader.Line.Count < HeaderWithExtendedHashesCount)
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