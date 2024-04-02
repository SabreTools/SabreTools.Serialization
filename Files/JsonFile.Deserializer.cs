using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other JSON serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class JsonFile<T> : IFileSerializer<T>
    {
        /// <inheritdoc/>
        public T? Deserialize(string? path)
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
            using (var data = PathProcessor.OpenStream(path))
            {
                return new Streams.JsonFile<T>().Deserialize(data, encoding);
            }
        }
    }
}
