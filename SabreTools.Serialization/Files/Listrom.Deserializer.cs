using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Listrom : IFileSerializer<Models.Listrom.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Listrom.MetadataFile? Deserialize(string? path)
        {
            var deserializer = new Listrom();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.Listrom.MetadataFile? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Listrom.Deserialize(stream);
        }
    }
}