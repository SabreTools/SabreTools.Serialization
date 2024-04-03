using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AACS : IFileSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.AACS.MediaKeyBlock? DeserializeFile(string? path)
        {
            var deserializer = new AACS();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.AACS.MediaKeyBlock? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.AACS.DeserializeStream(stream);
        }
    }
}