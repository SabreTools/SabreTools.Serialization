using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? Deserialize(string? path)
        {
            var deserializer = new Hashfile();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? Deserialize(string? path, Hash hash)
        {
            var deserializer = new Hashfile();
            return deserializer.DeserializeImpl(path, hash);
        }

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? DeserializeImpl(string? path)
            => DeserializeImpl(path, Hash.CRC);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? DeserializeImpl(string? path, Hash hash)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Hashfile.Deserialize(stream, hash);
        }
    }
}