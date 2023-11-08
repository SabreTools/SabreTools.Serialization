using SabreTools.Models.RomCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.RomCenter().Deserialize(stream);
            }
        }
    }
}