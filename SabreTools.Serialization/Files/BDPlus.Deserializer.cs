using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BDPlus : IFileSerializer<Models.BDPlus.SVM>
    {
        /// <inheritdoc/>
        public Models.BDPlus.SVM? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.BDPlus().Deserialize(stream);
        }
    }
}