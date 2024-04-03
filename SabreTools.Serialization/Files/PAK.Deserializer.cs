using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PAK : IFileSerializer<Models.PAK.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PAK.File? DeserializeFile(string? path)
        {
            var deserializer = new PAK();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.PAK.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.PAK.DeserializeStream(stream);
        }
    }
}