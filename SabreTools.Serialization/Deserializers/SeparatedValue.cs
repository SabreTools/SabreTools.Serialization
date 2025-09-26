using System.Collections.Generic;
using System.IO;
using System.Text;
using SabreTools.Data.Models.SeparatedValue;
using SabreTools.IO.Readers;

namespace SabreTools.Serialization.Deserializers
{
    public class SeparatedValue : BaseBinaryDeserializer<MetadataFile>
    {
        #region Constants

        public const int HeaderWithoutExtendedHashesCount = 14;

        public const int HeaderWithExtendedHashesCount = 17;

        #endregion

        #region IByteDeserializer

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, ',');

        /// <inheritdoc cref="Deserialize(byte[], int)"/>
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
            return Deserialize(dataStream, delim);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(string? path)
            => Deserialize(path, ',');

        /// <inheritdoc cref="Deserialize(string?)"/>
        public MetadataFile? Deserialize(string? path, char delim)
        {
            try
            {
                // If we don't have a file
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return default;

                // Open the file for deserialization
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Deserialize(stream, delim);
            }
            catch
            {
                // TODO: Handle logging the exception
                return default;
            }
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc/>
        public override MetadataFile? Deserialize(Stream? data)
            => Deserialize(data, ',');

        /// <inheritdoc cref="Deserialize(Stream)"/>
        public MetadataFile? Deserialize(Stream? data, char delim)
        {
            // If the data is invalid
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
