using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PFF : IFileSerializer<Models.PFF.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PFF.Archive? DeserializeFile(string? path)
        {
            var deserializer = new PFF();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PFF.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PFF.DeserializeStream(stream);
        }
    }
}