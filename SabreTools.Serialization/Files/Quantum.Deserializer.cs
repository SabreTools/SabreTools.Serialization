using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Quantum : IFileSerializer<Models.Quantum.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Quantum.Archive? Deserialize(string? path)
        {
            var deserializer = new Quantum();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.Quantum.Archive? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.Quantum().DeserializeImpl(stream);
        }
    }
}