using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Writers;
using SabreTools.Models.SeparatedValue;

namespace SabreTools.Serialization.Serializers
{
    // TODO: Create variants for the 3 common types: CSV, SSV, TSV
    public class SeparatedValue : BaseBinarySerializer<MetadataFile>
    {
        #region IByteSerializer

        /// <inheritdoc cref="Interfaces.IByteSerializer.SerializeArray(T?)"/>
        public static byte[]? SerializeBytes(MetadataFile? obj, char delim = ',', bool longHeader = false)
        {
            var serializer = new SeparatedValue();
            return serializer.SerializeArray(obj, delim, longHeader);
        }

        /// <inheritdoc/>
        public override byte[]? SerializeArray(MetadataFile? obj)
            => SerializeArray(obj, ',', false);

        /// <inheritdoc/>
        public byte[]? SerializeArray(MetadataFile? obj, char delim, bool longHeader)
        {
            using var stream = SerializeStream(obj, delim, longHeader);
            if (stream == null)
                return null;

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(MetadataFile? obj, string? path, char delim = ',', bool longHeader = false)
        {
            var serializer = new SeparatedValue();
            return serializer.Serialize(obj, path, delim, longHeader);
        }

        /// <inheritdoc/>
        public override bool Serialize(MetadataFile? obj, string? path)
            => Serialize(obj, path, ',', false);

        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path, char delim, bool longHeader)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = SerializeStream(obj, delim, longHeader);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(MetadataFile? obj, char delim = ',', bool longHeader = false)
        {
            var serializer = new SeparatedValue();
            return serializer.Serialize(obj, delim, longHeader);
        }

        /// <inheritdoc/>
        public override Stream? Serialize(MetadataFile? obj)
            => Serialize(obj, ',', false);

        /// <inheritdoc cref="Serialize(MetadataFile)"/>
        public Stream? Serialize(MetadataFile? obj, char delim, bool longHeader)
        {
            // If the metadata file is null
            if (obj == null)
                return null;

            // Setup the writer and output
            var stream = new MemoryStream();
            var writer = new SeparatedValueWriter(stream, Encoding.UTF8) { Separator = delim, Quotes = true };

            // Write the header
            WriteHeader(writer, longHeader);

            // Write out the rows, if they exist
            WriteRows(obj.Row, writer, longHeader);

            // Return the stream
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// Write header information to the current writer
        /// </summary>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        /// <param name="longHeader">True if the long variant of the row should be written, false otherwise</param>
        private static void WriteHeader(SeparatedValueWriter writer, bool longHeader)
        {
            string?[] headerArray;
            if (longHeader)
            {
                headerArray =
                [
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
                    "SHA384",
                    "SHA512",
                    "SpamSum",
                    "Status",
                ];
            }
            else
            {
                headerArray =
                [
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
                    "Status",
                ];
            }

            writer.WriteHeader(headerArray);
            writer.Flush();
        }

        /// <summary>
        /// Write rows information to the current writer
        /// </summary>
        /// <param name="rows">Array of Row objects representing the rows information</param>
        /// <param name="writer">SeparatedValueWriter representing the output</param>
        /// <param name="longHeader">True if the long variant of the row should be written, false otherwise</param>
        private static void WriteRows(Row[]? rows, SeparatedValueWriter writer, bool longHeader)
        {
            // If the games information is missing, we can't do anything
            if (rows == null || rows.Length == 0)
                return;

            // Loop through and write out the rows
            foreach (var row in rows)
            {
                string?[] rowArray;
                if (longHeader)
                {
                    rowArray =
                    [
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
                        row.SHA384,
                        row.SHA512,
                        row.SpamSum,
                        row.Status,
                    ];
                }
                else
                {
                    rowArray =
                    [
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
                        row.Status,
                    ];
                }

                writer.WriteValues(rowArray);
                writer.Flush();
            }
        }

        #endregion
    }
}