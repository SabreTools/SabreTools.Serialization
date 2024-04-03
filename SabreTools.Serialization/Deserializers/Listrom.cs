using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class Listrom : IFileDeserializer<Models.Listrom.MetadataFile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Listrom.MetadataFile? DeserializeFile(string? path)
        {
            var deserializer = new Listrom();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.Listrom.MetadataFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Listrom.DeserializeStream(stream);
        }

        #endregion
    }
}