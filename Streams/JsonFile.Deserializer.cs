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
        public T? Deserialize(Stream? data)
        {
            // If the stream is null
            if (data == null)
                return default;

            // Setup the serializer and the reader
            var serializer = JsonSerializer.Create();
            var streamReader = new StreamReader(data);
            var jsonReader = new JsonTextReader(streamReader);

            // Perform the deserialization and return
            return (T?)serializer.Deserialize(jsonReader, typeof(T?));
        }
    }
}
