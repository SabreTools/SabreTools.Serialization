using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<Models.PIC.DiscInformation>
    {
        /// <inheritdoc/>
        public Models.PIC.DiscInformation? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PIC().Deserialize(stream);
        }
    }
}