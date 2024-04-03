using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJPlaylist : IFileSerializer<Models.PlayJ.Playlist>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.PlayJ.Playlist? obj, string? path)
        {
            var serializer = new PlayJPlaylist();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.PlayJ.Playlist? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.PlayJPlaylist.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}