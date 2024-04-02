using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AACS : IFileSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc/>
        public Models.AACS.MediaKeyBlock? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.AACS().Deserialize(stream);
        }
    }
}