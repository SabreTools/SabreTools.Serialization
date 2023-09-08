namespace SabreTools.Serialization.Files
{
    /// <summary>
    /// Base class for other XML serializers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XmlFile<T> : IFileSerializer<T>
    {
        /// <inheritdoc/>
#if NET48
        public T Deserialize(string path)
#else
        public T? Deserialize(string? path)
#endif
        {
            using (var data = PathProcessor.OpenStream(path))
            {
                return new Streams.XmlFile<T>().Deserialize(data);
            }
        }
    }
}