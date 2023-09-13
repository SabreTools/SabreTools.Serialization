using SabreTools.Models.PIC;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<DiscInformation>
    {
        /// <inheritdoc/>
#if NET48
        public DiscInformation Deserialize(string path)
#else
        public DiscInformation? Deserialize(string? path)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.PIC().Deserialize(stream);
            }
        }
    }
}