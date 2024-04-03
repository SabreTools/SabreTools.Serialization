using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<Models.PIC.DiscInformation>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PIC.DiscInformation? Deserialize(string? path)
        {
            var obj = new PIC();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PIC.DiscInformation? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PIC().Deserialize(stream);
        }
    }
}