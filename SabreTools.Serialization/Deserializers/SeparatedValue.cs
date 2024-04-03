using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class SeparatedValue : IFileDeserializer<Models.SeparatedValue.MetadataFile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.SeparatedValue.MetadataFile? DeserializeFile(string? path, char delim = ',')
        {
            var deserializer = new SeparatedValue();
            return deserializer.Deserialize(path, delim);
        }

        /// <inheritdoc/>
        public Models.SeparatedValue.MetadataFile? Deserialize(string? path)
            => Deserialize(path, ',');

        /// <inheritdoc/>
        public Models.SeparatedValue.MetadataFile? Deserialize(string? path, char delim)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.SeparatedValue.DeserializeStream(stream, delim);
        }

        #endregion
    }
}