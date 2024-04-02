using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MSDOS : IFileSerializer<Models.MSDOS.Executable>
    {
        /// <inheritdoc/>
        public Models.MSDOS.Executable? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.MSDOS().Deserialize(stream);
            }
        }
    }
}