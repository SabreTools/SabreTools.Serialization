using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJPlaylist : IFileSerializer<Models.PlayJ.Playlist>
    {
        /// <inheritdoc/>
        public Models.PlayJ.Playlist? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PlayJPlaylist().Deserialize(stream);
        }
    }
}