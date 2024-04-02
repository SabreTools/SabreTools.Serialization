using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SeparatedValue : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path) => Deserialize(path, ',');

        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path, char delim)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.SeparatedValue().Deserialize(stream, delim);
            }
        }
    }
}