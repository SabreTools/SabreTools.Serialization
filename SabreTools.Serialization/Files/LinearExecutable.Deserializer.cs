using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class LinearExecutable : IFileSerializer<Models.LinearExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.LinearExecutable.Executable? DeserializeFile(string? path)
        {
            var deserializer = new LinearExecutable();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.LinearExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.LinearExecutable.DeserializeStream(stream);
        }
    }
}