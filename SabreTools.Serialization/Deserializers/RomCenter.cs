using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class RomCenter : IFileDeserializer<Models.RomCenter.MetadataFile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.RomCenter.MetadataFile? DeserializeFile(string? path)
        {
            var deserializer = new RomCenter();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.RomCenter.MetadataFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.RomCenter.DeserializeStream(stream);
        }

        #endregion
    }
}