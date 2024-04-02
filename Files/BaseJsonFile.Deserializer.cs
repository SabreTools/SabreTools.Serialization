using System.Text;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class BaseJsonFile<T>
    {
        /// <inheritdoc/>
        public T? Deserialize(string? path, Encoding encoding)
        {
            using (var data = PathProcessor.OpenStream(path))
            {
                return new Streams.BaseJsonFile<T>().Deserialize(data, encoding);
            }
        }
    }
}
