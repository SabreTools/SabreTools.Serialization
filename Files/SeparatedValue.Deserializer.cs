using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SeparatedValue : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(string path) => Deserialize(path, ',');
#else
        public MetadataFile? Deserialize(string? path) => Deserialize(path, ',');
#endif

        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(string path, char delim)
#else
        public MetadataFile? Deserialize(string? path, char delim)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.SeparatedValue().Deserialize(stream, delim);
            }
        }
    }
}