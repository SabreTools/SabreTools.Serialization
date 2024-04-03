using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MoPaQ : IFileSerializer<Models.MoPaQ.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.MoPaQ.Archive? Deserialize(string? path)
        {
            var obj = new MoPaQ();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.MoPaQ.Archive? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.MoPaQ().Deserialize(stream);
        }
    }
}