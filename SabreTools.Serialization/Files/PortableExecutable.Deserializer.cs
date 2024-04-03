using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PortableExecutable : IFileSerializer<Models.PortableExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PortableExecutable.Executable? DeserializeFile(string? path)
        {
            var deserializer = new PortableExecutable();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PortableExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PortableExecutable.DeserializeStream(stream);
        }
    }
}