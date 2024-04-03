using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJAudio : IFileSerializer<Models.PlayJ.AudioFile>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.PlayJ.AudioFile? obj, string? path)
        {
            var serializer = new PlayJAudio();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.PlayJ.AudioFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.PlayJAudio.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}