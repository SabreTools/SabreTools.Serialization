using SabreTools.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Listrom : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.Listrom().Deserialize(stream);
            }
        }
    }
}