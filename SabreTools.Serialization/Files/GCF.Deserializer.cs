using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class GCF : IFileSerializer<Models.GCF.File>
    {
        /// <inheritdoc/>
        public Models.GCF.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.GCF().Deserialize(stream);
        }
    }
}