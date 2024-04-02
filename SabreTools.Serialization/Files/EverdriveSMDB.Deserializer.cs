using SabreTools.Models.EverdriveSMDB;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class EverdriveSMDB : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public MetadataFile? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.EverdriveSMDB().Deserialize(stream);
            }
        }
    }
}