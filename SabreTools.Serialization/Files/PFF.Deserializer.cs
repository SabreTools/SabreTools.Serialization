using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PFF : IFileSerializer<Models.PFF.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PFF.Archive? Deserialize(string? path)
        {
            var deserializer = new PFF();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PFF.Archive? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PFF().Deserialize(stream);
        }
    }
}