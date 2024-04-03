using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PlayJAudio : IFileSerializer<Models.PlayJ.AudioFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PlayJ.AudioFile? Deserialize(string? path)
        {
            var obj = new PlayJAudio();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PlayJ.AudioFile? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PlayJAudio().Deserialize(stream);
        }
    }
}