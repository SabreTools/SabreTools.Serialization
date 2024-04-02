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
