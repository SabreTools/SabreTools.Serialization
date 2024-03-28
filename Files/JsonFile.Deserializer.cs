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
        {
            using (var data = PathProcessor.OpenStream(path))
            {
                return new Streams.JsonFile<T>().Deserialize(data);
            }
        }
    }
}
