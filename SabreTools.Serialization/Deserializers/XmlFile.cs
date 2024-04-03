using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    /// <summary>
    /// Base class for other XML deserializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlFile<T> : IFileDeserializer<T>
    {
        #region IFileDeserializer

        /// <inheritdoc/>
        public T? Deserialize(string? path)
        {
            using var data = PathProcessor.OpenStream(path);
            return new Streams.XmlFile<T>().Deserialize(data);
        }

        #endregion
    }
}