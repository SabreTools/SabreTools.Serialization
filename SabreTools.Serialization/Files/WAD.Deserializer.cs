using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class WAD : IFileSerializer<Models.WAD.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.WAD.File? DeserializeFile(string? path)
        {
            var deserializer = new WAD();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.WAD.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.WAD.DeserializeStream(stream);
        }
    }
}