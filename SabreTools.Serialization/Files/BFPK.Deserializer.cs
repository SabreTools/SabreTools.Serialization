using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BFPK : IFileSerializer<Models.BFPK.Archive>
    {
        /// <inheritdoc/>
        public Models.BFPK.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.BFPK().Deserialize(stream);
        }
    }
}