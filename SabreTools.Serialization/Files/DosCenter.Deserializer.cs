using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class DosCenter : IFileSerializer<Models.DosCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.DosCenter.MetadataFile? Deserialize(string? path)
        {
            var deserializer = new DosCenter();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.DosCenter.MetadataFile? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.DosCenter().Deserialize(stream);
        }
    }
}