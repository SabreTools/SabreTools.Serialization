using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MicrosoftCabinet : IFileSerializer<Models.MicrosoftCabinet.Cabinet>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.MicrosoftCabinet.Cabinet? Deserialize(string? path)
        {
            var deserializer = new MicrosoftCabinet();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.MicrosoftCabinet.Cabinet? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.MicrosoftCabinet.Deserialize(stream);
        }
    }
}