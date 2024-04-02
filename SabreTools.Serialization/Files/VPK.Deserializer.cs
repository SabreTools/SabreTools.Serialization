using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VPK : IFileSerializer<Models.VPK.File>
    {
        /// <inheritdoc/>
        public Models.VPK.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.VPK().Deserialize(stream);
        }
    }
}