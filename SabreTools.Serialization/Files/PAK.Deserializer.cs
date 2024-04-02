using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PAK : IFileSerializer<Models.PAK.File>
    {
        /// <inheritdoc/>
        public Models.PAK.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PAK().Deserialize(stream);
        }
    }
}