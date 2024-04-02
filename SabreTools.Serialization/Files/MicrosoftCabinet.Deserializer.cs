using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MicrosoftCabinet : IFileSerializer<Models.MicrosoftCabinet.Cabinet>
    {
        /// <inheritdoc/>
        public Models.MicrosoftCabinet.Cabinet? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.MicrosoftCabinet().Deserialize(stream);
        }
    }
}