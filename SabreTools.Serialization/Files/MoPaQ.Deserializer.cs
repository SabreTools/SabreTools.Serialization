using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MoPaQ : IFileSerializer<Models.MoPaQ.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.MoPaQ.Archive? DeserializeFile(string? path)
        {
            var deserializer = new MoPaQ();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.MoPaQ.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.MoPaQ.DeserializeStream(stream);
        }
    }
}