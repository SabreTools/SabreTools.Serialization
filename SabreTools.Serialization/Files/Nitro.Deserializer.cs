using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Nitro : IFileSerializer<Models.Nitro.Cart>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Nitro.Cart? DeserializeFile(string? path)
        {
            var deserializer = new Nitro();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.Nitro.Cart? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Nitro.DeserializeStream(stream);
        }
    }
}