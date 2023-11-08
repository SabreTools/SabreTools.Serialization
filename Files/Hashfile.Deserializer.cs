using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(string? path) => Deserialize(path, Hash.CRC);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(string? path, Hash hash)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.Hashfile().Deserialize(stream, hash);
            }
        }
    }
}