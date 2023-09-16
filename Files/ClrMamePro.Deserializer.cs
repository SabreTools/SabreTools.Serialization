using SabreTools.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(string path) => Deserialize(path, true);
#else
        public MetadataFile? Deserialize(string? path) => Deserialize(path, true);
#endif

        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(string path, bool quotes)
#else
        public MetadataFile? Deserialize(string? path, bool quotes)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.ClrMamePro().Deserialize(stream, quotes);
            }
        }
    }
}