using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class N3DS : IFileSerializer<Models.N3DS.Cart>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.N3DS.Cart? Deserialize(string? path)
        {
            var obj = new N3DS();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.N3DS.Cart? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.N3DS().Deserialize(stream);
        }
    }
}