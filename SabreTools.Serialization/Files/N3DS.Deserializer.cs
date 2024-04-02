using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class N3DS : IFileSerializer<Models.N3DS.Cart>
    {
        /// <inheritdoc/>
        public Models.N3DS.Cart? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.N3DS().Deserialize(stream);
        }
    }
}