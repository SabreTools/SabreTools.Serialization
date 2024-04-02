using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SGA : IFileSerializer<Models.SGA.File>
    {
        /// <inheritdoc/>
        public Models.SGA.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.SGA().Deserialize(stream);
        }
    }
}