using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class LinearExecutable : IFileSerializer<Models.LinearExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.LinearExecutable.Executable? Deserialize(string? path)
        {
            var deserializer = new LinearExecutable();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.LinearExecutable.Executable? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.LinearExecutable().Deserialize(stream);
        }
    }
}