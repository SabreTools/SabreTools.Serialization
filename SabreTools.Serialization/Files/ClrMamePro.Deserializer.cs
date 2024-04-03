using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<Models.ClrMamePro.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.ClrMamePro.MetadataFile? Deserialize(string? path)
        {
            var obj = new ClrMamePro();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.ClrMamePro.MetadataFile? Deserialize(string? path, bool quotes)
        {
            var obj = new ClrMamePro();
            return obj.DeserializeImpl(path, quotes);
        }

        /// <inheritdoc/>
        public Models.ClrMamePro.MetadataFile? DeserializeImpl(string? path)
            => DeserializeImpl(path, true);

        /// <inheritdoc/>
        public Models.ClrMamePro.MetadataFile? DeserializeImpl(string? path, bool quotes)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.ClrMamePro().Deserialize(stream, quotes);
        }
    }
}