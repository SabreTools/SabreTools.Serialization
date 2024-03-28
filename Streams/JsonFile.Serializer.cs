using System.IO;
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
        {
            // If the object is null
            if (obj == null)
                return null;

            // Setup the serializer and the writer
            var serializer = JsonSerializer.Create();
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            var jsonWriter = new JsonTextWriter(streamWriter);

            // Perform the deserialization and return
            serializer.Serialize(jsonWriter, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
