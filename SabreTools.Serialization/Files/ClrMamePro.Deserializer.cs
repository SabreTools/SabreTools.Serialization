using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<Models.ClrMamePro.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.ClrMamePro.MetadataFile? DeserializeFile(string? path, bool quotes = true)
        {
            var deserializer = new ClrMamePro();
            return deserializer.Deserialize(path, quotes);
        }

        /// <inheritdoc/>
        public Models.ClrMamePro.MetadataFile? Deserialize(string? path)
            => Deserialize(path, true);

        /// <inheritdoc/>
        public Models.ClrMamePro.MetadataFile? Deserialize(string? path, bool quotes)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.ClrMamePro.DeserializeStream(stream, quotes);
        }
    }
}