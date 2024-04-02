using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PFF : IFileSerializer<Models.PFF.Archive>
    {
        /// <inheritdoc/>
        public Models.PFF.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PFF().Deserialize(stream);
        }
    }
}