using SabreTools.Models.AttractMode;

namespace SabreTools.Serialization.Files
{
    public partial class AttractMode : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public MetadataFile Deserialize(string path)
#else
        public MetadataFile? Deserialize(string? path)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.AttractMode().Deserialize(stream);
            }
        }
    }
}