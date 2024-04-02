using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PortableExecutable : IFileSerializer<Models.PortableExecutable.Executable>
    {
        /// <inheritdoc/>
        public Models.PortableExecutable.Executable? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.PortableExecutable().Deserialize(stream);
            }
        }
    }
}