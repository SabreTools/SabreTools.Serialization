using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AttractMode : IFileSerializer<Models.AttractMode.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
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
    }
}