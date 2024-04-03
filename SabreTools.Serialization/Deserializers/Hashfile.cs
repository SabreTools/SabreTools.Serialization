using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class Hashfile : IFileDeserializer<Models.Hashfile.Hashfile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Hashfile.Hashfile? DeserializeFile(string? path, Hash hash = Hash.CRC)
        {
            var deserializer = new Hashfile();
            return deserializer.Deserialize(path, hash);
        }

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(string? path)
            => Deserialize(path, Hash.CRC);

        /// <inheritdoc/>
        public Models.Hashfile.Hashfile? Deserialize(string? path, Hash hash)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.Hashfile.DeserializeStream(stream, hash);
        }
    
        #endregion
    }
}