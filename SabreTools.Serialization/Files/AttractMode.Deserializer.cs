using SabreTools.Models.AttractMode;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AttractMode : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.AttractMode().Deserialize(stream);
            }
        }
    }
}