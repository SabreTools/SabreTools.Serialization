using System.IO;
using System.Text;
using Newtonsoft.Json;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class JsonFile<T> : IStreamSerializer<T>
    {
        /// <inheritdoc/>
        public T? Deserialize(Stream? data)
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
            // If the stream is null
            if (data == null)
                return default;

            // Setup the serializer and the reader
            var serializer = JsonSerializer.Create();
            var streamReader = new StreamReader(data, encoding);
            var jsonReader = new JsonTextReader(streamReader);

            // Perform the deserialization and return
            return serializer.Deserialize<T>(jsonReader);
        }
    }
}
