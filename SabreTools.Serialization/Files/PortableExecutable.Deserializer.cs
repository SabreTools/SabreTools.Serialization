using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PortableExecutable : IFileSerializer<Models.PortableExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PortableExecutable.Executable? Deserialize(string? path)
        {
            var obj = new PortableExecutable();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PortableExecutable.Executable? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PortableExecutable().Deserialize(stream);
        }
    }
}