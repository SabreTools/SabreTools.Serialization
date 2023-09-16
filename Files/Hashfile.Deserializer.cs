using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
#if NET48
        public Models.Hashfile.Hashfile Deserialize(string path) => Deserialize(path, Hash.CRC);
#else
        public Models.Hashfile.Hashfile? Deserialize(string? path) => Deserialize(path, Hash.CRC);
#endif

        /// <inheritdoc/>
#if NET48
        public Models.Hashfile.Hashfile Deserialize(string path, Hash hash)
#else
        public Models.Hashfile.Hashfile? Deserialize(string? path, Hash hash)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.Hashfile().Deserialize(stream, hash);
            }
        }
    }
}