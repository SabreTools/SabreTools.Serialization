using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<Models.RomCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
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
    }
}