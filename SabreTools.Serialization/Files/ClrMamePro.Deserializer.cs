using SabreTools.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path) => Deserialize(path, true);

        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path, bool quotes)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.ClrMamePro().Deserialize(stream, quotes);
        }
    }
}