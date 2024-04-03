using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? Deserialize(string? path)
        {
            var obj = new Hashfile();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? Deserialize(string? path, Hash hash)
        {
            var obj = new Hashfile();
            return obj.DeserializeImpl(path, hash);
        }

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? DeserializeImpl(string? path)
            => DeserializeImpl(path, Hash.CRC);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? DeserializeImpl(string? path, Hash hash)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.Hashfile().Deserialize(stream, hash);
        }
    }
}