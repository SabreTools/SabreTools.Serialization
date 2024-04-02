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
        public Stream? Serialize(T? obj)
            => Serialize(obj, new UTF8Encoding(false));

        /// <summary>
        /// Serialize a <typeparamref name="T"/> into a Stream
        /// </summary>
        /// <typeparam name="T">Type of object to serialize from</typeparam>
        /// <param name="obj">Data to serialize</param>
        /// <param name="encoding"></param>
        /// <returns>Filled object on success, null on error</returns>
        public Stream? Serialize(T? obj, Encoding encoding)
        {
            // If the object is null
            if (obj == null)
                return null;

            // Setup the serializer and the writer
            var serializer = JsonSerializer.Create();
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, encoding);
            var jsonWriter = new JsonTextWriter(streamWriter);

            // Perform the deserialization and return
            serializer.Serialize(jsonWriter, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
