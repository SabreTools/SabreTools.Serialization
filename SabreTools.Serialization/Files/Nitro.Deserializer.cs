using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Nitro : IFileSerializer<Models.Nitro.Cart>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Nitro.Cart? Deserialize(string? path)
        {
            var deserializer = new Nitro();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.Nitro.Cart? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.Nitro().DeserializeImpl(stream);
        }
    }
}