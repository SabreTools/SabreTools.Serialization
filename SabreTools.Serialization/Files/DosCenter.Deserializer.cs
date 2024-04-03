using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class DosCenter : IFileSerializer<Models.DosCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.DosCenter.MetadataFile? DeserializeFile(string? path)
        {
            var deserializer = new DosCenter();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.DosCenter.MetadataFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.DosCenter.DeserializeStream(stream);
        }
    }
}