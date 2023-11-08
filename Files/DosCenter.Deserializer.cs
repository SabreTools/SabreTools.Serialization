using SabreTools.Models.DosCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class DosCenter : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.DosCenter().Deserialize(stream);
            }
        }
    }
}