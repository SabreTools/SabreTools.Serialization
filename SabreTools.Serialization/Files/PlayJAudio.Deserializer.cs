using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJAudio : IFileSerializer<Models.PlayJ.AudioFile>
    {
        /// <inheritdoc/>
        public Models.PlayJ.AudioFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PlayJAudio().Deserialize(stream);
        }
    }
}