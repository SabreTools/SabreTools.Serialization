using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Nitro : IFileSerializer<Models.Nitro.Cart>
    {
        /// <inheritdoc/>
        public Models.Nitro.Cart? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.Nitro().Deserialize(stream);
        }
    }
}