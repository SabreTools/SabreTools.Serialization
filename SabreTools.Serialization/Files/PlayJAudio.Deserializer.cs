using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJAudio : IFileSerializer<Models.PlayJ.AudioFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PlayJ.AudioFile? DeserializeFile(string? path)
        {
            var deserializer = new PlayJAudio();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PlayJ.AudioFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PlayJAudio.DeserializeStream(stream);
        }
    }
}