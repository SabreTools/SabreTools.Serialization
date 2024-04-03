using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class AttractMode : IFileDeserializer<Models.AttractMode.MetadataFile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.AttractMode.MetadataFile? DeserializeFile(string? path)
        {
            var deserializer = new AttractMode();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.AttractMode.MetadataFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.AttractMode.DeserializeStream(stream);
        }

        #endregion
    }
}