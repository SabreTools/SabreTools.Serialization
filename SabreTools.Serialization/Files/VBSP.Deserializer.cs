using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VBSP : IFileSerializer<Models.VBSP.File>
    {
        /// <inheritdoc/>
        public Models.VBSP.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.VBSP().Deserialize(stream);
        }
    }
}