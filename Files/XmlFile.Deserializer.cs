using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XmlFile<T> : IFileSerializer<T>
    {
        /// <inheritdoc/>
        public T? Deserialize(string? path)
        {
            using (var data = PathProcessor.OpenStream(path))
            {
                return new Streams.XmlFile<T>().Deserialize(data);
            }
        }
    }
}