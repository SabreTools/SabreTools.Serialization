using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MoPaQ : IFileSerializer<Models.MoPaQ.Archive>
    {
        /// <inheritdoc/>
        public Models.MoPaQ.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.MoPaQ().Deserialize(stream);
        }
    }
}