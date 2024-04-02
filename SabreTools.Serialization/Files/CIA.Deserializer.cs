using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CIA : IFileSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc/>
        public Models.N3DS.CIA? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.CIA().Deserialize(stream);
        }
    }
}