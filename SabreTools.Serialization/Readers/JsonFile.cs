using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Readers
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonFile<T> : BaseBinaryDeserializer<T>
    {
        #region IByteDeserializer

        /// <inheritdoc/>
        public override T? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, new UTF8Encoding(false));

        /// <summary>
        /// Deserialize a byte array into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="encoding">Encoding to parse text as</param>
        /// <returns>Filled object on success, null on error</returns>
        public T? Deserialize(byte[]? data, int offset, Encoding encoding)
        {
            // If the data is invalid
            if (data == null || data.Length == 0)
                return default;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return default;

            // Create a memory stream and parse that
            var dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Deserialize(dataStream, encoding);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc/>
        public override T? Deserialize(string? path)
            => Deserialize(path, new UTF8Encoding(false));

        /// <summary>
        /// Deserialize a file into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="path">Path to deserialize from</param>
        /// <param name="encoding">Encoding to parse text as</param>
        /// <returns>Filled object on success, null on error</returns>
        public T? Deserialize(string? path, Encoding encoding)
        {
            try
            {
                // If we don't have a file
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return default;

                // Open the file for deserialization
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return Deserialize(stream, encoding);
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
        public override T? Deserialize(Stream? data)
            => Deserialize(data, new UTF8Encoding(false));

        /// <summary>
        /// Deserialize a Stream into <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="data">Stream to parse</param>
        /// <param name="encoding">Text encoding to use</param>
        /// <returns>Filled object on success, null on error</returns>
        public T? Deserialize(Stream? data, Encoding encoding)
        {
            // If the stream is invalid
            if (data == null || !data.CanRead)
                return default;

            try
            {
                // Setup the serializer and the reader
                var serializer = JsonSerializer.Create();
                var streamReader = new StreamReader(data, encoding);
                var jsonReader = new JsonTextReader(streamReader);

                // Perform the deserialization and return
                return serializer.Deserialize<T>(jsonReader);
            }
            catch
            {
                // Ignore the actual error
                return default;
            }
        }

        #endregion
    }
}
