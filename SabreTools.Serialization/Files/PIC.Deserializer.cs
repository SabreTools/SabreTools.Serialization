using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<Models.PIC.DiscInformation>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PIC.DiscInformation? Deserialize(string? path)
        {
            var deserializer = new PIC();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PIC.DiscInformation? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PIC.Deserialize(stream);
        }
    }
}