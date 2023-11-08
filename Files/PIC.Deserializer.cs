using SabreTools.Models.PIC;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<DiscInformation>
    {
        /// <inheritdoc/>
        public DiscInformation? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.PIC().Deserialize(stream);
            }
        }
    }
}