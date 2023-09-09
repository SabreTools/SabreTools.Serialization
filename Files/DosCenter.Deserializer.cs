using SabreTools.Models.DosCenter;

namespace SabreTools.Serialization.Files
{
    public partial class DosCenter : IFileSerializer<MetadataFile>
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
                return new Streams.DosCenter().Deserialize(stream);
            }
        }
    }
}