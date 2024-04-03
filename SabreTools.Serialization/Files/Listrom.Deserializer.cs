using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Listrom : IFileSerializer<Models.Listrom.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
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
    }
}