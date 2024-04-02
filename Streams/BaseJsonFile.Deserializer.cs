using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SabreTools.Serialization.Streams
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class BaseJsonFile<T>
    {
        /// <inheritdoc/>
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
