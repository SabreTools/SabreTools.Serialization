using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Quantum : IFileSerializer<Models.Quantum.Archive>
    {
        /// <inheritdoc/>
        public Models.Quantum.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.Quantum().Deserialize(stream);
        }
    }
}