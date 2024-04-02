using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CueSheet : IFileSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
        public Models.CueSheets.CueSheet? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.CueSheet().Deserialize(stream);
        }
    }
}
