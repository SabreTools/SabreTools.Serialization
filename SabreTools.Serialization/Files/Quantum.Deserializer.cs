using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Quantum : IFileSerializer<Models.Quantum.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Quantum.Archive? DeserializeFile(string? path)
        {
            var deserializer = new Quantum();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.Quantum.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Quantum.DeserializeStream(stream);
        }
    }
}