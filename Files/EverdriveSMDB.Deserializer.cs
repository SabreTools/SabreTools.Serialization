using SabreTools.Models.EverdriveSMDB;

namespace SabreTools.Serialization.Files
{
    public partial class EverdriveSMDB : IFileSerializer<MetadataFile>
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
                return new Streams.EverdriveSMDB().Deserialize(stream);
            }
        }
    }
}