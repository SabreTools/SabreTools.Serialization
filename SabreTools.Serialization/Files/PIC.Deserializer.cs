using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<Models.PIC.DiscInformation>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PIC.DiscInformation? DeserializeFile(string? path)
        {
            var deserializer = new PIC();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PIC.DiscInformation? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PIC.DeserializeStream(stream);
        }
    }
}