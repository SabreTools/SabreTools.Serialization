using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<Models.RomCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.RomCenter.MetadataFile? Deserialize(string? path)
        {
            var deserializer = new RomCenter();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.RomCenter.MetadataFile? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.RomCenter().Deserialize(stream);
        }
    }
}