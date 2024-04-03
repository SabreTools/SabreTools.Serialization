using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AttractMode : IFileSerializer<Models.AttractMode.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.AttractMode.MetadataFile? Deserialize(string? path)
        {
            var deserializer = new AttractMode();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.AttractMode.MetadataFile? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.AttractMode().DeserializeImpl(stream);
        }
    }
}