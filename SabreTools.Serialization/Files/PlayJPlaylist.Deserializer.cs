using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJPlaylist : IFileSerializer<Models.PlayJ.Playlist>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PlayJ.Playlist? DeserializeFile(string? path)
        {
            var deserializer = new PlayJPlaylist();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PlayJ.Playlist? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PlayJPlaylist.DeserializeStream(stream);
        }
    }
}