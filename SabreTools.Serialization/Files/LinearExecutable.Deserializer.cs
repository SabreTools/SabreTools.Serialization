using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class LinearExecutable : IFileSerializer<Models.LinearExecutable.Executable>
    {
        /// <inheritdoc/>
        public Models.LinearExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.LinearExecutable().Deserialize(stream);
        }
    }
}