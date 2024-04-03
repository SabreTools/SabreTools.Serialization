using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class ClrMamePro : IFileDeserializer<Models.ClrMamePro.MetadataFile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.ClrMamePro.MetadataFile? DeserializeFile(string? path, bool quotes = true)
        {
            var deserializer = new ClrMamePro();
            return deserializer.Deserialize(path, quotes);
        }

        /// <inheritdoc/>
        public Models.ClrMamePro.MetadataFile? Deserialize(string? path)
            => Deserialize(path, true);

        /// <inheritdoc/>
        public Models.ClrMamePro.MetadataFile? Deserialize(string? path, bool quotes)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.ClrMamePro.DeserializeStream(stream, quotes);
        }

        #endregion
    }
}