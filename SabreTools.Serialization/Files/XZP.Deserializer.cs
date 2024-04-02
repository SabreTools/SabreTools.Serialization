using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XZP : IFileSerializer<Models.XZP.File>
    {
        /// <inheritdoc/>
        public Models.XZP.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.XZP().Deserialize(stream);
        }
    }
}